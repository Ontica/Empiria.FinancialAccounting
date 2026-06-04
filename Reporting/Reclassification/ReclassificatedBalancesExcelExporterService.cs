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

using Empiria.FinancialAccounting.Reclassification;
using Empiria.FinancialAccounting.Reclassification.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Reclassification {

  /// <summary>Main service to export balances information to Microsoft Excel.</summary>
  public class ReclassificatedBalancesExcelExporterService {

    public FileDto Export(DynamicDto<BalanzaTradicionalRealDto> trialBalance) {
      throw new NotImplementedException();
    }


    public FileDto Export(DynamicDto<BalanzaValorizadaEntry> trialBalance) {
      Assertion.Require(trialBalance, nameof(trialBalance));

      var templateUID = $"TrialBalanceTemplate.BalanzaValorizada";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new BalanzaValorizadaExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(trialBalance);

      return excelFile.ToFileDto();
    }

  } // class BalancesExcelExporterService

} // namespace Empiria.FinancialAccounting.Reporting.Reclassification
