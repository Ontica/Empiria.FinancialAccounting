/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return financial reports.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return financial reports.</summary>
  public class FinancialReportDto {

    public FinancialReportCommand Command {
      get; internal set;
    } = new FinancialReportCommand();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<FinancialReportEntryDto> Entries {
      get; internal set;
    } = new FixedList<FinancialReportEntryDto>();


  }  // class FinancialReportDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
