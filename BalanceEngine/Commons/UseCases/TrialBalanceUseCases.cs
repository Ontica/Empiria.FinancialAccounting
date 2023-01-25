/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalanceUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build trial balances.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to build trial balances.</summary>
  public class TrialBalanceUseCases : UseCase {

    #region Constructors and parsers

    protected TrialBalanceUseCases() {
      // no-op
    }

    static public TrialBalanceUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TrialBalanceUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public async Task<AnaliticoDeCuentasDto> BuildAnaliticoDeCuentas(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas,
                       "query.TrialBalanceType must be 'AnaliticoDeCuentas'.");

      var builder = new AnaliticoDeCuentasBuilder(query);

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(query);

      FixedList<AnaliticoDeCuentasEntry> entries = await Task.Run(() => builder.Build(baseAccountEntries))
                                                             .ConfigureAwait(false);

      return AnaliticoDeCuentasMapper.Map(query, entries);
    }


    public async Task<BalanzaComparativaDto> BuildBalanzaComparativa(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa,
                       "query.TrialBalanceType must be 'BalanzaValorizadaComparativa'.");

      var builder = new BalanzaComparativaBuilder(query);

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(query);

      FixedList<BalanzaComparativaEntry> entries = await Task.Run(() => builder.Build(baseAccountEntries))
                                                             .ConfigureAwait(false);

      return BalanzaComparativaMapper.Map(query, entries);
    }


    public async Task<BalanzaColumnasMonedaDto> BuildBalanzaColumnasMoneda(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.BalanzaEnColumnasPorMoneda,
                       "query.TrialBalanceType must be 'BalanzaEnColumnasPorMoneda'.");

      var builder = new BalanzaColumnasMonedaBuilder(query);

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(query);

      FixedList<BalanzaColumnasMonedaEntry> entries = await Task.Run(() => builder.Build(baseAccountEntries))
                                                             .ConfigureAwait(false);

      return BalanzaColumnasMonedaMapper.Map(query, entries);
    }


    public async Task<BalanzaContabilidadesCascadaDto> BuildBalanzaContabilidadesCascada(
                                                       TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada,
                       "query.TrialBalanceType must be 'BalanzaConContabilidadesEnCascada'.");

      var builder = new BalanzaContabilidadesCascadaBuilder(query);
      TrialBalance entries = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return BalanzaContabilidadesCascadaMapper.Map(entries);
    }


    public async Task<BalanzaDolarizadaDto> BuildBalanzaDolarizada(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.BalanzaDolarizada,
                       "query.TrialBalanceType must be 'BalanzaDolarizada'.");

      var builder = new BalanzaDolarizadaBuilder(query);

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(query);

      FixedList<BalanzaDolarizadaEntry> entries = await Task.Run(() => builder.Build(baseAccountEntries))
                                                             .ConfigureAwait(false);

      return BalanzaDolarizadaMapper.Map(query, entries);
    }


    public async Task<BalanzaTradicionalDto> BuildBalanzaTradicional(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.Balanza,
                       "query.TrialBalanceType must be 'Balanza'.");

      var builder = new BalanzaTradicionalBuilder(query);

      TrialBalance entries = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return BalanzaTradicionalMapper.Map(entries);
    }


    public async Task<SaldosPorAuxiliarDto> BuildSaldosPorAuxiliar(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar,
                       "query.TrialBalanceType must be 'SaldosPorAuxiliar'.");

      var builder = new SaldosPorAuxiliarBuilder(query);
      TrialBalance entries = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return SaldosPorAuxiliarMapper.Map(entries);
    }


    public async Task<SaldosPorCuentaDto> BuildSaldosPorCuenta(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta,
                       "query.TrialBalanceType must be 'SaldosPorCuenta'.");

      var builder = new SaldosPorCuentaBuilder(query);
      TrialBalance entries = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return SaldosPorCuentaMapper.Map(entries);
    }


    public async Task<ValorizacionEstimacionPreventivaDto> BuildValorizacion(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      Assertion.Require(query.TrialBalanceType == TrialBalanceType.ValorizacionEstimacionPreventiva,
                       "query.TrialBalanceType must be 'Valorizacion'.");

      var builder = new ValorizacionEstimacionPreventivaBuilder(query);

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(query);

      FixedList<ValorizacionEstimacionPreventivaEntry> entries = await Task.Run(() => builder.Build(baseAccountEntries))
                                                             .ConfigureAwait(false);

      return ValorizacionEstimacionPreventivaMapper.Map(query, entries);
    }


    public TrialBalanceDto BuildTrialBalance(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      var trialBalanceEngine = new TrialBalanceEngine(query);

      TrialBalance trialBalance = trialBalanceEngine.BuildTrialBalance();

      return TrialBalanceMapper.Map(trialBalance);
    }


    public async Task<SaldosEncerradosDto> BuildSaldosEncerrados(SaldosEncerradosQuery query) {
      Assertion.Require(query, nameof(query));

      var builder = new SaldosEncerradosService(query);

      FixedList<SaldosEncerradosBaseEntryDto> entries = await Task.Run(() => builder.Build()).ConfigureAwait(false);

      return SaldosEncerradosMapper.Map(entries);
    }


    #endregion Use cases

  } // class TrialBalanceUseCases

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
