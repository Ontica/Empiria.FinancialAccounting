/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Service                               *
*  Type     : TrialBalanceExcelFileCreator                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Microsoft Excel file with trial balance information.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Creates a Microsoft Excel file with trial balance information.</summary>
  internal class TrialBalanceExcelFileCreator {

    private readonly ExcelTemplateConfiguration _templateConfig;

    public TrialBalanceExcelFileCreator(ExcelTemplateConfiguration templateConfig) {
      _templateConfig = templateConfig;
    }

    internal ExcelFile CreateExcelFile(TrialBalanceDto trialBalance) {
      return new ExcelFile();
    }

  }  // class TrialBalanceExcelFileCreator

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
