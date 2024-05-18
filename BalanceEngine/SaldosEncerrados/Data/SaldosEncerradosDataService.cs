/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : SaldosEncerradosDataService                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for saldos encerrados.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for saldos encerrados.</summary>
  internal class SaldosEncerradosDataService {


    static internal FixedList<Account> GetAccountsWithChanges(AccountsChart accountsChart,
                                                              DateTime fromDate, DateTime toDate) {
      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
               $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
               $"AND {DataCommonMethods.FormatSqlDbDate(fromDate)} <= FECHA_FIN " +
               $"AND FECHA_FIN <= {DataCommonMethods.FormatSqlDbDate(toDate)} " +
               $"ORDER BY FECHA_INICIO";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }


  } // class SaldosEncerradosDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
