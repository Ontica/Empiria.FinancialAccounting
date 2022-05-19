/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : AnaliticoCuentasTestCommandCase            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test command case enumeration with extensions for 'Analítico de cuentas' report.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Json;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test command case enumeration with extensions for 'Analítico de cuentas' report.</summary>
  public enum AnaliticoCuentasTestCommandCase {

    Default,

    ConAuxiliares,

    EnCascada,

    EnCascadaConAuxiliares

  }  // enum AnaliticoCuentasTestCommandCase



  /// <summary>Extension methods for AnaliticoCuentasTestCommandCase.</summary>
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


    static public FixedList<AnaliticoDeCuentasEntryDto> GetExpectedEntries(this AnaliticoCuentasTestCommandCase commandCase,
                                                                           TrialBalanceItemType filteredBy) {
      var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

      string path = Path.Combine(directory.Parent.FullName,
                                @"tests-data",
                                $"analitico-cuentas.{commandCase}.test-data.json");

      string jsonString = File.ReadAllText(path);

      AnaliticoDeCuentasDto dto = JsonConverter.ToObject<AnaliticoDeCuentasDto>(jsonString);

      return dto.Entries.FindAll(x => x.ItemType == filteredBy);
    }

  }  // class AnaliticoCuentasTestCommandCaseExtensions

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
