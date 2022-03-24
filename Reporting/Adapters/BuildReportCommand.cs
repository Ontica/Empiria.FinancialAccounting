/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Command payload                      *
*  Type     : BuildReportCommand                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Command payload used to generate financial accounting reports.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Command payload used to generate financial accounting reports.</summary>
  public class BuildReportCommand {

    public string ReportType {
      get; set;
    }

    public FileType ExportTo {
      get; set;
    } = FileType.Xml;


    public string AccountsChartUID {
      get; set;
    }

    public DateTime FromDate {
      get; set;
    } = DateTime.Now;

    public DateTime ToDate {
      get; set;
    }

    public string[] Ledgers {
      get; set;
    } = new string[0];

    public string AccountNumber {
      get; set;
    } = string.Empty;

    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;

    public SendType SendType {
      get; set;
    } = SendType.N;

    public bool WithSubledgerAccount {
      get; set;
    } = false;
  } // class BuildReportCommand

  public enum SendType {

    N,

    C

  }

} // namespace Empiria.FinancialAccounting.Reporting
