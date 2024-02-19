/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : SaldosPorCuentaHelper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balances by account report.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.Collections;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balances by account report.</summary>
  internal class SaldosPorCuentaHelper {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorCuentaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    internal List<TrialBalanceEntry> GetCalculatedParentAccounts(
                                     FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>();

      var detailParentAccount = new List<TrialBalanceEntry>();
      var trialBalanceHelper = new TrialBalanceHelper(_query);

      foreach (var entry in accountEntries) {

        StandardAccount currentParent;

        bool isCalculatedAccount = trialBalanceHelper.ValidateEntryToAssignCurrentParentAccount(
                                                      entry, out currentParent);

        if (!trialBalanceHelper.ValidateEntryForSummaryParentAccount(entry, isCalculatedAccount)) {
          continue;
        }

        GetOrSumParentAccounts(detailParentAccount, parentAccounts, entry, currentParent);

      } // foreach

      trialBalanceHelper.AssignLastChangeDatesToParentEntries(accountEntries, parentAccounts.ToFixedList());

      return detailParentAccount;
    }


    internal List<TrialBalanceEntry> CombineCurrencyTotalsAndPostingEntries(
                                      List<TrialBalanceEntry> accountEntries,
                                      List<TrialBalanceEntry> totalsByCurrency) {

      if (totalsByCurrency.Count == 0) {
        return accountEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var currencyEntry in totalsByCurrency) {
        var listSummaryByCurrency = accountEntries.Where(a => a.Ledger.Equals(currencyEntry.Ledger) &&
                                                              a.Currency.Equals(currencyEntry.Currency))
                                                  .ToList();

        if (listSummaryByCurrency.Count > 0) {
          listSummaryByCurrency.Add(currencyEntry);
          returnedEntries.AddRange(listSummaryByCurrency);
        }
      }
      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineDebtorCreditorAndPostingEntries(
                                      List<TrialBalanceEntry> accountEntries,
                                      List<TrialBalanceEntry> totalsByDebtorOrCreditor) {

      if (totalsByDebtorOrCreditor.Count == 0) {
        return accountEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var debtorSummaryEntry in totalsByDebtorOrCreditor) {
        var debtorsSummaryList = accountEntries.Where(a => a.Ledger.Equals(debtorSummaryEntry.Ledger) &&
                                                           a.Currency.Equals(debtorSummaryEntry.Currency) &&
                                                           a.DebtorCreditor == debtorSummaryEntry.DebtorCreditor)
                                               .ToList();

        if (debtorsSummaryList.Count > 0) {
          debtorsSummaryList.Add(debtorSummaryEntry);
          returnedEntries.AddRange(debtorsSummaryList);
        }
      }

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> AccountEntriesWithSubledgerAccounts(
                                     List<TrialBalanceEntry> accountEntries) {

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>(accountEntries);

      if (!_query.WithSubledgerAccount) {
        returnedEntries = returnedEntries.Where(a => a.SubledgerNumberOfDigits == 0).ToList();
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(
                                      List<TrialBalanceEntry> parentAccounts,
                                      FixedList<TrialBalanceEntry> accountEntries) {

      if (parentAccounts.Count == 0) {
        return accountEntries.ToList();
      }

      var returnedAccountEntries = new List<TrialBalanceEntry>(accountEntries);

      foreach (var entry in parentAccounts.Where(a => a.SubledgerAccountIdParent > 0)) {
        returnedAccountEntries.Add(entry);
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.SetSubledgerAccountInfoByEntry(returnedAccountEntries);

      List<TrialBalanceEntry> orderingAccountEntries =
         trialBalanceHelper.OrderingParentsAndAccountEntries(returnedAccountEntries);

      return orderingAccountEntries;
    }


    internal List<TrialBalanceEntry> GenerateTotalsByCurrency(
                                     List<TrialBalanceEntry> totalsByDebtorCreditorEntries) {

      if (totalsByDebtorCreditorEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var totalsByCurrency = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var debtorOrCreditorEntry in totalsByDebtorCreditorEntries) {

        SummaryByCurrencyEntries(totalsByCurrency, debtorOrCreditorEntry);
      }

      return totalsByCurrency.ToFixedList().ToList();
    }


    internal TrialBalanceEntry GenerateTotalConsolidated(
                                      List<TrialBalanceEntry> totalsByCurrency) {

      if (totalsByCurrency.Count == 0) {
        return null;
      }

      var totalConsolidated = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var currencyEntry in totalsByCurrency) {

        TrialBalanceEntry entry = currencyEntry.CreatePartialCopy();
        entry.GroupName = "TOTAL CONSOLIDADO GENERAL";
        string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}";

        if ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
             _query.ShowCascadeBalances) {

          hash = $"{entry.GroupName}||{Sector.Empty.Code}";
        }

        var trialBalanceHelper = new TrialBalanceHelper(_query);
        trialBalanceHelper.GenerateOrIncreaseEntries(totalConsolidated, entry,
                                                     StandardAccount.Empty, Sector.Empty,
                                                     TrialBalanceItemType.BalanceTotalConsolidated, hash);
      } // foreach

      return totalConsolidated.Values.FirstOrDefault();
    }


    internal List<TrialBalanceEntry> GenerateTotalsDebtorOrCreditor(
                                      FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var totalsByDebtorOrCredtor = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries.Where(a => !a.HasParentPostingEntry)) {

        SummaryByDebtorCreditorEntries(totalsByDebtorOrCredtor, entry);
      }
      return totalsByDebtorOrCredtor.Values.ToList();
    }


    internal void SummaryByCurrencyEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                          TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = -1 * entry.InitialBalance;
        entry.CurrentBalance = -1 * entry.CurrentBalance;
      }
      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;

      string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      trialBalanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, StandardAccount.Empty,
                         Sector.Empty, TrialBalanceItemType.BalanceTotalCurrency, hash);
    }


    #region Public methods



    #endregion Public methods


    #region Private methods


    private void GetOrSumParentAccounts(List<TrialBalanceEntry> detailParentAccount,
                                        EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                        TrialBalanceEntry entry,
                                        StandardAccount currentParent) {
      int cont = 0;
      while (true) {
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        if (entry.Level > 1) {
          SummaryByAccountEntry(parentAccounts, entry, currentParent,
                          entry.Sector);

          ValidateSectorizationForSummaryAccountEntry(parentAccounts, entry, currentParent);
        }

        cont++;
        if (cont == 1) {
          GetDetailParentAccounts(detailParentAccount, parentAccounts, currentParent, entry);
        }
        if (!currentParent.HasParent && entry.HasSector) {
          GetAccountEntriesWithParentSector(parentAccounts, entry, currentParent);
          break;

        } else if (!currentParent.HasParent) {
          break;

        } else {
          currentParent = currentParent.GetParent();
        }

      } // while
    }


    private void GetAccountEntriesWithParentSector(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                   TrialBalanceEntry entry, StandardAccount currentParent) {
      if (!_query.WithSectorization) {
        SummaryByAccountEntry(summaryEntries, entry, currentParent, Sector.Empty);
      } else {
        var parentSector = entry.Sector.Parent;
        while (true) {
          SummaryByAccountEntry(summaryEntries, entry, currentParent, parentSector);
          if (parentSector.IsRoot) {
            break;
          } else {
            parentSector = parentSector.Parent;
          }
        }
      }
    }


    private void GetDetailParentAccounts(List<TrialBalanceEntry> detailParentAccount,
                                         EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                         StandardAccount currentParent, TrialBalanceEntry entry) {

      TrialBalanceEntry detailsEntry;
      string key = $"{currentParent.Number}||{entry.Sector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      parentAccounts.TryGetValue(key, out detailsEntry);

      if (detailsEntry != null) {
        var existEntry = detailParentAccount.Contains(detailsEntry);

        if (!existEntry) {
          detailParentAccount.Add(detailsEntry);
        }
      }
    }


    private void SummaryByAccountEntry(EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                 TrialBalanceEntry entry,
                                 StandardAccount targetAccount, Sector targetSector) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.GenerateOrIncreaseEntries(parentAccounts, entry, targetAccount,
                                                   targetSector, TrialBalanceItemType.Summary, hash);
    }


    private void SummaryByDebtorCreditorEntries(EmpiriaHashTable<TrialBalanceEntry> totalsByDebtorOrCredtor,
                                                TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;
      TrialBalanceItemType itemType = TrialBalanceItemType.BalanceTotalDebtor;

      if (entry.Account.DebtorCreditor == DebtorCreditorType.Deudora) {
        entry.GroupName = "TOTAL DEUDORAS ";
      }

      if (entry.Account.DebtorCreditor == DebtorCreditorType.Acreedora) {
        entry.GroupName = "TOTAL ACREEDORAS ";
        itemType = TrialBalanceItemType.BalanceTotalCreditor;
      }

      entry.GroupName += entry.Currency.FullName;
      string hash = $"{entry.GroupName}||{entry.Currency.Id}";

      if ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
           _query.ShowCascadeBalances) {

        hash = $"{entry.Ledger.Id}||{entry.Currency.Id}||{entry.GroupName}";
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.GenerateOrIncreaseEntries(totalsByDebtorOrCredtor, entry, StandardAccount.Empty,
                                                   Sector.Empty, itemType, hash);
    }


    private void ValidateSectorizationForSummaryAccountEntry(
                  EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                  TrialBalanceEntry entry, StandardAccount currentParent) {
      if (!_query.UseNewSectorizationModel || !_query.WithSectorization) {
        return;
      }

      if (!currentParent.HasParent || !entry.HasSector) {
        return;
      }

      SummaryByAccountEntry(parentAccounts, entry, currentParent, entry.Sector.Parent);
    }



    #endregion Private methods

  } // class SaldosPorCuentaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
