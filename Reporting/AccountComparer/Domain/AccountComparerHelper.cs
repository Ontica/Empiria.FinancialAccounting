/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : AccountComparerHelper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build account comparer information.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain {

  /// <summary>Helper methods to build account comparer information.</summary>
  internal class AccountComparerHelper {

    private readonly ReportBuilderQuery _query;

    internal AccountComparerHelper(ReportBuilderQuery query) {
      Assertion.Require(query, nameof(query));

      _query = query;
    }


    internal List<AccountComparerEntry> CombineComparerEntriesWithTotals(
                                        List<AccountComparerEntry> comparerEntries,
                                        List<AccountComparerEntry> totalByCurrency) {

      var returnedComparers = new List<AccountComparerEntry>();

      foreach (var total in totalByCurrency) {
        var comparersByCurrency = comparerEntries.FindAll(a=>a.CurrencyCode == total.CurrencyCode);

        returnedComparers.AddRange(comparersByCurrency);
        returnedComparers.Add(total);

      }

      return returnedComparers;
    }


    internal List<AccountComparerEntry> GetTotalByCurrency(List<AccountComparerEntry> comparerEntries) {

      var totalsByCurrency = new EmpiriaHashTable<AccountComparerEntry>();

      foreach (var comparer in comparerEntries) {

        SummaryByCurrency(comparer, totalsByCurrency);

      }

      return totalsByCurrency.Values.ToList();
    }


    

    #region Public methods


    public List<AccountComparerEntry> GetComparerEntries(AccountsList group,
                              IEnumerable<BalanzaTradicionalEntryDto> entries) {

      var returnedComparerEntries = new List<AccountComparerEntry>();

      foreach (var item in group.GetItems<AccountsListItem>()) {

        List<AccountComparerEntry> comparerEntries = new List<AccountComparerEntry>();

        MergeAccountsIntoComparerEntries(group.Id, item, comparerEntries, entries);

        MergeTargetAccountsIntoComparerEntries(group.Id, item, comparerEntries, entries);

        returnedComparerEntries.AddRange(comparerEntries);
      }

      GetDifferenceByComparerEntries(returnedComparerEntries);

      return returnedComparerEntries.OrderBy(a => a.CurrencyCode).ToList();
    }


    #endregion Public methods


    #region Private methods


    private void GenerateOrIncreaseEntry(EmpiriaHashTable<AccountComparerEntry> totalsByCurrency,
                                         AccountComparerEntry comparerTotal) {

      AccountComparerEntry summaryComparer;

      string hash = $"{comparerTotal.CurrencyCode}";

      totalsByCurrency.TryGetValue(hash, out summaryComparer);

      if (summaryComparer == null) {

        summaryComparer = new AccountComparerEntry {
          ItemType = BalanceEngine.TrialBalanceItemType.Total,
          AccountGroupId = comparerTotal.AccountGroupId,
          CurrencyCode = comparerTotal.CurrencyCode,
          AccountName = comparerTotal.AccountName,
          TargetAccountName = comparerTotal.TargetAccountName
        };

        summaryComparer.Sum(comparerTotal);

        totalsByCurrency.Insert(hash, summaryComparer);

      } else {

        summaryComparer.Sum(comparerTotal);

      }
    }


    private void GetDifferenceByComparerEntries(List<AccountComparerEntry> returnedComparerEntries) {

      foreach (var comparer in returnedComparerEntries) {
        comparer.BalanceDifferenceByAccount();
      }

    }


    private void MergeAccountsIntoComparerEntries(int groupId, AccountsListItem item,
                                          List<AccountComparerEntry> comparerEntries,
                                          IEnumerable<BalanzaTradicionalEntryDto> entries) {

      var accounts = entries.Where(a => a.AccountNumber == item.AccountNumber).ToList();

      foreach (var active in accounts) {
        AccountComparerEntry comparerEntry = new AccountComparerEntry();

        var entry = entries.First(a => a.AccountNumber == item.TargetAccountNumber);

        comparerEntry.MergeAccountIntoComparerEntry(groupId, active);
        comparerEntry.TargetAccountNumber = item.TargetAccountNumber;
        comparerEntry.TargetAccountName = entry?.AccountName ?? "";

        comparerEntries.Add(comparerEntry);
      }

    }


    private void MergeTargetAccountsIntoComparerEntries(int groupId, AccountsListItem item,
                                                List<AccountComparerEntry> comparerEntries,
                                                IEnumerable<BalanzaTradicionalEntryDto> entries) {

      var targetAccounts = entries.Where(a => a.AccountNumber == item.TargetAccountNumber).ToList();

      foreach (var target in targetAccounts) {

        var findAccount = comparerEntries.Find(a => a.AccountNumber == item.AccountNumber &&
                                                    item.TargetAccountNumber == target.AccountNumber &&
                                                    a.CurrencyCode == target.CurrencyCode);
        if (findAccount != null) {

          findAccount.TargetAccountNumber = target.AccountNumber;
          findAccount.TargetAccountName = target.AccountName;
          findAccount.TargetBalance = (decimal) target.CurrentBalance;

        } else {

          var newComparer = new AccountComparerEntry();

          var entry = entries.First(a => a.AccountNumber == item.AccountNumber);

          newComparer.MergeTargetAccountIntoComparerEntry(groupId, target);
          newComparer.AccountNumber = item.AccountNumber;
          newComparer.AccountName = entry?.AccountName ?? "";

          comparerEntries.Add(newComparer);
        }
      } // foreach

    }


    private void SummaryByCurrency(AccountComparerEntry comparer,
                                   EmpiriaHashTable<AccountComparerEntry> totalsByCurrency) {

      AccountComparerEntry comparerTotal = comparer.CreatePartialComparer();
      comparerTotal.AccountName = "TOTAL CUENTAS";
      comparerTotal.TargetAccountName = "TOTAL CONTRACUENTAS";

      GenerateOrIncreaseEntry(totalsByCurrency, comparerTotal);

    }


    #endregion Private methods

  } // class AccountComparerHelper

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain
