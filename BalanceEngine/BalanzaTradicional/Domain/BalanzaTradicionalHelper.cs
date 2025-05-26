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
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Helper methods to build traditional trial balances.</summary>
  internal class BalanzaTradicionalHelper {

    private readonly TrialBalanceQuery _query;

    internal BalanzaTradicionalHelper(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods


    internal List<TrialBalanceEntry> GenerateTotalByCurrency(
                                      List<TrialBalanceEntry> totalDebtorCreditorEntries) {

      if (totalDebtorCreditorEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var totalCurrenciesEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var debtorCreditorEntry in totalDebtorCreditorEntries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                     a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        SummaryByCurrencyEntries(totalCurrenciesEntries, debtorCreditorEntry);
      }

      return totalCurrenciesEntries.Values.ToList();
    }


    internal TrialBalanceEntry TryGenerateTotalConsolidated(
                                      List<TrialBalanceEntry> totalByCurrencyEntries) {
      Assertion.Require(totalByCurrencyEntries, nameof(totalByCurrencyEntries));

      if (totalByCurrencyEntries.Count == 0) {
        return null;
      }

      var totalConsolidated = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var totalByCurrency in totalByCurrencyEntries) {

        TrialBalanceEntry entry = totalByCurrency.CreatePartialCopy();
        entry.GroupName = "TOTAL CONSOLIDADO GENERAL";
        string hash = $"{entry.GroupName}";

        var trialBalanceHelper = new TrialBalanceHelper(_query);
        trialBalanceHelper.GenerateOrIncreaseEntries(totalConsolidated, entry, StandardAccount.Empty,
                            Sector.Empty, TrialBalanceItemType.BalanceTotalConsolidated, hash);
      } // foreach

      return totalConsolidated.Values.First();
    }


    internal List<TrialBalanceEntry> GenerateTotalsConsolidatedByLedger(
                                      List<TrialBalanceEntry> totalsByCurrency) {

      if (totalsByCurrency.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var totalsConsolidatedByLedger = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var currencyEntry in totalsByCurrency) {

        TrialBalanceEntry entry = currencyEntry.CreatePartialCopy();

        entry.GroupName = $"TOTAL CONSOLIDADO {entry.Ledger.FullName}";
        entry.Currency = Currency.Empty;
        string hash = $"{entry.Ledger.Id}||{entry.GroupName}||{Sector.Empty.Code}";

        var trialBalanceHelper = new TrialBalanceHelper(_query);

        trialBalanceHelper.GenerateOrIncreaseEntries(totalsConsolidatedByLedger, entry,
                            StandardAccount.Empty, Sector.Empty,
                            TrialBalanceItemType.BalanceTotalConsolidatedByLedger, hash);
      }
      return totalsConsolidatedByLedger.Values.OrderBy(a => a.Ledger.Number).ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalDebtorCreditorsByCurrency(
                                     List<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries.Where(a => !a.HasParentPostingEntry)) {

        SummaryByDebtorCreditorAccountEntry(totalSummaryDebtorCredtor, entry,
                                       StandardAccount.Empty, Sector.Empty);
      }
      return totalSummaryDebtorCredtor.Values.ToList();
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalGroupEntries(
                                          FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<TrialBalanceEntry>();
      }

      var toReturnTotalGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries.Where(a => !a.HasParentPostingEntry)) {
        SumAccountEntriesToTotalGroup(toReturnTotalGroupEntries, entry);
      }

      return toReturnTotalGroupEntries.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetCalculatedParentAccounts(
                                          FixedList<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return new FixedList<TrialBalanceEntry>();
      }

      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);
      var trialBalanceHelper = new TrialBalanceHelper(_query);

      foreach (var entry in accountEntries) {

        StandardAccount currentParent;

        bool isCalculatedAccount = trialBalanceHelper.ValidateEntryToAssignCurrentParentAccount(
                                                      entry, out currentParent);

        if (!trialBalanceHelper.ValidateEntryForSummaryParentAccount(entry, isCalculatedAccount)) {
          continue;
        }

        GenerateOrIncreaseParentAccounts(parentAccounts, entry, currentParent);

      } // foreach

      trialBalanceHelper.AssignLastChangeDatesToParentEntries(accountEntries, parentAccounts.ToFixedList());

      return parentAccounts.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetPostingEntries() {

      FixedList<TrialBalanceEntry> accountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      if (_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation) {
        trialBalanceHelper.ValuateAccountEntriesToExchangeRate(accountEntries);

        if (_query.ConsolidateBalancesToTargetCurrency) {
          accountEntries = ConsolidateAccountEntriesToTargetCurrency(accountEntries);
        }
      }

      trialBalanceHelper.RoundDecimals(accountEntries);

      return accountEntries;
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

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.GenerateOrIncreaseEntries(totalsByCurrency, entry,
                                                   StandardAccount.Empty, Sector.Empty,
                                                   TrialBalanceItemType.BalanceTotalCurrency, hash);
    }


    #endregion Public methods


    #region Private methods


    private FixedList<TrialBalanceEntry> ConsolidateAccountEntriesToTargetCurrency(
                                          FixedList<TrialBalanceEntry> trialBalance) {

      var targetCurrency = Currency.Parse(_query.InitialPeriod.ValuateToCurrrencyUID);
      var AccountEntriesToConsolidate = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = GetHashCodeToConsolidateAccountEntries(targetCurrency, entry);

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

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      while (true) {
        
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        if (entry.Level > 1) {
          trialBalanceHelper.SummaryByAccountEntry(parentAccounts, entry, currentParent, entry.Sector);

          trialBalanceHelper.ValidateSectorizationForSummaryParentEntry(
                             parentAccounts, entry, currentParent);
        }

        if (!currentParent.HasParent && entry.HasSector) {
          trialBalanceHelper.GetAccountEntriesAndParentSector(parentAccounts, entry, currentParent);
          break;

        } else if (!currentParent.HasParent) {
          break;

        } else {

          currentParent = currentParent.GetParent();
        }

      } // while
    }


    private string GetHashCodeToConsolidateAccountEntries(Currency targetCurrency, TrialBalanceEntry entry) {

      if (_query.WithSubledgerAccount) {
        return $"{entry.Account.Number}||{entry.SubledgerAccountId}||" +
               $"{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}";

      }

      return $"{entry.Account.Number}||{entry.Sector.Code}||" +
             $"{targetCurrency.Id}||{entry.Ledger.Id}";
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

      if ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
             _query.ShowCascadeBalances) {

        hash = $"{accountEntry.Ledger.Id}||{totalGroupEntry.DebtorCreditor}||" +
               $"{totalGroupEntry.Currency.Id}||{totalGroupEntry.GroupNumber}";
      }

      TrialBalanceItemType itemType = TrialBalanceItemType.BalanceTotalGroupDebtor;

      if (accountEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupCreditor;
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.GenerateOrIncreaseEntries(totalGroupEntries, totalGroupEntry,
                                                   StandardAccount.Empty, Sector.Empty,
                                                   itemType, hash);
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
      if ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
           _query.ShowCascadeBalances) {

        hash = $"{entry.Ledger.Id}||{entry.Currency.Id}||{entry.GroupName}";
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount,
                                                   targetSector, itemType, hash);
    }


    #endregion Private methods


  } // class BalanzaTradicionalHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
