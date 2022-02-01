/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : FinancialReportExportService                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to export financial reports to files.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

using Empiria.FinancialAccounting.Reporting.Exporters.Excel;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services used to export financial reports to files. </summary>
  public class FinancialReportExportService : Service {

    #region Constructors and parsers

    private FinancialReportExportService() {
      // no-op
    }

    static public FinancialReportExportService ServiceInteractor() {
      return Service.CreateInstance<FinancialReportExportService>();
    }

    #endregion Constructors and parsers

    #region Services

    public FileReportDto Export(FinancialReportDto financialReport) {
      Assertion.AssertObject(financialReport, "financialReport");

      IFinancialReportBuilder reportBuilder = GetReportBuilder(financialReport.Command);

      return reportBuilder.Build(financialReport);
    }

    #endregion Services

    #region Helpers

    private IFinancialReportBuilder GetReportBuilder(FinancialReportCommand command) {
      Assertion.AssertObject(command, "command");

      var reportType = command.GetFinancialReportType();

      switch (command.ExportTo) {
        case "Excel":

          var templateConfig = FileTemplateConfig.Parse(reportType.TemplateFileId);

          return new FinancialReportExcelExporter(templateConfig);

        case "SITI":

          return new FinancialReportTextFileExporter();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled reportType exportTo '{command.ExportTo}'.");
      }
    }

    #endregion Helpers

  } // class FinancialReportExportService

} // namespace Empiria.FinancialAccounting.Reporting
