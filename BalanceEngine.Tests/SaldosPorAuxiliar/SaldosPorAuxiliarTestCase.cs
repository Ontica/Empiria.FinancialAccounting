/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : SaldosPorAuxiliarTestCase                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Saldos por auxiliar' report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for the 'Saldos por auxiliar' report.</summary>
  public enum SaldosPorAuxiliarTestCase {

    CatalogoAnterior,

    Default,

    Valorizados

  }  // enum SaldosPorAuxiliarTestCase



  /// <summary>Extension methods for SaldosPorAuxiliarTestCase.</summary>
  static public class SaldosPorAuxiliarTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this SaldosPorAuxiliarTestCase testcase) {
      SaldosPorAuxiliarDto dto = ReadSaldosPorAuxiliarFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<SaldosPorAuxiliarEntryDto> GetExpectedEntries(this SaldosPorAuxiliarTestCase testcase) {
      SaldosPorAuxiliarDto dto = ReadSaldosPorAuxiliarFromFile(testcase);

      return dto.Entries;
    }


    static public FixedList<SaldosPorAuxiliarEntryDto> GetExpectedEntries(this SaldosPorAuxiliarTestCase testcase,
                                                                          TrialBalanceItemType filteredBy) {

      SaldosPorAuxiliarDto dto = ReadSaldosPorAuxiliarFromFile(testcase);

      return dto.Entries.FindAll(x => x.ItemType == filteredBy);
    }


    static public bool SkipTest(this SaldosPorAuxiliarTestCase testcase) {
      return false;
    }

    #region Helpers

    static private SaldosPorAuxiliarDto ReadSaldosPorAuxiliarFromFile(SaldosPorAuxiliarTestCase testcase) {
      string fileNamePrefix = $"{TrialBalanceType.SaldosPorAuxiliar}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<SaldosPorAuxiliarDto>(fileNamePrefix);
    }

    #endregion Helpers

  }  // class SaldosPorAuxiliarTestCaseExtensions

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
