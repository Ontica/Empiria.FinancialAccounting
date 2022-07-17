/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : BalanzaColumnasMonedaTestCase                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Test cases for the 'Balanza en columnas por moneda' report.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda {


  /// <summary>Test cases for the 'Balanza en columnas por moneda' report.</summary>
  public enum BalanzaColumnasMonedaTestCase {

    CatalogoAnterior,

    Default,

    Filtrada

  }  // enum BalanzaColumnasMonedaTestCase 


  /// <summary>Test cases for the 'Balanza en columnas por moneda' report.</summary>
  static public class BalanzaBalanzaColumnasMonedaTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this BalanzaColumnasMonedaTestCase testcase) {
      BalanzaColumnasMonedaDto dto = ReadBalanzaColumnasMonedaFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<BalanzaColumnasMonedaEntryDto> GetExpectedEntries(
                                                           this BalanzaColumnasMonedaTestCase testcase) {
      BalanzaColumnasMonedaDto dto = ReadBalanzaColumnasMonedaFromFile(testcase);

      return dto.Entries;
    }


    #region Helpers

    static private BalanzaColumnasMonedaDto ReadBalanzaColumnasMonedaFromFile(
                                            BalanzaColumnasMonedaTestCase testcase) {

      string fileNamePrefix = $"{TrialBalanceType.BalanzaEnColumnasPorMoneda}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<BalanzaColumnasMonedaDto>(fileNamePrefix);
    }

    #endregion Helpers

  } // class BalanzaBalanzaColumnasMonedaTestCaseExtensions

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda
