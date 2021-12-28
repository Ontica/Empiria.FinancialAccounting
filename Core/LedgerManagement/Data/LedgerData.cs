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
      CreateLedgerAccount(ledger, standardAccount);

      return GetLedgerAccount(ledger, standardAccount);
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


    static internal FixedList<Subledger> GetSubledgers(Ledger ledger) {
      var sql = "SELECT * FROM COF_MAYOR_AUXILIAR " +
                $"WHERE (ID_MAYOR = {ledger.Id} OR ID_MAYOR_ADICIONAL = {ledger.Id}) AND Eliminado = 0 " +
                 "ORDER BY NOMBRE_MAYOR_AUXILIAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Subledger>(dataOperation);
    }


    static private long NextLedgerAccountId() {
      return CommonMethods.GetNextObjectId("SEC_ID_CUENTA");
    }


    static internal FixedList<LedgerAccount> SearchAssignedAccountsForEdition(Ledger ledger, DateTime date, string filter) {
      var dataOperation = DataOperation.Parse("@qry_cof_busca_cuentas_para_edicion",
                                              ledger.Id, CommonMethods.FormatSqlDate(date), filter);

      return DataReader.GetFixedList<LedgerAccount>(dataOperation);
    }


    static internal FixedList<Account> SearchUnassignedAccountsForEdition(Ledger ledger, DateTime date, string filter) {

      string sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST WHERE " +
                  $"id_tipo_cuentas_std = {ledger.AccountsChart.Id} AND " +
                  $"rol_cuenta <> 'S' AND " +
                  $"fecha_inicio <= '{CommonMethods.FormatSqlDate(date)}' AND " +
                  $"'{CommonMethods.FormatSqlDate(date)}' <= fecha_fin AND " +
                  $"{filter} AND " +
                  $"id_cuenta_estandar NOT IN " +
                        $"(SELECT id_cuenta_estandar FROM COF_CUENTA WHERE id_mayor = {ledger.Id}) " +
                  "ORDER BY numero_cuenta_estandar";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }


    static internal SubledgerAccount TryGetSubledgerAccount(Ledger ledger, string formattedAccountNo) {
      var sql = "SELECT COF_CUENTA_AUXILIAR.* " +
                "FROM COF_CUENTA_AUXILIAR INNER JOIN VW_COF_CUENTA_AUXILIAR " +
                "ON COF_CUENTA_AUXILIAR.ID_CUENTA_AUXILIAR = VW_COF_CUENTA_AUXILIAR.ID_CUENTA_AUXILIAR " +
               $"WHERE (VW_COF_CUENTA_AUXILIAR.ID_MAYOR = {ledger.Id} OR " +
               $"VW_COF_CUENTA_AUXILIAR.ID_MAYOR_ADICIONAL = {ledger.Id}) " +
               $"AND COF_CUENTA_AUXILIAR.NUMERO_CUENTA_AUXILIAR = '{formattedAccountNo}' " +
               $"AND COF_CUENTA_AUXILIAR.ELIMINADA = 0";


      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<SubledgerAccount>(dataOperation, null);
    }


    static private void CreateLedgerAccount(Ledger ledger,
                                            StandardAccount standardAccount) {
      var newLedgerAccountId = NextLedgerAccountId();

      var dataOperation = DataOperation.Parse("apd_cof_cuenta",
                                              newLedgerAccountId, ledger.Id, standardAccount.Id);

      DataWriter.Execute(dataOperation);
    }


    static private LedgerAccount GetLedgerAccount(Ledger ledger,
                                                  StandardAccount standardAccount) {
      LedgerAccount ledgerAccount = TryGetLedgerAccount(ledger, standardAccount);

      Assertion.AssertObject(ledgerAccount, "ledgerAccount");

      return ledgerAccount;
    }


  }  // class LedgerData

}  // namespace Empiria.FinancialAccounting.Data
