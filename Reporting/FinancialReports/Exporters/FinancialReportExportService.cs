﻿/* Empiria Financial *****************************************************************************************
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
using Empiria.Storage;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters {

  /// <summary>Provides services used to export financial reports to files. </summary>
  public class FinancialReportExportService : Service {

    #region Constructors and parsers

    protected FinancialReportExportService() {
      // no-op
    }

    static public FinancialReportExportService ServiceInteractor() {
      return Service.CreateInstance<FinancialReportExportService>();
    }

    #endregion Constructors and parsers

    #region Services

    public FileDto Export(FinancialReportDto financialReport) {
      Assertion.Require(financialReport, "financialReport");

      IFinancialReportBuilder reportBuilder = GetReportBuilder(financialReport.Query);

      return reportBuilder.Build(financialReport);
    }

    #endregion Services

    #region Helpers

    private IFinancialReportBuilder GetReportBuilder(FinancialReportQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var reportType = buildQuery.GetFinancialReportType();

      var exportTo = reportType.GetExportToConfig(buildQuery.ExportTo);

      switch (exportTo.FileType) {
        case "Excel":

          var templateConfig = FileTemplateConfig.Parse(exportTo.TemplateId);

          return new FinancialReportExcelExporter(templateConfig);

        case "CSV":

          return new FinancialReportTextFileExporter();


        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled reportType exportTo '{buildQuery.ExportTo}'.");
      }
    }

    #endregion Helpers

  } // class FinancialReportExportService

} // namespace Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters
