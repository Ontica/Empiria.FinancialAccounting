/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Static data services                    *
*  Type     : SistemaLegadoData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Servicios de acceso a datos al sistema legado de flujo de efectivo.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

using Empiria.Financial.Integration;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Servicios de acceso a datos al sistema legado de flujo de efectivo.</summary>
  static internal class SistemaLegadoData {

    static internal FixedList<MovimientoSistemaLegado> LeerMovimientos(long idPoliza) {
      var sql = "SELECT MCOM_NUM_VOL, MCOM_FOLIO_VOL, MCOM_REG_CONTABLE, MCOM_NUM_AUX, MCOM_SECTOR, " +
                       "MCOM_MONEDA, MCOM_DISPONIB, MCOM_IMPORTE, MCOM_CVE_MOV, MCOM_CONCEPTO " +
                "FROM Z_MOVS_PYC " +
                $"WHERE MCOM_NUM_VOL = {idPoliza} " +
                $"ORDER BY MCOM_FOLIO_VOL";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<MovimientoSistemaLegado>(op);
    }


    static internal FixedList<MovimientoSistemaLegado> LeerMovimientos(FixedList<long> idsPolizas) {
      Assertion.Require(idsPolizas, nameof(idsPolizas));

      if (idsPolizas.Count == 0) {
        return new FixedList<MovimientoSistemaLegado>();
      }

      var filter = SearchExpression.ParseInSet("MCOM_NUM_VOL", idsPolizas);

      var sql = "SELECT MCOM_NUM_VOL, MCOM_FOLIO_VOL, MCOM_REG_CONTABLE, MCOM_NUM_AUX, MCOM_SECTOR, " +
                       "MCOM_MONEDA, MCOM_DISPONIB, MCOM_IMPORTE, MCOM_CVE_MOV, MCOM_CONCEPTO " +
                "FROM Z_MOVS_PYC " +
                $"WHERE {filter} " +
                $"ORDER BY MCOM_NUM_VOL, MCOM_FOLIO_VOL";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<MovimientoSistemaLegado>(op);
    }


    static internal void LimpiarTransacciones() {
      var sql = "SELECT MCOM_REG_CONTABLE, MCOM_NUM_VOL, MCOM_FOLIO_VOL " +
                "FROM Z_MOVS_PYC " +
                "WHERE SUBSTR(MCOM_REG_CONTABLE, -1) = '0'";

      var op = DataOperation.Parse(sql);

      var data = DataReader.GetDataTable(op);

      for (int i = 0; i < data.Rows.Count; i++) {
        var cuenta = (string) data.Rows[i]["MCOM_REG_CONTABLE"];
        var idVolante = (long) (float) data.Rows[i]["MCOM_NUM_VOL"];
        var idMovimiento = (long) (float) data.Rows[i]["MCOM_FOLIO_VOL"];

        sql = "UPDATE Z_MOVS_PYC " +
             $"SET MCOM_REG_CONTABLE = '{IntegrationLibrary.FormatAccountNumber(cuenta)}' " +
             $"WHERE MCOM_NUM_VOL = {idVolante} AND MCOM_FOLIO_VOL = {idMovimiento} AND MCOM_REG_CONTABLE = '{cuenta}'";

        op = DataOperation.Parse(sql);

        DataWriter.Execute(op);
      }
    }

  }  // class SistemaLegadoData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
