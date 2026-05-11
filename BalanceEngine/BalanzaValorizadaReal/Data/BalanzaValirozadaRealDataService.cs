/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras SIAL Services                     Component : Data Layer                              *
*  Assembly : Banobras.PYC.ExternalInterfaces.dll        Pattern   : Data Services                           *
*  Type     : SialDataService                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data access for SIAL payroll data.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.Data;



namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data access for SIAL payroll data.</summary>
  static internal class BalanzaValirozadaRealDataService {

    internal static FixedList<BalanzaValorizadaReal> GetBalance(DateTime fromDate, DateTime toDate) {

      var sql = "WITH SDO_INICIAL AS " +
"        ( " +
           " SELECT " +
                "ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_MONEDA, ID_MONEDA_REAL, " +
                "SUM(DEBE) - SUM(HABER) SALDO_INICIAL, " +
                "SUM(DEBE_MONEDA_REAL) - SUM(HABER_MONEDA_REAL) SALDO_INICIAL_REAL " +
            "FROM VW_COF_MOVIMIENTO_BIS " +
            "WHERE TO_DATE('01-01-2026', 'DD-MM-YYYY') <= FECHA_AFECTACION  " +
            "GROUP BY ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_MONEDA, ID_MONEDA_REAL " +
        "), MOVTOS AS" +
        "( " +
        "    SELECT " +
        "        ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_MONEDA, ID_MONEDA_REAL, " +
        "        SUM(DEBE) DEBE,  " +
        "        SUM(HABER) HABER, " +
        "        SUM(DEBE_MONEDA_REAL) DEBE_MONEDA_REAL, " +
        "        SUM(HABER_MONEDA_REAL) HABER_MONEDA_REAL " +
        "    FROM VW_COF_MOVIMIENTO_BIS " +
        $"    WHERE {DataCommonMethods.FormatSqlDbDate(fromDate)}  <= FECHA_AFECTACION AND FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(toDate)} " +
        "    GROUP BY ID_CUENTA_ESTANDAR, NUMERO_CUENTA_ESTANDAR, ID_MONEDA, ID_MONEDA_REAL " +
        " ) " +
        " SELECT " +
        "      nvl(SDO_INICIAL.ID_CUENTA_ESTANDAR,MOVTOS.ID_CUENTA_ESTANDAR) ID_CUENTA_ESTANDAR " +
        "    , nvl(SDO_INICIAL.NUMERO_CUENTA_ESTANDAR, MOVTOS.NUMERO_CUENTA_ESTANDAR) NUMERO_CUENTA_ESTANDAR " +
        "    , nvl(SDO_INICIAL.ID_MONEDA, MOVTOS.ID_MONEDA) ID_MONEDA " +
        "    , nvl(SDO_INICIAL.ID_MONEDA_REAL, MOVTOS.ID_MONEDA_REAL) ID_MONEDA_REAL " +
        "    , nvl(SDO_INICIAL.SALDO_INICIAL,0) SALDO_INICIAL " +
        "    , nvl(MOVTOS.DEBE,0) DEBE " +
        "    , nvl(MOVTOS.HABER,0) HABER " +
        "    , nvl(SDO_INICIAL.SALDO_INICIAL_REAL,0) SALDO_INICIAL_REAL " +
        "    , nvl(MOVTOS.DEBE_MONEDA_REAL,0) DEBE_MONEDA_REAL " +
        "    , nvl(MOVTOS.HABER_MONEDA_REAL,0) HABER_MONEDA_REAL " +
        " FROM SDO_INICIAL " +
        "    FULL OUTER JOIN MOVTOS ON SDO_INICIAL.ID_CUENTA_ESTANDAR = MOVTOS.ID_CUENTA_ESTANDAR " +
        "    AND SDO_INICIAL.ID_MONEDA = MOVTOS.ID_MONEDA " +
        "    AND SDO_INICIAL.ID_MONEDA_REAL = MOVTOS.ID_MONEDA_REAL " +
        "ORDER BY NUMERO_CUENTA_ESTANDAR ";

      var op = DataOperation.Parse(sql);

      //var op = DataOperation.Parse("QRY_BALANZA_REAL", "'" + fromDate.ToString("d") + "'", "'" + toDate.ToString("d") + "'");

      return DataReader.GetPlainObjectFixedList<BalanzaValorizadaReal>(op);
    }


    internal static FixedList<BalanzaValorizadaReal> GetBalanza(DateTime fromDate, DateTime toDate) {

      var op = DataOperation.Parse("@qry_cof_balanza_real",
                                                           DataCommonMethods.FormatSqlDbDate(fromDate),
                                                           DataCommonMethods.FormatSqlDbDate(toDate)
                                                           );

      return DataReader.GetPlainObjectFixedList<BalanzaValorizadaReal>(op);
    }


  }  // class BalanzaValirozadaRealDataService

}  // namespace Empiria.BanobrasIntegration.Sial.Data
