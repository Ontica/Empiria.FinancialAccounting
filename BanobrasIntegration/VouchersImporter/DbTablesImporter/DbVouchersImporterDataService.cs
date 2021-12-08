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

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data methods used to read and write data for vouchers importation data tables.</summary>
  static internal class DbVouchersImporterDataService {

    static internal List<Encabezado> GetEncabezados() {
      var sql = "SELECT * FROM VW_MC_ENCABEZADOS";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectList<Encabezado>(op);
    }


    static internal List<Movimiento> GetMovimientos() {
      var sql = "SELECT * FROM VW_MC_MOVIMIENTOS";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectList<Movimiento>(op);
    }

    static internal ImportVouchersResult GetEncabezadosTotals() {
      var sql = "SELECT ENC_SISTEMA, COUNT(*) AS TOTAL " +
                "FROM VW_MC_ENCABEZADOS " +
                "GROUP BY ENC_SISTEMA ";

      var op = DataOperation.Parse(sql);

      var view = DataReader.GetDataView(op);

      var list = new List<ImportVouchersTotals>();

      int vouchersCount = 0;

      for (int i = 0; i < view.Count; i++) {
        var system = TransactionalSystem.Get(x => x.SourceSystemId == (int) (view[i]["ENC_SISTEMA"]));

        Assertion.AssertObject(system, "system");

        var totals = new ImportVouchersTotals {
          Description = system.Name,
          UID = system.UID,
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

  }  // class DbVouchersImporterDataService

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
