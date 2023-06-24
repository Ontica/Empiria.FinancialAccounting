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
using Empiria.FinancialAccounting.ExternalData.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

using Empiria.FinancialAccounting.Reporting.ExternalData.Exporters;
using Empiria.FinancialAccounting.Reporting.FinancialConceptsEntriesTree.Exporters;
using Empiria.FinancialAccounting.Reporting.Reconciliation.Exporters;
using Empiria.FinancialAccounting.Reporting.StoredBalanceSet.Exporters;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class ExcelExporterService {


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


    public FileReportDto Export(FixedList<FinancialConceptTreeNodeDto> treeNodes) {
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


    public FileReportDto Export(ExternalValuesDto externalValues) {
      Assertion.Require(externalValues, nameof(externalValues));

      var templateConfig = FileTemplateConfig.Parse(externalValues.Query.ExportTo);

      var exporter = new ExternalValuesExcelExporter(templateConfig);

      ExcelFile exportedFile = exporter.CreateExcelFile(externalValues);

      return exportedFile.ToFileReportDto();
    }


    public FileReportDto Export(VoucherDto voucher) {
      Assertion.Require(voucher, nameof(voucher));

      var templateUID = "AccountsChartTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new VouchersExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(voucher);

      return excelFile.ToFileReportDto();
    }

  }  // class ExcelExporter

} // namespace Empiria.FinancialAccounting.Reporting
