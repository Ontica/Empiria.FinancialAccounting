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


    static internal void DeleteBalances(StoredBalanceSet storedBalanceSet) {
      var sql = $"DELETE FROM COF_SALDOS " +
                $"WHERE (ID_GRUPO_SALDOS = {storedBalanceSet.Id})";

      var dataOperation = DataOperation.Parse(sql);

      DataWriter.Execute(dataOperation);
    }


    static internal void WriteStoredBalance(StoredBalance o) {
      var sql = "INSERT INTO COF_SALDOS (" +
                   "ID_SALDO, ID_CUENTA_ESTANDAR, ID_CUENTA, " +
                   "ID_MONEDA, ID_SECTOR, ID_CUENTA_AUXILIAR, " +
                   "FECHA_SALDO, SALDO_INICIAL, ID_GRUPO_SALDOS" +
                ") VALUES (" +
                  $"{o.Id}, {o.StandardAccountId}, {o.LedgerAccountId}, " +
                  $"{o.Currency.Id}, {o.Sector.Id}, {o.SubsidiaryAccountId}, " +
                  $"'{CommonMethods.FormatSqlDate(o.StoredBalanceSet.BalancesDate)}', " +
                  $"{o.Balance}, {o.StoredBalanceSet.Id})";

      var dataOperation = DataOperation.Parse(sql);

      DataWriter.Execute(dataOperation);

      //var dataOperation = DataOperation.Parse("APD_COF_SALDOS", o.Id, o.StandardAccountId,
      //                                        o.LedgerAccountId, o.Currency.Id, o.Sector.Id,
      //                                        o.SubsidiaryAccountId, o.StoredBalanceSet.BalancesDate,
      //                                        o.Balance, o.StoredBalanceSet.Id);

      //DataWriter.Execute(dataOperation);
    }

  }  // class StoredBalanceDataService

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Data
