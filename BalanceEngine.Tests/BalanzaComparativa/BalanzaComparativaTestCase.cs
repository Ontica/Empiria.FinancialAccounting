/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : BalanzaComparativaTestCase                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Balanza comparativa' report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for the 'Balanza comparativa' report.</summary>
  public enum BalanzaComparativaTestCase {

    CatalogoAnterior,

    Default,

    EnCascada

  } // enum BalanzaComparativaTestCase


  /// <summary>Extension methods for BalanzaComparativaTestCase.</summary>
  static public class BalanzaComparativaTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this BalanzaComparativaTestCase testcase) {
      BalanzaComparativaDto dto = ReadBalanzaComparativaFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<BalanzaComparativaEntryDto> GetExpectedEntries(this BalanzaComparativaTestCase testcase) {
      BalanzaComparativaDto dto = ReadBalanzaComparativaFromFile(testcase);

      return dto.Entries;
    }


    static public FixedList<BalanzaComparativaEntryDto> GetExpectedEntries(this BalanzaComparativaTestCase testcase,
                                                                           TrialBalanceItemType filteredBy) {

      BalanzaComparativaDto dto = ReadBalanzaComparativaFromFile(testcase);

      return dto.Entries.FindAll(x => x.ItemType == filteredBy);
    }


    static public bool SkipTest(this BalanzaComparativaTestCase testcase) {
      return false;
    }

    #region Helpers

    static private BalanzaComparativaDto ReadBalanzaComparativaFromFile(BalanzaComparativaTestCase testcase) {
      string fileNamePrefix = $"{TrialBalanceType.BalanzaValorizadaComparativa}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<BalanzaComparativaDto>(fileNamePrefix);
    }

    #endregion Helpers

  } // class BalanzaComparativaTestCaseExtensions

} // namespace Empiria.FinancialAccounting.Tests.BalanzaComparativa
