/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : AnaliticoDeCuentasTestCase                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Analítico de cuentas' report.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for the 'Analítico de cuentas' report.</summary>
  public enum AnaliticoDeCuentasTestCase {

    CatalogoAnterior,

    ConAuxiliares,

    Default,

    EnCascada,

    EnCascadaConAuxiliares,

    Sectorizado

  }  // enum AnaliticoDeCuentasTestCase



  /// <summary>Extension methods for AnaliticoDeCuentasTestCase.</summary>
  static public class AnaliticoDeCuentasTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this AnaliticoDeCuentasTestCase testcase) {
      AnaliticoDeCuentasDto dto = ReadAnaliticoDeCuentasFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<AnaliticoDeCuentasEntryDto> GetExpectedEntries(this AnaliticoDeCuentasTestCase testcase) {
      AnaliticoDeCuentasDto dto = ReadAnaliticoDeCuentasFromFile(testcase);

      return dto.Entries;
    }


    static public FixedList<AnaliticoDeCuentasEntryDto> GetExpectedEntries(this AnaliticoDeCuentasTestCase testcase,
                                                                           TrialBalanceItemType filteredBy) {

      AnaliticoDeCuentasDto dto = ReadAnaliticoDeCuentasFromFile(testcase);

      return dto.Entries.FindAll(x => x.ItemType == filteredBy);
    }


    static public bool SkipTest(this AnaliticoDeCuentasTestCase testcase) {
      return false;
    }

    #region Helpers

    static private AnaliticoDeCuentasDto ReadAnaliticoDeCuentasFromFile(AnaliticoDeCuentasTestCase testcase) {
      string fileNamePrefix = $"{TrialBalanceType.AnaliticoDeCuentas}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<AnaliticoDeCuentasDto>(fileNamePrefix);
    }

    #endregion Helpers

  }  // class AnaliticoDeCuentasTestCaseExtensions

}  // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
