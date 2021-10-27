/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration                         Component : Operational Reports                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : OperationalReportExporter                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export accounting and balances information to xml or excel.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters;


namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary>Main service to export accounting and balances information to xml or excel.</summary>
  public class OperationalReportExporter {

    public FileReportDto Export(OperationalReportDto operationalReport, OperationalReportCommand command) {
      Assertion.AssertObject(operationalReport, "operationalReport");
      Assertion.AssertObject(command, "command");

      if (command.FileType == FileType.Excel) {

        return ExportToExcel(operationalReport, command);

      } else if (command.FileType == FileType.Xml) {

        return ExportToXmlFile(operationalReport, command);

      } else {
        throw Assertion.AssertNoReachThisCode();

      }
    }


    private FileReportDto ExportToExcel(OperationalReportDto reportDTO,
                                        OperationalReportCommand command) {
      var templateUID = ReportTemplate(command);


      throw new NotImplementedException();

    }


    private FileReportDto ExportToXmlFile(OperationalReportDto reportDTO,
                                          OperationalReportCommand command) {
      var xmlFileCreator = new XmlFileCreator();

      OperationalReportFile xmlFile = xmlFileCreator.CreateOperationalReportFile(reportDTO, command);

      return ExcelFileMapper.MapXml(xmlFile);
    }


    #region Private methods

    private string ReportTemplate(OperationalReportCommand command) {
      if (command.ReportType == OperationalReportType.BalanzaSat) {
        return "TrialBalanceTemplate";
      } else if (command.ReportType == OperationalReportType.CatalogoDeCuentaSat) {
        return "AccountsChartTemplate";
      } else {
        return "";
      }
    }

    #endregion

  } // class OperationalReportExporter

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
