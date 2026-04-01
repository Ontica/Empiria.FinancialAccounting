/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalancesUseCasesV3                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build trial balances version 3.0.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria;
using Empiria.DynamicData;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using System.Linq;

/// <summary>Output DTO used to return trial balances.</summary>
public class TrialBalanceDtoV3 {

  public TrialBalanceQuery Query {
    get; internal set;
  }


  public FixedList<DataTableColumn> Columns {
    get; internal set;
  }


  public FixedList<ITrialBalanceEntryDto> Entries {
    get; internal set;
  }

}  // class TrialBalanceDtoV3



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

    public DynamicDto<TrialBalanceValued> GetValuatedTrialBalance(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      var trialBalanceEngine = new TrialBalanceEngine(query);

      query = trialBalanceEngine.Query;

      FixedList<TrialBalanceValued> entries = BalancesDataServiceV3.GetDailyMovements(query.InitialPeriod.FromDate,
                                                                                      query.InitialPeriod.ToDate);

      entries = entries.FindAll(x => x.NumeroCuenta.StartsWith("1") ||
                                     x.NumeroCuenta.StartsWith("2") ||
                                     x.NumeroCuenta.StartsWith("3"));

      entries = entries.OrderBy(x => x.NumeroCuenta)
                       .ThenBy(x => x.FechaAfectacion)
                       .ToFixedList();

      var exchangeRates = ExchangeRate.GetList(query.InitialPeriod.FromDate.AddDays(-20),
                                               query.InitialPeriod.ToDate);

      foreach (var entry in entries) {
        entry.SetExchangeRate(exchangeRates, query.InitialPeriod.FromDate.AddDays(-20), query.InitialPeriod.ToDate);
      }

      var columns = new DataTableColumn[] {
        new DataTableColumn("numeroCuenta", "Cuenta", "text-nowrap"),
        new DataTableColumn("nombreCuenta", "Descripción", "text"),
        new DataTableColumn("codigoMoneda", "Moneda", "text"),
        new DataTableColumn("fechaAfectacion", "Fecha", "date"),
        new DataTableColumn("saldoInicial", "Saldo inicial MO", "decimal"),
        new DataTableColumn("debe", "Cargos MO", "decimal"),
        new DataTableColumn("haber", "Abonos MO", "decimal"),
        new DataTableColumn("saldoFinal", "Saldo final MO", "decimal"),
        new DataTableColumn("tipoCambio", "T. Cambio", "decimal", 4),
        new DataTableColumn("saldoInicialMXN", "Saldo inicial MXN", "decimal", 6),
        new DataTableColumn("debeMXN", "Cargos MXN", "decimal", 6),
        new DataTableColumn("haberMXN", "Abonos MXN", "decimal", 6),
        new DataTableColumn("saldoFinalMXN", "Saldo final MXN", "decimal", 6),
      }.ToFixedList();

      return new DynamicDto<TrialBalanceValued>(query, columns, entries);
    }

    #endregion Use cases

  } // class TrialBalancesUseCasesV3

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
