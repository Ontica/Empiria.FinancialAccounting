/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : TrialBalance                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of a trial balance                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of a trial balance.</summary>
  public class TrialBalance {

    #region Constructors and parsers

    internal TrialBalance(TrialBalanceQuery query,
                          FixedList<ITrialBalanceEntry> entries) {
      Assertion.Require(query,nameof(query));
      Assertion.Require(entries, nameof(entries));

      this.Query = query;
      this.Entries = entries;
    }


    internal FixedList<DataTableColumn> DataColumns() {
      switch (this.Query.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          return AnaliticoDeCuentasMapper.DataColumns(this.Query);

        case TrialBalanceType.Balanza:
          return BalanzaTradicionalMapper.DataColumns(this.Query);

        case TrialBalanceType.SaldosPorCuenta:
          return SaldosPorCuentaMapper.DataColumns(this.Query);

        case TrialBalanceType.SaldosPorAuxiliar:
          return SaldosPorAuxiliarMapper.DataColumns(this.Query);

        case TrialBalanceType.GeneracionDeSaldos:
        case TrialBalanceType.BalanzaConContabilidadesEnCascada:
          return TrialBalanceDataColumns();

        case TrialBalanceType.BalanzaEnColumnasPorMoneda:
          return TrialBalanceByCurrencyDataColumns();

        case TrialBalanceType.BalanzaValorizadaComparativa:
          return TwoBalancesComparativeDataColumns();

        case TrialBalanceType.BalanzaDolarizada:
          return ValuedTrialBalanceDataColumns();

        default:
          throw Assertion.EnsureNoReachThisCode(
                $"Unhandled trial balance type {this.Query.TrialBalanceType}.");
      }
    }


    private FixedList<DataTableColumn> TrialBalanceDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (Query.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));

      if (Query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta ||
          Query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar) {
        columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
        columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
        if (Query.WithAverageBalance) {
          columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));

        }
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
      } else {
        columns.Add(new DataTableColumn("initialBalance", "Saldo anterior", "decimal"));
        columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
        columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
        columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
        if (Query.InitialPeriod.ExchangeRateTypeUID != string.Empty ||
            Query.InitialPeriod.UseDefaultValuation) {
          columns.Add(new DataTableColumn("exchangeRate", "TC", "decimal", 6));
        }
        if (Query.WithAverageBalance) {
          columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
          columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
        }
      }

      return columns.ToFixedList();
    }


    private FixedList<DataTableColumn> TwoBalancesComparativeDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountParent", "Cta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("subledgerAccountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("firstTotalBalance", $"{Query.InitialPeriod.FromDate:MMM_yyyy}", "decimal"));
      columns.Add(new DataTableColumn("firstExchangeRate", "Tc_Ini", "decimal", 6));
      columns.Add(new DataTableColumn("firstValorization", $"{Query.InitialPeriod.FromDate:MMM}_VAL_A", "decimal"));

      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
      columns.Add(new DataTableColumn("secondTotalBalance", $"{Query.FinalPeriod.FromDate:MMM_yyyy}", "decimal"));
      columns.Add(new DataTableColumn("secondExchangeRate", "Tc_Fin", "decimal", 6));
      columns.Add(new DataTableColumn("secondValorization", $"{Query.FinalPeriod.FromDate:MMM}_VAL_B", "decimal"));

      columns.Add(new DataTableColumn("accountName", "Nom_Cta", "text"));
      columns.Add(new DataTableColumn("debtorCreditor", "Nat", "text"));
      columns.Add(new DataTableColumn("variation", "Variación", "decimal"));
      columns.Add(new DataTableColumn("variationByER", "Variación por TC", "decimal"));
      columns.Add(new DataTableColumn("realVariation", "Variación por TC", "decimal"));
      if (Query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
      }

      return columns.ToFixedList();
    }


    private FixedList<DataTableColumn> ValuedTrialBalanceDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("currencyName", "Moneda", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Clave Mon.", "text"));

      columns.Add(new DataTableColumn("totalBalance", "Importe Mon. Ext.", "decimal"));
      //columns.Add(new DataTableColumn("exchangeRate", "Valor M.N.", "decimal", 6));
      columns.Add(new DataTableColumn("valuedExchangeRate", "Tipo cambio", "decimal", 6));
      columns.Add(new DataTableColumn("totalEquivalence", "Equivalencia en dólares", "decimal"));


      return columns.ToFixedList();
    }


    private FixedList<DataTableColumn> TrialBalanceByCurrencyDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));
      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));

      return columns.ToFixedList();
    }


    #endregion Constructors and parsers

    #region Properties

    public TrialBalanceQuery Query {
      get;
    }


    public FixedList<ITrialBalanceEntry> Entries {
      get;
    }

    #endregion Properties

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
