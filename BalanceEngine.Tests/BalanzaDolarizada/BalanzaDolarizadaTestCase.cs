/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : BalanzaDolarizadaTestCase                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Balanza dolarizada' report.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for the 'Balanza dolarizada' report.</summary>
  public enum BalanzaDolarizadaTestCase {

    CatalogoAnterior,

    Default,

    OficinasCentrales

  } // enum BalanzaDolarizadaTestCase


  /// <summary>Extension methods for BalanzaDolarizadaTestCase.</summary>
  static public class BalanzaDolarizadaTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this BalanzaDolarizadaTestCase testcase) {
      BalanzaDolarizadaDto dto = ReadBalanzaDolarizadaFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<BalanzaDolarizadaEntryDto> GetExpectedEntries(this BalanzaDolarizadaTestCase testcase) {
      BalanzaDolarizadaDto dto = ReadBalanzaDolarizadaFromFile(testcase);

      return dto.Entries;
    }


    static public FixedList<BalanzaDolarizadaEntryDto> GetExpectedEntries(this BalanzaDolarizadaTestCase testcase,
                                                                          TrialBalanceItemType filteredBy) {

      BalanzaDolarizadaDto dto = ReadBalanzaDolarizadaFromFile(testcase);

      return dto.Entries.FindAll(x => x.ItemType == filteredBy);
    }


    static public bool SkipTest(this BalanzaDolarizadaTestCase testcase) {
      return false;
    }

    #region Helpers

    static private BalanzaDolarizadaDto ReadBalanzaDolarizadaFromFile(BalanzaDolarizadaTestCase testcase) {
      string fileNamePrefix = $"{TrialBalanceType.BalanzaDolarizada}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<BalanzaDolarizadaDto>(fileNamePrefix);
    }

    #endregion Helpers

  }  // class BalanzaDolarizadaTestCaseExtensions

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine

