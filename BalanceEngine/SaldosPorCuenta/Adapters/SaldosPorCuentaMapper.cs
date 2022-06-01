/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : SaldosPorCuentaMapper                   License   : Please read LICENSE.txt file               *
*                                                                                                            *
*  Summary  : Methods used to map Saldos por cuenta entries.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map Saldos por cuenta entries.</summary>
  static internal class SaldosPorCuentaMapper {

    #region Public methods


    static internal BalanzaTradicionalDto Map(TrialBalance entries) {
      return new BalanzaTradicionalDto {
        Query = entries.Query,
        Columns = DataColumns(entries.Query),
        Entries = Map(entries.Entries, entries.Query)
      };
    }


    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery Query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      LedgerAndSubledgerAccountConditions(columns, Query);

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
      if (Query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));

      }
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));

      return columns.ToFixedList();
    }


    static private FixedList<BalanzaTradicionalEntryDto> Map(FixedList<ITrialBalanceEntry> entries,
                                                             TrialBalanceQuery query) {
      throw new NotImplementedException();
    }


    #endregion Public methods


    #region Private methods

    static private void LedgerAndSubledgerAccountConditions(List<DataTableColumn> columns,
                                                            TrialBalanceQuery Query) {
      if (Query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));

      }
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (!Query.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));

      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      }
    }


    #endregion Private methods

  } // class SaldosPorCuentaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
