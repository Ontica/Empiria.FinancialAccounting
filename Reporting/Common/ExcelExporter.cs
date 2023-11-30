/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : File Exportation Services            *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : ExcelExporter                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to export financial accounting reports to Excel files.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

using Empiria.FinancialAccounting.Reporting.AccountComparer.Exporters;
using Empiria.FinancialAccounting.Reporting.FiscalReports.Exporters;
using Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Exporters;

namespace Empiria.FinancialAccounting.Reporting {

  internal interface IExcelExporter {

    FileReportDto CreateExcelFile();

  }  // interface IExcelExporter



  /// <summary>Provides services used to export financial accounting reports to Excel files.</summary>
  internal class ExcelExporter : IReportExporter {

    public FileReportDto Export(ReportDataDto reportData) {
      Assertion.Require(reportData, "reportData");

      IExcelExporter excelExporter = GetExcelExporter(reportData);

      return excelExporter.CreateExcelFile();
    }


    private IExcelExporter GetExcelExporter(ReportDataDto reportData) {
      FileTemplateConfig templateConfig = GetExcelTemplate(reportData);

      switch (reportData.Query.ReportType) {

        case ReportTypes.ActivoFijoDepreciacion:
        case ReportTypes.ActivoFijoDepreciado:
          return new ActivoFijoDepreciacionExcelExporter(reportData, templateConfig);

        case ReportTypes.BalanzaSAT:
          return new BalanzaSatExcelExporter(reportData, templateConfig);

        case ReportTypes.BalanzaDeterminarImpuestos:
          return new BalanzaCalculoImpuestosExcelExporter(reportData, templateConfig);

        case ReportTypes.CatalogoCuentasSAT:
          return new CatalogoCuentasSatExcelExporter(reportData, templateConfig);

        case ReportTypes.ListadoDePolizas:
          return new ListadoPolizasExcelExporter(reportData, templateConfig);

        case ReportTypes.ListadoDePolizasPorCuenta:
          return new ListadoPolizasPorCuentaExcelExporter(reportData, templateConfig);

        case ReportTypes.DerramaSwapsCoberturaConsolidado:
          return new DerramaSwapsCoberturaConsolidadoExcelExporter(reportData, templateConfig);

        case ReportTypes.DerramaSwapsCoberturaDesglosado:
          return new DerramaSwapsCoberturaExcelExporter(reportData, templateConfig);

        case ReportTypes.IntegracionSaldosCapital:
          return new IntegracionSaldosCapitalExcelExporter(reportData, templateConfig);

        case ReportTypes.IntegracionSaldosCapitalInteresesConsolidado:
          return new IntegracionSaldosCapitalInteresesConsolidadoExcelExporter(reportData, templateConfig);

        case ReportTypes.IntegracionSaldosCapitalInteresesDesglosado:
          return new IntegracionSaldosCapitalInteresesDesglosadoExcelExporter(reportData, templateConfig);


        case ReportTypes.ComparativoDeCuentas:
          return new AccountComparerExcelExporter(reportData, templateConfig);

        case ReportTypes.ValorizacionEstimacionPreventiva:
          return new ValorizacionPreventivaExcelExporter(reportData, templateConfig);

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled reportType '{reportData.Query.ReportType}'.");
      }
    }


    private FileTemplateConfig GetExcelTemplate(ReportDataDto reportData) {
      var templateUID = $"OperationalReportTemplate.{reportData.Query.ReportType}";

      return FileTemplateConfig.Parse(templateUID);
    }


  }  // class ExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters
