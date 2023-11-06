/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : AccountsListData                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for accounts lists.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.AccountsLists.Data {

  /// <summary>Data access layer for accounts lists.</summary>
  static internal class AccountsListData {

    static internal FixedList<T> GetAccounts<T>(AccountsList accountsList) where T : BaseObject, IAccountListItem {
      var sql = "SELECT * FROM COF_LISTA_CUENTAS " +
                $"WHERE ID_LISTA = {accountsList.Id} AND STATUS_ELEMENTO_LISTA <> 'X' " +
                $"ORDER BY POSICION, NUMERO_CUENTA_ESTANDAR, ID_MAYOR, NUMERO_CUENTA_AUXILIAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<T>(dataOperation);
    }

    static internal FixedList<T> GetAccounts<T>(AccountsList accountsList, string keywords) where T : BaseObject, IAccountListItem {
      var sql = "SELECT * FROM COF_LISTA_CUENTAS " +
          $"WHERE ID_LISTA = {accountsList.Id} AND STATUS_ELEMENTO_LISTA <> 'X' KEYWORDS_FILTER " +
          $"ORDER BY POSICION, NUMERO_CUENTA_ESTANDAR, ID_MAYOR, NUMERO_CUENTA_AUXILIAR";

      var filter = SearchExpression.ParseAndLikeKeywords("KEYWORDS_ELEMENTO_LISTA", keywords);

      if (filter.Length > 0) {
        sql = sql.Replace("KEYWORDS_FILTER", $"AND ({filter})");
      } else {
        sql = sql.Replace("KEYWORDS_FILTER", string.Empty);
      }
      var dataOperation = DataOperation.Parse(sql);


      return DataReader.GetFixedList<T>(dataOperation);
    }


    static internal void Write(ConciliacionDerivadosListItem o) {
      var op = DataOperation.Parse("write_cof_lista_cuentas",
        o.Id, o.UID, o.List.Id, o.GetEmpiriaType().Id, -1, o.Account.Number,
        string.Empty, string.Empty, string.Empty,
        string.Empty, string.Empty, 1,
        o.StartDate, o.EndDate, o.Keywords, (char) o.Status);

      DataWriter.Execute(op);
    }


    internal static void Write(SwapsCoberturaListItem o) {
      var op = DataOperation.Parse("write_cof_lista_cuentas",
        o.Id, o.UID, o.List.Id, o.GetEmpiriaType().Id, -1, string.Empty,
        string.Empty, string.Empty, o.SubledgerAccount.Number,
        string.Empty, o.ExtData.ToString(), 1,
        o.StartDate, o.EndDate, o.Keywords, (char) o.Status);

      DataWriter.Execute(op);
    }

    internal static void Write(DepreciacionActivoFijoListItem o) {
      var op = DataOperation.Parse("write_cof_lista_cuentas",
        o.Id, o.UID, o.List.Id, o.GetEmpiriaType().Id, o.Ledger.Id, string.Empty,
        string.Empty, string.Empty, o.AuxiliarHistorico.Number,
        string.Empty, o.ExtData.ToString(), 1,
        o.StartDate, o.EndDate, o.Keywords, (char) o.Status);

      DataWriter.Execute(op);
    }

    internal static void Write(PrestamosInterbancariosListItem o) {
      var op = DataOperation.Parse("write_cof_lista_cuentas",
        o.Id, o.UID, o.List.Id, o.GetEmpiriaType().Id, -1, string.Empty,
        o.Sector.Code, o.Currency.Code, o.SubledgerAccount.Number,
        string.Empty, o.ExtData.ToString(), 1,
        o.StartDate, o.EndDate, o.Keywords, (char) o.Status);

      DataWriter.Execute(op);
    }
  }  // class AccountsListData

}  // namespace Empiria.FinancialAccounting.Data
