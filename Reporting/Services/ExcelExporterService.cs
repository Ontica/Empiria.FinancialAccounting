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
using Empiria.FinancialAccounting.Rules.Adapters;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class ExcelExporterService {

    public FileReportDto Export(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      var templateUID = $"TrialBalanceTemplate.{trialBalance.Command.TrialBalanceType}";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new TrialBalanceExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountsChartDto accountsChart,
                                AccountsSearchCommand searchCommand) {
      Assertion.AssertObject(accountsChart, "accountsChart");
      Assertion.AssertObject(searchCommand, "searchCommand");

      var templateUID = "AccountsChartTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new AccountsChartExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(accountsChart);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(ReconciliationResultDto reconciliationResult) {
      Assertion.AssertObject(reconciliationResult, nameof(reconciliationResult));

      var templateUID = "ReconciliationResult.ExcelTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new ReconciliationExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(reconciliationResult);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<ExchangeRateDescriptorDto> exchangeRates) {
      Assertion.AssertObject(exchangeRates, "exchangeRates");

      var templateUID = "ExchangeRatesTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new ExchangeRateExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(exchangeRates);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<GroupingRulesTreeItemDto> rulesTreeItems) {
      Assertion.AssertObject(rulesTreeItems, "rulesTreeItems");

      var templateUID = $"GroupingRulesReportTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new GroupingRulesReportExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(rulesTreeItems);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(StoredBalanceSetDto balanceSet) {
      Assertion.AssertObject(balanceSet, "balanceSet");

      var templateUID = "BalanceSetTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new StoredBalanceSetExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(balanceSet);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountStatementDto vouchers) {
      Assertion.AssertObject(vouchers, "vouchers");

      var templateUID = $"VouchersByAccountTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new VouchersByAccountExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(vouchers);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<TransactionSlipDto> transactionSlips,
                                string exportationType) {

      Assertion.AssertObject(transactionSlips, "transactionSlips");
      Assertion.AssertObject(exportationType, "exportationType");

      string templateUID;

      if (exportationType == "slips") {
        templateUID = $"TransactionSlipsTemplate";
      } else if (exportationType == "issues") {
        templateUID = $"TransactionSlipsIssuesTemplate";
      } else {
        throw Assertion.AssertNoReachThisCode($"Invalid exportation type '{exportationType}'.");
      }

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new TransactionSlipExporter(templateConfig);

      ExcelFile excelFile;

      if (exportationType == "slips") {
        excelFile = exporter.CreateExcelFile(transactionSlips);

      } else if (exportationType == "issues") {
        excelFile = exporter.CreateIsuesExcelFile(transactionSlips);

      } else {
        throw Assertion.AssertNoReachThisCode($"Invalid exportation type '{exportationType}'.");
      }

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(BalanceDto balance) {
      Assertion.AssertObject(balance, "balance");

      var templateUID = $"BalanceTemplate.{balance.Command.TrialBalanceType}";

      if (balance.Command.ExportTo != BalanceEngine.FileReportVersion.V1) {
        templateUID = $"BalanceTemplate.{balance.Command.TrialBalanceType}" +
                      $"{balance.Command.ExportTo}";
      }

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanceExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(balance);

      return excelFile.ToFileReportDto();
    }

  }  // class ExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
