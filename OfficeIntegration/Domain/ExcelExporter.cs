/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Service                               *
*  Type     : ExcelExporter                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export accounting information to Microsoft Excel.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.OfficeIntegration.Adapters;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class ExcelExporter {

    public ExcelFileDto Export(TrialBalanceDto trialBalance, TrialBalanceCommand command) {
      Assertion.AssertObject(trialBalance, "trialBalance");
      Assertion.AssertObject(command, "command");

      var templateConfig = ExcelTemplateConfiguration.GetFor(command.TrialBalanceType);

      var creator = new TrialBalanceExcelFileCreator(templateConfig);

      ExcelFile excelFile = creator.CreateExcelFile(trialBalance);

      return ExcelFileMapper.Map(excelFile);
    }

  }  // class ExcelExporter

} // namespace Empiria.FinancialAccounting.OfficeIntegration
