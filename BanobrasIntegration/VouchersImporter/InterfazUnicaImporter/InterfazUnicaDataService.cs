/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Service                         *
*  Type     : InterfazUnicaDataService                      License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data methods used to store vouchers using Banobras' defined tables.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Data methods used to store vouchers using Banobras' defined tables.</summary>
  static internal class InterfazUnicaDataService {

    static internal void WriteEncabezado(EncabezadoDto o) {
      var operation = DataOperation.Parse("apd_mc_encabezado",
                            o.ENC_TIPO_CONT, o.ENC_FECHA_VOL, o.ENC_NUM_VOL,
                            o.ENC_DESCRIP, o.ENC_TOT_CARGOS, o.ENC_TOT_ABONOS,
                            o.ENC_TIPO_POLIZA, o.ENC_FIDEICOMISO, o.ENC_FECHA_CAP,
                            o.ENC_USUARIO, o.ENC_AREA_CAP, o.ENC_SISTEMA,
                            o.ENC_NO_OPERACION, o.ENC_STATUS, o.ENC_GRUPO_CTL);

      DataWriter.Execute(operation);
    }


    static internal void WriteMovimiento(EncabezadoDto e, MovimientoDto m) {
      var operation = DataOperation.Parse("apd_mc_movimiento",
                            e.ENC_TIPO_CONT, e.ENC_FECHA_VOL, e.ENC_NUM_VOL, m.MCO_FOLIO,
                            m.MCO_AREA, m.MCO_REG_CONTABLE, m.MCO_SECTOR, m.MCO_NUM_AUX,
                            m.MCO_GPO_CTLA, m.MCO_GPO_CTB, m.MCO_MONEDA, m.MCO_T_CAMBIO,
                            m.MCO_CVE_MOV, m.MCO_IMPORTE, m.MCO_DESCRIP, m.MCO_DISPONIB,
                            m.MCO_CONCEPTO, e.ENC_SISTEMA, m.MCO_NO_OPERACION, m.MCO_STATUS,
                            e.ENC_FIDEICOMISO, m.MCO_STATUS_ANT);

      DataWriter.Execute(operation);
    }


  }  // class InterfazUnicaDataService

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
