/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Information Holder                    *
*  Type     : ExcelTemplateConfiguration                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds configuration information about a Microsoft Excel template file.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Holds configuration information about a Microsoft Excel template file.</summary>
  internal class ExcelTemplateConfiguration {

    static internal ExcelTemplateConfiguration GetFor(TrialBalanceType trialBalanceType) {
      return new ExcelTemplateConfiguration();
    }

  }  // class ExcelTemplateConfiguration

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
