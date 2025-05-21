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
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
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

      TrialBalanceQuery query = GetDefaultQuery();
      var sut = GetBalanceEntries(query);

      Assert.NotNull(sut);
    }


    [Fact]
    public void Should_Be_Same_Analitico_VS_Balance_Entries() {

      TrialBalanceQuery query = GetDefaultQuery();
      query.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;

      var analitico = GetTrialBalanceDto(query);
      var analiticoEntries = analitico.Entries.Select(x => (AnaliticoDeCuentasEntryDto) x);
      Assert.NotNull(analitico);
      Assert.NotEmpty(analiticoEntries);

      var balanceEntries = GetBalanceEntries(query);
      Assert.NotEmpty(balanceEntries);

      foreach (var analiticoEntry in analiticoEntries) {

        var filtered = BalanceEntryBuilder.GetBalancesByAccountAndSector(analiticoEntry.AccountNumber, analiticoEntry.SectorCode, balanceEntries);
        var entriesMN = BalanceEntryBuilder.GetSumByMN(filtered);
        var entriesME = BalanceEntryBuilder.GetSumByME(filtered);

        Assert.True((entriesMN - analiticoEntry.DomesticBalance) <= 1 ||
                    (entriesMN - analiticoEntry.DomesticBalance) >= -1,
                    $"Diferencia MN en cuenta {analiticoEntry.AccountNumber}, " +
                    $"analítico = {analiticoEntry.DomesticBalance}. Suma movimientos = {entriesMN}");

        Assert.True((entriesME - analiticoEntry.ForeignBalance) <= 1 ||
                    (entriesME - analiticoEntry.ForeignBalance) >= -1,
                    $"Diferencia ME en cuenta {analiticoEntry.AccountNumber}, " +
                    $"analítico = {analiticoEntry.ForeignBalance}. Suma movimientos = {entriesME}");

        Assert.True(((entriesMN + entriesME) - analiticoEntry.TotalBalance) <= 1 ||
                    ((entriesMN + entriesME) - analiticoEntry.TotalBalance) >= -1,
                    $"Diferencia en Total en cuenta {analiticoEntry.AccountNumber}, " +
                    $"analítico = {analiticoEntry.TotalBalance}. Suma movimientos = {entriesMN + entriesME}");
      }
    }


    #region Helpers

    private FixedList<BalanceEntry> GetBalanceEntries(TrialBalanceQuery query) {

      GetQueryFiltersForBalanceEntries(query);

      BalanceEntryBuilder builder = new BalanceEntryBuilder(query);

      return builder.GetBalanceEntries();
    }


    private TrialBalanceQuery GetDefaultQuery() {

      return new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = false,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        FromAccount = "",
        ToAccount = "",
        ConsolidateBalancesToTargetCurrency = false,
        UseDefaultValuation = true,
        InitialPeriod = new BalancesPeriod {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE
        }
      };
    }


    private TrialBalanceDto GetTrialBalanceDto(TrialBalanceQuery query) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecase.BuildTrialBalance(query);
        var analiticoEntries = trialBalance.Entries.Select(x => (AnaliticoDeCuentasEntryDto) x);

        return trialBalance;
      }
    }


    private void GetQueryFiltersForBalanceEntries(TrialBalanceQuery query) {
      query.UseDefaultValuation = false;
      query.ConsolidateBalancesToTargetCurrency = true;
      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.InitialPeriod.ExchangeRateDate = TestingConstants.TO_DATE;
      query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
      query.InitialPeriod.ValuateToCurrrencyUID = "01";
    }

    #endregion Helpers

  } // class BalancesComparatorTests
}
