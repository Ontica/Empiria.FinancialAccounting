/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Interface                            *
*  Type     : IReportBuilder                                License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Common interface for all report data builders.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Common interface for all report data builders.</summary>
  internal interface IReportBuilder {

    ReportDataDto Build(BuildReportCommand command);

  }  // interface IReportBuilder

} // namespace Empiria.FinancialAccounting.Reporting
