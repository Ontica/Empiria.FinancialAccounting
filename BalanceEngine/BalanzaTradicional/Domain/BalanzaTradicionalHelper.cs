/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanzaTradicionalHelper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build traditional trial balances.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using System.Linq;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Helper methods to build traditional trial balances.</summary>
  internal class BalanzaTradicionalHelper {

    private readonly TrialBalanceCommand _command;

    public BalanzaTradicionalHelper(TrialBalanceCommand Command) {

      _command = Command;
    }


    #region Public methods


    internal FixedList<TrialBalanceEntry> GetCalculatedParentAccounts(
                                     FixedList<TrialBalanceEntry> accountEntries) {

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);

      foreach (var entry in accountEntries) {

        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountNumber = SubledgerAccount.Parse(entry.SubledgerAccountId).Number ?? "";

        StandardAccount currentParent;

        bool isCalculatedAccount = ValidateEntryForSummaryParentAccount(entry, out currentParent);

        if (!isCalculatedAccount) {
          continue;
        }

        if (entry.HasParentPostingEntry) {
          continue;
        }

        while (true) {
          entry.DebtorCreditor = entry.Account.DebtorCreditor;
          entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

          if (entry.Level > 1) {
            SummaryByAccountEntry(parentAccounts, entry, currentParent,
                            entry.Sector, TrialBalanceItemType.Summary);

            ValidateSectorizationForSummaryParentEntry(parentAccounts, entry, currentParent);
          }

          if (!currentParent.HasParent && entry.HasSector) {
            trialBalanceHelper.GetEntriesAndParentSector(parentAccounts, entry, currentParent);
            break;

          } else if (!currentParent.HasParent) {
            break;

          } else {
            currentParent = currentParent.GetParent();
          }

        } // while

      } // foreach

      trialBalanceHelper.AssignLastChangeDatesToSummaryEntries(accountEntries, parentAccounts);

      return parentAccounts.ToFixedList();
    }


    internal List<TrialBalanceEntry> CombineTotalGroupEntriesAndAccountEntries(
                                      List<TrialBalanceEntry> balanzaEntries,
                                      FixedList<TrialBalanceEntry> groupTotalsEntries) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalGroupEntry in groupTotalsEntries) {

        var accountEntries = balanzaEntries.Where(
                                  a => a.Account.GroupNumber == totalGroupEntry.GroupNumber &&
                                  a.Ledger.Id == totalGroupEntry.Ledger.Id &&
                                  a.Currency.Id == totalGroupEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == totalGroupEntry.DebtorCreditor).ToList();

        accountEntries.Add(totalGroupEntry);
        returnedEntries.AddRange(accountEntries);
      }

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineParentsAndAccountEntries(
                                      List<TrialBalanceEntry> parentAccountEntries,
                                      List<TrialBalanceEntry> accountEntries) {

      var returnedCombineEntries = new List<TrialBalanceEntry>(accountEntries);

      returnedCombineEntries.AddRange(parentAccountEntries);

      SetSubledgerAccountInfo(returnedCombineEntries);

      returnedCombineEntries = OrderingParentAndPostingAccountEntries(returnedCombineEntries);

      return returnedCombineEntries;
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalGroupEntries(FixedList<TrialBalanceEntry> accountEntries) {

      var toReturnTotalGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries.Where(a => !a.HasParentPostingEntry)) {
        SumAccountEntriesToTotalGroup(toReturnTotalGroupEntries, entry);
      }

      return toReturnTotalGroupEntries.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetPostingEntries() {

      FixedList<TrialBalanceEntry> accountEntries = BalancesDataService.GetTrialBalanceEntries(_command);

      if (_command.ValuateBalances || _command.InitialPeriod.UseDefaultValuation) {
        ValuateAccountEntriesToExchangeRate(accountEntries);

        if (_command.ConsolidateBalancesToTargetCurrency) {
          accountEntries = ConsolidateAccountEntriesToTargetCurrency(accountEntries);
        }
      }

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      trialBalanceHelper.RoundDecimals(accountEntries);

      return accountEntries;
    }

    
    internal void SummaryByAccountEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                 TrialBalanceEntry entry,
                                 StandardAccount targetAccount, Sector targetSector,
                                 TrialBalanceItemType itemType) {

      var trialBalanceHelper = new TrialBalanceHelper(_command);

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      trialBalanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount,
                                                   targetSector, itemType, hash);
    }


    #endregion Public methods


    #region Private methods


    private FixedList<TrialBalanceEntry> ConsolidateAccountEntriesToTargetCurrency(
                                          FixedList<TrialBalanceEntry> trialBalance) {

      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);
      var AccountEntriesToConsolidate = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||" +
                      $"{targetCurrency.Id}||{entry.Ledger.Id}";

        if (_command.WithSubledgerAccount) {
          hash = $"{entry.Account.Number}||{entry.SubledgerAccountId}||" +
                 $"{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}";

        }

        if (entry.Currency.Equals(targetCurrency)) {
          AccountEntriesToConsolidate.Insert(hash, entry);
        } else if (AccountEntriesToConsolidate.ContainsKey(hash)) {
          AccountEntriesToConsolidate[hash].Sum(entry);
        } else {
          entry.Currency = targetCurrency;
          AccountEntriesToConsolidate.Insert(hash, entry);
        }
      }

      return AccountEntriesToConsolidate.Values.ToFixedList();
    }


    private List<TrialBalanceEntry> OrderingParentAndPostingAccountEntries(List<TrialBalanceEntry> entries) {

      if (_command.WithSubledgerAccount) {

        return entries.OrderBy(a => a.Ledger.Number)
                      .ThenBy(a => a.Currency.Code)
                      .ThenByDescending(a => a.Account.DebtorCreditor)
                      .ThenBy(a => a.Account.Number)
                      .ThenBy(a => a.Sector.Code)
                      .ThenBy(a => a.SubledgerNumberOfDigits)
                      .ThenBy(a => a.SubledgerAccountNumber)
                      .ToList();
      } else {
        return entries.OrderBy(a => a.Ledger.Number)
                      .ThenBy(a => a.Currency.Code)
                      .ThenByDescending(a => a.Account.DebtorCreditor)
                      .ThenBy(a => a.Account.Number)
                      .ThenBy(a => a.Sector.Code)
                      .ThenBy(a => a.SubledgerAccountNumber)
                      .ToList();
      }
    }


    private void SetSubledgerAccountInfo(List<TrialBalanceEntry> entries) {
      if (!_command.WithSubledgerAccount) {
        return;
      }

      foreach (var entry in entries) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        if (subledgerAccount.IsEmptyInstance) {
          continue;
        }
        entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ?
                                        subledgerAccount.Number : "";
        entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Length;
      }
    }


    private void SumAccountEntriesToTotalGroup(EmpiriaHashTable<TrialBalanceEntry> totalGroupEntries,
                                       TrialBalanceEntry accountEntry) {

      TrialBalanceEntry totalGroupEntry = accountEntry.CreatePartialCopy();

      totalGroupEntry.GroupName = $"TOTAL GRUPO {accountEntry.Account.GroupNumber}";
      totalGroupEntry.GroupNumber = accountEntry.Account.GroupNumber;
      totalGroupEntry.DebtorCreditor = accountEntry.Account.DebtorCreditor;
      totalGroupEntry.Account = StandardAccount.Empty;
      totalGroupEntry.Sector = Sector.Empty;

      string hash = $"{totalGroupEntry.DebtorCreditor}||{totalGroupEntry.Currency.Id}||" +
                    $"{totalGroupEntry.GroupNumber}";

      if ((_command.WithSubledgerAccount && _command.ShowCascadeBalances) ||
             _command.ShowCascadeBalances) {

        hash = $"{accountEntry.Ledger.Id}||{totalGroupEntry.DebtorCreditor}||" +
               $"{totalGroupEntry.Currency.Id}||{totalGroupEntry.GroupNumber}";
      }

      var trialBalanceHelper = new TrialBalanceHelper(_command);

      var debtorCreditor = accountEntry.DebtorCreditor == DebtorCreditorType.Deudora ?
                           TrialBalanceItemType.BalanceTotalGroupDebtor :
                           TrialBalanceItemType.BalanceTotalGroupCreditor;

      trialBalanceHelper.GenerateOrIncreaseEntries(totalGroupEntries, totalGroupEntry,
                                                   StandardAccount.Empty, Sector.Empty,
                                                   debtorCreditor, hash);
    }


    private bool ValidateEntryForSummaryParentAccount(TrialBalanceEntry entry, 
                                                      out StandardAccount currentParent) {

      if ((entry.Account.NotHasParent) || _command.WithSubledgerAccount) {
        currentParent = entry.Account;

      } else if (_command.DoNotReturnSubledgerAccounts && entry.Account.HasParent) {
        currentParent = entry.Account.GetParent();

      } else if (_command.DoNotReturnSubledgerAccounts && entry.Account.NotHasParent) {
        currentParent = entry.Account;
        return false;

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
      return true;
    }


    private void ValidateSectorizationForSummaryParentEntry(
                  EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                  TrialBalanceEntry entry, StandardAccount currentParent) {

      if (!_command.UseNewSectorizationModel || !_command.WithSectorization) {
        return;
      }

      if (!currentParent.HasParent || !entry.HasSector) {
        return;
      }

      SummaryByAccountEntry(parentAccounts, entry, currentParent,
                            entry.Sector.Parent, TrialBalanceItemType.Summary);
    }


    private void ValuateAccountEntriesToExchangeRate(FixedList<TrialBalanceEntry> entries) {

      if (_command.InitialPeriod.UseDefaultValuation) {
        _command.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
        _command.InitialPeriod.ValuateToCurrrencyUID = "01";
        _command.InitialPeriod.ExchangeRateDate = _command.InitialPeriod.ToDate;
      }

      var exchangeRateType = ExchangeRateType.Parse(_command.InitialPeriod.ExchangeRateTypeUID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType,
                                                                   _command.InitialPeriod.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Code != "01")) {

        var exchangeRate = exchangeRates.FirstOrDefault(
                            a => a.FromCurrency.Code == _command.InitialPeriod.ValuateToCurrrencyUID &&
                            a.ToCurrency.Code == entry.Currency.Code);

        Assertion.AssertObject(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                             $"moneda {entry.Currency.FullName} con la fecha proporcionada.");

        entry.MultiplyBy(exchangeRate.Value);
      }
    }


    #endregion Private methods


  } // class BalanzaTradicionalHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
