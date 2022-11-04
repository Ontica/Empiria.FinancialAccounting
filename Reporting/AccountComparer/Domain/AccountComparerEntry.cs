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
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain {

  /// <summary>Represents an entry for an account comparer.</summary>
  public class AccountComparerEntry {


    public int AccountGroupId {
      get; internal set;
    }


    public string ActiveAccount {
      get; internal set;
    }


    public decimal ActiveBalance {
      get; internal set;
    }


    public string PasiveAccount {
      get; internal set;
    }


    public decimal PasiveBalance {
      get; internal set;
    }


    public decimal BalanceDifference {
      get; internal set;
    }


    internal void MergeAccountItemIntoAccountComparerEntry(AccountsListItem item, int groupId) {

      this.AccountGroupId = groupId;
      this.ActiveAccount = item.AccountNumber;
      this.PasiveAccount = item.TargetAccountNumber;

    }

    internal void MapBalancesToAccountComparerEntry(BalanzaTradicionalEntryDto activeAccount,
                                                    BalanzaTradicionalEntryDto pasiveAccount) {

      this.ActiveBalance = (decimal) activeAccount.CurrentBalance;
      this.PasiveBalance = (decimal) pasiveAccount.CurrentBalance;
      this.BalanceDifference = (decimal) ((activeAccount.CurrentBalance) - (pasiveAccount.CurrentBalance));

    }
  } // class AccountComparerEntry

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain
