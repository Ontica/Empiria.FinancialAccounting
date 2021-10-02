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

using Empiria.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data methods used to read and write data for vouchers importation data tables.</summary>
  static internal class DbVouchersImporterDataService {

    static readonly DateTime DB_START_IMPORTATION_DATE = new DateTime(2021, 8, 1);

    static internal FixedList<Encabezado> GetEncabezados() {
      var sql = "SELECT * FROM VW_MC_ENCABEZADOS " +
               $"WHERE ENC_FECHA_VOL >= '{CommonMethods.FormatSqlDate(DB_START_IMPORTATION_DATE)}'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<Encabezado>(op);
    }

    static internal FixedList<Movimiento> GetMovimientos() {
      var sql = "SELECT * FROM VW_MC_MOVIMIENTOS " +
         $"WHERE MCO_FECHA_VOL >= '{CommonMethods.FormatSqlDate(DB_START_IMPORTATION_DATE)}'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<Movimiento>(op);
    }

  }  // class DbVouchersImporterDataService

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
