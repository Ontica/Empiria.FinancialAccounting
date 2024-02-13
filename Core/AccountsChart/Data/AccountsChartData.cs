/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : AccountsChartData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for accounts charts reading operations.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.Collections;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for accounts charts reading operations.</summary>
  static internal class AccountsChartData {

    static internal string BuildSearchAccountsFilter(AccountsChart accountsChart, string keywords) {
      keywords = EmpiriaString.TrimSpacesAndControl(keywords);

      string[] keywordsParts = keywords.Split(' ');

      string accountNumber = string.Empty;

      for (int i = 0; i < keywordsParts.Length; i++) {
        string part = keywordsParts[i];

        part = EmpiriaString.RemovePunctuation(part)
                            .Replace(" ", string.Empty);

        if (EmpiriaString.IsInteger(part)) {
          accountNumber = part;
          keywordsParts[i] = string.Empty;
          break;
        }
      }

      if (accountNumber.Length != 0 && keywordsParts.Length == 1) {
        accountNumber = accountsChart.FormatAccountNumber(accountNumber);

        return $"NUMERO_CUENTA_ESTANDAR LIKE '{accountNumber}%'";

      } else if (accountNumber.Length != 0 && keywordsParts.Length > 1) {
        accountNumber = accountsChart.FormatAccountNumber(accountNumber);

        return $"NUMERO_CUENTA_ESTANDAR LIKE '{accountNumber}%' AND " +
               SearchExpression.ParseAndLikeKeywords("keywords_cuenta_estandar_hist", String.Join(" ", keywordsParts));

      } else {
        return SearchExpression.ParseAndLikeKeywords("keywords_cuenta_estandar_hist", keywords);
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


    static internal FixedList<AreaRule> GetAccountsAreasRules(AccountsChart accountsChart) {
      var sql = "SELECT COF_MAPEO_AREA.* " +
                $"FROM COF_MAPEO_AREA INNER JOIN COF_CUENTA_ESTANDAR " +
                $"ON COF_MAPEO_AREA.ID_CUENTA_ESTANDAR = COF_CUENTA_ESTANDAR.ID_CUENTA_ESTANDAR " +
                $"WHERE COF_CUENTA_ESTANDAR.ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"ORDER BY COF_MAPEO_AREA.ID_CUENTA_ESTANDAR, COF_MAPEO_AREA.PATRON_AREA, FECHA_FIN DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<AreaRule>(dataOperation);
    }


    static internal EmpiriaHashTable<FixedList<CurrencyRule>> GetAccountsCurrenciesRules(AccountsChart accountsChart) {
      var sql = "SELECT COF_MAPEO_MONEDA.* " +
                $"FROM AO_CURRENCIES INNER JOIN COF_MAPEO_MONEDA " +
                $"ON AO_CURRENCIES.CURRENCY_ID = COF_MAPEO_MONEDA.ID_MONEDA " +
                $"INNER JOIN COF_CUENTA_ESTANDAR " +
                $"ON COF_MAPEO_MONEDA.ID_CUENTA_ESTANDAR = COF_CUENTA_ESTANDAR.ID_CUENTA_ESTANDAR " +
                $"WHERE COF_CUENTA_ESTANDAR.ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"ORDER BY COF_MAPEO_MONEDA.ID_CUENTA_ESTANDAR, AO_CURRENCIES.O_ID_MONEDA, FECHA_FIN DESC";

      var op = DataOperation.Parse(sql);

      var groupedList = DataReader.GetPlainObjectFixedList<CurrencyRule>(op)
                                  .GroupBy(x => x.StandardAccountId)
                                  .ToFixedList();

      var hashTable = new EmpiriaHashTable<FixedList<CurrencyRule>>(groupedList.Count);

      foreach (var item in groupedList) {
        hashTable.Insert(item.Key.ToString(),
                         item.ToFixedList());
      }

      return hashTable;
    }


    static internal FixedList<LedgerRule> GetAccountsLedgersRules(AccountsChart accountsChart) {
      var sql = "SELECT COF_CUENTA.* " +
                $"FROM COF_MAYOR INNER JOIN COF_CUENTA " +
                $"ON COF_MAYOR.ID_MAYOR = COF_CUENTA.ID_MAYOR " +
                $"INNER JOIN COF_CUENTA_ESTANDAR " +
                $"ON COF_CUENTA.ID_CUENTA_ESTANDAR = COF_CUENTA_ESTANDAR.ID_CUENTA_ESTANDAR " +
                $"WHERE COF_CUENTA_ESTANDAR.ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"ORDER BY COF_CUENTA.ID_CUENTA_ESTANDAR, COF_MAYOR.NUMERO_MAYOR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<LedgerRule>(dataOperation);
    }


    static internal FixedList<SectorRule> GetAccountsSectorsRules(AccountsChart accountsChart) {
      var sql = "SELECT COF_MAPEO_SECTOR.* " +
                $"FROM COF_SECTOR INNER JOIN COF_MAPEO_SECTOR " +
                $"ON COF_SECTOR.ID_SECTOR = COF_MAPEO_SECTOR.ID_SECTOR " +
                $"INNER JOIN COF_CUENTA_ESTANDAR " +
                $"ON COF_MAPEO_SECTOR.ID_CUENTA_ESTANDAR = COF_CUENTA_ESTANDAR.ID_CUENTA_ESTANDAR " +
                $"WHERE COF_CUENTA_ESTANDAR.ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
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


    static internal EmpiriaHashTable<StandardAccount> GetStandardAccounts(AccountsChart accountsChart) {
      var sql = "SELECT * FROM COF_CUENTA_ESTANDAR " +
                $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
                $"ORDER BY NUMERO_CUENTA_ESTANDAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetHashTable<StandardAccount>(dataOperation, x => x.Number);
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
