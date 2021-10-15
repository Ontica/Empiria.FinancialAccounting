/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : LedgerData                                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data service layer for accounting ledgers.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data service layer for accounting ledgers.</summary>
  static internal class LedgerData {


    static internal LedgerAccount AssignStandardAccount(Ledger ledger,
                                                        StandardAccount standardAccount) {
      var newLedgerAccountId = NextLedgerAccountId();

      var dataOperation = DataOperation.Parse("apd_cof_cuenta",
                                              newLedgerAccountId, ledger.Id, standardAccount.Id);

      DataWriter.Execute(dataOperation);

      LedgerAccount ledgerAccount = TryGetLedgerAccount(ledger, standardAccount);

      Assertion.AssertObject(ledgerAccount, "ledgerAccount");

      return ledgerAccount;
    }


    static internal bool ContainsLedgerAccount(Ledger ledger, StandardAccount standardAccount) {
      var sql = "SELECT * FROM COF_CUENTA " +
               $"WHERE id_mayor = {ledger.Id} AND " +
               $"id_cuenta_estandar = {standardAccount.Id}";

      var dataOperation = DataOperation.Parse(sql);

      return !DataReader.IsEmpty(dataOperation);
    }


    static internal LedgerAccount TryGetLedgerAccount(Ledger ledger, StandardAccount standardAccount) {
      var sql = "SELECT * FROM COF_CUENTA " +
               $"WHERE id_mayor = {ledger.Id} AND " +
               $"id_cuenta_estandar = {standardAccount.Id}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetObject<LedgerAccount>(dataOperation, null);
    }


    static internal FixedList<SubsidiaryLedger> GetSubledgers(Ledger ledger) {
      var sql = "SELECT * FROM COF_MAYOR_AUXILIAR " +
                $"WHERE (ID_MAYOR = {ledger.Id} OR ID_MAYOR_ADICIONAL = {ledger.Id}) AND Eliminado = 0 " +
                 "ORDER BY NOMBRE_MAYOR_AUXILIAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<SubsidiaryLedger>(dataOperation);
    }


    static private long NextLedgerAccountId() {
      var sql = "SELECT SEC_ID_CUENTA.NEXTVAL FROM DUAL";

      var operation = DataOperation.Parse(sql);

      return Convert.ToInt64(DataReader.GetScalar<decimal>(operation));
    }


    static internal FixedList<Account> SearchUnassignedAccountsForEdition(Ledger ledger,
                                                                          string keywords,
                                                                          DateTime date) {

      string sqlKeywords = SearchExpression.ParseAndLikeKeywords("keywords_cuenta_estandar_hist",
                                                                  keywords);

      string sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST WHERE " +
                  $"id_tipo_cuentas_std = {ledger.AccountsChart.Id} AND " +
                  $"rol_cuenta <> 'S' AND " +
                  $"fecha_inicio <= '{CommonMethods.FormatSqlDate(date)}' AND " +
                  $"{sqlKeywords} AND " +
                  $"'{CommonMethods.FormatSqlDate(date)}' <= fecha_fin AND " +
                  $"id_cuenta_estandar NOT IN " +
                        $"(SELECT id_cuenta_estandar FROM COF_CUENTA WHERE id_mayor = {ledger.Id}) " +
                  "ORDER BY numero_cuenta_estandar";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }


    internal static SubsidiaryAccount TryGetSubledgerAccount(Ledger ledger, string formattedAccountNo) {
      var sql = "SELECT COF_CUENTA_AUXILIAR.* " +
                "FROM COF_CUENTA_AUXILIAR INNER JOIN VW_COF_CUENTA_AUXILIAR " +
                "ON COF_CUENTA_AUXILIAR.ID_CUENTA_AUXILIAR = VW_COF_CUENTA_AUXILIAR.ID_CUENTA_AUXILIAR " +
               $"WHERE (VW_COF_CUENTA_AUXILIAR.ID_MAYOR = {ledger.Id} OR " +
               $"VW_COF_CUENTA_AUXILIAR.ID_MAYOR_ADICIONAL = {ledger.Id}) " +
               $"AND COF_CUENTA_AUXILIAR.NUMERO_CUENTA_AUXILIAR = '{formattedAccountNo}' " +
               $"AND COF_CUENTA_AUXILIAR.ELIMINADA = 0";


      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetObject<SubsidiaryAccount>(dataOperation, null);
    }


  }  // class LedgerData

}  // namespace Empiria.FinancialAccounting.Data
