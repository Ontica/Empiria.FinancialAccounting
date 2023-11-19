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
      List<IIntegracionSaldosCapitalInteresesEntry> converted = BuildEntries(buildQuery, true);

      return IntegracionSaldosCapitalInteresesMapper.MapToReportDataDto(buildQuery, converted);
    }


    public List<IIntegracionSaldosCapitalInteresesEntry> BuildEntries(ReportBuilderQuery buildQuery,
                                                                      bool includeBaseEntries) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      var baseEntries = new List<IntegracionSaldosCapitalInteresesEntry>(128);

      BuildEntries(baseEntries, buildQuery.ToDate, "2.02.02.05.01", Campo.CapitalCortoPlazo);
      BuildEntries(baseEntries, buildQuery.ToDate, "2.02.03.05.01", Campo.CapitalLargoPlazo);
      BuildEntries(baseEntries, buildQuery.ToDate, "2.02.02.05.02", Campo.Intereses);

      BuildEntries(baseEntries, buildQuery.ToDate, "2.02.02.06.01", Campo.CapitalCortoPlazo);
      BuildEntries(baseEntries, buildQuery.ToDate, "2.02.03.06.01", Campo.CapitalLargoPlazo);
      BuildEntries(baseEntries, buildQuery.ToDate, "2.02.02.06.02", Campo.Intereses);

      SetPrestamosBase(baseEntries);

      var entries = new List<IntegracionSaldosCapitalInteresesEntry>();

      if (includeBaseEntries) {
        entries.AddRange(baseEntries);
      }

      var totals = GetTotalsByPrestamo(baseEntries);

      entries.AddRange(totals);

      var totals2 = GetTotalsByClassificationAndCurrency(baseEntries);

      entries.AddRange(totals2);

      entries = Sort(entries);

      var totals3 = GetTotalsByClassification(baseEntries);

      List<IIntegracionSaldosCapitalInteresesEntry> returnEntries = MergeTotalsByClassification(entries, totals3);

      return returnEntries;
    }


    private List<IIntegracionSaldosCapitalInteresesEntry> MergeTotalsByClassification(List<IntegracionSaldosCapitalInteresesEntry> entries,
                                                                                      List<IntegracionSaldosCapitalInteresesSubTotal> totals) {
      List<IIntegracionSaldosCapitalInteresesEntry> returnEntries = new List<IIntegracionSaldosCapitalInteresesEntry>(entries);

      foreach (var total in totals) {
        int index = returnEntries.FindIndex(x => x.Classification == total.Classification);

        returnEntries.Insert(index, new IntegracionSaldosCapitalInteresesTitle {
          Title = total.Classification.DisplayName(),
          Classification = total.Classification
        });

        index = returnEntries.FindLastIndex(x => x.Classification == total.Classification);

        returnEntries.Insert(index + 1, total);
        returnEntries.Insert(index + 2, new IntegracionSaldosCapitalInteresesTitle { Classification = total.Classification });
        returnEntries.Insert(index + 3, new IntegracionSaldosCapitalInteresesTitle { Classification = total.Classification });
      }

      return returnEntries;
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


    private List<IntegracionSaldosCapitalInteresesEntry> GetTotalsByPrestamo(List<IntegracionSaldosCapitalInteresesEntry> entries) {
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
                      SubledgerAccount = $"Préstamo {x.First().PrestamoBase.Name} ({Currency.Parse(x.First().CurrencyCode).ShortName})",
                    })
                 .ToList();
    }


    private List<IntegracionSaldosCapitalInteresesEntry> GetTotalsByClassificationAndCurrency(List<IntegracionSaldosCapitalInteresesEntry> entries) {
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
             SubledgerAccount = $"{x.First().PrestamoBase.Classification.DisplayName()} ({Currency.Parse(x.First().CurrencyCode).ShortName})"
           })
           .ToList();
    }


    private List<IntegracionSaldosCapitalInteresesSubTotal> GetTotalsByClassification(List<IntegracionSaldosCapitalInteresesEntry> entries) {
      return entries.GroupBy(x => x.PrestamoBase.Classification)
           .Select(x => new IntegracionSaldosCapitalInteresesSubTotal {
             Title = "Total " + x.First().Classification.DisplayName(),
             Classification = x.First().Classification,
             CapitalMonedaNacional = x.Sum(y => y.CapitalMonedaNacional),
             InteresesMonedaNacional = x.Sum(y => y.InteresesMonedaNacional),
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
