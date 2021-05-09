﻿/* Empiria Financial *****************************************************************************************
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


    static internal FixedList<Currency> GetAccountCurrencies(Account account) {
      var sql = "SELECT AO_CURRENCIES.* " +
                $"FROM AO_CURRENCIES INNER JOIN COF_MAPEO_MONEDA " +
                $"ON AO_CURRENCIES.CURRENCY_ID = COF_MAPEO_MONEDA.ID_MONEDA " +
                $"WHERE COF_MAPEO_MONEDA.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY AO_CURRENCIES.ABBREV";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Currency>(dataOperation);
    }


    static internal FixedList<Sector> GetAccountSectors(Account account) {
      var sql = "SELECT COF_SECTOR.* " +
                $"FROM COF_SECTOR INNER JOIN COF_MAPEO_SECTOR " +
                $"ON COF_SECTOR.ID_SECTOR = COF_MAPEO_SECTOR.ID_SECTOR " +
                $"WHERE COF_MAPEO_SECTOR.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY COF_SECTOR.CLAVE_SECTOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Sector>(dataOperation);
    }

  }  // class AccountsChartData

}  // namespace Empiria.FinancialAccounting.Data
