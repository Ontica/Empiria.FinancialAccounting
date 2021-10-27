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
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.Rules.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary>Main service to export accounting and balances information to xml or excel.</summary>
  public class OperationalReportExporter {

    public FileReportDto Export(OperationalReportDto operationalReport, OperationalReportCommand command) {
      Assertion.AssertObject(operationalReport, "operationalReport");

      var templateUID = ReportTemplate(command);

      var templateConfig = OperationalReportTemplateConfig.Parse(templateUID);

      return GetReportType(operationalReport, command, templateConfig);

    }

    private FileReportDto GetReportType(OperationalReportDto operationalReport, 
                                               OperationalReportCommand command,
                                               OperationalReportTemplateConfig templateConfig) {
      if (command.Format == OperationalReportFormat.Xml) {

        var creator = new XmlFileCreator(templateConfig);

        OperationalReportFile xmlFile = creator.CreateOperationalReportFile(operationalReport, command);

        return ExcelFileMapper.MapXml(xmlFile);

      } else if (command.Format == OperationalReportFormat.Excel) {

        throw new NotImplementedException();

      } else {
        throw new NotImplementedException();
      }
      
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
