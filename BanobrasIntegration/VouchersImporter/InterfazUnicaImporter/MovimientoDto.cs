/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : MovimientoDto                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : DTO de un registro de la tabla MC_MOVIMIENTOS (Banobras) con un cargo o abono en una póliza.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>DTO de un registro de la tabla MC_MOVIMIENTOS (Banobras)
  /// con un cargo o abono en una póliza.</summary>
  public class MovimientoDto {

    //public int MCO_TIPO_CONT {
    //  get; set;
    //}


    //public DateTime MCO_FECHA_VOL {
    //  get; set;
    //}


    //public int MCO_NUM_VOL {
    //  get; set;
    //}


    public int MCO_FOLIO {
      get; set;
    }


    public string MCO_AREA {
      get; set;
    }


    public string MCO_REG_CONTABLE {
      get; set;
    }


    public string MCO_SECTOR {
      get; set;
    }


    public string MCO_NUM_AUX {
      get; set;
    }


    public int MCO_MONEDA {
      get; set;
    }


    public decimal MCO_T_CAMBIO {
      get; set;
    }


    public int MCO_CVE_MOV {
      get; set;
    }


    public decimal MCO_IMPORTE {
      get; set;
    }


    public string MCO_DESCRIP {
      get; set;
    }


    public int MCO_DISPONIB {
      get; set;
    }


    public int MCO_CONCEPTO {
      get; set;
    }


    //public int MCO_SISTEMA {
    //  get; set;
    //}


    public int MCO_NO_OPERACION {
      get; set;
    }


    public int MCO_STATUS {
      get; set;
    }


    //public int MCO_FIDEICOMISO {
    //  get; set;
    //}


    public int MCO_STATUS_ANT {
      get; set;
    }


    public int MCO_GPO_CTLA {
      get; set;
    }


    public int MCO_GPO_CTB {
      get; set;
    }

  }  // class MovimientoDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters
