/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : SaldosEncerradosBaseEntryDto                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return account comparer report data.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return account comparer report data.</summary>
  public class SaldosEncerradosBaseEntryDto {


    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public string ItemName {
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


    public string RoleChange {
      get; internal set;
    }


    public DateTime RoleChangeDate {
      get; internal set;
    }


    public bool CanGenerateVoucher {
      get; internal set;
    } = false;


    internal SaldosEncerradosBaseEntryDto CreateGroupEntry() {
      return new SaldosEncerradosBaseEntryDto {
        LedgerUID = this.LedgerUID,
        LedgerNumber = this.LedgerNumber,
        ItemName = $"({this.LedgerNumber}) {this.LedgerName}".ToUpper(),
        ItemType = TrialBalanceItemType.Group,
        CanGenerateVoucher = true,
        RoleChangeDate = this.RoleChangeDate
      };

    }


  } // class SaldosEncerradosBaseEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters

