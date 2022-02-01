/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : SubledgerData                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data service layer for subledger accounts.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data service layer for subledger accounts.</summary>
  static internal class SubledgerData {

    static private EmpiriaHashTable<SubledgerAccount> _subledgerAccountsCache =
                                                             new EmpiriaHashTable<SubledgerAccount>();

    static internal SubledgerAccount GetSubledgerAccount(int id) {
      SubledgerAccount cachedValue;

      if (_subledgerAccountsCache.TryGetValue(id.ToString(), out cachedValue)) {
        return cachedValue;
      }

      var sql = $"SELECT * FROM COF_CUENTA_AUXILIAR WHERE ID_CUENTA_AUXILIAR = {id}";

      var dataOperation = DataOperation.Parse(sql);

      var subledgerAccount = DataReader.GetPlainObject<SubledgerAccount>(dataOperation);

      _subledgerAccountsCache.Insert(id.ToString(), subledgerAccount);

      return subledgerAccount;
    }


    static internal FixedList<SubledgerAccount> GetSubledgerAccountsList() {
      var sql = $"SELECT * FROM COF_CUENTA_AUXILIAR";

      var dataOperation = DataOperation.Parse(sql);

      _subledgerAccountsCache = DataReader.GetPlainObjectHashTable<SubledgerAccount>(dataOperation, x => $"{x.Id}");

      return _subledgerAccountsCache.ToFixedList();
    }


    static internal FixedList<SubledgerAccount> GetSubledgerAccountsList(AccountsChart accountsChart,
                                                                         string filter) {
      var op = DataOperation.Parse("@qry_cof_busca_auxiliares",
                                   accountsChart.Id, filter);

      return DataReader.GetPlainObjectFixedList<SubledgerAccount>(op);
    }


    static internal int NextSubledgerAccountId() {
      return Convert.ToInt32(CommonMethods.GetNextObjectId("SEC_ID_CUENTA_AUXILIAR"));
    }


    static internal SubledgerAccount TryGetSubledgerAccount(string subledgerAccountNumber) {
      var sql = $"SELECT * FROM " +
                $"COF_CUENTA_AUXILIAR " +
                $"WHERE NUMERO_CUENTA_AUXILIAR = '{subledgerAccountNumber}'";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<SubledgerAccount>(dataOperation, null);
    }


    static internal void WriteSubledgerAccount(SubledgerAccount o) {
      var op = DataOperation.Parse("write_cof_cuenta_auxiliar",
                                    o.Id, o.Subledger.Id,
                                    o.Number, o.Name, o.Description,
                                    o.Suspended ? 1: 0);

      DataWriter.Execute(op);

      _subledgerAccountsCache.Insert(o.Id.ToString(), o);
    }


  }  // class SubledgerData

}  // namespace Empiria.FinancialAccounting.Data
