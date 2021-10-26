/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : XmlExporter                                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export accounting and balances information to xml.                             *
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

  /// <summary>Main service to export accounting and balances information to xml.</summary>
  public class XmlExporter {

    public FileReportDto Export(OperationalReportDto operationalReport, OperationalReportCommand command) {
      Assertion.AssertObject(operationalReport, "operationalReport");

      var templateUID = ReportTemplate(command);

      var templateConfig = OperationalReportTemplateConfig.Parse(templateUID);

      return GetMappingReportType(operationalReport, command, templateConfig);

    }

    private FileReportDto GetMappingReportType(OperationalReportDto operationalReport, 
                                               OperationalReportCommand command,
                                               OperationalReportTemplateConfig templateConfig) {
      if (true) {

        var creator = new XmlFileCreator(templateConfig);

        XmlFile xmlFile = creator.CreateOperationalReportFile(operationalReport, command);

        return ExcelFileMapper.MapFromXmlFile(xmlFile);

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

  } // class XmlExporter

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
