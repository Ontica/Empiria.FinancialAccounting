/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportBreakdownDto                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for financial reports breakdown data.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Data transfer object for financial reports breakdown data.</summary>
  public class FinancialReportBreakdownDto {

    public FinancialReportCommand Command {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }

    public FixedList<FinancialReportBreakdownEntryDto> Entries {
      get; internal set;
    }

  }  // class FinancialReportBreakdownDto

}  // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
