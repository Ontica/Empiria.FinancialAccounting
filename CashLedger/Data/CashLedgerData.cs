/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Static data services                    *
*  Type     : CashLedgerData                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services used to retrive and write cash ledger transactions.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Provides services used to retrive and write cash ledger transactions.</summary>
  static internal class CashLedgerData {

    static internal CashTransaction GetTransaction(long id) {
      var sql = "SELECT * FROM VW_COF_TRANSACCION " +
               $"WHERE ID_TRANSACCION = {id} AND ESTA_ABIERTA = 0";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<CashTransaction>(op);
    }


    static internal FixedList<CashEntry> GetTransactionEntries(CashTransaction transaction) {
      var op = DataOperation.Parse("qry_cof_movimiento", transaction.Id, 0);

      return DataReader.GetPlainObjectFixedList<CashEntry>(op);
    }


    static internal FixedList<CashTransaction> GetTransactions(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
            "SELECT * FROM VW_COF_TRANSACCION " +
            $"WHERE {filter} " +
            $"ORDER BY {sort}) " +
         $"WHERE ROWNUM <= {pageSize}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashTransaction>(op);
    }

  }  // class CashLedgerData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
