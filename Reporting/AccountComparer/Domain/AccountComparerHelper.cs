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
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain {

  /// <summary>Helper methods to build account comparer information.</summary>
  internal class AccountComparerHelper {

    private readonly ReportBuilderQuery _query;
    
    internal AccountComparerHelper(ReportBuilderQuery query) {
      Assertion.Require(query, nameof(query));

      _query = query;
    }


    internal List<AccountComparerEntry> GetComparerAccountsLists() {

      var accountComparerList = new List<AccountComparerEntry>();
      var accountGroupsList = AccountsList.GetList<AccountsList>("", "");

      foreach (var accountGroup in accountGroupsList) {

        var group = AccountsList.Parse(accountGroup.UID);
        if (group != null) {

          List<AccountComparerEntry> comparerEntries = MergeItemIntoAccountComparer(
                                                        group.GetItems(), accountGroup.Id);
          accountComparerList.AddRange(comparerEntries);
        }
      }

      return accountComparerList.OrderBy(a => a.ActiveAccount)
                                //.ThenBy(a => a.AccountGroupId)
                                .ToList();
    }


    private List<AccountComparerEntry> MergeItemIntoAccountComparer(
                                        FixedList<AccountsListItem> accountsListItems, int groupId) {

      List<AccountComparerEntry> returnedComparerEntries = new List<AccountComparerEntry>();

      foreach (var item in accountsListItems) {
        AccountComparerEntry entry = new AccountComparerEntry();
        
        if (item.AccountNumber != string.Empty && item.TargetAccountNumber != string.Empty) {
          
          entry.MergeAccountItemIntoAccountComparerEntry(item, groupId);
          returnedComparerEntries.Add(entry);
        }
        
      }

      return returnedComparerEntries;
    }


    internal List<AccountComparerEntry> MergeAccountsIntoAccountComparer(
                                        List<AccountComparerEntry> accountList,
                                        FixedList<ITrialBalanceEntryDto> balanceEntries) {

      if (balanceEntries.Count == 0) {
        return accountList;
      }

      var entries = balanceEntries.Select(a => (BalanzaTradicionalEntryDto) a);

      List<AccountComparerEntry> returnedAccountList = new List<AccountComparerEntry>(accountList);

      foreach (var account in returnedAccountList) {
        var activeAccount = entries.FirstOrDefault(a => a.AccountNumber == account.ActiveAccount &&
                                                        a.CurrencyCode == Currency.MXN.Code);
        var pasiveAccount = entries.FirstOrDefault(a => a.AccountNumber == account.PasiveAccount &&
                                                        a.CurrencyCode == Currency.MXN.Code);

        if (activeAccount != null && pasiveAccount != null) {
          account.MapBalancesToAccountComparerEntry(activeAccount, pasiveAccount);
        }
      }

      return returnedAccountList;
    }


    #region Public methods

    #endregion Public methods

  } // class AccountComparerHelper

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain
