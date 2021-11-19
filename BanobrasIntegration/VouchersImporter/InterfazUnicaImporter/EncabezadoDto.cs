/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : EncabezadoDto                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : DTO de un registro de la tabla MC_ENCABEZADOS (Banobras) con el encabezado de un voucher.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>DTO de un registro de la tabla MC_ENCABEZADOS (Banobras)
  /// con el encabezado de un voucher.</summary>
  public class EncabezadoDto {

    public int ENC_TIPO_CONT {
      get; set;
    }

    public DateTime ENC_FECHA_VOL {
      get; set;
    }

    public int ENC_NUM_VOL {
      get; set;
    }

    public string ENC_AREA_CAP {
      get; set;
    }

    public string ENC_DESCRIP {
      get; set;
    }

    public decimal ENC_TOT_CARGOS {
      get; set;
    }

    public decimal ENC_TOT_ABONOS {
      get; set;
    }

    public int ENC_FIDEICOMISO {
      get; set;
    }

    public DateTime ENC_FECHA_CAP {
      get; set;
    }

    public string ENC_USUARIO {
      get; set;
    }

    public int ENC_SISTEMA {
      get; set;
    }

    public int ENC_TIPO_POLIZA {
      get; set;
    }

    public int ENC_NO_OPERACION {
      get; set;
    }

    public int ENC_STATUS {
      get; set;
    }

    public int ENC_GRUPO_CTL {
      get; set;
    }

    public MovimientoDto[] MOVIMIENTOS {
      get; set;
    }

  }  // class EncabezadoDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters
