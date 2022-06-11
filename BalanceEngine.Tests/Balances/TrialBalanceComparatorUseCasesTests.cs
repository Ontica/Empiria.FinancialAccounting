/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Use cases tests                         *
*  Type     : TrialBalanceComparatorUseCasesTests        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for comparison between trial balance reports.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine {

  /// <summary>Test cases for comparison between trial balance reports.</summary>
  public class TrialBalanceComparatorUseCasesTests {

    #region Initialization

    public TrialBalanceComparatorUseCasesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Be_Same_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.GeneracionDeSaldos;
      query.ShowCascadeBalances = false;
      query.WithSubledgerAccount = false;

      TrialBalanceDto balances = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(balances);
      Assert.NotEmpty(balances.Entries);
      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);

      var _balances = balances.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var wrongEntries = 0;

      foreach (var balance in _balances.Where(a => !a.AccountNumber.Contains("undefined"))) {
        var entry = _trialBalance.FirstOrDefault(a => a.CurrencyCode == balance.CurrencyCode &&
                                                      !a.AccountNumber.Contains("undefined") &&
                                                      a.AccountNumber == balance.AccountNumber &&
                                                      a.AccountNumberForBalances == balance.AccountNumberForBalances &&
                                                      a.DebtorCreditor == balance.DebtorCreditor &&
                                                      a.LedgerNumber == balance.LedgerNumber &&
                                                      a.SectorCode == balance.SectorCode
                                                      );
        wrongEntries = entry != null && balance.CurrentBalance != entry.CurrentBalance ?
                       wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Be_Same_Domestic_Balance_With_Analytics() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.ShowCascadeBalances = true;
      query.UseDefaultValuation = true;
      query.WithSubledgerAccount = false;

      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto twoColumns = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(twoColumns);
      Assert.NotEmpty(twoColumns.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _twoColumnsEntries = twoColumns.Entries.Select(x => (AnaliticoDeCuentasEntryDto) x);
      var domesticWrongEntries = 0;

      foreach (var entry in _twoColumnsEntries.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var balances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          (a.CurrencyCode == "01" || a.CurrencyCode == "44") &&
                                          a.LedgerNumber == entry.LedgerNumber &&
                                          a.SectorCode == entry.SectorCode &&
                                          a.DebtorCreditor == entry.DebtorCreditor.ToString()
                                         ).Sum(a => a.CurrentBalance);

        domesticWrongEntries = entry.DomesticBalance != balances ? domesticWrongEntries + 1 : domesticWrongEntries;
      }
      Assert.True(domesticWrongEntries == 0);
    }


    [Fact]
    public void Should_Be_Same_Foreign_Balance_With_Analytics() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.ShowCascadeBalances = false;
      query.UseDefaultValuation = true;

      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto twoColumns = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(twoColumns);
      Assert.NotEmpty(twoColumns.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _twoColumnsEntries = twoColumns.Entries.Select(x => (AnaliticoDeCuentasEntryDto) x);
      var foreignWrongEntries = 0;

      foreach (var entry in _twoColumnsEntries.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var foreignBalances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          a.CurrencyCode != "01" && a.CurrencyCode != "44" &&
                                          a.LedgerNumber == entry.LedgerNumber &&
                                          a.SectorCode == entry.SectorCode &&
                                          a.DebtorCreditor == entry.DebtorCreditor.ToString()
                                         ).Sum(a => a.CurrentBalance);

        foreignWrongEntries = entry.ForeignBalance != foreignBalances ? foreignWrongEntries + 1 : foreignWrongEntries;
      }

      Assert.True(foreignWrongEntries == 0);
    }


    [Fact]
    public void Should_Match_Cascade_Balance_With_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.ShowCascadeBalances = true;

      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada;
      TrialBalanceDto cascadeBalance = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(cascadeBalance);
      Assert.NotEmpty(cascadeBalance.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _cascadeBalance = cascadeBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var wrongEntries = 0;

      foreach (var cascadeEntry in _cascadeBalance.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var balanceEntry = _trialBalance.Where(a => a.AccountNumber == cascadeEntry.AccountNumber &&
                                                    a.CurrencyCode == cascadeEntry.CurrencyCode &&
                                                    a.LedgerNumber == cascadeEntry.LedgerNumber &&
                                                    a.SectorCode == cascadeEntry.SectorCode &&
                                                    a.ItemType == TrialBalanceItemType.Entry)
                                        .Sum(a => a.CurrentBalance);
        wrongEntries = cascadeEntry.CurrentBalance != balanceEntry ? wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Match_Totals_In_Balanza_Contabilidades_Cascada() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.UseDefaultValuation = false;
      query.WithSubledgerAccount = false;
      query.WithAverageBalance = false;
      query.ShowCascadeBalances = true;

      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.Equal(query, trialBalance.Query);
      Assert.NotEmpty(trialBalance.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);

      var totalDebtorsCreditors = GetTotalDebtorsCreditors(_trialBalance);

      var totalCurrencies = _trialBalance.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                                         .Sum(a => a.CurrentBalance);

      Assert.True(totalDebtorsCreditors == totalCurrencies);
    }


    [Fact]
    public void Should_Match_BalanceBySubledger_With_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.SaldosPorAuxiliar;
      query.BalancesType = BalancesType.WithCurrentBalance;
      query.ShowCascadeBalances = true;
      query.WithSubledgerAccount = true;

      TrialBalanceDto balanceBySubledger = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(balanceBySubledger);
      Assert.NotEmpty(balanceBySubledger.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _balanceBySubledger = balanceBySubledger.Entries.Select(x => (TrialBalanceEntryDto) x);
      var wrongEntries = 0;

      foreach (var cascadeEntry in _balanceBySubledger.Where(a => a.ItemType == TrialBalanceItemType.Summary)) {
        var balanceEntry = _trialBalance.Where(a => a.AccountNumber == cascadeEntry.AccountNumber &&
                                                    a.CurrencyCode == cascadeEntry.CurrencyCode &&
                                                    a.LedgerNumber == cascadeEntry.LedgerNumber &&
                                                    a.ItemType == TrialBalanceItemType.Entry
                                                    ).Sum(a => a.CurrentBalance);
        wrongEntries = cascadeEntry.CurrentBalanceForBalances != balanceEntry ?
                       wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Match_BalanceByAccount_With_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();

      query.BalancesType = BalancesType.WithCurrentBalance;
      query.ShowCascadeBalances = true;
      query.TrialBalanceType = TrialBalanceType.SaldosPorCuenta;
      query.WithSubledgerAccount = true;

      TrialBalanceDto balanceByAccount = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(balanceByAccount);
      Assert.NotEmpty(balanceByAccount.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _balanceByAccount = balanceByAccount.Entries.Select(x => (TrialBalanceEntryDto) x);
      var wrongEntries = 0;

      foreach (var entry in _balanceByAccount.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var balanceEntry = _trialBalance.Where(a => a.AccountNumber == entry.AccountNumber &&
                                              a.AccountNumberForBalances == entry.AccountNumberForBalances &&
                                              a.CurrencyCode == entry.CurrencyCode &&
                                              a.LedgerNumber == entry.LedgerNumber &&
                                              a.SectorCode == entry.SectorCode &&
                                              a.ItemType == TrialBalanceItemType.Entry)
                                        .Sum(a => a.CurrentBalance);
        wrongEntries = entry.CurrentBalance != balanceEntry ?
                       wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Match_Currencies_With_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.Balanza;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.ShowCascadeBalances = false;

      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);
      query.TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda;
      TrialBalanceDto balancesByCurrency = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(balancesByCurrency);
      Assert.NotEmpty(balancesByCurrency.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _balancesByCurrency = balancesByCurrency.Entries.Select(x => (TrialBalanceByCurrencyDto) x);
      var wrongEntries = 0;

      foreach (var balanceByCurrency in _balancesByCurrency) {
        var domesticBalance = _trialBalance.Where(a => a.AccountNumber == balanceByCurrency.AccountNumber &&
                                                       a.CurrencyCode == "01" &&
                                                       a.SectorCode == balanceByCurrency.SectorCode &&
                                                       (a.ItemType == TrialBalanceItemType.Entry || a.ItemType == TrialBalanceItemType.Summary))
                                           .Sum(a => a.CurrentBalance);
        wrongEntries = balanceByCurrency.DomesticBalance != domesticBalance ? wrongEntries + 1 : wrongEntries;

        var dollarBalance = _trialBalance.Where(a => a.AccountNumber == balanceByCurrency.AccountNumber &&
                                                       a.CurrencyCode == "02" &&
                                                       (a.ItemType == TrialBalanceItemType.Entry || a.ItemType == TrialBalanceItemType.Summary))
                                           .Sum(a => a.CurrentBalance);
        wrongEntries = balanceByCurrency.DollarBalance != dollarBalance ? wrongEntries + 1 : wrongEntries;

        var yenBalance = _trialBalance.Where(a => a.AccountNumber == balanceByCurrency.AccountNumber &&
                                                       a.CurrencyCode == "06" &&
                                                       (a.ItemType == TrialBalanceItemType.Entry || a.ItemType == TrialBalanceItemType.Summary))
                                           .Sum(a => a.CurrentBalance);
        wrongEntries = balanceByCurrency.YenBalance != yenBalance ? wrongEntries + 1 : wrongEntries;

        var euroBalance = _trialBalance.Where(a => a.AccountNumber == balanceByCurrency.AccountNumber &&
                                                       a.CurrencyCode == "27" &&
                                                       (a.ItemType == TrialBalanceItemType.Entry || a.ItemType == TrialBalanceItemType.Summary))
                                           .Sum(a => a.CurrentBalance);
        wrongEntries = balanceByCurrency.EuroBalance != euroBalance ? wrongEntries + 1 : wrongEntries;

        var udisBalance = _trialBalance.Where(a => a.AccountNumber == balanceByCurrency.AccountNumber &&
                                                       a.CurrencyCode == "44" &&
                                                       (a.ItemType == TrialBalanceItemType.Entry || a.ItemType == TrialBalanceItemType.Summary))
                                           .Sum(a => a.CurrentBalance);
        wrongEntries = balanceByCurrency.UdisBalance != udisBalance ? wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Match_Dollarized_Balance_With_Trial_Balance() {
      TrialBalanceQuery query = GetDefaultTrialBalanceQuery();
      query.TrialBalanceType = TrialBalanceType.BalanzaDolarizada;
      query.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      query.ShowCascadeBalances = false;
      query.UseDefaultValuation = true;

      TrialBalanceDto dollarizedBalance = BalanceEngineProxy.BuildTrialBalance(query);
      query.UseDefaultValuation = false;
      query.InitialPeriod.UseDefaultValuation = false;
      query.InitialPeriod.ValuateToCurrrencyUID = "";
      query.InitialPeriod.ExchangeRateTypeUID = "";
      query.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = BalanceEngineProxy.BuildTrialBalance(query);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(dollarizedBalance);
      Assert.NotEmpty(dollarizedBalance.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _dollarizedBalance = dollarizedBalance.Entries.Select(x => (BalanzaValorizadaEntryDto) x);
      var wrongEntries = 0;

      wrongEntries = Should_Match_With_Dollar_Currency(_dollarizedBalance, _trialBalance);
      wrongEntries = Should_Match_With_Yen_Currency(_dollarizedBalance, _trialBalance);
      wrongEntries = Should_Match_With_Euro_Currency(_dollarizedBalance, _trialBalance);


      Assert.True(wrongEntries == 0);
    }


    #endregion Facts

    #region Private methods


    private decimal GetTotalDebtorsCreditors(IEnumerable<TrialBalanceEntryDto> _trialBalance) {

      var totalDebtors = _trialBalance.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                                      .Sum(a => a.CurrentBalance);

      var totalCreditors = _trialBalance.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                                        .Sum(a => a.CurrentBalance);

      return (decimal) (totalDebtors - totalCreditors);
    }


    private int Should_Match_With_Dollar_Currency(IEnumerable<BalanzaValorizadaEntryDto> dollarizedBalance,
                                                  IEnumerable<TrialBalanceEntryDto> trialBalance) {
      int wrongEntries = 0;
      foreach (var cascadeEntry in dollarizedBalance.Where(a => a.ItemType == TrialBalanceItemType.Summary)) {
        var balanceEntry = trialBalance.Where(a => a.AccountNumber == cascadeEntry.AccountNumber &&
                                                    a.CurrencyCode == cascadeEntry.CurrencyCode &&
                                                    a.SectorCode == cascadeEntry.SectorCode &&
                                                    (a.ItemType == TrialBalanceItemType.Entry ||
                                                    a.ItemType == TrialBalanceItemType.Summary)
                                                    )
                                        .Sum(a => a.CurrentBalance);
        wrongEntries = cascadeEntry.TotalBalance != balanceEntry ? wrongEntries + 1 : wrongEntries;
      }
      return wrongEntries;
    }


    private int Should_Match_With_Euro_Currency(IEnumerable<BalanzaValorizadaEntryDto> dollarizedBalance,
                                                IEnumerable<TrialBalanceEntryDto> trialBalance) {
      int wrongEntries = 0;
      foreach (var cascadeEntry in dollarizedBalance.Where(a => a.CurrencyCode == "27")) {
        var balanceEntry = trialBalance.Where(a => a.AccountNumber == cascadeEntry.AccountNumber &&
                                                    a.CurrencyCode == cascadeEntry.CurrencyCode &&
                                                    a.SectorCode == cascadeEntry.SectorCode &&
                                                    (a.ItemType == TrialBalanceItemType.Entry ||
                                                    a.ItemType == TrialBalanceItemType.Summary)
                                                    )
                                        .Sum(a => a.CurrentBalance);
        wrongEntries = cascadeEntry.TotalBalance != balanceEntry ? wrongEntries + 1 : wrongEntries;
      }
      return wrongEntries;
    }


    private int Should_Match_With_Yen_Currency(IEnumerable<BalanzaValorizadaEntryDto> dollarizedBalance,
                                               IEnumerable<TrialBalanceEntryDto> trialBalance) {
      int wrongEntries = 0;
      foreach (var cascadeEntry in dollarizedBalance.Where(a => a.CurrencyCode == "06")) {
        var balanceEntry = trialBalance.Where(a => a.AccountNumber == cascadeEntry.AccountNumber &&
                                                    a.CurrencyCode == cascadeEntry.CurrencyCode &&
                                                    a.SectorCode == cascadeEntry.SectorCode &&
                                                    (a.ItemType == TrialBalanceItemType.Entry ||
                                                    a.ItemType == TrialBalanceItemType.Summary)
                                                    )
                                        .Sum(a => a.CurrentBalance);
        wrongEntries = cascadeEntry.TotalBalance != balanceEntry ? wrongEntries + 1 : wrongEntries;
      }
      return wrongEntries;
    }

    #endregion

    #region Helpers

    private TrialBalanceQuery GetDefaultTrialBalanceQuery() {
      return new TrialBalanceQuery() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        TrialBalanceType = TrialBalanceType.Balanza,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        InitialPeriod = new BalancesPeriod {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE
        },
        FinalPeriod = new BalancesPeriod {
          FromDate = new DateTime(2021, 06, 01),
          ToDate = new DateTime(2021, 06, 30)
        }
      };
    }

    #endregion Helpers

  } // class TrialBalanceComparatorUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
