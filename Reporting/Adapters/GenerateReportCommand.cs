/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Command payload                      *
*  Type     : GenerateReportCommand                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Command payload used to generate financial accounting reports.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Command payload used to generate financial accounting reports.</summary>
  public class GenerateReportCommand {

    public string ReportType {
      get; set;
    }

    public FileType ExportTo {
      get; set;
    } = FileType.Xml;


    public string AccountsChartUID {
      get; set;
    }

    public DateTime ToDate {
      get; set;
    }

  } // class GenerateReportCommand

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
