/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : AccountComparerEntry                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for an account comparer.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain {

  /// <summary>Represents an entry for an account comparer.</summary>
  public class AccountComparerEntry {


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    } = TrialBalanceItemType.Entry;


    public int AccountGroupId {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }


    public decimal AccountBalance {
      get; internal set;
    }


    public string TargetAccountNumber {
      get; internal set;
    }


    public string TargetAccountName {
      get; internal set;
    }


    public decimal TargetBalance {
      get; internal set;
    }


    public decimal BalanceDifference {
      get; internal set;
    }


    internal void MergeAccountIntoComparerEntry(int groupId,
                          BalanzaTradicionalEntryDto active) {

      this.AccountGroupId = groupId;
      this.CurrencyCode = active.CurrencyCode;
      this.AccountNumber = active.AccountNumber;
      this.AccountName = active.AccountName;
      this.AccountBalance = (decimal) active.CurrentBalance;

    }


    internal void MergeTargetAccountIntoComparerEntry(int groupId,
                          BalanzaTradicionalEntryDto target) {

      this.AccountGroupId = groupId;
      this.CurrencyCode = target.CurrencyCode;
      this.TargetAccountNumber = target.AccountNumber;
      this.TargetAccountName = target.AccountName;
      this.TargetBalance = (decimal) target.CurrentBalance;

    }


    internal void MergeAccountItemIntoAccountComparerEntry(AccountsListItem item, int groupId) {

      this.AccountGroupId = groupId;
      this.CurrencyCode = item.CurrencyCode;
      this.AccountNumber = item.AccountNumber;
      this.TargetAccountNumber = item.TargetAccountNumber;

    }

    internal void MapBalancesToAccountComparerEntry(BalanzaTradicionalEntryDto activeAccount,
                                                    BalanzaTradicionalEntryDto pasiveAccount) {
      
      this.AccountName = activeAccount.AccountName ?? string.Empty;
      this.AccountBalance = (decimal) activeAccount.CurrentBalance;

      this.TargetAccountName = pasiveAccount.AccountName ?? string.Empty;
      this.TargetBalance = (decimal) pasiveAccount.CurrentBalance;

      this.BalanceDifference = (decimal) ((activeAccount.CurrentBalance) - (pasiveAccount.CurrentBalance));

    }


    internal void BalanceDifferenceByAccount() {
      this.BalanceDifference = this.AccountBalance - this.TargetBalance;
    }


    internal AccountComparerEntry CreatePartialComparer() {
      return new AccountComparerEntry {
        CurrencyCode = this.CurrencyCode,
        AccountGroupId = this.AccountGroupId,
        AccountBalance = this.AccountBalance,
        TargetBalance = this.TargetBalance,
        BalanceDifference = this.BalanceDifference
      };
    }

    internal void Sum(AccountComparerEntry comparerTotal) {
      this.AccountBalance += comparerTotal.AccountBalance;
      this.TargetBalance += comparerTotal.TargetBalance;
      this.BalanceDifference += comparerTotal.BalanceDifference;
    }
  } // class AccountComparerEntry

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain
