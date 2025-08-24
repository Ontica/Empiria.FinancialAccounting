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

    static internal void CloseVoucher(Voucher o) {

      var dataOperation = DataOperation.Parse("do_close_cof_transaccion",
                                              o.Id, o.Ledger.Id, o.Number,
                                              o.AuthorizedBy.IsEmptyInstance ? 0 : o.AuthorizedBy.Id,
                                              o.ClosedBy.IsEmptyInstance ? 0 : o.ClosedBy.Id,
                                              o.RecordingDate);

      DataWriter.Execute(dataOperation);
    }


    static internal void DeleteVoucher(Voucher voucher) {
      var dataOperation = DataOperation.Parse("do_delete_cof_transaccion", voucher.Id);

      DataWriter.Execute(dataOperation);
    }


    static internal void DeleteVoucherEntry(VoucherEntry voucherEntry) {
      var dataOperation = DataOperation.Parse("do_delete_cof_movimiento_tmp",
                                              voucherEntry.Voucher.Id, voucherEntry.Id);

      DataWriter.Execute(dataOperation);
    }


    static internal FixedList<EventType> EventTypes() {
      var sql = "SELECT * " +
                "FROM COF_EVENTOS_CARTERA " +
                "ORDER BY COF_EVENTOS_CARTERA.DESCRICPION_EVENTO";    // 'DESCRICPION' is 'ok'

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<EventType>(dataOperation);
    }


    static internal FixedList<TransactionalSystemRule> GetTransactionalSystemRules(TransactionalSystem system) {
      var sql = "SELECT * " +
                "FROM COF_MAPEO_SISTEMAS_POLIZAS " +
               $"WHERE ID_SISTEMA = {system.Id} ";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionalSystemRule>(dataOperation);
    }


    static internal Voucher GetVoucher(long id) {
      var sql = $"SELECT * FROM COF_TRANSACCION WHERE ID_TRANSACCION = {id}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<Voucher>(dataOperation);
    }


    static internal FixedList<Voucher> GetVouchers(int[] ids) {
      if (ids.Length == 0) {
        return new FixedList<Voucher>();
      }

      string filter = SearchExpression.ParseInSet("ID_TRANSACCION", ids);

      if (filter.Length == 0) {
        return new FixedList<Voucher>();
      }

      var sql = $"SELECT * FROM COF_TRANSACCION WHERE ({filter})";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<Voucher>(dataOperation);
    }


    static internal string GetVoucherNumberFor(Voucher voucher) {
      // Please check do_close_transaction stored procedure validation code

      var prefix = $"{voucher.AccountingDate.Year}-{voucher.AccountingDate.Month.ToString("00")}";

      var sql = "SELECT MAX(NUMERO_TRANSACCION) " +
               $"FROM COF_TRANSACCION " +
               $"WHERE ID_MAYOR = {voucher.Ledger.Id} AND " +
               $"NUMERO_TRANSACCION LIKE '{prefix}-%' AND ESTA_ABIERTA = 0";

      var dataOperation = DataOperation.Parse(sql);

      string maxNumber = DataReader.GetScalar(dataOperation, String.Empty);

      int number = maxNumber.Length == 0 ? 1 : int.Parse(maxNumber.Substring(prefix.Length + 1)) + 1;

      return $"{prefix}-{number.ToString("000000")}";
    }


    static internal FixedList<Voucher> GetVouchers(string filter, string sort, int pageSize) {
      var sql = "SELECT * FROM (" +
                  "SELECT * FROM VW_COF_TRANSACCION " +
                  $"WHERE {filter} " +
                  $"ORDER BY {sort}) " +
               $"WHERE ROWNUM <= {pageSize}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<Voucher>(dataOperation);
    }


    static internal FixedList<VoucherEntry> GetVoucherEntries(Voucher o) {
      var dataOperation = DataOperation.Parse("qry_cof_movimiento", o.Id, o.IsOpened ? 1 : 0);

      return DataReader.GetPlainObjectFixedList<VoucherEntry>(dataOperation, () => new VoucherEntry(o));
    }


    static internal long NextVoucherId() {
      return DataCommonMethods.GetNextObjectId("SEC_ID_TRANSACCION");
    }


    static internal long NextVoucherEntryId() {
      return DataCommonMethods.GetNextObjectId("SEC_ID_MOVIMIENTO_TMP");
    }


    static internal FixedList<SubledgerAccount> SearchSubledgerAccountsForVoucherEdition(Voucher voucher,
                                                                                         string keywords) {
      string sqlKeywords = SearchExpression.ParseAndLikeKeywords("keywords_cuenta_auxiliar", keywords);

      DataOperation operation = DataOperation.Parse("@qry_cof_busca_auxiliares_para_edicion",
                                                    voucher.Ledger.Id,
                                                    sqlKeywords);

      return DataReader.GetPlainObjectFixedList<SubledgerAccount>(operation);
    }


    static internal Voucher TryGetVoucher(Ledger ledger, string voucherNumber) {
      var sql = "SELECT * FROM VW_COF_TRANSACCION " +
               $"WHERE Numero_Transaccion = '{voucherNumber}' AND Id_Mayor = {ledger.Id}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<Voucher>(dataOperation, null);
    }


    static internal void UpdateVoucherConcept(Voucher voucher) {
      var sql = "UPDATE COF_TRANSACCION " +
               $"SET CONCEPTO_TRANSACCION = '{voucher.Concept}' " +
               $"WHERE (ID_TRANSACCION = {voucher.Id} AND NUMERO_TRANSACCION = '{voucher.Number}')";

      var dataOperation = DataOperation.Parse(sql);

      DataWriter.Execute(dataOperation);
    }


    static internal void WriteVoucher(Voucher o) {
      Assertion.Require(o.IsOpened, "Voucher must be open to be modified in the database.");

      var op = DataOperation.Parse("write_cof_transaccion", o.Id, o.Number,
                  o.Ledger.Id, o.FunctionalArea.Id,
                  o.TransactionType.Id, o.VoucherType.Id,
                  o.Concept, o.AccountingDate, o.RecordingDate,
                  o.ElaboratedBy.Id,
                  o.AuthorizedBy.IsEmptyInstance ? 0 : o.AuthorizedBy.Id);

      DataWriter.Execute(op);
    }


    static internal void WriteVoucherEntry(VoucherEntry o) {
      Assertion.Require(o.Voucher.IsOpened, "Voucher must be open to be modified in the database.");

      var op = DataOperation.Parse("write_cof_movimiento_tmp", o.Id, o.Voucher.Id, o.LedgerAccount.Id,
          o.SubledgerAccount.IsEmptyInstance ? 0 : o.SubledgerAccount.Id,
          o.Sector.IsEmptyInstance ? 0 : o.Sector.Id,
          o.BusinessOperationId, o.ResponsibilityArea.IsEmptyInstance ? 0 : o.ResponsibilityArea.Id,
          o.BudgetConcept, o.EventType.Id, o.VerificationNumber, (char) o.VoucherEntryType,
          o.HasDate ? (object) o.Date : DBNull.Value, o.Concept, o.Currency.Id,
          o.Amount, o.BaseCurrencyAmount, o.Protected ? 1 : 0);

      DataWriter.Execute(op);
    }

  }  // class VoucherData

}  // namespace Empiria.FinancialAccounting.Vouchers.Data
