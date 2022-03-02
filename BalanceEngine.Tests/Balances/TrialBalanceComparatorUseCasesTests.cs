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
      command.WithSubledgerAccount = false;
      command.BalancesType = BalancesType.AllAccounts;
      command.ShowCascadeBalances = false;
      command.FromAccount = "1101";
      command.ToAccount = "1299";

      TrialBalanceDto balances = _usecases.BuildBalancesByAccount(command);
      
      command.TrialBalanceType = TrialBalanceType.Balanza;
      TrialBalanceDto trialBalance = _usecases.BuildBalancesByAccount(command);

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
        wrongEntries = entry != null && balance.CurrentBalance != entry.CurrentBalance ? wrongEntries + 1 : wrongEntries;
      }
      Assert.True(wrongEntries == 0);
    }


    [Fact]
    public void Should_Be_Same_Domestic_Balance_With_Analytics() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = false;
      command.WithSubledgerAccount = false;
      command.UseDefaultValuation = true;
      command.FromAccount = "1";
      command.ToAccount = "1";

      TrialBalanceDto trialBalance = _usecases.BuildBalancesByAccount(command);

      command.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto twoColumns = _usecases.BuildBalancesByAccount(command);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(twoColumns);
      Assert.NotEmpty(twoColumns.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _twoColumns = twoColumns.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x);
      var domesticWrongEntries = 0;
      var foreignWrongEntries = 0;

      foreach (var entry in _twoColumns.Where((a => a.ItemType == TrialBalanceItemType.Entry ||
                                               a.ItemType == TrialBalanceItemType.Summary))) {
        var Balances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          a.AccountNumberForBalances == entry.AccountNumberForBalances &&
                                          (a.CurrencyCode == "01" || a.CurrencyCode == "44") &&
                                          a.LedgerNumber == entry.LedgerNumber &&
                                          a.SectorCode == entry.SectorCode &&
                                          a.DebtorCreditor == entry.DebtorCreditor.ToString()
                                         ).ToList();

        decimal currentBalance = 0;
        if (Balances.Count > 0) {
          currentBalance += (decimal) Balances.Sum(a => a.CurrentBalance);
          domesticWrongEntries = entry.DomesticBalance != currentBalance ? domesticWrongEntries + 1 : domesticWrongEntries;
        }
      }
      Assert.True(domesticWrongEntries == 0);

      foreach (var entry in _twoColumns.Where((a => a.ItemType == TrialBalanceItemType.Entry ||
                                               a.ItemType == TrialBalanceItemType.Summary))) {
        var foreignBalances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          a.AccountNumberForBalances == entry.AccountNumberForBalances &&
                                          a.CurrencyCode != "01" && a.CurrencyCode != "44" &&
                                          a.LedgerNumber == entry.LedgerNumber &&
                                          a.SectorCode == entry.SectorCode &&
                                          a.DebtorCreditor == entry.DebtorCreditor.ToString()
                                         ).ToList();

        decimal foreignCurrentBalance = 0;
        if (foreignBalances.Count > 0) {
          foreignCurrentBalance += (decimal) foreignBalances.Sum(a => a.CurrentBalance);
          foreignWrongEntries = entry.ForeignBalance != foreignCurrentBalance ? foreignWrongEntries + 1 : foreignWrongEntries;
        }
      }
      Assert.True(foreignWrongEntries == 0);
    }


    [Fact]
    public void Should_Be_Same_Foreign_Balance_With_Analytics() {
      TrialBalanceCommand command = GetDefaultTrialBalanceCommand();
      command.TrialBalanceType = TrialBalanceType.Balanza;
      command.AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a";
      command.BalancesType = BalancesType.WithCurrentBalanceOrMovements;
      command.ShowCascadeBalances = false;
      command.WithSubledgerAccount = false;
      command.UseDefaultValuation = true;
      command.FromAccount = "1";
      command.ToAccount = "1";

      TrialBalanceDto trialBalance = _usecases.BuildBalancesByAccount(command);

      command.TrialBalanceType = TrialBalanceType.AnaliticoDeCuentas;
      TrialBalanceDto twoColumns = _usecases.BuildBalancesByAccount(command);

      Assert.NotNull(trialBalance);
      Assert.NotEmpty(trialBalance.Entries);
      Assert.NotNull(twoColumns);
      Assert.NotEmpty(twoColumns.Entries);

      var _trialBalance = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);
      var _twoColumns = twoColumns.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x);
      var foreignWrongEntries = 0;

      foreach (var entry in _twoColumns.Where((a => a.ItemType == TrialBalanceItemType.Entry ||
                                               a.ItemType == TrialBalanceItemType.Summary))) {
        var foreignBalances = _trialBalance.Where(
                                          a => a.AccountNumber == entry.AccountNumber &&
                                          a.AccountNumberForBalances == entry.AccountNumberForBalances &&
                                          a.CurrencyCode != "01" && a.CurrencyCode != "44" &&
                                          a.LedgerNumber == entry.LedgerNumber &&
                                          a.SectorCode == entry.SectorCode &&
                                          a.DebtorCreditor == entry.DebtorCreditor.ToString()
                                         ).ToList();

        decimal foreignCurrentBalance = 0;
        if (foreignBalances.Count > 0) {
          foreignCurrentBalance += (decimal) foreignBalances.Sum(a => a.CurrentBalance);
          foreignWrongEntries = entry.ForeignBalance != foreignCurrentBalance ? foreignWrongEntries + 1 : foreignWrongEntries;
        }
      }
      Assert.True(foreignWrongEntries == 0);
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
          ToDate = TestingConstants.TO_DATE,
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
