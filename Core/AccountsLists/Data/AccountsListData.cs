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

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for accounts lists.</summary>
  static internal class AccountsListData {

    static internal FixedList<AccountsListItem> GetAccounts(AccountsList accountsList) {
      var sql = "SELECT * FROM COF_LISTA_CUENTAS " +
                $"WHERE ID_LISTA = {accountsList.Id} " +
                $"ORDER BY POSICION";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<AccountsListItem>(dataOperation);
    }

  }  // class AccountsListData

}  // namespace Empiria.FinancialAccounting.Data
