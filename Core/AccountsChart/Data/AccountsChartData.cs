/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : AccountsChartData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for the accounts chart.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for the accounts chart.</summary>
  static internal class AccountsChartData {

    static internal FixedList<Account> GetAccounts(AccountsChart accountsChart) {
      var sql = "SELECT * FROM COF_CUENTA_ESTANDAR " +
                $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"ORDER BY NUMERO_CUENTA_ESTANDAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }

  }  // class AccountsChartData

}  // namespace Empiria.FinancialAccounting.Data
