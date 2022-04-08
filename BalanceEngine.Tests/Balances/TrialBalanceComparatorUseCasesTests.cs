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
using Xunit;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine;
using System.Linq;

namespace Empiria.FinancialAccounting.Tests.Balances {

  /// <summary>Test cases for comparison between trial balance reports.</summary>
  public class TrialBalanceComparatorUseCasesTests {

    #region Fields

    private readonly TrialBalanceComparatorUseCases _usecases;

    #endregion Fields

    #region Initialization

    public TrialBalanceComparatorUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = TrialBalanceComparatorUseCases.UseCaseInteractor();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Be_Same_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.GeneracionDeSaldos;
      command.ShowCascadeBalances = false;
      command.WithSubledgerAccount = false;

      TrialBalanceDto balances = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);

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
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.ShowCascadeBalances = false;
      command.UseDefaultValuation = true;
      command.WithSubledgerAccount = true;
      command.FromAccount = "1";
      command.ToAccount = "1";

      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto twoColumns = _usecases.BuildBalances(command);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(twoColumns);
      Assert.NotEmpty(twoColumns.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _twoColumnsEntries = twoColumns.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x);
      var domesticWrongEntries = 0;

      foreach (var entry in _twoColumnsEntries.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var balances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          a.AccountNumberForBalances == entry.AccountNumberForBalances &&
                                          (a.CurrencyCode == "01" || a.CurrencyCode == "44") &&
                                          a.LedgerNumber == entry.LedgerNumber &&
                                          a.SectorCode == entry.SectorCode &&
                                          a.DebtorCreditor == entry.DebtorCreditor.ToString()
                                         ).Sum(a => a.CurrentBalance);

        domesticWrongEntries = entry.DomesticBalance != balances ? domesticWrongEntries + 1 : domesticWrongEntries;
        if (domesticWrongEntries > 0) {
          domesticWrongEntries = 0;
        }
      }
      Assert.True(domesticWrongEntries == 0);
    }


    [Fact]
    public void Should_Be_Same_Foreign_Balance_With_Analytics() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = false;
      command.UseDefaultValuation = true;

      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto twoColumns = _usecases.BuildBalances(command);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(twoColumns);
      Assert.NotEmpty(twoColumns.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _twoColumnsEntries = twoColumns.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x);
      var foreignWrongEntries = 0;

      foreach (var entry in _twoColumnsEntries.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var foreignBalances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          a.AccountNumberForBalances == entry.AccountNumberForBalances &&
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
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = true;

      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.BalanzaConContabilidadesEnCascada;
      TrialBalanceDto cascadeBalance = _usecases.BuildBalances(command);

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
    public void Should_Match_Currencies_With_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = false;

      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.BalanzaEnColumnasPorMoneda;
      TrialBalanceDto balancesByCurrency = _usecases.BuildBalances(command);

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
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.BalanzaDolarizada;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = false;
      command.UseDefaultValuation = true;

      TrialBalanceDto dollarizedBalance = _usecases.BuildBalances(command);
      command.UseDefaultValuation = false;
      command.InitialPeriod.UseDefaultValuation = false;
      command.InitialPeriod.ValuateToCurrrencyUID = "";
      command.InitialPeriod.ExchangeRateTypeUID = "";
      command.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(dollarizedBalance);
      Assert.NotEmpty(dollarizedBalance.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _dollarizedBalance = dollarizedBalance.Entries.Select(x => (ValuedTrialBalanceDto) x);
      var wrongEntries = 0;

      foreach (var cascadeEntry in _dollarizedBalance.Where(a => a.ItemType == TrialBalanceItemType.Summary)) {
        var balanceEntry = _trialBalance.Where(a => a.AccountNumber == cascadeEntry.AccountNumber &&
                                                    a.CurrencyCode == cascadeEntry.CurrencyCode &&
                                                    a.SectorCode == cascadeEntry.SectorCode &&
                                                    (a.ItemType == TrialBalanceItemType.Entry ||
                                                    a.ItemType == TrialBalanceItemType.Summary)
                                                    )
                                        .ToList();
        if (balanceEntry.Count > 0) {
          decimal balance = (decimal) balanceEntry.Sum(a => a.CurrentBalance);
          wrongEntries = cascadeEntry.TotalEquivalence != balance ? wrongEntries + 1 : wrongEntries;
        }
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Match_BalanceBySubledger_With_Trial_Balance() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.SaldosPorAuxiliar;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalance;
      command.ShowCascadeBalances = true;
      command.WithSubledgerAccount = true;
      command.FromAccount = "1";
      command.ToAccount = "1";

      TrialBalanceDto balanceBySubledger = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);

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
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalance;
      command.ShowCascadeBalances = false;
      command.TrialBalanceType = TrialBalanceType.SaldosPorCuenta;

      TrialBalanceDto balanceByAccount = _usecases.BuildBalances(command);
      command.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = _usecases.BuildBalances(command);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(balanceByAccount);
      Assert.NotEmpty(balanceByAccount.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _balanceByAccount = balanceByAccount.Entries.Select(x => (TrialBalanceEntryDto) x);
      var wrongEntries = 0;

      foreach (var entry in _balanceByAccount.Where(a => a.ItemType == TrialBalanceItemType.Entry)) {
        var balanceEntry = _trialBalance.Where(a => a.AccountNumber == entry.AccountNumber &&
                                                    a.CurrencyCode == entry.CurrencyCode &&
                                                    a.LedgerNumber == entry.LedgerNumber &&
                                                    a.SectorCode == entry.SectorCode &&
                                                    a.ItemType == TrialBalanceItemType.Entry
                                                    ).Sum(a => a.CurrentBalance);
        wrongEntries = entry.CurrentBalance != balanceEntry ?
                       wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }

    #endregion Facts


    #region Helpers

    private TrialBalanceCommand GetDefaultTrialBalanceCommand() {
      return new TrialBalanceCommand() {
        AccountsChartUID = TestingConstants.ACCOUNTS_CHART_UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        TrialBalanceType = TrialBalanceType.Balanza,
        Ledgers = TestingConstants.BALANCE_LEDGERS_ARRAY,
        InitialPeriod = new TrialBalanceCommandPeriod() {
          FromDate = TestingConstants.FROM_DATE,
          ToDate = TestingConstants.TO_DATE
          //ExchangeRateDate = new DateTime(2021, 06, 30),
          //ExchangeRateTypeUID = "96c617f6-8ed9-47f3-8d2d-f1240e446e1d",
          //ValuateToCurrrencyUID = "01"
        },
        FinalPeriod = new TrialBalanceCommandPeriod() {
          FromDate = new DateTime(2021, 06, 01),
          ToDate = new DateTime(2021, 06, 30)
        }

      };
    }

    #endregion Helpers

  } // class TrialBalanceComparatorUseCasesTests

} // namespace Empiria.FinancialAccounting.Tests.Balances
