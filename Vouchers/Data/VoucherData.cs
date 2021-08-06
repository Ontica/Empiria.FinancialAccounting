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

    static internal void DeleteVoucher(Voucher voucher) {
      var dataOperation = DataOperation.Parse("do_delete_cof_transaccion", voucher.Id);

      DataWriter.Execute(dataOperation);
    }


    internal static void DeleteVoucherEntry(VoucherEntry voucherEntry) {
      var dataOperation = DataOperation.Parse("do_delete_cof_movimiento_tmp",
                                              voucherEntry.VoucherId, voucherEntry.Id);

      DataWriter.Execute(dataOperation);
    }


    static internal FixedList<Voucher> GetVouchers(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                  "SELECT * FROM VW_COF_TRANSACCION " +
                  $"WHERE {filter} " +
                  $"ORDER BY {sort}) " +
                $"WHERE ROWNUM <= {pageSize}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Voucher>(dataOperation);
    }


    static internal FixedList<VoucherEntry> GetVoucherEntries(Voucher o) {
      var dataOperation = DataOperation.Parse("qry_cof_movimiento", o.Id, o.IsOpened ? 1 : 0);

      return DataReader.GetFixedList<VoucherEntry>(dataOperation);
    }


    static internal void WriteVoucher(Voucher o) {
      var dataOperation = DataOperation.Parse("write_cof_transaccion", o.Id, o.Number,
                                              o.Ledger.Id, o.FunctionalArea.Id,
                                              o.TransactionType.Id, o.VoucherType.Id,
                                              o.Concept, o.AccountingDate, o.RecordingDate,
                                              o.ElaboratedBy.Id, o.AuthorizedBy.Id, o.IsOpened ? 1 : 0,
                                              o.ClosedBy.Id);

      DataWriter.Execute(dataOperation);
    }


  }  // class VoucherData

}  // namespace Empiria.FinancialAccounting.Vouchers.Data
