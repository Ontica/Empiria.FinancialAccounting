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
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

using Empiria.FinancialAccounting.Reporting.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class ExcelExporterService {

    public FileReportDto Export(TrialBalanceDto trialBalance) {
      Assertion.Require(trialBalance, "trialBalance");

      var templateUID = $"TrialBalanceTemplate.{trialBalance.Command.TrialBalanceType}";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new TrialBalanceExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountsChartDto accountsChart) {
      Assertion.Require(accountsChart, nameof(accountsChart));

      var templateUID = "AccountsChartTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new AccountsChartExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(accountsChart);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(ReconciliationResultDto reconciliationResult) {
      Assertion.Require(reconciliationResult, nameof(reconciliationResult));

      var templateUID = "ReconciliationResult.ExcelTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new ReconciliationExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(reconciliationResult);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<ExchangeRateDescriptorDto> exchangeRates) {
      Assertion.Require(exchangeRates, "exchangeRates");

      var templateUID = "ExchangeRatesTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new ExchangeRateExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(exchangeRates);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<FinancialConceptEntryAsTreeNodeDto> treeNodes) {
      Assertion.Require(treeNodes, "treeNodes");

      var templateUID = $"GroupingRulesReportTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new FinancialConceptsEntriesTreeExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(treeNodes);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(StoredBalanceSetDto balanceSet) {
      Assertion.Require(balanceSet, "balanceSet");

      var templateUID = "BalanceSetTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new StoredBalanceSetExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(balanceSet);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(AccountStatementDto vouchers) {
      Assertion.Require(vouchers, "vouchers");

      var templateUID = $"VouchersByAccountTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new VouchersByAccountExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(vouchers);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(FixedList<TransactionSlipDto> transactionSlips,
                                string exportationType) {

      Assertion.Require(transactionSlips, "transactionSlips");
      Assertion.Require(exportationType, "exportationType");

      string templateUID;

      if (exportationType == "slips") {
        templateUID = $"TransactionSlipsTemplate";
      } else if (exportationType == "issues") {
        templateUID = $"TransactionSlipsIssuesTemplate";
      } else {
        throw Assertion.EnsureNoReachThisCode($"Invalid exportation type '{exportationType}'.");
      }

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new TransactionSlipExporter(templateConfig);

      ExcelFile excelFile;

      if (exportationType == "slips") {
        excelFile = exporter.CreateExcelFile(transactionSlips);

      } else if (exportationType == "issues") {
        excelFile = exporter.CreateIsuesExcelFile(transactionSlips);

      } else {
        throw Assertion.EnsureNoReachThisCode($"Invalid exportation type '{exportationType}'.");
      }

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(BalanceDto balance) {
      Assertion.Require(balance, "balance");

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
