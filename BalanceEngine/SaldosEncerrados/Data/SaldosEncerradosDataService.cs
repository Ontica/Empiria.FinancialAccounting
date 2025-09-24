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
  static internal class SaldosEncerradosDataService {

    static internal FixedList<Account> GetAccountsHistory(AccountsChart accountsChart,
                                                          DateTime fromDate, DateTime toDate) {
      const string CUENTA_RECLASIFICACION_SALDOS_INICIALES = "9.04";

      var sql = "SELECT * FROM VW_COF_CUENTA_ESTANDAR_HIST " +
               $"WHERE ID_TIPO_CUENTAS_STD = {accountsChart.Id} " +
               $"AND NUMERO_CUENTA_ESTANDAR NOT LIKE '{CUENTA_RECLASIFICACION_SALDOS_INICIALES}%' " +
               $"AND {DataCommonMethods.FormatSqlDbDate(fromDate)} <= FECHA_FIN " +
               $"AND FECHA_FIN <= {DataCommonMethods.FormatSqlDbDate(toDate)} " +
               $"ORDER BY FECHA_INICIO, NUMERO_CUENTA_ESTANDAR";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Account>(dataOperation);
    }

  } // class SaldosEncerradosDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
