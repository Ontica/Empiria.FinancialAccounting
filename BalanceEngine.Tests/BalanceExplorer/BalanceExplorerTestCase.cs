/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : BalanceExplorerTestCase                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the Balance explorer reports.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.Tests;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for the Balance explorer reports.</summary>
  public enum BalanceExplorerTestCase {

    CatalogoAnterior,

    ConAuxiliares,

    Default

  } // enum BalanceExplorerTestCase


  /// <summary>Extension methods for BalanceExplorerTestCase.</summary>
  static public class BalanceExplorerTestCaseExtensions {

    static public BalanceExplorerQuery GetInvocationQuery(this BalanceExplorerTestCase testcase) {
      BalanceExplorerDto dto = ReadBalanceExplorerFromFile(testcase);

      return dto.Query;
    }


    static public FixedList<BalanceExplorerEntryDto> GetExpectedEntries(this BalanceExplorerTestCase testcase) {
      BalanceExplorerDto dto = ReadBalanceExplorerFromFile(testcase);

      return dto.Entries;
    }


    static public FixedList<BalanceExplorerEntryDto> GetExpectedEntries(this BalanceExplorerTestCase testcase,
                                                                           TrialBalanceItemType filteredBy) {

      BalanceExplorerDto dto = ReadBalanceExplorerFromFile(testcase);

      return dto.Entries.FindAll(x => x.ItemType == filteredBy);
    }


    static public bool SkipTest(this BalanceExplorerTestCase testcase) {
      return false;
    }

    #region Helpers

    static private BalanceExplorerDto ReadBalanceExplorerFromFile(BalanceExplorerTestCase testcase) {
      //TODO CONFIGURAR TrialBalanceType PARA SaldosPorCuentaConsultaRapida/SaldosPorAuxiliarConsultaRapida

      string fileNamePrefix = $"{TrialBalanceType.SaldosPorCuentaConsultaRapida}.{testcase}";

      return TestsCommonMethods.ReadTestDataFromFile<BalanceExplorerDto>(fileNamePrefix);
    }

    #endregion Helpers

  }  // class BalanceExplorerTestCaseExtensions


} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
