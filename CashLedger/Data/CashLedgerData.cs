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
