/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : AccountsChartData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for accounts charts.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for accounts charts.</summary>
  static internal class AccountsChartData {

    static internal EmpiriaHashTable<Account> GetAccounts(AccountsChart accountsChart) {
      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
                $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"AND FECHA_INICIO <= '{CommonMethods.FormatSqlDate(DateTime.Today)}' AND + " +
                $"'{CommonMethods.FormatSqlDate(DateTime.Today)}' <= FECHA_FIN " +
                $"ORDER BY NUMERO_CUENTA_ESTANDAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetHashTable<Account>(dataOperation, x => x.Number);
    }


    static internal FixedList<AreaRule> GetAccountAreasRules(Account account) {
      var sql = "SELECT COF_MAPEO_AREA.* " +
                $"FROM COF_MAPEO_AREA " +
                $"WHERE COF_MAPEO_AREA.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY COF_MAPEO_AREA.PATRON_AREA, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<AreaRule>(dataOperation);
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


    static internal FixedList<CurrencyRule> GetAccountCurrenciesRules(Account account) {
      var sql = "SELECT COF_MAPEO_MONEDA.* " +
                $"FROM AO_CURRENCIES INNER JOIN COF_MAPEO_MONEDA " +
                $"ON AO_CURRENCIES.CURRENCY_ID = COF_MAPEO_MONEDA.ID_MONEDA " +
                $"WHERE COF_MAPEO_MONEDA.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY AO_CURRENCIES.ABBREV, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CurrencyRule>(dataOperation);
    }


    static internal FixedList<Ledger> GetAccountLedgers(Account account) {
      var sql = "SELECT COF_MAYOR.* " +
                $"FROM COF_MAYOR INNER JOIN COF_CUENTA " +
                $"ON COF_MAYOR.ID_MAYOR = COF_CUENTA.ID_MAYOR " +
                $"WHERE COF_CUENTA.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY COF_MAYOR.NUMERO_MAYOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Ledger>(dataOperation);
    }


    static internal FixedList<LedgerRule> GetAccountLedgersRules(Account account) {
      var sql = "SELECT COF_CUENTA.* " +
                $"FROM COF_MAYOR INNER JOIN COF_CUENTA " +
                $"ON COF_MAYOR.ID_MAYOR = COF_CUENTA.ID_MAYOR " +
                $"WHERE COF_CUENTA.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY COF_MAYOR.NUMERO_MAYOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<LedgerRule>(dataOperation);
    }


    static internal FixedList<Sector> GetAccountSectors(Account account) {
      var sql = "SELECT COF_SECTOR.* " +
                $"FROM COF_SECTOR INNER JOIN COF_MAPEO_SECTOR " +
                $"ON COF_SECTOR.ID_SECTOR = COF_MAPEO_SECTOR.ID_SECTOR " +
                $"WHERE COF_MAPEO_SECTOR.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY COF_SECTOR.CLAVE_SECTOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<Sector>(dataOperation);
    }


    static internal FixedList<SectorRule> GetAccountSectorsRules(Account account) {
      var sql = "SELECT COF_MAPEO_SECTOR.* " +
                $"FROM COF_SECTOR INNER JOIN COF_MAPEO_SECTOR " +
                $"ON COF_SECTOR.ID_SECTOR = COF_MAPEO_SECTOR.ID_SECTOR " +
                $"WHERE COF_MAPEO_SECTOR.ID_CUENTA_ESTANDAR = {account.Id} " +
                $"ORDER BY COF_SECTOR.CLAVE_SECTOR, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<SectorRule>(dataOperation);
    }


    static internal FixedList<Account> SearchAccounts(AccountsChart accountsChart,
                                                      string filter) {
      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
                $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                (filter.Length != 0 ? $" AND ({filter}) " : String.Empty) +
                $"ORDER BY NUMERO_CUENTA_ESTANDAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }

  }  // class AccountsChartData

}  // namespace Empiria.FinancialAccounting.Data
