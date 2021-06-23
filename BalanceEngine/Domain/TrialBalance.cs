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
        case TrialBalanceType.BalanzaConAuxiliares:
        case TrialBalanceType.Saldos:
        case TrialBalanceType.SaldosPorAuxiliar:
        case TrialBalanceType.SaldosPorCuenta:
        case TrialBalanceType.SaldosPorCuentaConDelegaciones:
          return TrialBalanceDataColumns();

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

      if (Command.ReturnSubledgerAccounts) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("initialBalance", "Saldo anterior", "decimal"));
      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));

      return columns.ToFixedList();
    }


    private FixedList<DataTableColumn> TwoCurrenciesDataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Command.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (Command.ReturnSubledgerAccounts) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "Saldo Mon. Nal.", "decimal"));
      columns.Add(new DataTableColumn("foreignBalance", "Saldo Mon. Ext.", "decimal"));

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
