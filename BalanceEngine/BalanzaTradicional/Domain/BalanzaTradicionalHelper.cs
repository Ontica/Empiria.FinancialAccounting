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


    internal List<TrialBalanceEntry> CombineTotalsByCurrencyAndAccountEntries(
                                      List<TrialBalanceEntry> balanceEntries,
                                      List<TrialBalanceEntry> totalsByCurrency) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var currencyEntry in totalsByCurrency) {

        var entriesByCurrency = balanceEntries.Where(a => a.Ledger.Id == currencyEntry.Ledger.Id &&
                                                     a.Currency.Code == currencyEntry.Currency.Code)
                                              .ToList();
        if (entriesByCurrency.Count > 0) {
          entriesByCurrency.Add(currencyEntry);
          returnedEntries.AddRange(entriesByCurrency);
        }
      }
      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineTotalConsolidatedByLedgerAndAccountEntries(
                                      List<TrialBalanceEntry> balanceEntries,
                                      List<TrialBalanceEntry> totalConsolidatedByLedger) {
      if (totalConsolidatedByLedger.Count == 0) {
        return balanceEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalByLedger in totalConsolidatedByLedger) {
        var entries = balanceEntries.Where(a => a.Ledger.Id == totalByLedger.Ledger.Id)
                                     .ToList();

        if (entries.Count > 0) {
          entries.Add(totalByLedger);
          returnedEntries.AddRange(entries);

        }
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineTotalDebtorCreditorsByCurrencyAndAccountEntries(
                                     List<TrialBalanceEntry> balanceEntries,
                                     List<TrialBalanceEntry> totalDebtorCreditors) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var debtorCreditorEntry in totalDebtorCreditors) {

        var entries = balanceEntries.Where(a => a.Ledger.Id == debtorCreditorEntry.Ledger.Id &&
                                           a.Currency.Code == debtorCreditorEntry.Currency.Code &&
                                           a.DebtorCreditor == debtorCreditorEntry.DebtorCreditor).ToList();
        if (entries.Count > 0) {
          entries.Add(debtorCreditorEntry);
          returnedEntries.AddRange(entries);
        }
      }

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
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


    internal List<TrialBalanceEntry> GenerateTotalByCurrency(
                                      List<TrialBalanceEntry> totalDebtorCreditorEntries) {

      var totalCurrenciesEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var debtorCreditorEntry in totalDebtorCreditorEntries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                     a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        SummaryByCurrencyEntries(totalCurrenciesEntries, debtorCreditorEntry);
      }

      return totalCurrenciesEntries.Values.ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalsConsolidatedByLedger(
                                      List<TrialBalanceEntry> totalsByCurrency) {

      var totalsConsolidatedByLedger = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var currencyEntry in totalsByCurrency) {

        TrialBalanceEntry entry = currencyEntry.CreatePartialCopy();

        entry.GroupName = $"TOTAL CONSOLIDADO {entry.Ledger.FullName}";
        entry.Currency = Currency.Empty;
        string hash = $"{entry.Ledger.Id}||{entry.GroupName}||{Sector.Empty.Code}";

        var trialBalanceHelper = new TrialBalanceHelper(_command);

        trialBalanceHelper.GenerateOrIncreaseEntries(totalsConsolidatedByLedger, entry,
                            StandardAccount.Empty, Sector.Empty,
                            TrialBalanceItemType.BalanceTotalConsolidatedByLedger, hash);
      }
      return totalsConsolidatedByLedger.Values.OrderBy(a => a.Ledger.Number).ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalDebtorCreditorsByCurrency(
                                     List<TrialBalanceEntry> postingEntries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in postingEntries.Where(a => !a.HasParentPostingEntry)) {

        SummaryByDebtorCreditorAccountEntry(totalSummaryDebtorCredtor, entry,
                                       StandardAccount.Empty, Sector.Empty);
      }
      return totalSummaryDebtorCredtor.Values.ToList();
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalGroupEntries(FixedList<TrialBalanceEntry> accountEntries) {

      var toReturnTotalGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries.Where(a => !a.HasParentPostingEntry)) {
        SumAccountEntriesToTotalGroup(toReturnTotalGroupEntries, entry);
      }

      return toReturnTotalGroupEntries.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetCalculatedParentAccounts(
                                     FixedList<TrialBalanceEntry> accountEntries) {

      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);

      foreach (var entry in accountEntries) {

        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountNumber = SubledgerAccount.Parse(entry.SubledgerAccountId).Number ?? "";

        StandardAccount currentParent;

        bool isCalculatedAccount = ValidateEntryForSummaryParentAccount(entry, out currentParent);

        if (!isCalculatedAccount) {
          continue;
        }

        GenerateOrIncreaseParentAccounts(parentAccounts, entry, currentParent);

      } // foreach

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      trialBalanceHelper.AssignLastChangeDatesToSummaryEntries(accountEntries, parentAccounts);

      return parentAccounts.ToFixedList();
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


    private void GenerateOrIncreaseParentAccounts(EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                                  TrialBalanceEntry entry, StandardAccount currentParent) {

      var trialBalanceHelper = new TrialBalanceHelper(_command);

      while (true) {
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        if (entry.Level > 1) {
          SummaryByAccountEntry(parentAccounts, entry, currentParent, entry.Sector);

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

      var debtorCreditor = accountEntry.DebtorCreditor == DebtorCreditorType.Deudora ?
                           TrialBalanceItemType.BalanceTotalGroupDebtor :
                           TrialBalanceItemType.BalanceTotalGroupCreditor;

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      trialBalanceHelper.GenerateOrIncreaseEntries(totalGroupEntries, totalGroupEntry,
                                                   StandardAccount.Empty, Sector.Empty,
                                                   debtorCreditor, hash);
    }


    private void SummaryByAccountEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                       TrialBalanceEntry entry, StandardAccount targetAccount,
                                       Sector targetSector) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      trialBalanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount,
                                                   targetSector, TrialBalanceItemType.Summary, hash);
    }


    internal void SummaryByCurrencyEntries(EmpiriaHashTable<TrialBalanceEntry> totalsByCurrency,
                                           TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = -1 * entry.InitialBalance;
        entry.CurrentBalance = -1 * entry.CurrentBalance;
      }
      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;
      string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      trialBalanceHelper.GenerateOrIncreaseEntries(totalsByCurrency, entry,
                                                   StandardAccount.Empty, Sector.Empty,
                                                   TrialBalanceItemType.BalanceTotalCurrency, hash);
    }


    private void SummaryByDebtorCreditorAccountEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                     TrialBalanceEntry balanceEntry,
                                                     StandardAccount targetAccount, Sector targetSector) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;

      var itemType = new TrialBalanceItemType();

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Deudora) {
        entry.GroupName = "TOTAL DEUDORAS " + entry.Currency.FullName;
        itemType = TrialBalanceItemType.BalanceTotalDebtor;
      }

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {
        entry.GroupName = "TOTAL ACREEDORAS " + entry.Currency.FullName;
        itemType = TrialBalanceItemType.BalanceTotalCreditor;
      }

      string hash = $"{entry.GroupName}||{entry.Currency.Id}";
      if ((_command.WithSubledgerAccount && _command.ShowCascadeBalances) ||
           _command.ShowCascadeBalances) {

        hash = $"{entry.Ledger.Id}||{entry.Currency.Id}||{entry.GroupName}";
      }

      var trialBalanceHelper = new TrialBalanceHelper(_command);
      trialBalanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount,
                                                   targetSector, itemType, hash);
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
        throw Assertion.EnsureNoReachThisCode();
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

      SummaryByAccountEntry(parentAccounts, entry, currentParent, entry.Sector.Parent);
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

        // ToDo: URGENT This require must be checked before any state change
        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} con la fecha proporcionada.");

        entry.MultiplyBy(exchangeRate.Value);
      }
    }


    #endregion Private methods


  } // class BalanzaTradicionalHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
