/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : ExcelExporter                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Main service to export accounting information to Microsoft Excel.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Reporting.Exporters.Excel;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.Rules.Adapters;
using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class ExcelExporterService {

    public FileReportDto Export(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      var templateUID = $"TrialBalanceTemplate.{trialBalance.Command.TrialBalanceType}";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new TrialBalanceExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(trialBalance);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountsChartDto accountsChart,
                                AccountsSearchCommand searchCommand) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(searchCommand, "searchCommand");

      var templateUID = "AccountsChartTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new AccountsChartExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(accountsChart);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<GroupingRulesTreeItemDto> rulesTreeItems) {
      Assertion.AssertObject(rulesTreeItems, "rulesTreeItems");

      var templateUID = $"GroupingRulesReportTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new GroupingRulesReportExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(rulesTreeItems);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FinancialReportDto financialReport) {
      Assertion.AssertObject(financialReport, "financialReport");

      var templateId = financialReport.Command.GetFinancialReportType().TemplateFileId;

      var templateConfig = ExcelTemplateConfig.Parse(templateId);

      var creator = new FinancialReportExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(financialReport);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(StoredBalanceSetDto balanceSet) {
      Assertion.AssertObject(balanceSet, "balanceSet");

      var templateUID = "BalanceSetTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new StoredBalanceSetExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(balanceSet);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountStatementDto vouchers) {
      Assertion.AssertObject(vouchers, "vouchers");

      var templateUID = $"VouchersByAccountTemplate";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new VouchersByAccountExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(vouchers);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(BalanceDto balance) {
      Assertion.AssertObject(balance, "balance");

      var templateUID = $"BalanceTemplate.{balance.Command.TrialBalanceType}";

      var templateConfig = ExcelTemplateConfig.Parse(templateUID);

      var creator = new BalanceExcelExporter(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(balance);

      return excelFile.ToFileReportDto();
    }

  }  // class ExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
