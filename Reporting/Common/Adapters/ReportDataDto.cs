/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : ReportDataDto                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return financial accounting report's data.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.Reporting {

  public interface IReportEntryDto {

  }

  /// <summary>Output DTO used to return financial accounting report's data.</summary>
  public class ReportDataDto {

    [JsonProperty]
    public ReportBuilderQuery Query {
      get; internal set;
    } = new ReportBuilderQuery();


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    [JsonProperty]
    public FixedList<IReportEntryDto> Entries {
      get; internal set;
    } = new FixedList<IReportEntryDto>();

  } // class ReportDataDto

} // Empiria.FinancialAccounting.Reporting
