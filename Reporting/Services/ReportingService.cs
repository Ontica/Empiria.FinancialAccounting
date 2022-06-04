/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : ReportingService                              License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to generate financial accounting reports.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Reporting.Builders;

using Empiria.FinancialAccounting.Reporting.Exporters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services used to generate financial accounting reports.</summary>
  public class ReportingService : Service {

    #region Constructors and parsers

    private ReportingService() {
      // no-op
    }

    static public ReportingService ServiceInteractor() {
      return Service.CreateInstance<ReportingService>();
    }

    #endregion Constructors and parsers

    #region Services

    public FileReportDto ExportReport(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      ReportDataDto reportData = GenerateReport(buildQuery);

      IReportExporter exporter = GetReportExporter(buildQuery.ExportTo);

      return exporter.Export(reportData);
    }


    public ReportDataDto GenerateReport(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      IReportBuilder reportBuilder = GetReportBuilder(buildQuery.ReportType);

      return reportBuilder.Build(buildQuery);
    }


    public FixedList<ReportTypeDto> GetReportTypes() {
      FixedList<ReportType> list = ReportType.GetList();

      return ReportTypeMapper.Map(list);
    }

    #endregion Services

    #region Helpers

    private IReportBuilder GetReportBuilder(string reportType) {
      switch (reportType) {
        case "BalanzaSAT":
          return new BalanzaSat();

        case "CatalogoCuentasSAT":
          return new CatalogoCuentasSat();

        case "BalanzaDeterminarImpuestos":
          return new BalanzaCalculoImpuestos();

        case "ListadoDePolizas":
          return new ListadoPolizas();

        case "ListadoDePolizasPorCuenta":
          return new ListadoPolizasPorCuenta();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled reportType '{reportType}'.");
      }
    }


    private IReportExporter GetReportExporter(FileType fileType) {
      switch (fileType) {
        case FileType.Excel:
          return new ExcelExporter();

        case FileType.Xml:
          return new XmlExporter();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled exportTo file type '{fileType}'.");
      }
    }


    #endregion Helpers

  } // class ReportingService

} // namespace Empiria.FinancialAccounting.Reporting
