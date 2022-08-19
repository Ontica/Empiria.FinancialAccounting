/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : BalancesExcelExporterService                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Main service to export balances information to Microsoft Excel.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

using Empiria.FinancialAccounting.Reporting.Exporters.Excel;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Main service to export balances information to Microsoft Excel.</summary>
  public class BalancesExcelExporterService {

    public FileReportDto Export(TrialBalanceDto trialBalance) {
      Assertion.Require(trialBalance, "trialBalance");

      var templateUID = $"TrialBalanceTemplate.{trialBalance.Query.TrialBalanceType}";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new TrialBalanceExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileReportDto();
    }


    public FileReportDto Export(BalanceExplorerDto dto) {
      Assertion.Require(dto, nameof(dto));

      var templateUID = $"BalanceTemplate.{dto.Query.TrialBalanceType}";

      if (dto.Query.ExportTo != BalanceEngine.FileReportVersion.V1) {
        templateUID = $"BalanceTemplate.{dto.Query.TrialBalanceType}" +
                      $"{dto.Query.ExportTo}";
      }

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanceExplorerExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(dto);

      return excelFile.ToFileReportDto();
    }


  } // class BalancesExcelExporterService

} // namespace Empiria.FinancialAccounting.Reporting
