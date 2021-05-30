/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Data Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll        Pattern   : Data Service                          *
*  Type     : StoredBalanceDataService                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Data methods to store and return account balances.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Data methods to store and return account balances.</summary>
  static internal class StoredBalanceDataService {

    static internal FixedList<StoredBalance> GetBalances(StoredBalanceSet balanceSet) {
      var sql = "SELECT * FROM VW_COF_SALDOS " +
               $"WHERE ID_GRUPO_SALDOS = {balanceSet.Id} " +
               $"ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
               $"NUMERO_CUENTA_AUXILIAR";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<StoredBalance>(op);
    }

  }  // class StoredBalanceDataService

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Data
