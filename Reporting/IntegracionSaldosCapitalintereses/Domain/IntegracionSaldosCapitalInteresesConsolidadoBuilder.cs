/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                                    Component : Report Builders              *
*  Assembly : FinancialAccounting.Reporting.dll                     Pattern   : Report builder               *
*  Type     : IntegracionSaldosCapitalInteresesConsolidadoBuilder   License   : Please read LICENSE.txt file *
*                                                                                                            *
*  Summary  : Generates report 'Integración de saldos de capital e intereses'.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses {

  /// <summary>Generates report 'Integración de saldos de capital e intereses'.</summary>
  internal class IntegracionSaldosCapitalInteresesConsolidadoBuilder : IReportBuilder {

    private enum Campo {

      Capital,

      Intereses

    }

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      var baseBuilder = new IntegracionSaldosCapitalInteresesBuilder();

      List<IIntegracionSaldosCapitalInteresesEntry> data = baseBuilder.BuildEntries(buildQuery, false);

      var total1 = (IntegracionSaldosCapitalInteresesSubTotal) data[data.Count - 1];

      total1.Title = "SUBTOTAL PRESTAMOS INTERBANCARIOS Y DE OTROS ORGANISMOS";

      data.Add(new IntegracionSaldosCapitalInteresesTitle { Classification = AccountsLists.SpecialCases.PrestamoBaseClasificacion.RegistroBackOffice });
      data.Add(new IntegracionSaldosCapitalInteresesTitle { Classification = AccountsLists.SpecialCases.PrestamoBaseClasificacion.RegistroBackOffice });
      data.Add(new IntegracionSaldosCapitalInteresesTitle { Classification = AccountsLists.SpecialCases.PrestamoBaseClasificacion.RegistroBackOffice,
                                                            Title = "REGISTROS BACK OFFICE (1)"});

      var registrosBackOffice = GetRegistrosBackOffice(buildQuery);

      data.AddRange(registrosBackOffice);

      var total2 = (IntegracionSaldosBackOfficeTotal) data[data.Count - 1];

      data.Add(new IntegracionSaldosCapitalInteresesTitle { Classification = AccountsLists.SpecialCases.PrestamoBaseClasificacion.RegistroBackOffice });

      data.Add(GetGrandTotal(total1, total2));

      return IntegracionSaldosCapitalInteresesConsolidadoMapper.MapToReportDataDto(buildQuery, data);
    }


    #endregion Public methods

    #region Private methods

    private IEnumerable<IIntegracionSaldosCapitalInteresesEntry> GetRegistrosBackOffice(ReportBuilderQuery buildQuery) {

      var baseEntries = new List<IntegracionSaldosBackOfficeEntry>();

      var returnedEntries = new List<IIntegracionSaldosCapitalInteresesEntry>();

      BuildEntry(baseEntries, buildQuery.ToDate, BackOfficeRow.CallMoneyMXN, Currency.MXN);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.CallMoneyMXN, "2.02.01.01", Currency.MXN, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.CallMoneyMXN, "2.02.01.02", Currency.MXN, Campo.Intereses);

      BuildEntry(baseEntries, buildQuery.ToDate, BackOfficeRow.CallMoneyUSD, Currency.USD);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.CallMoneyUSD, "2.02.01.01", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.CallMoneyUSD, "2.02.01.02", Currency.USD, Campo.Intereses);

      BuildEntry(baseEntries, buildQuery.ToDate, BackOfficeRow.SubastasMXN, Currency.MXN);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.SubastasMXN, "2.02.02.01.02.01", Currency.MXN, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.SubastasMXN, "2.02.02.01.02.02", Currency.MXN, Campo.Intereses);

      BuildEntry(baseEntries, buildQuery.ToDate, BackOfficeRow.PrestamosCortoPlazoUSD, Currency.USD);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.PrestamosCortoPlazoUSD, "2.02.02.04.01", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.PrestamosCortoPlazoUSD, "2.02.02.04.02", Currency.USD, Campo.Intereses);

      returnedEntries.AddRange(baseEntries);

      returnedEntries.Add(GetBackOfficeSubtotal(baseEntries));

      BuildEntry(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, Currency.USD);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.01.01.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.01.02.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.01.03.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.02.01.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.02.02.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.03.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.04.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.05.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.06.03", Currency.USD, Campo.Capital);
      BuildEntries(baseEntries, buildQuery.ToDate, BackOfficeRow.EfectoValuacionDerivados, "2.02.02.07.03", Currency.USD, Campo.Capital);

      returnedEntries.Add(baseEntries.Find(x => x.BackOfficeEntry == BackOfficeRow.EfectoValuacionDerivados));

      returnedEntries.Add(GetBackOfficeTotal(baseEntries));

      return returnedEntries;
    }

    private IntegracionSaldosBackOfficeTotal GetBackOfficeTotal(List<IntegracionSaldosBackOfficeEntry> baseEntries) {
      return new IntegracionSaldosBackOfficeTotal {
        BackOfficeEntry = BackOfficeRow.Total,
        CapitalMonedaNacional = baseEntries.FindAll(x => x.BackOfficeEntry != BackOfficeRow.Subtotal)
                                        .Sum(x => x.CapitalMonedaNacional),
        InteresesMonedaNacional = baseEntries.FindAll(x => x.BackOfficeEntry != BackOfficeRow.Subtotal)
                                        .Sum(x => x.InteresesMonedaNacional),
      };
    }

    private IntegracionSaldosBackOfficeTotal GetBackOfficeSubtotal(List<IntegracionSaldosBackOfficeEntry> baseEntries) {
      return new IntegracionSaldosBackOfficeTotal {
        BackOfficeEntry = BackOfficeRow.Subtotal,
        CapitalMonedaNacional = baseEntries.Sum(x => x.CapitalMonedaNacional),
        InteresesMonedaNacional = baseEntries.Sum(x => x.InteresesMonedaNacional),
      };
    }

    private IIntegracionSaldosCapitalInteresesEntry GetGrandTotal(IntegracionSaldosCapitalInteresesSubTotal total1,
                                                                  IntegracionSaldosBackOfficeTotal total2) {
      return new IntegracionSaldosCapitalInteresesSubTotal {
        Classification = AccountsLists.SpecialCases.PrestamoBaseClasificacion.Total,
        Title = "TOTAL PRESTAMOS INTERBANCARIOS Y DE OTROS ORGANISMOS",
        CapitalMonedaNacional = total1.CapitalMonedaNacional + total2.CapitalMonedaNacional,
        InteresesMonedaNacional = total1.InteresesMonedaNacional + total2.InteresesMonedaNacional,
      };
    }

    private void BuildEntry(List<IntegracionSaldosBackOfficeEntry> baseEntries, DateTime date, BackOfficeRow backOfficeEntry, Currency currency) {
      var entry = new IntegracionSaldosBackOfficeEntry {
        BackOfficeEntry = backOfficeEntry,
        CurrencyCode = currency.Code,
        TipoCambio = GetExchangeRate(currency.Code, date),
      };

      baseEntries.Add(entry);
    }

    private void BuildEntries(List<IntegracionSaldosBackOfficeEntry> list,
                              DateTime toDate, BackOfficeRow backofficeRow, string accountNumber, Currency currency, Campo field) {
      FixedList<SaldosPorCuentaEntryDto> incomeBalances = GetBalances(toDate, accountNumber, currency);

      foreach (var balance in incomeBalances) {
        IntegracionSaldosBackOfficeEntry entry = ConvertBalanceToListEntry(backofficeRow, balance, field, toDate);

        var item = list.Find(x => x.BackOfficeEntry == backofficeRow);

        if (item == null) {
          list.Add(entry);
        } else {
          item.Sum(entry);
        }
      }
    }

    private IntegracionSaldosBackOfficeEntry ConvertBalanceToListEntry(BackOfficeRow backOfficeEntry,
                                                                       SaldosPorCuentaEntryDto balance,
                                                                       Campo campo, DateTime date) {
      return new IntegracionSaldosBackOfficeEntry {
        BackOfficeEntry = backOfficeEntry,
        CurrencyCode = balance.CurrencyCode,
        TipoCambio = GetExchangeRate(balance.CurrencyCode, date),
        CapitalMonedaOrigen = campo == Campo.Capital ? balance.CurrentBalanceForBalances : 0m,
        InteresesMonedaOrigen = campo == Campo.Intereses ? balance.CurrentBalanceForBalances : 0m,
      };
    }


    private FixedList<SaldosPorCuentaEntryDto> GetBalances(DateTime toDate, string accountNumber, Currency currency) {

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery trialBalanceQuery = this.MapToTrialBalanceQuery(toDate, accountNumber);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceQuery);

        var entries = trialBalance.Entries.FindAll(x => x.ItemType == TrialBalanceItemType.Entry);

        return entries.Select(x => (SaldosPorCuentaEntryDto) x)
                      .ToFixedList()
                      .FindAll(x => x.CurrencyCode == currency.Code);
      }
    }

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
        BalancesType = BalancesType.WithCurrentBalance,
        ShowCascadeBalances = false,
        WithSubledgerAccount = false,
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

  } // class IntegracionSaldosCapitalInteresesConsolidadoBuilder

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses
