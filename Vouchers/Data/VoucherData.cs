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


    static internal FixedList<EventType> EventTypes() {
      var sql = "SELECT * " +
                "FROM COF_EVENTOS_CARTERA " +
                "ORDER BY COF_EVENTOS_CARTERA.DESCRICPION_EVENTO";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<EventType>(dataOperation);
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

      return DataReader.GetPlainObjectFixedList<VoucherEntry>(dataOperation);
    }


    static internal long NextVoucherEntryId() {
      var sql = "SELECT SEC_ID_MOVIMIENTO_TMP.NEXTVAL FROM DUAL";

      var operation = DataOperation.Parse(sql);

      return Convert.ToInt64(DataReader.GetScalar<decimal>(operation));
    }


    static internal FixedList<LedgerAccount> SearchAccountsForVoucherEdition(Voucher voucher, string keywords) {
      string sqlKeywords = SearchExpression.ParseAndLikeKeywords("keywords_cuenta_estandar_hist", keywords);

      DataOperation operation = DataOperation.Parse("@qry_cof_busca_cuentas_para_edicion",
                                                    voucher.Ledger.Id,
                                                    CommonMethods.FormatSqlDate(voucher.AccountingDate),
                                                    sqlKeywords);

      return DataReader.GetFixedList<LedgerAccount>(operation);
    }


    static internal FixedList<SubsidiaryAccount> SearchSubledgerAccountsForVoucherEdition(Voucher voucher,
                                                                                          string keywords) {
      string sqlKeywords = SearchExpression.ParseAndLikeKeywords("keywords_cuenta_auxiliar", keywords);

      DataOperation operation = DataOperation.Parse("@qry_cof_busca_auxiliares_para_edicion",
                                                    voucher.Ledger.Id,
                                                    sqlKeywords);

      return DataReader.GetFixedList<SubsidiaryAccount>(operation);
    }


    static internal void WriteVoucher(Voucher o) {
      var op = DataOperation.Parse("write_cof_transaccion", o.Id, o.Number,
                                    o.Ledger.Id, o.FunctionalArea.Id,
                                    o.TransactionType.Id, o.VoucherType.Id,
                                    o.Concept, o.AccountingDate, o.RecordingDate,
                                    o.ElaboratedBy.Id, o.AuthorizedBy.Id, o.IsOpened ? 1 : 0,
                                    o.ClosedBy.Id);

      DataWriter.Execute(op);
    }


    static internal void WriteVoucherEntry(VoucherEntry o) {
      var op = DataOperation.Parse("write_cof_movimiento_tmp", o.Id, o.VoucherId, o.LedgerAccount.Id,
                                    o.SubledgerAccount.IsEmptyInstance ? 0 : o.SubledgerAccount.Id,
                                    o.Sector.IsEmptyInstance ? 0: o.Sector.Id,
                                    o.ReferenceEntryId,
                                    o.ResponsibilityArea.IsEmptyInstance ? 0: o.ResponsibilityArea.Id,
                                    o.BudgetConcept, o.AvailabilityCode, o.VerificationNumber, (char) o.VoucherEntryType,
                                    o.HasDate ? (object) o.Date : DBNull.Value, o.Concept, o.Currency.Id,
                                    o.Amount, o.BaseCurrrencyAmount, o.Protected ? 1 : 0);

      DataWriter.Execute(op);
    }


  }  // class VoucherData

}  // namespace Empiria.FinancialAccounting.Vouchers.Data
