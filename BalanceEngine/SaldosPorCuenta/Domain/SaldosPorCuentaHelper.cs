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

    internal FixedList<TrialBalanceEntry> GetAccountEntries() {

      FixedList<TrialBalanceEntry> accountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      if (_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation) {
        trialBalanceHelper.ValuateAccountEntriesToExchangeRate(accountEntries);

        if (_query.ConsolidateBalancesToTargetCurrency) {

          accountEntries = trialBalanceHelper.ConsolidateToTargetCurrency(
                                              accountEntries, _query.InitialPeriod);
        }
      }
      trialBalanceHelper.RoundDecimals(accountEntries);
      return accountEntries;
    }


    internal List<TrialBalanceEntry> GetCalculatedParentAccounts(
                                     FixedList<TrialBalanceEntry> accountEntries) {

      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);

      var detailParentAccount = new List<TrialBalanceEntry>();
      var trialBalanceHelper = new TrialBalanceHelper(_query);

      foreach (var entry in accountEntries) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountNumber = SubledgerAccount.Parse(entry.SubledgerAccountId).Number ?? "";

        StandardAccount currentParent;

        bool isCalculatedAccount = trialBalanceHelper.ValidateEntryForSummaryParentAccount(
                                                      entry, out currentParent);

        if (!isCalculatedAccount) {
          continue;
        }

        GenOrSumParentAccounts(detailParentAccount, parentAccounts, entry, currentParent);
        
      } // foreach

      trialBalanceHelper.AssignLastChangeDatesToSummaryEntries(accountEntries, parentAccounts);

      return detailParentAccount;
    }


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(
                                      List<TrialBalanceEntry> parentAccounts,
                                      FixedList<TrialBalanceEntry> accountEntries) {
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


    


    #region Public methods



    #endregion Public methods


    #region Private methods


    private void GenOrSumParentAccounts(List<TrialBalanceEntry> detailParentAccount,
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


    private void GetDetailParentAccounts(List<TrialBalanceEntry> detailSummaryEntries,
                                         EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                         StandardAccount currentParent, TrialBalanceEntry entry) {

      TrialBalanceEntry detailsEntry;
      string key = $"{currentParent.Number}||{entry.Sector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      summaryEntries.TryGetValue(key, out detailsEntry);

      if (detailsEntry != null) {
        var existEntry = detailSummaryEntries.Contains(detailsEntry);

        if (!existEntry) {
          detailSummaryEntries.Add(detailsEntry);
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
