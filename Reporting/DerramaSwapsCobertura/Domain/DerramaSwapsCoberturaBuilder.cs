/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : DerramaSwapsCoberturaBuilder                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates report 'Derrama de intereses de Swaps con fines de cobertura'.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters;

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura {

  /// <summary>Generates report 'Derrama de intereses de Swaps con fines de cobertura'.</summary>
  internal class DerramaSwapsCoberturaBuilder : IReportBuilder {

    #region Public methods


    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var entries = new List<DerramaSwapsCoberturaEntry>(1024);

      BuildIncomeEntries(entries, buildQuery.ToDate);

      BuildExpensesEntries(entries, buildQuery.ToDate);

      SetClassifications(entries);

      Sort(entries);

      SetTotalEntry(entries);

      return DerramaSwapsCoberturaMapper.MapToReportDataDto(buildQuery, entries);
    }


    private void BuildExpensesEntries(List<DerramaSwapsCoberturaEntry> list, DateTime toDate) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances(toDate, "6.01.07");

      foreach (var balance in incomeBalances) {
        DerramaSwapsCoberturaEntry entry = ConvertBalanceToListEntry(balance, false);

        var item = list.Find(x => x.SubledgerAccount == balance.SubledgerAccountNumber);

        if (item == null) {
          list.Add(entry);
        } else {
          item.Sum(entry);
        }
      }
    }


    private void BuildIncomeEntries(List<DerramaSwapsCoberturaEntry> list, DateTime toDate) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances(toDate, "5.01.04");

      foreach (var balance in incomeBalances) {
        DerramaSwapsCoberturaEntry entry = ConvertBalanceToListEntry(balance, true);

        var item = list.Find(x => x.SubledgerAccount == balance.SubledgerAccountNumber);

        if (item == null) {
          list.Add(entry);
        } else {
          item.Sum(entry);
        }
      }
    }


    private DerramaSwapsCoberturaEntry ConvertBalanceToListEntry(SaldosPorCuentaEntryDto balance, bool isIncome) {
      return new DerramaSwapsCoberturaEntry {
        ItemType = "Entry",
        SubledgerAccount = balance.SubledgerAccountNumber,
        SubledgerAccountName = balance.AccountName,
        IncomeAccountTotal = isIncome ? balance.CurrentBalanceForBalances : 0m,
        ExpensesAccountTotal = isIncome ? 0m : -1 * balance.CurrentBalanceForBalances
      };
    }


    private void SetClassifications(List<DerramaSwapsCoberturaEntry> list) {
      var swapsCoberturaList = SwapsCoberturaList.Parse().GetItems();

      foreach (var item in list) {
        var swapCobertura = swapsCoberturaList.Find(x => x.SubledgerAccount.Number == item.SubledgerAccount);

        if (swapCobertura == null) {
          item.Classification = "Sin clasificar";
        } else {
          item.Classification = swapCobertura.Classification;
        }
      }
    }


    private void SetTotalEntry(List<DerramaSwapsCoberturaEntry> list) {
      list.Add(new DerramaSwapsCoberturaEntry {
        ItemType = "Total",
        SubledgerAccountName = "TOTALES",
        IncomeAccountTotal = list.Sum(x => x.IncomeAccountTotal),
        ExpensesAccountTotal = list.Sum(x => x.ExpensesAccountTotal),
      });
    }


    private void Sort(List<DerramaSwapsCoberturaEntry> entries) {
      entries.Sort((x, y) => x.SubledgerAccount.CompareTo(y.SubledgerAccount));
    }


    private FixedList<SaldosPorCuentaEntryDto> GetBalances(DateTime toDate, string accountNumber) {

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery trialBalanceQuery = this.MapToTrialBalanceQuery(toDate, accountNumber);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceQuery);

        var entries = trialBalance.Entries.FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

        return entries.Select(x => (SaldosPorCuentaEntryDto) x)
                      .ToFixedList();
      }
    }

    #endregion Public methods

    #region Private methods


    private TrialBalanceQuery MapToTrialBalanceQuery(DateTime toDate, string account) {

      return new TrialBalanceQuery {
        TrialBalanceType = TrialBalanceType.SaldosPorCuenta,
        AccountsChartUID = AccountsChart.IFRS.UID,
        BalancesType = BalancesType.WithCurrentBalance,
        ShowCascadeBalances = false,
        WithSubledgerAccount = true,
        WithAverageBalance = false,
        FromAccount = account,
        ToAccount = account,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(toDate.Year, toDate.Month, 1),
          ToDate = toDate
        },
        IsOperationalReport = true,
      };

    }


    #endregion Private methods

  } // class DerramaSwapsCoberturaBuilder

} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura
