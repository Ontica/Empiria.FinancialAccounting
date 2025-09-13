/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Static data services                    *
*  Type     : CashAccountTotalsData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data services used to retrive cash ledger accounts totals.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Data services used to retrive cash ledger totals.</summary>
  static internal class CashAccountTotalsData {

    static internal FixedList<CashAccountTotal> GetTotals(string filter) {
      Assertion.Require(filter, nameof(filter));

      var sql = "SELECT ID_CUENTA_FLUJO, NUM_CONCEPTO_FLUJO, " +
        "        ID_MONEDA, SUM(DEBE) DEBE, SUM(HABER) HABER " +
                "FROM VW_COF_MOVIMIENTO_BIS " +
                $"WHERE {filter} " +
                $"GROUP BY ID_CUENTA_FLUJO, NUM_CONCEPTO_FLUJO, ID_MONEDA " +
                $"ORDER BY NUM_CONCEPTO_FLUJO";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CashAccountTotal>(op);
    }
  }  // class CashAccountTotalsData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
