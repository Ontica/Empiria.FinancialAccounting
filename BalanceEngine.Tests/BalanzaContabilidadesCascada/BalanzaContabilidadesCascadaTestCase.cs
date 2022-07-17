/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : BalanzaContabilidadesCascadaTestCase       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Balanza con contabilidades en cascada' report.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada {

  /// <summary>Test cases for the 'Balanza con contabilidades en cascada' report.</summary>
  public enum BalanzaContabilidadesCascadaTestCase {

    CatalogoAnterior,

    Default,

    Valorizada

  }  // enum BalanzaContabilidadesCascadaTestCase


  /// <summary>Extension methods for BalanzaContabilidadesCascadaTestCase.</summary>
  static public class BalanzaContabilidadesCascadaTestCaseExtensions {

    static public TrialBalanceQuery GetInvocationQuery(this BalanzaContabilidadesCascadaTestCase testcase) {
      BalanzaContabilidadesCascadaDto dto = ReadBalanzaContabilidadesCascadaFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<BalanzaContabilidadesCascadaEntryDto> GetExpectedEntries(
                                       this BalanzaContabilidadesCascadaTestCase testcase) {
      BalanzaContabilidadesCascadaDto dto = ReadBalanzaContabilidadesCascadaFromFile(testcase);

      return dto.Entries;
    }


    #region Helpers

    static private BalanzaContabilidadesCascadaDto ReadBalanzaContabilidadesCascadaFromFile(
                                                    BalanzaContabilidadesCascadaTestCase testcase) {

      string fileNamePrefix = $"{TrialBalanceType.BalanzaConContabilidadesEnCascada}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<BalanzaContabilidadesCascadaDto>(fileNamePrefix);
    }

    #endregion Helpers

  } // class BalanzaContabilidadesCascadaTestCaseExtensions

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada
