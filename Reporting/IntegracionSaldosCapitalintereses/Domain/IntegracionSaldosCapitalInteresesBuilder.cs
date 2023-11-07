/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : IntegracionSaldosCapitalInteresesBuilder      License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates report 'Integración de saldos de capital e intereses'.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses {

  /// <summary>Generates report 'Integración de saldos de capital e intereses'.</summary>
  internal class IntegracionSaldosCapitalInteresesBuilder : IReportBuilder {

    private enum Campo {
      CapitalCortoPlazo,
      CapitalLargoPlazo,
      Intereses
    }

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var entries = new List<IntegracionSaldosCapitalInteresesEntry>(128);

      BuildEntries(entries, buildQuery.ToDate, "2.02.02.05.01", Campo.CapitalCortoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.03.05.01", Campo.CapitalLargoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.02.05.02", Campo.Intereses);

      BuildEntries(entries, buildQuery.ToDate, "2.02.02.06.01", Campo.CapitalCortoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.03.06.01", Campo.CapitalLargoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.02.06.02", Campo.Intereses);

      SetPrestamos(entries);

      entries = Sort(entries);

      // SetTotalEntry(entries);

      return IntegracionSaldosCapitalInteresesMapper.MapToReportDataDto(buildQuery, entries);
    }

    private void BuildEntries(List<IntegracionSaldosCapitalInteresesEntry> list,
                              DateTime toDate, string accountNumber, Campo field) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances(toDate, accountNumber);

      foreach (var balance in incomeBalances) {
        IntegracionSaldosCapitalInteresesEntry entry = ConvertBalanceToListEntry(balance, field, toDate);

        var item = list.Find(x => x.SubledgerAccount == balance.SubledgerAccountNumber);

        if (item == null) {
          list.Add(entry);
        } else {
          item.Sum(entry);
        }
      }
    }


    private IntegracionSaldosCapitalInteresesEntry ConvertBalanceToListEntry(SaldosPorCuentaEntryDto balance,
                                                                             Campo campo, DateTime date) {
      return new IntegracionSaldosCapitalInteresesEntry {
        ItemType = "Entry",
        SubledgerAccount = balance.SubledgerAccountNumber,
        SubledgerAccountName = balance.AccountName,
        CurrencyCode = balance.CurrencyCode,
        SectorCode = balance.SectorCode,
        TipoCambio = GetExchangeRate(balance.CurrencyCode, date),
        CapitalCortoPlazoMonedaOrigen = campo == Campo.CapitalCortoPlazo ? balance.CurrentBalanceForBalances : 0m,
        CapitalLargoPlazoMonedaOrigen = campo == Campo.CapitalLargoPlazo ? balance.CurrentBalanceForBalances : 0m,
        InteresesMonedaOrigen = campo == Campo.Intereses ? balance.CurrentBalanceForBalances : 0m
      };
    }

    private decimal GetExchangeRate(string currencyCode, DateTime date) {
      var exchangeRates = ExchangeRate.GetList(ExchangeRateType.ValorizacionBanxico, date);

      return exchangeRates.Find(x => x.ToCurrency.Code == currencyCode).Value;
    }

    private void SetPrestamos(List<IntegracionSaldosCapitalInteresesEntry> list) {
      var prestamosInterbancarios = PrestamosInterbancariosList.Parse().GetItems();

      foreach (var item in list) {
        var prestamo = prestamosInterbancarios.Find(x => x.SubledgerAccount.Number == item.SubledgerAccount &&
                                                         x.Currency.Code == item.CurrencyCode &&
                                                         x.Sector.Code == item.SectorCode);
        if (prestamo == null) {
          item.Prestamo = new PrestamosInterbancariosListItem();
        } else {
          item.Prestamo = prestamo;
        }
      }
    }


    private void SetTotalEntry(List<IntegracionSaldosCapitalInteresesEntry> list) {
      decimal incomeAccountTotal = 0m;
      decimal expensesAccountTotal = 0m;
      decimal total = 0m;

      foreach (var entry in list) {
        incomeAccountTotal += entry.CapitalCortoPlazoMonedaOrigen;
        expensesAccountTotal += entry.InteresesMonedaOrigen;
        total += entry.TotalMonedaOrigen;
      }

      list.Add(new IntegracionSaldosCapitalInteresesEntry {
       ItemType = "Total",
       SubledgerAccountName = "TOTALES",
       CapitalCortoPlazoMonedaOrigen = incomeAccountTotal,
       InteresesMonedaOrigen = expensesAccountTotal
      });
    }


    private List<IntegracionSaldosCapitalInteresesEntry> Sort(List<IntegracionSaldosCapitalInteresesEntry> entries) {
      return entries.OrderBy(x => x.Prestamo.Prestamo.Order)
                    .ThenBy(x => x.SubledgerAccount).ToList();
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
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
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

  } // class IntegracionSaldosCapitalInteresesBuilder

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses
