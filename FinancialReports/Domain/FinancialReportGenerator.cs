/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialReportGenerator                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Provides services to generate a trial balance.</summary>
  internal class FinancialReportGenerator {

    internal FinancialReportGenerator(FinancialReportCommand command) {
      Assertion.AssertObject(command, "command");

      this.Command = command;
    }


    public FinancialReportCommand Command {
      get;
    }


    internal FinancialReport BuildFinancialReport() {
      switch (this.Command.FinancialReportType) {

        case FinancialReportType.R01:
        case FinancialReportType.R01_Banxico:
          var r01 = new R01(this.Command);

          return r01.Generate();

        case FinancialReportType.R01_Integracion:
        case FinancialReportType.R01_Banxico_Integracion:
          var r01Integracion = new R01(this.Command);

          return r01Integracion.Generate();

        default:
          throw Assertion.AssertNoReachThisCode(
                    $"Unhandled trial balance type {this.Command.FinancialReportType}.");
      }
    }

  } // class FinancialReportGenerator

} // namespace Empiria.FinancialAccounting.FinancialReports
