/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Balances Exporter                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Data Service                          *
*  Type     : ExportBalancesDataService                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Data methods to store exported balances.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.Data;

using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Data {

  /// <summary>Data methods to store exported balances.</summary>
  static internal class ExportBalancesDataService {

    #region Public methods

    static internal EmpiriaHashTable<CalificacionMoneda> GetCalificacionMonedaHashTable() {
      var sql = "SELECT * FROM CALIFICA_MONEDA " +
                "WHERE CALIFICA_SALDO IS NOT NULL";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectHashTable<CalificacionMoneda>(dataOperation,
                                                                    x => $"{x.Cuenta}||{x.Sector}||{x.Auxiliar}");
    }


    static internal void StoreBalances(ExportBalancesCommand command,
                                       FixedList<ExportedBalancesDto> balances) {
      DeleteStoredBalances(command);

      foreach (var balance in balances) {
        WriteBalances(command.StoreInto, balance);
      }
    }

    #endregion Public methods

    #region Private methods


    static private void DeleteStoredBalances(ExportBalancesCommand command) {
      var op = DataOperation.Parse("del_saldos_tabla_intermedia",
                                    command.StoreInto.ToString(),
                                    command.AccountsChartId,
                                    command.ToDate, command.ToDate.Year,
                                    command.ToDate.Month, command.ToDate.Day);

      DataWriter.Execute(op);
    }


    static private void WriteBalances(StoreBalancesInto storeInto,
                                      ExportedBalancesDto o) {
      string nullString = null;

      var op = DataOperation.Parse("apd_saldo_tabla_intermedia",
                              storeInto.ToString(),
                              o.Empresa,
                              o.Fecha, o.Anio, o.Mes, o.Dia,
                              o.Area, o.Moneda, o.NumeroMayor,
                              o.Cuenta, o.Sector, o.Auxiliar, o.FechaUltimoMovimiento,
                              o.Saldo, o.MonedaOrigen, o.NaturalezaCuenta, o.SaldoPromedio,
                              o.MontoDebito, o.MontoCredito, o.SaldoAnterior,
                              o.CalificaMoneda == "null" ? nullString : o.CalificaMoneda);

      DataWriter.Execute(op);
    }

    #endregion Private methods

  }  // class ExportBalancesDataService

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Data
