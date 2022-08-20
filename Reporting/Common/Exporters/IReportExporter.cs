/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : File Exportation Services            *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Interface                            *
*  Type     : IReportExporter                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Common interface for all report data exporters.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Common interface for all report data exporters.</summary>
  internal interface IReportExporter {
    FileReportDto Export(ReportDataDto reportData);
  }

}  // namespace Empiria.FinancialAccounting.Reporting
