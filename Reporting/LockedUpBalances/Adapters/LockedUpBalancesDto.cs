/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : AccountComparerDto                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return account comparer report data.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Reporting.LockedUpBalances.Adapters {


  /// <summary>Output DTO used to return vouchers by account.</summary>
  public class LockedUpBalancesDto {

    //public BalanceExplorerQuery Query {
    //  get; internal set;
    //} = new BalanceExplorerQuery();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<LockedUpBalancesEntryDto> Entries {
      get; internal set;
    } = new FixedList<LockedUpBalancesEntryDto>();


    public string Title {
      get; internal set;
    }

  } // class VouchersByAccountDto

  public class LockedUpBalancesEntryDto {


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }


    public int StandardAccountId {
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


    public string SectorCode {
      get; internal set;
    }


    public decimal CurrentBalance {
      get; internal set;
    }


    public DateTime LastChangeDate {
      get; set;
    }


    public DateTime RoleChangeDate {
      get; internal set;
    }


    public string Role {
      get; internal set;
    }


    public string NewRole {
      get; internal set;
    }
  } // class LockedUpBalancesEntryDto

} // namespace Empiria.FinancialAccounting.Reporting.Balances.LockedUpBalances.Adapters
