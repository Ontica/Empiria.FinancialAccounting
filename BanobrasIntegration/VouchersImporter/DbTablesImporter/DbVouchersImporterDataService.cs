/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Data Service                         *
*  Type     : DbVouchersImporterDataService                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data methods used to read and write data for vouchers importation data tables.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;
using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;
using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data methods used to read and write data for vouchers importation data tables.</summary>
  static internal class DbVouchersImporterDataService {

    static internal List<Encabezado> GetEncabezados(string importationSetUID) {
      string filter = GetVouchersFilter(importationSetUID, true);

      var sql = "SELECT * FROM VW_MC_ENCABEZADOS " +
                "WHERE " + filter;

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectList<Encabezado>(op);
    }


    static internal List<Movimiento> GetMovimientos(string importationSetUID) {
      string filter = GetVouchersFilter(importationSetUID, false);

      var sql = "SELECT * FROM VW_MC_MOVIMIENTOS " +
                "WHERE " + filter;

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectList<Movimiento>(op);
    }


    static internal ImportVouchersResult GetEncabezadosTotals() {
      var sql = "SELECT ENC_SISTEMA, ENC_TIPO_CONT, ENC_FECHA_VOL, COUNT(*) AS TOTAL " +
                "FROM VW_MC_ENCABEZADOS " +
                "GROUP BY ENC_SISTEMA, ENC_TIPO_CONT, ENC_FECHA_VOL ";

      var op = DataOperation.Parse(sql);

      var view = DataReader.GetDataView(op);

      var list = new List<ImportVouchersTotals>();

      int vouchersCount = 0;

      for (int i = 0; i < view.Count; i++) {
        int idSistema = (int) view[i]["ENC_SISTEMA"];
        int tipoContabilidad = (int) view[i]["ENC_TIPO_CONT"];
        DateTime fechaAfectacion = (DateTime) view[i]["ENC_FECHA_VOL"];

        var totals = new ImportVouchersTotals {
          Description = GetImportationSetDescription(idSistema, tipoContabilidad, fechaAfectacion),
          UID = GetImportationSetUID(idSistema, tipoContabilidad, fechaAfectacion),
          VouchersCount = (int) (decimal) view[i]["TOTAL"]
        };
        vouchersCount += totals.VouchersCount;
        list.Add(totals);
      }

      var result = new ImportVouchersResult();

      list.Sort((x, y) => x.Description.CompareTo(y.Description));

      result.VoucherTotals = list.ToFixedList();
      result.VouchersCount = vouchersCount;

      return result;
    }


    static internal string GetImportationSetUID(int idSistema, int tipoContabilidad, DateTime fechaAfectacion) {
      return $"{idSistema}|{tipoContabilidad}|{fechaAfectacion.ToString("yyyy-MM-dd")}";
    }


    static private string GetImportationSetDescription(int idSistema, int tipoContabilidad, DateTime fechaAfectacion) {
      var system = TransactionalSystem.Get(x => x.SourceSystemId == idSistema);

      Assertion.AssertObject(system, "system");

      return $"{system.Name}, {fechaAfectacion.ToString("yyyy/MM/dd")}, Tipo Cont. {tipoContabilidad}";
    }


    static internal void Reallocate(int idSistema, int tipoContabilidad, DateTime fechaAfectacion) {
      var operation = DataOperation.Parse("do_store_cof_volantes",
                                          idSistema, tipoContabilidad, fechaAfectacion);

      DataWriter.Execute(operation);
    }

    internal static long NextIdVolante() {
      return CommonMethods.GetNextObjectId("SEC_ID_VOLANTE");
    }

    internal static long NextIdVolanteIssue() {
      return CommonMethods.GetNextObjectId("SEC_ID_VOLANTE_ISSUE");
    }

    internal static void StoreIssues(Encabezado encabezado, ToImportVoucher item, long idVolante) {
      foreach (var issue in item.AllIssues) {
        var operation = DataOperation.Parse("apd_cof_volante_issue",
                                            NextIdVolanteIssue(), idVolante, -1, issue.Description);
        DataWriter.Execute(operation);
      }
      MergeVouchers(encabezado, idVolante, -1);
    }


    internal static void MergeVouchers(Encabezado encabezado, long idVolante, long voucherId) {
      var operation = DataOperation.Parse("do_merge_cof_volante_poliza",
                                           encabezado.IdSistema,
                                           encabezado.TipoContabilidad, encabezado.FechaAfectacion,
                                           encabezado.NumeroVolante, idVolante, voucherId);
      DataWriter.Execute(operation);
    }


    static private string GetVouchersFilter(string importationSetUID, bool forEncabezados) {
      string[] parts = importationSetUID.Split('|');

      int idSistema = int.Parse(parts[0]);
      int tipoContabilidad = int.Parse(parts[1]);

      string[] fechaAfectacionParts = parts[2].Split('-');

      DateTime fechaAfectacion = new DateTime(int.Parse(fechaAfectacionParts[0]),
                                              int.Parse(fechaAfectacionParts[1]),
                                              int.Parse(fechaAfectacionParts[2]));

      if (forEncabezados) {
        return $"(ENC_SISTEMA = {idSistema} AND " +
                $"ENC_TIPO_CONT = {tipoContabilidad} AND " +
                $"ENC_FECHA_VOL = '{CommonMethods.FormatSqlDate(fechaAfectacion)}')";

      } else {
        return $"(MCO_SISTEMA = {idSistema} AND " +
                $"MCO_TIPO_CONT = {tipoContabilidad} AND " +
                $"MCO_FECHA_VOL = '{CommonMethods.FormatSqlDate(fechaAfectacion)}')";

      }
    }


  }  // class DbVouchersImporterDataService

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
