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

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Provides services used to retrive and write cash ledger transactions.</summary>
  static internal class CashLedgerData {

    static internal FixedList<CashEntryExtended> GetExtendedEntries(FixedList<long> ids) {
      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      var filter = SearchExpression.ParseInSet("ID_MOVIMIENTO", ids);

      var sql = "SELECT * FROM VW_COF_MOVIMIENTO_BIS " +
                $"WHERE {filter} " +
                $"ORDER BY ID_MAYOR, NUMERO_TRANSACCION, ID_MOVIMIENTO";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashEntryExtended>(op);
    }


    static internal CashTransaction GetTransaction(long id) {

      var sql = "SELECT * FROM VW_COF_TRANSACCION_FLUJO " +
               $"WHERE ID_TRANSACCION = {id}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<CashTransaction>(op);
    }


    static internal FixedList<CashTransaction> GetTransactions(FixedList<long> ids) {
      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      var filter = SearchExpression.ParseInSet("ID_TRANSACCION", ids);

      var sql = "SELECT * FROM VW_COF_TRANSACCION_FLUJO " +
               $"WHERE {filter}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashTransaction>(op);
    }


    static internal FixedList<CashEntry> GetTransactionEntries(CashTransaction transaction) {

      var sql = "SELECT * FROM VW_COF_MOVIMIENTO_JOINED " +
               $"WHERE ID_TRANSACCION = {transaction.Id} " +
               $"ORDER BY ID_MOVIMIENTO";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashEntry>(op);
    }


    static internal FixedList<CashEntry> GetTransactionEntries(FixedList<long> ids) {
      Assertion.Require(ids, nameof(ids));
      Assertion.Require(ids.Count > 0, nameof(ids));

      var filter = SearchExpression.ParseInSet("ID_TRANSACCION", ids);

      var sql = "SELECT * FROM VW_COF_MOVIMIENTO_JOINED " +
                $"WHERE {filter} " +
                $"ORDER BY ID_TRANSACCION, ID_MOVIMIENTO";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashEntry>(op);
    }


    static internal FixedList<CashEntryExtended> SearchEntries(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                    "SELECT * FROM VW_COF_MOVIMIENTO_FLUJO " +
                    $"WHERE {filter} " +
                    $"ORDER BY {sort}) " +
                 $"WHERE ROWNUM <= {pageSize}";

      var op = DataOperation.Parse(sql);

      EmpiriaLog.Debug(sql);

      return DataReader.GetPlainObjectFixedList<CashEntryExtended>(op);
    }


    static internal FixedList<CashTransaction> SearchTransactions(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                  "SELECT * FROM VW_COF_TRANSACCION_FLUJO " +
                  $"WHERE {filter} " +
                  $"ORDER BY {sort}) " +
               $"WHERE ROWNUM <= {pageSize}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashTransaction>(op);
    }


    static internal void UpdateCashEntriesAccounts(FixedList<CashEntryFields> entries) {
      var stringBuilder = new StringBuilder(entries.Count * 350);

      stringBuilder.Append("BEGIN ");

      foreach (var entry in entries) {
        var localSql =
              "UPDATE COF_MOVIMIENTO_BIS SET " +
                   $"ID_CUENTA_FLUJO = {entry.CashAccountId}, " +
                   $"NUM_CONCEPTO_FLUJO = '{entry.CashAccountNo}', " +
                   $"REGLA_FLUJO = '{entry.AppliedRule}', " +
                   $"ID_USUARIO_FLUJO = {entry.UserId}, " +
                   $"FECHA_REGISTRO_FLUJO = {DataCommonMethods.FormatSqlDbDateTime(DateTime.Now)} " +
              $"WHERE ID_MOVIMIENTO = {entry.EntryId}; ";

        stringBuilder.Append(localSql);
      }

      stringBuilder.AppendLine("END;");

      var op = DataOperation.Parse(stringBuilder.ToString());

      DataWriter.Execute(op);
    }

  }  // class CashLedgerData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
