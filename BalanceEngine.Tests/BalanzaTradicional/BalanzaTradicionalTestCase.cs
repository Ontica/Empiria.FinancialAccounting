/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : BalanzaTradicionalTestCase                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Balanza tradicional' report.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for the 'Balanza tradicional' report.</summary>
  public enum BalanzaTradicionalTestCase {

    CatalogoAnterior,

    ConAuxiliares,

    Default,

    EnCascada,

    EnCascadaConAuxiliares,

    Sectorizada,
    
  }  // enum BalanzaTradicionalTestCase 



  /// <summary>Test cases for the 'Balanza tradicional' report.</summary>
  static public class BalanzaTradicionalTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this BalanzaTradicionalTestCase testcase) {
      BalanzaTradicionalDto dto = ReadBalanzaTradicionalFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<BalanzaTradicionalEntryDto> GetExpectedEntries(this BalanzaTradicionalTestCase testcase) {
      BalanzaTradicionalDto dto = ReadBalanzaTradicionalFromFile(testcase);

      return dto.Entries;
    }


    #region Helpers

    static private BalanzaTradicionalDto ReadBalanzaTradicionalFromFile(BalanzaTradicionalTestCase testcase) {
      string fileNamePrefix = $"{TrialBalanceType.Balanza}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<BalanzaTradicionalDto>(fileNamePrefix);
    }

    #endregion Helpers

  } // class BalanzaTradicionalTestCase

} // namespace Empiria.FinancialAccounting.Tests
