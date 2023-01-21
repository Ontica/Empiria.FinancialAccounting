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

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return vouchers by account.</summary>
  public class SaldosEncerradosDto {

    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<SaldosEncerradosEntryDto> Entries {
      get; internal set;
    } = new FixedList<SaldosEncerradosEntryDto>();


  } // class VouchersByAccountDto

  public class SaldosEncerradosEntryDto {


    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public int StandardAccountId {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public string LedgerUID {
      get; internal set;
    }


    public string LedgerNumber {
      get; internal set;
    }


    public string LedgerName {
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


    public string SubledgerAccount {
      get; internal set;
    }


    public decimal? LockedBalance {
      get; internal set;
    }


    public DateTime LastChangeDate {
      get; set;
    }


    public DateTime RoleChangeDate {
      get; internal set;
    }


    public string RoleChange {
      get; internal set;
    }


    public string NewRole {
      get; internal set;
    }


    public bool canGenerateVoucher {
      get; internal set;
    } = false;


    internal SaldosEncerradosEntryDto CreateGroupEntry() {
      return new SaldosEncerradosEntryDto {
        LedgerUID = this.LedgerUID,
        LedgerNumber = this.LedgerNumber,
        AccountName = $"({this.LedgerNumber}) {this.LedgerName}".ToUpper(),
        ItemType = TrialBalanceItemType.Group,
        canGenerateVoucher = true,
        RoleChangeDate = this.RoleChangeDate,
        LastChangeDate = ExecutionServer.DateMaxValue,
      };


    }
  } // class LockedUpBalancesEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
