/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Enumerated type with extensions         *
*  Type     : SaldosPorCuentaTestCase                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the 'Saldos por cuenta' report.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

    /// <summary>Test cases for the 'Saldos por cuenta' report.</summary>
    public enum SaldosPorCuentaTestCase {

      CatalogoAnterior,

      ConAuxiliares,

      Default,

      EnCascada,

      EnCascadaConAuxiliares,

      Sectorizado

    }  // enum SaldosPorCuentaTestCase



    /// <summary>Extension methods for SaldosPorCuentaTestCase.</summary>
    static public class SaldosPorCuentaTestCaseExtensions {

      static public TrialBalanceQuery GetInvocationQuery(this SaldosPorCuentaTestCase testcase) {
        SaldosPorCuentaDto dto = ReadSaldosPorCuentaFromFile(testcase);

        return dto.Query;
      }


      static public FixedList<SaldosPorCuentaEntryDto> GetExpectedEntries(this SaldosPorCuentaTestCase testcase) {
        SaldosPorCuentaDto dto = ReadSaldosPorCuentaFromFile(testcase);

        return dto.Entries;
      }


      static public FixedList<SaldosPorCuentaEntryDto> GetExpectedEntries(this SaldosPorCuentaTestCase testcase,
                                                                             TrialBalanceItemType filteredBy) {

        SaldosPorCuentaDto dto = ReadSaldosPorCuentaFromFile(testcase);

        return dto.Entries.FindAll(x => x.ItemType == filteredBy);
      }


      static public bool SkipTest(this SaldosPorCuentaTestCase testcase) {
        return false;
      }

      #region Helpers

      static private SaldosPorCuentaDto ReadSaldosPorCuentaFromFile(SaldosPorCuentaTestCase testcase) {
        string fileNamePrefix = $"{TrialBalanceType.SaldosPorCuenta}.{testcase}";

        return TestsCommonMethods.ReadTestDataFromFile<SaldosPorCuentaDto>(fileNamePrefix);
      }

    #endregion Helpers

  } // class SaldosPorCuentaTestCaseExtensions

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine
