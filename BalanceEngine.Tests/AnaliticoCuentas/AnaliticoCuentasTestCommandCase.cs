/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : AnaliticoCuentasTestCommandCase            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for Analitico de Cuentas report.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  public enum AnaliticoCuentasTestCommandCase {

    Default,

    ConAuxiliares,

    EnCascada,

    EnCascadaConAuxiliares

  }  // enum AnaliticoCuentasTestCommandCase


  static public class AnaliticoCuentasTestCommandCaseExtensions {

    static public TrialBalanceCommand BuildCommand(this AnaliticoCuentasTestCommandCase commandCase) {
      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas,
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        InitialPeriod = new BalanceEngineCommandPeriod() {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE
        }
      };
    }

  }  // class AnaliticoCuentasTestCommandCaseExtensions

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
