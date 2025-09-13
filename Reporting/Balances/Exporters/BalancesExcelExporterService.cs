﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : BalancesExcelExporterService                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Main service to export balances information to Microsoft Excel.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Balances {

  /// <summary>Main service to export balances information to Microsoft Excel.</summary>
  public class BalancesExcelExporterService {

    public FileDto Export(TrialBalanceDto trialBalance) {
      Assertion.Require(trialBalance, "trialBalance");

      var templateUID = $"TrialBalanceTemplate.{trialBalance.Query.TrialBalanceType}";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalancesExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileDto();
    }


    public FileDto Export(SaldosEncerradosDto reportData) {
      Assertion.Require(reportData, "reportData");

      var templateUID = $"TrialBalanceTemplate.SaldosEncerrados";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new SaldosEncerradosExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(reportData);

      return excelFile.ToFileDto();
    }


    public FileDto Export(BalanceExplorerDto dto) {
      Assertion.Require(dto, nameof(dto));

      var templateUID = $"BalanceTemplate.{dto.Query.TrialBalanceType}";

      if (dto.Query.ExportTo != BalanceEngine.FileReportVersion.V1) {
        templateUID = $"BalanceTemplate.{dto.Query.TrialBalanceType}" +
                      $"{dto.Query.ExportTo}";
      }

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanceExplorerExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(dto);

      return excelFile.ToFileDto();
    }


  } // class BalancesExcelExporterService

} // namespace Empiria.FinancialAccounting.Reporting.Balances
