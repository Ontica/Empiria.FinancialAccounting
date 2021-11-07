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

using Empiria.FinancialAccounting.Reporting.Exporters.Excel;

namespace Empiria.FinancialAccounting.Reporting.Exporters {

  internal interface IExcelExporter {

    FileReportDto CreateExcelFile();

  }  // interface IExcelExporter



  /// <summary>Provides services used to export financial accounting reports to Excel files.</summary>
  internal class ExcelExporter : IReportExporter {

    public FileReportDto Export(ReportDataDto reportData) {
      Assertion.AssertObject(reportData, "reportData");

      IExcelExporter excelExporter = GetExcelExporter(reportData);

      return excelExporter.CreateExcelFile();
    }


    private IExcelExporter GetExcelExporter(ReportDataDto reportData) {
      ExcelTemplateConfig templateConfig = GetExcelTemplate(reportData);

      switch (reportData.Command.ReportType) {
        case "BalanzaSAT":
          return new BalanzaSatExcelExporter(reportData, templateConfig);

        case "CatalogoSAT":
          return new CatalogoCuentasSatExcelExporter(reportData, templateConfig);

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled reportType '{reportData.Command.ReportType}'.");
      }
    }


    private ExcelTemplateConfig GetExcelTemplate(ReportDataDto reportData) {
      var templateUID = $"OperationalReportTemplate.{reportData.Command.ReportType}";

      return ExcelTemplateConfig.Parse(templateUID);
    }


  }  // class ExcelExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters
