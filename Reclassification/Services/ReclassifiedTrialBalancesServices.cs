/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification                           Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Use case interactor class               *
*  Type     : ReclassifiedTrialBalancesServices          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services that build and return reclassified trial balances.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;

using Empiria.DynamicData;
using Empiria.Services;

using Empiria.FinancialAccounting.Reclassification.Adapters;
using Empiria.FinancialAccounting.Reclassification.Data;

namespace Empiria.FinancialAccounting.Reclassification.Services {

  /// <summary>Provides services that build and return reclassified trial balances.</summary>
  public class ReclassifiedTrialBalancesServices : UseCase {

    #region Constructors and parsers

    protected ReclassifiedTrialBalancesServices() {
      // no-op
    }

    static public ReclassifiedTrialBalancesServices UseCaseInteractor() {
      return UseCase.CreateInstance<ReclassifiedTrialBalancesServices>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public DynamicDto<BalanzaEnColumnasRealDto> BalanzaEnColumnas(DateTime fromDate, DateTime toDate) {

      FixedList<AccountReclassifiedBalances> balances =
                            ReclassifiedBalancesDataService.GetBalances(fromDate, toDate);

      FixedList<BalanzaReal> balanzaReal = MapToBalanzaReal(balances);

      return BalanzaEnMonedasMapper.Map(balanzaReal);
    }


    public DynamicDto<BalanzaTradicionalRealDto> BalanzaTradicional(DateTime fromDate, DateTime toDate) {

      FixedList<AccountReclassifiedBalances> balances =
                            ReclassifiedBalancesDataService.GetBalances(fromDate, toDate);

      return BalanzaValorizadaRealMapper.Map(balances);
    }

    //public DynamicDto<BalanzaValorizadaEntry> BalanzaValorizada(TrialBalanceQuery query) {
    //  Assertion.Require(query, nameof(query));

    //  var period = new TimePeriod(query.InitialPeriod.FromDate, query.InitialPeriod.ToDate);

    //  FixedList<MovimientosPorDia> movsDiarios = MovimientosPorDia.GetList(period);

    //  movsDiarios = movsDiarios.FindAll(x => x.CuentaEstandar.Number.StartsWith("1") ||
    //                                         x.CuentaEstandar.Number.StartsWith("2") ||
    //                                         x.CuentaEstandar.Number.StartsWith("3"));

    //  movsDiarios = movsDiarios.OrderBy(x => x.CuentaEstandar.Number)
    //                           .ThenBy(x => x.FechaAfectacion)
    //                           .ToFixedList();

    //  var initialBalanceQuery = new TrialBalanceQuery() {
    //    AccountsChartUID = AccountsChart.IFRS.UID,
    //    TrialBalanceType = TrialBalanceType.Balanza,
    //    FromAccount = "1",
    //    ToAccount = "3.99",
    //    InitialPeriod = new BalancesPeriod {
    //      FromDate = period.StartTime,
    //      ToDate = period.EndTime
    //    },
    //    BalancesType = BalancesType.AllAccounts,
    //  };

    //  var intialBalancesBuilder = new BalanzaTradicionalBuilder(initialBalanceQuery);

    //  var initialBalances = intialBalancesBuilder.BuildBalanzaTradicional()
    //                                             .Entries.Cast<TrialBalanceEntry>()
    //                                             .ToFixedList();

    //  var exchangeRates = ExchangeRate.GetList(period.StartTime.AddDays(-1), period.EndTime);

    //  var builder = new BalanzaValorizadaBuilder(initialBalances, movsDiarios, exchangeRates);

    //  FixedList<BalanzaValorizadaEntry> entries = builder.Build(period);

    //  FixedList<BalanzaValorizadaEntry> summaries = builder.BuildSummaryAccounts(entries);

    //  entries = FixedList<BalanzaValorizadaEntry>.MergeDistinct(summaries, entries);

    //  entries = entries.OrderBy(x => x.NumeroCuenta)
    //                   .ThenBy(x => x.Moneda.Code)
    //                   .ThenBy(x => x.FechaAfectacion)
    //                   .ToFixedList();

    //  return new DynamicDto<BalanzaValorizadaEntry>(query, builder.GetColumns(), entries);
    //}

    #endregion Use cases

    #region Helpers

    private FixedList<BalanzaReal> MapToBalanzaReal(FixedList<AccountReclassifiedBalances> balances) {
      var idCuentaStandard = -1;

      BalanzaReal entryBalance = new BalanzaReal();
      List<BalanzaReal> balanzaEntries = new List<BalanzaReal>();

      balances = balances.Sort((x, y) => x.StdAccount.Number.CompareTo(y.StdAccount.Number));

      foreach (var entry in balances) {

        if (entry.StdAccount.Id != idCuentaStandard) {
          balanzaEntries.Add(entryBalance);
          entryBalance = new BalanzaReal();

          entryBalance.CuentaEstandar = entry.StdAccount;
          idCuentaStandard = entry.StdAccount.Id;
        }

        var currencyBalance = new CurrencyBalance {
          InitialBalance = entry.InitialBalance,
          Credits = entry.Credits,
          Debits = entry.Debits,
          FinalBalance = entry.FinalBalance,
          Currency = entry.Currency
        };

        entryBalance.SaldosPorMoneda.Add(currencyBalance);

        var realCurrencyBalance = new CurrencyBalance {
          InitialBalance = entry.RealInitialBalance,
          Credits = entry.RealCredits,
          Debits = entry.RealDebits,
          FinalBalance = entry.RealFinalBalance,
          Currency = entry.RealCurrency
        };

        entryBalance.SaldosPorMonedaReal.Add(realCurrencyBalance);
      }

      balanzaEntries.RemoveAt(0);

      return balanzaEntries.ToFixedList();
    }

    #endregion Helpers

  } // class ReclassifiedTrialBalancesServices

} // namespace Empiria.FinancialAccounting.Reclassification.Services
