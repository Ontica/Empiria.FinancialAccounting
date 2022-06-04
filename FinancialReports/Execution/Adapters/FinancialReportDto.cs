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

    public FinancialReportQuery Query {
      get; internal set;
    } = new FinancialReportQuery();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<DynamicFinancialReportEntryDto> Entries {
      get; internal set;
    } = new FixedList<DynamicFinancialReportEntryDto>();


  }  // class FinancialReportDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
