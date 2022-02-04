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


    internal static void TestViews() {

      for (int i = 0; i < 20; i++) {
        int DD = EmpiriaMath.GetRandom(1, 28);
        int MM = EmpiriaMath.GetRandom(1, 12);
        int YYYY = EmpiriaMath.GetRandom(2019, 2022);

        var date = new DateTime(YYYY, MM, DD);

        var condition = $"FECHA_PRUEBA >= '{CommonMethods.FormatSqlDate(date)}'";

        var sql = $"SELECT COUNT(*) FROM vw_cof_movimiento_test_a where {condition}";

        var dataOperation = DataOperation.Parse(sql);

        DateTime now = DateTime.Now;

        decimal rows = DataReader.GetScalar<decimal>(dataOperation, 0);

        EmpiriaLog.Info($"Ejecución segs: {DateTime.Now.Subtract(now).TotalSeconds} [{condition}], " +
                        $"days = {DateTime.Today.Subtract(date).TotalDays}, rows = {rows}");
      }
    }



    static internal EmpiriaHashTable<Account> GetAccounts(AccountsChart accountsChart) {
      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
                $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"ORDER BY NUMERO_CUENTA_ESTANDAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetHashTable<Account>(dataOperation, x => x.Number);
    }


    static internal FixedList<Account> GetAccountHistory(AccountsChart accountsChart,
                                                         string accountNumber) {
      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
               $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
               $"AND NUMERO_CUENTA_ESTANDAR = '{accountNumber}' " +
               $"ORDER BY FECHA_INICIO";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }

    static internal FixedList<Ledger> GetAccountChartLedgers(AccountsChart accountsChart) {
      var sql = "SELECT COF_MAYOR.* " +
                $"FROM COF_MAYOR " +
                $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} AND ELIMINADO = 0 " +
                $"ORDER BY NUMERO_MAYOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Ledger>(dataOperation);
    }


    static internal FixedList<AreaRule> GetAccountsAreasRules() {
      var sql = "SELECT COF_MAPEO_AREA.* " +
                $"FROM COF_MAPEO_AREA " +
                $"ORDER BY ID_CUENTA_ESTANDAR, COF_MAPEO_AREA.PATRON_AREA, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<AreaRule>(dataOperation);
    }


    static internal FixedList<CurrencyRule> GetAccountsCurrenciesRules() {
      var sql = "SELECT COF_MAPEO_MONEDA.* " +
                $"FROM AO_CURRENCIES INNER JOIN COF_MAPEO_MONEDA " +
                $"ON AO_CURRENCIES.CURRENCY_ID = COF_MAPEO_MONEDA.ID_MONEDA " +
                $"ORDER BY COF_MAPEO_MONEDA.ID_CUENTA_ESTANDAR, AO_CURRENCIES.O_ID_MONEDA, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<CurrencyRule>(dataOperation);
    }


    static internal FixedList<LedgerRule> GetAccountsLedgersRules() {
      var sql = "SELECT COF_CUENTA.* " +
                $"FROM COF_MAYOR INNER JOIN COF_CUENTA " +
                $"ON COF_MAYOR.ID_MAYOR = COF_CUENTA.ID_MAYOR " +
                $"ORDER BY COF_MAYOR.NUMERO_MAYOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<LedgerRule>(dataOperation);
    }


    static internal FixedList<SectorRule> GetAccountsSectorsRules() {
      var sql = "SELECT COF_MAPEO_SECTOR.* " +
               $"FROM COF_SECTOR INNER JOIN COF_MAPEO_SECTOR " +
               $"ON COF_SECTOR.ID_SECTOR = COF_MAPEO_SECTOR.ID_SECTOR " +
               $"ORDER BY COF_MAPEO_SECTOR.ID_CUENTA_ESTANDAR, COF_SECTOR.CLAVE_SECTOR, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<SectorRule>(dataOperation);
    }


    static internal Account GetCurrentAccountWithStandardAccountId(int standardAccountId) {
      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
               $"WHERE ID_CUENTA_ESTANDAR = {standardAccountId} " +
               $"ORDER BY FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetObject<Account>(dataOperation);
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
