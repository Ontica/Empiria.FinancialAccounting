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

    public FileReportDto ExportReport(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      ReportDataDto reportData = GenerateReport(command);

      IReportExporter exporter = GetReportExporter(command);

      return exporter.Export(reportData);
    }


    public ReportDataDto GenerateReport(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      IReportBuilder reportBuilder = GetReportBuilder(command);

      return reportBuilder.Build(command);
    }


    public FixedList<ReportTypeDto> GetReportTypes() {
      FixedList<ReportType> list = ReportType.GetList();

      return ReportTypeMapper.Map(list);
    }

    #endregion Services

    #region Helpers

    private IReportBuilder GetReportBuilder(BuildReportCommand command) {
      switch (command.ReportType) {
        case "BalanzaSAT":
          return new BalanzaSat();

        case "CatalogoSAT":
          return new CatalogoCuentasSat();

        case "BalanzaDeterminarImpuestos":
          return new BalanzaCalculoImpuestos();

        case "Polizas":
          return new PolizasActualizadas();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled reportType '{command.ReportType}'.");
      }
    }


    private IReportExporter GetReportExporter(BuildReportCommand command) {
      switch (command.ExportTo) {
        case FileType.Excel:
          return new ExcelExporter();

        case FileType.Xml:
          return new XmlExporter();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled exportTo file type '{command.ExportTo}'.");
      }
    }


    #endregion Helpers

  } // class ReportingService

} // namespace Empiria.FinancialAccounting.Reporting
