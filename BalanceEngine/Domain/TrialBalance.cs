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

    internal TrialBalance(TrialBalanceCommand command,
                          FixedList<ITrialBalanceEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.Command = command;
      this.Entries = entries;
    }


    internal FixedList<DataTableColumn> DataColumns() {
      switch (this.Command.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          return TwoCurrenciesDataColumns();

        case TrialBalanceType.Balanza:
        case TrialBalanceType.GeneracionDeSaldos:
        case TrialBalanceType.Saldos:
        case TrialBalanceType.SaldosPorAuxiliar:
        case TrialBalanceType.SaldosPorCuenta:
        case TrialBalanceType.BalanzaConContabilidadesEnCascada:
          return TrialBalanceDataColumns();

        case TrialBalanceType.BalanzaConsolidadaPorMoneda:
          return TrialBalanceByCurrencyDataColumns();

        case TrialBalanceType.BalanzaValorizadaComparativa:
          return TwoBalancesComparativeDataColumns();

        case TrialBalanceType.BalanzaValorizadaEnDolares:
          return ValuedTrialBalanceDataColumns();

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled trial balance type {this.Command.TrialBalanceType}.");
      }
    }


    private FixedList<DataTableColumn> TrialBalanceDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Command.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (Command.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      
      if (Command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta ||
          Command.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar) {
        columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
        columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "text"));
      } else {
        columns.Add(new DataTableColumn("initialBalance", "Saldo anterior", "decimal"));
        columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
        columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
        columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
        if (Command.InitialPeriod.ExchangeRateTypeUID != string.Empty ||
            Command.InitialPeriod.UseDefaultValuation) {
          columns.Add(new DataTableColumn("exchangeRate", "TC", "decimal"));
        }
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
      }

      return columns.ToFixedList();
    }


    private FixedList<DataTableColumn> TwoBalancesComparativeDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Command.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountParent", "Cta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("subledgerAccountNumber", "Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("subledgerAccountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("firstTotalBalance", $"{Command.InitialPeriod.FromDate:MMM_yyyy}", "decimal"));
      columns.Add(new DataTableColumn("firstExchangeRate", "Tc_Ini", "decimal", 6));
      columns.Add(new DataTableColumn("firstValorization", $"{Command.InitialPeriod.FromDate:MMM}_VAL_A", "decimal"));

      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
      columns.Add(new DataTableColumn("secondTotalBalance", $"{Command.FinalPeriod.FromDate:MMM_yyyy}", "decimal"));
      columns.Add(new DataTableColumn("secondExchangeRate", "Tc_Fin", "decimal", 6));
      columns.Add(new DataTableColumn("secondValorization", $"{Command.FinalPeriod.FromDate:MMM}_VAL_B", "decimal"));

      columns.Add(new DataTableColumn("accountName", "Nom_Cta", "text"));
      columns.Add(new DataTableColumn("debtorCreditor", "Nat", "text"));
      columns.Add(new DataTableColumn("variation", "Variación", "decimal"));
      columns.Add(new DataTableColumn("variationByER", "Variación por TC", "decimal"));
      columns.Add(new DataTableColumn("realVariation", "Variación por TC", "decimal"));

      return columns.ToFixedList();
    }


    private FixedList<DataTableColumn> TwoCurrenciesDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Command.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (Command.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "Saldo Mon. Nal.", "decimal"));
      columns.Add(new DataTableColumn("foreignBalance", "Saldo Mon. Ext.", "decimal"));
      columns.Add(new DataTableColumn("totalBalance", "Total", "decimal"));
      if (Command.InitialPeriod.ExchangeRateTypeUID != string.Empty) {
        //columns.Add(new DataTableColumn("exchangeRate", "TC", "decimal"));
      }
      columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
      // columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "text"));

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
      columns.Add(new DataTableColumn("totalEquivalence", "Equivalencia M.N.", "decimal"));


      return columns.ToFixedList();
    }

    private FixedList<DataTableColumn> TrialBalanceByCurrencyDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));
      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("euroBalance", "Euros(27)", "decimal"));
      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));

      return columns.ToFixedList();
    }

    #endregion Constructors and parsers

    #region Properties

    public TrialBalanceCommand Command {
      get;
    }


    public FixedList<ITrialBalanceEntry> Entries {
      get;
    }

    #endregion Properties

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
