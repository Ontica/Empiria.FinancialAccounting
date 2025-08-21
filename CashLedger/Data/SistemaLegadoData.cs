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

using Empiria.Financial.Integration.CashLedger;

namespace Empiria.FinancialAccounting.CashLedger.Data {

  /// <summary>Servicios de acceso a datos al sistema legado de flujo de efectivo.</summary>
  static internal class SistemaLegadoData {

    static internal FixedList<MovimientoSistemaLegado> LeerMovimientos(long idPoliza) {
      var sql = "SELECT * FROM Z_MOVS_PYC " +
                $"WHERE MCOM_NUM_VOL = {idPoliza} " +
                $"ORDER BY MCOM_FOLIO_VOL";

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
             $"SET MCOM_REG_CONTABLE = '{FormatAccountNumber(cuenta)}' " +
             $"WHERE MCOM_NUM_VOL = {idVolante} AND MCOM_FOLIO_VOL = {idMovimiento} AND MCOM_REG_CONTABLE = '{cuenta}'";

        op = DataOperation.Parse(sql);

        DataWriter.Execute(op);
      }
    }


    static private string FormatAccountNumber(string accountNumber) {
      string temp = EmpiriaString.TrimSpacesAndControl(accountNumber);

      if (temp.Length == 0) {
        return temp;
      }

      if (temp.Contains("*") || temp.Contains("]") || temp.Contains("^") || temp.Contains("~")) {
        return temp;
      }

      char separator = '.';
      string pattern = "0.00.00.00.00.00.00.00.00.00.00";

      temp = temp.Replace(separator.ToString(), string.Empty);

      temp = temp.TrimEnd('0');

      if (temp.Length > EmpiriaString.CountOccurences(pattern, '0')) {
        Assertion.RequireFail($"Number of placeholders in pattern ({pattern}) is less than the " +
                              $"number of characters in the input string ({accountNumber}).");
      } else {
        temp = temp.PadRight(EmpiriaString.CountOccurences(pattern, '0'), '0');
      }

      for (int i = 0; i < pattern.Length; i++) {
        if (pattern[i] == separator) {
          temp = temp.Insert(i, separator.ToString());
        }
      }

      while (true) {
        if (temp.EndsWith($"{separator}0000")) {
          temp = temp.Remove(temp.Length - 5);

        } else if (temp.EndsWith($"{separator}000")) {
          temp = temp.Remove(temp.Length - 4);

        } else if (temp.EndsWith($"{separator}00")) {
          temp = temp.Remove(temp.Length - 3);

        } else if (temp.EndsWith($"{separator}0")) {
          temp = temp.Remove(temp.Length - 2);

        } else {
          break;
        }
      }

      return temp;
    }

  }  // class SistemaLegadoData

}  // namespace Empiria.FinancialAccounting.CashLedger.Data
