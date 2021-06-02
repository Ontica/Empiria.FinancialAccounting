/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Data Service                            *
*  Type     : VoucherData                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for accounting vouchers.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Vouchers.Data {

  /// <summary>Data access layer for accounting vouchers.</summary>
  static internal class VoucherData {

    static internal FixedList<Voucher> GetVouchers(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                  "SELECT * FROM VW_COF_TRANSACCION " +
                  $"WHERE {filter} " +
                  $"ORDER BY {sort})" +
                $"WHERE ROWNUM <= {pageSize}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Voucher>(dataOperation);
    }

  }  // class VoucherData

}  // namespace Empiria.FinancialAccounting.Vouchers.Data
