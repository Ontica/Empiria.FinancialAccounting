/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Static data services                    *
*  Type     : SistemaLegadoData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Servicios de acceso a datos al sistema legado de flujo de efectivo.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Text;

using Empiria.Data;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Servicios de acceso a datos al sistema legado de flujo de efectivo.</summary>
  static internal class SistemaLegadoData {

    static internal void ActualizarMovimientos(FixedList<CashEntry> entries) {
      var stringBuilder = new StringBuilder(entries.Count * 116);

      stringBuilder.Append("BEGIN ");

      foreach (var entry in entries) {
        var localSql =
              "UPDATE COF_MOVIMIENTO_BIS SET " +
                   $"CONCEPTO_FLUJO_LEGADO = '{entry.CuentaSistemaLegado}' " +
              $"WHERE ID_MOVIMIENTO = {entry.Id}; ";

        stringBuilder.Append(localSql);
      }

      stringBuilder.AppendLine("END;");

      var op = DataOperation.Parse(stringBuilder.ToString());

      DataWriter.Execute(op);
    }


    static internal FixedList<MovimientoSistemaLegado> LeerMovimientos(FixedList<long> idsPolizas) {
      Assertion.Require(idsPolizas, nameof(idsPolizas));
      Assertion.Require(idsPolizas.Count > 0, nameof(idsPolizas));

      var filter = SearchExpression.ParseInSet("MCOM_NUM_VOL", idsPolizas);

      var sql = "SELECT MCOM_NUM_VOL, MCOM_FOLIO_VOL, MCOM_REG_CONTABLE, MCOM_NUM_AUX, MCOM_SECTOR, " +
                       "MCOM_MONEDA, MCOM_DISPONIB, MCOM_IMPORTE, MCOM_CVE_MOV, MCOM_CONCEPTO " +
                "FROM Z_MOVS_PYC " +
                  $"WHERE {filter} " +
                $"ORDER BY MCOM_NUM_VOL, MCOM_FOLIO_VOL";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<MovimientoSistemaLegado>(op);
    }


    static internal FixedList<long> TransaccionesSinActualizar() {
      var sql = "SELECT DISTINCT ID_TRANSACCION " +
                  "FROM VW_COF_MOVIMIENTO_BIS " +
                "WHERE CONCEPTO_FLUJO_LEGADO IS NULL";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFieldValues<long>(op)
                       .ToFixedList();
    }

  }  // class SistemaLegadoData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
