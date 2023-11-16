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
      List<IIntegracionSaldosCapitalInteresesEntry> converted = BuildEntries(buildQuery);

      return IntegracionSaldosCapitalInteresesMapper.MapToReportDataDto(buildQuery, converted);
    }


    public List<IIntegracionSaldosCapitalInteresesEntry> BuildEntries(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var entries = new List<IntegracionSaldosCapitalInteresesEntry>(128);

      BuildEntries(entries, buildQuery.ToDate, "2.02.02.05.01", Campo.CapitalCortoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.03.05.01", Campo.CapitalLargoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.02.05.02", Campo.Intereses);

      BuildEntries(entries, buildQuery.ToDate, "2.02.02.06.01", Campo.CapitalCortoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.03.06.01", Campo.CapitalLargoPlazo);
      BuildEntries(entries, buildQuery.ToDate, "2.02.02.06.02", Campo.Intereses);

      SetPrestamosBase(entries);

      var returnEntries = new List<IntegracionSaldosCapitalInteresesEntry>(entries);

      var totals = GetTotalByPrestamoEntries(entries);

      returnEntries.AddRange(totals);

      var totals2 = GetTotalByClassificationAndCurrencyEntries(entries);

      returnEntries.AddRange(totals2);

      returnEntries = Sort(returnEntries);

      return new List<IIntegracionSaldosCapitalInteresesEntry>(returnEntries);
    }


    private void BuildEntries(List<IntegracionSaldosCapitalInteresesEntry> list,
                              DateTime toDate, string accountNumber, Campo field) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances(toDate, accountNumber);

      foreach (var balance in incomeBalances) {
        IntegracionSaldosCapitalInteresesEntry entry = ConvertBalanceToListEntry(balance, field, toDate);

        var item = list.Find(x => x.SubledgerAccount == balance.SubledgerAccountNumber &&
                                  x.CurrencyCode == balance.CurrencyCode &&
                                  x.SectorCode == balance.SectorCode);

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


    private void SetPrestamosBase(List<IntegracionSaldosCapitalInteresesEntry> list) {
      var prestamosInterbancarios = PrestamosInterbancariosList.Parse().GetItems();

      foreach (var item in list) {
        var prestamo = prestamosInterbancarios.Find(x => x.SubledgerAccount.Number == item.SubledgerAccount &&
                                                         x.Currency.Code == item.CurrencyCode &&
                                                         x.Sector.Code == item.SectorCode);
        if (prestamo == null) {
          item.PrestamoBase = PrestamoBase.Unknown;
          item.Classification = PrestamoBase.Unknown.Classification;
        } else {
          item.PrestamoBase = prestamo.PrestamoBase;
          item.Vencimiento = prestamo.Vencimiento;
          item.Classification = prestamo.PrestamoBase.Classification;
        }
      }
    }


    private List<IntegracionSaldosCapitalInteresesEntry> GetTotalByPrestamoEntries(List<IntegracionSaldosCapitalInteresesEntry> entries) {
      return entries.GroupBy(x => new { x.PrestamoBase.UID, x.CurrencyCode })
                    .Select(x => new IntegracionSaldosCapitalInteresesEntry {
                      ItemType = "Total",
                      PrestamoBase = x.First().PrestamoBase,
                      Classification = x.First().Classification,
                      CurrencyCode = x.First().CurrencyCode,
                      CapitalCortoPlazoMonedaOrigen = x.Sum(y => y.CapitalCortoPlazoMonedaOrigen),
                      CapitalLargoPlazoMonedaOrigen = x.Sum(y => y.CapitalLargoPlazoMonedaOrigen),
                      InteresesMonedaOrigen = x.Sum(y => y.InteresesMonedaOrigen),
                      TipoCambio = x.First().TipoCambio,
                      SubledgerAccount = "Total"
                    })
                 .ToList();
    }


    private List<IntegracionSaldosCapitalInteresesEntry> GetTotalByClassificationAndCurrencyEntries(List<IntegracionSaldosCapitalInteresesEntry> entries) {
      return entries.GroupBy(x => new { x.PrestamoBase.Classification, x.CurrencyCode })
           .Select(x => new IntegracionSaldosCapitalInteresesEntry {
             ItemType = "Total",
             PrestamoBase = PrestamoBase.Empty,
             Classification = x.First().Classification,
             CurrencyCode = x.First().CurrencyCode,
             CapitalCortoPlazoMonedaOrigen = x.Sum(y => y.CapitalCortoPlazoMonedaOrigen),
             CapitalLargoPlazoMonedaOrigen = x.Sum(y => y.CapitalLargoPlazoMonedaOrigen),
             InteresesMonedaOrigen = x.Sum(y => y.InteresesMonedaOrigen),
             TipoCambio = x.First().TipoCambio,
             SubledgerAccount = $"Subtotal {x.First().PrestamoBase.GetClassificationName()}"
           })
           .ToList();
    }


    private List<IntegracionSaldosCapitalInteresesEntry> Sort(List<IntegracionSaldosCapitalInteresesEntry> entries) {
       return entries.OrderBy(x => x.Classification)
                     .ThenBy(x => x.CurrencyCode)
                     .ThenBy(x => x.PrestamoBase.Order)
                     .ThenBy(x => x.SubledgerAccount)
                     .ToList();
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

    private decimal GetExchangeRate(string currencyCode, DateTime date) {
      var exchangeRates = ExchangeRate.GetList(ExchangeRateType.ValorizacionBanxico, date);

      var exchangeRate = exchangeRates.Find(x => x.ToCurrency.Code == currencyCode);

      if (exchangeRate == null) {
        Assertion.RequireFail($"No se ha dado de alta el tipo de cambio para la fecha proporcionada.");
      }

      return exchangeRate.Value;
    }

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
