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
                          FixedList<TrialBalanceEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.Command = command;
      this.Entries = entries;
    }


    internal FixedList<DataTableColumn> DataColumns() {
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


    #endregion Constructors and parsers

    #region Properties

    public TrialBalanceCommand Command {
      get;
    }


    public FixedList<TrialBalanceEntry> Entries {
      get;
    }

    #endregion Properties

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
