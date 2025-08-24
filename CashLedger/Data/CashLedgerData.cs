/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Static data services                    *
*  Type     : CashLedgerData                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services used to retrive and write cash ledger transactions.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Text;

using Empiria.Data;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Provides services used to retrive and write cash ledger transactions.</summary>
  static internal class CashLedgerData {

    static internal FixedList<CashEntryExtended> GetEntries(FixedList<long> ids) {
      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      var filter = SearchExpression.ParseInSet("ID_MOVIMIENTO", ids);

      var sql = "SELECT * FROM VW_COF_MOVIMIENTO " +
                $"WHERE {filter} " +
                $"ORDER BY ID_MAYOR, NUMERO_TRANSACCION, FECHA_AFECTACION, ID_MOVIMIENTO";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashEntryExtended>(op);
    }


    static internal CashTransaction GetTransaction(long id) {
      var sql = "SELECT * FROM VW_COF_TRANSACCION " +
               $"WHERE ID_TRANSACCION = {id} AND ESTA_ABIERTA = 0";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<CashTransaction>(op);
    }


    static internal FixedList<CashTransaction> GetTransactions(FixedList<long> ids) {
      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      var filter = SearchExpression.ParseInSet("ID_TRANSACCION", ids);

      var sql = "SELECT * FROM VW_COF_TRANSACCION " +
               $"WHERE {filter} AND ESTA_ABIERTA = 0";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashTransaction>(op);
    }


    static internal FixedList<CashEntry> GetTransactionEntries(CashTransaction transaction) {
      var op = DataOperation.Parse("qry_cof_movimiento", transaction.Id, 0);

      return DataReader.GetPlainObjectFixedList<CashEntry>(op);
    }


    static internal FixedList<CashEntryExtended> SearchEntries(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                    "SELECT * FROM VW_COF_MOVIMIENTO " +
                    $"WHERE {filter} " +
                    $"ORDER BY {sort}) " +
                 $"WHERE ROWNUM <= {pageSize}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashEntryExtended>(op);
    }


    static internal FixedList<CashTransaction> SearchTransactions(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                  "SELECT * FROM VW_COF_TRANSACCION " +
                  $"WHERE {filter} " +
                  $"ORDER BY {sort}) " +
               $"WHERE ROWNUM <= {pageSize}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashTransaction>(op);
    }


    static internal void WriteCashEntriesAccounts(FixedList<CashEntryFields> entries) {
      var stringBuilder = new StringBuilder(entries.Count * 300);

      stringBuilder.Append("BEGIN ");

      foreach (var entry in entries) {
        var localSql =
                $"DELETE FROM COF_MOVIMIENTO_BIS WHERE ID_MOVIMIENTO = {entry.EntryId}; " +

                "INSERT INTO COF_MOVIMIENTO_BIS VALUES " +
                 $"({entry.EntryId}, {entry.CashAccountId}, '{entry.CashAccountId}', " +
                 $"'Rule', -1, -1, {DataCommonMethods.FormatSqlDbDateTime(DateTime.Now)}, 0); ";

        stringBuilder.Append(localSql);
      }

      stringBuilder.AppendLine("END;");

      var op = DataOperation.Parse(stringBuilder.ToString());

      DataWriter.Execute(op);
    }

  }  // class CashLedgerData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
