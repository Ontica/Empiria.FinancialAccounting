/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalancesDataServiceV3                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data services to retrieve accounting balances (version 3.0).                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Data services to retrieve accounting balances (version 3.0).</summary>
  static internal class BalancesDataServiceV3 {

    #region Methods

    static internal FixedList<TrialBalanceValued> GetDailyMovements(DateTime fromDate, DateTime toDate) {

      var sql =
        "SELECT id_cuenta_estandar, fecha_afectacion, id_moneda, " +
               "SUM(debe) Debe, SUM(haber) Haber " +
        "FROM vw_cof_movimiento " +
        $"WHERE " +
            $"fecha_afectacion >= {DataCommonMethods.FormatSqlDbDate(fromDate)} AND " +
            $"fecha_afectacion < {DataCommonMethods.FormatSqlDbDate(toDate.AddDays(1))} AND " +
            "id_moneda <> 1 " +
        "GROUP BY " +
            "id_cuenta_estandar, fecha_afectacion, id_moneda";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TrialBalanceValued>(op);
    }

    #endregion Methods

  } // class BalancesDataServiceV3

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
