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

using Empiria.DynamicData;
using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Reclassification.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Reporting.Reclassification {

  /// <summary>Main service to export balances information to Microsoft Excel.</summary>
  public class ReclassificatedBalancesExcelExporterService {

    public FileDto BalanzaAnaliticaOperaciones(DynamicDto<BalanzaAnaliticaOperacionesDto> trialBalance) {
      Assertion.Require(trialBalance, nameof(trialBalance));

      var templateUID = $"TrialBalanceTemplate.BalanzaAnaliticaOperaciones";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanzaAnaliticaOperacionesExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileDto();
    }


    public FileDto BalanzaEnColumnas(DynamicDto<BalanzaEnColumnasRealDto> trialBalance) {
      Assertion.Require(trialBalance, nameof(trialBalance));

      var templateUID = $"TrialBalanceTemplate.BalanzaMonedaOrigenReclasificada";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanzaValorizadaExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileDto();
    }


    public FileDto BalanzaTradicional(DynamicDto<BalanzaTradicionalRealDto> trialBalance) {
      Assertion.Require(trialBalance, nameof(trialBalance));

      var templateUID = $"TrialBalanceTemplate.BalanzaTradicionalReclasificada";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanzaTradicionalReclasificadaExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileDto();
    }

  } // class BalancesExcelExporterService

} // namespace Empiria.FinancialAccounting.Reporting.Reclassification
