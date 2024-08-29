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
using Empiria.Storage;

using Empiria.FinancialAccounting.Reporting.Exporters;

using Empiria.FinancialAccounting.Reporting.FiscalReports.Builders;

using Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion;
using Empiria.FinancialAccounting.Reporting.AccountsComparer.Domain;
using Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura;
using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses;
using Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Domain;
using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain;

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

    public FileDto ExportReport(ReportBuilderQuery buildQuery, ReportDataDto reportData) {

      Assertion.Require(buildQuery, nameof(buildQuery));
      Assertion.Require(reportData, nameof(reportData));
      
      IReportExporter exporter = GetReportExporter(buildQuery.ExportTo);

      return exporter.Export(reportData);
    }


    public ReportDataDto GenerateReport(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      IReportBuilder reportBuilder = GetReportBuilder(buildQuery.ReportType);

      return reportBuilder.Build(buildQuery);
    }


    public FixedList<ReportTypeDto> GetReportTypes() {
      FixedList<BaseReportType> list = BaseReportType.GetList();

      list = base.RestrictUserDataAccessTo(list);

      return ReportTypeMapper.Map(list);
    }

    #endregion Services

    #region Helpers

    private IReportBuilder GetReportBuilder(ReportTypes reportType) {
      switch (reportType) {

        case ReportTypes.ActivoFijoDepreciacion:
          return new ActivoFijoDepreciacionBuilder();

        case ReportTypes.ActivoFijoDepreciado:
          return new ActivoFijoDepreciadoBuilder();

        case ReportTypes.BalanzaSAT:
          return new BalanzaSat();

        case ReportTypes.BalanzaDeterminarImpuestos:
          return new BalanzaCalculoImpuestos();

        case ReportTypes.CatalogoCuentasSAT:
          return new CatalogoCuentasSat();

        case ReportTypes.ListadoDePolizas:
          return new ListadoPolizas();

        case ReportTypes.ListadoDePolizasPorCuenta:
          return new ListadoPolizasPorCuenta();

        case ReportTypes.MovimientosPorNumeroDeVerificacion:
          return new MovimientosNumeroVerificacion();

        case ReportTypes.ComparativoDeCuentas:
          return new AccountComparerBuilder();

        case ReportTypes.DerramaSwapsCoberturaConsolidado:
          return new DerramaSwapsCoberturaConsolidadoBuilder();

        case ReportTypes.DerramaSwapsCoberturaDesglosado:
          return new DerramaSwapsCoberturaBuilder();

        case ReportTypes.IntegracionSaldosCapital:
          return new IntegracionSaldosCapitalBuilder();

        case ReportTypes.IntegracionSaldosCapitalInteresesConsolidado:
          return new IntegracionSaldosCapitalInteresesConsolidadoBuilder();

        case ReportTypes.IntegracionSaldosCapitalInteresesDesglosado:
          return new IntegracionSaldosCapitalInteresesBuilder();

        case ReportTypes.ValorizacionEstimacionPreventiva:
          return new ValorizacionPreventivaBuilder();

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
