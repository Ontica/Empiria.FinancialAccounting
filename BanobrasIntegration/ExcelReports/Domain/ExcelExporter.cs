/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : ExcelExporter                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export accounting information to Microsoft Excel.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.Rules.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class ExcelExporter {

    public FileReportDto Export(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      var templateUID = $"TrialBalanceTemplate.{trialBalance.Command.TrialBalanceType}";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new TrialBalanceExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(trialBalance);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountsChartDto accountsChart,
                                AccountsSearchCommand searchCommand) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(searchCommand, "searchCommand");

      var templateUID = "AccountsChartTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new AccountsChartExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(accountsChart);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<GroupingRulesTreeItemDto> rulesTreeItems) {
      Assertion.AssertObject(rulesTreeItems, "rulesTreeItems");

      var templateUID = $"GroupingRulesReportTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new GroupingRulesReportExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(rulesTreeItems);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FinancialReportDto financialReport) {
      Assertion.AssertObject(financialReport, "financialReport");

      var templateId = financialReport.Command.GetFinancialReportType().TemplateFileId;

      var templateConfig = ExcelTemplateConfig.Parse(templateId);

      var creator = new FinancialReportExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(financialReport);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(StoredBalanceSetDto balanceSet) {
      Assertion.AssertObject(balanceSet, "balanceSet");

      var templateUID = "BalanceSetTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new StoredBalanceSetExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(balanceSet);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto ExportToExcel(OperationalReportDto reportDto,
                                       OperationalReportCommand command) {
      var templateUID = $"OperationalReportTemplate.{command.ReportType}";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new OperationalReportExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(reportDto);

      return excelFile.ToFileReportDto();
    }

  }  // class ExcelExporter

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
