/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : AccountComparerDto                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return account comparer report data.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return vouchers by account.</summary>
  public class SaldosEncerradosDto {

    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<SaldosEncerradosBaseEntryDto> Entries {
      get; internal set;
    } = new FixedList<SaldosEncerradosBaseEntryDto>();


  } // class VouchersByAccountDto


  public class SaldosEncerradosEntryDto : SaldosEncerradosBaseEntryDto {


    public int StandardAccountId {
      get; internal set;
    }

  } // class LockedUpBalancesEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
