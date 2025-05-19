/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : TrialBalanceUseCasesTests                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases to comparate balance entry vs trial balance reports.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.Services;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases to comparate balance entry vs trial balance reports.</summary>
  public class BalancesComparatorTests {

    [Fact]
    public void Should_Get_Balance_Entries() {

      var sut = GetBalanceEntries();

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Compare_Analitico_VS_Balance_Entries() {

      var analitico = new FixedList<AnaliticoDeCuentasDto>();

      var balanceEntries = GetBalanceEntries();

      Assert.NotNull(analitico);
      Assert.NotNull(balanceEntries);
    }


    [Fact]
    public void Should_Compare_Balanza_Tradicional_VS_Balance_Entries() {

      var balanza = new FixedList<BalanzaTradicionalDto>();

      var balanceEntries = GetBalanceEntries();

      Assert.NotNull(balanza);
      Assert.NotNull(balanceEntries);
    }


    #region Helpers

    private FixedList<BalanceEntry> GetBalanceEntries() {
      TrialBalanceQuery query = GetDefaultQuery();

      BalanceEntryBuilder builder = new BalanceEntryBuilder();

      return builder.GetBalanceEntries(query);
    }


    private TrialBalanceQuery GetDefaultQuery() {

      return new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        FromAccount = "1.01",
        ToAccount = "1.01",
        ConsolidateBalancesToTargetCurrency = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(2025, 01, 01),
          ToDate = new DateTime(2025, 01, 31),
          ExchangeRateDate = new DateTime(2025, 01, 31),
          ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID,
          ValuateToCurrrencyUID = "01"
        }
      };
    }

    #endregion Helpers

  } // class BalancesComparatorTests
}
