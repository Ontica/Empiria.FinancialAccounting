/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalancesUseCasesV3                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build trial balances version 3.0.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Linq;

using Empiria.DynamicData;
using Empiria.Services;
using Empiria.Time;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary> Use cases used to build trial balances version 3.0.</summary>
  public class TrialBalancesUseCasesV3 : UseCase {

    #region Constructors and parsers

    protected TrialBalancesUseCasesV3() {
      // no-op
    }

    static public TrialBalancesUseCasesV3 UseCaseInteractor() {
      return UseCase.CreateInstance<TrialBalancesUseCasesV3>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public DynamicDto<BalanzaValorizadaEntry> BalanzaValorizada(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      var period = new TimePeriod(query.InitialPeriod.FromDate, query.InitialPeriod.ToDate);

      FixedList<MovimientosPorDia> movsDiarios = MovimientosPorDia.GetList(period);

      movsDiarios = movsDiarios.FindAll(x => x.CuentaEstandar.Number.StartsWith("1") ||
                                             x.CuentaEstandar.Number.StartsWith("2") ||
                                             x.CuentaEstandar.Number.StartsWith("3"));

      movsDiarios = movsDiarios.OrderBy(x => x.CuentaEstandar.Number)
                               .ThenBy(x => x.FechaAfectacion)
                               .ToFixedList();

      var initialBalanceQuery = new TrialBalanceQuery() {
        AccountsChartUID = AccountsChart.IFRS.UID,
        TrialBalanceType = TrialBalanceType.Balanza,
        FromAccount = "1",
        ToAccount = "3.99",
        InitialPeriod = new BalancesPeriod {
          FromDate = period.StartTime,
          ToDate = period.EndTime
        },
        BalancesType = BalancesType.AllAccounts,
      };

      var intialBalancesBuilder = new BalanzaTradicionalBuilder(initialBalanceQuery);

      var initialBalances = intialBalancesBuilder.BuildBalanzaTradicional()
                                                 .Entries.Cast<TrialBalanceEntry>()
                                                 .ToFixedList();

      var exchangeRates = ExchangeRate.GetList(period.StartTime.AddDays(-1), period.EndTime);

      var builder = new BalanzaValorizadaBuilder(initialBalances, movsDiarios, exchangeRates);

      FixedList<BalanzaValorizadaEntry> entries = builder.Build(period);

      FixedList<BalanzaValorizadaEntry> summaries = builder.BuildSummaryAccounts(entries);

      entries = FixedList<BalanzaValorizadaEntry>.MergeDistinct(summaries, entries);

      entries = entries.OrderBy(x => x.NumeroCuenta)
                       .ThenBy(x => x.Moneda.Code)
                       .ThenBy(x => x.FechaAfectacion)
                       .ToFixedList();

      return new DynamicDto<BalanzaValorizadaEntry>(query, builder.GetColumns(), entries);
    }

    #endregion Use cases

  } // class TrialBalancesUseCasesV3

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
