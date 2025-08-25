/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Output DTO                           *
*  Type     : MovimientoSistemaLegado                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to retrieve cash ledger transaction entries in legacy system.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Output DTO used to retrieve cash ledger transaction entries for use in lists.</summary>
  internal class MovimientoSistemaLegado {

    [DataField("MCOM_NUM_VOL", ConvertFrom = typeof(long))]
    public long IdPoliza {
      get; set;
    }


    [DataField("MCOM_FOLIO_VOL")]
    public int IdConsecutivo {
      get; set;
    }

    [DataField("MCOM_REG_CONTABLE")]
    public string CuentaContable {
      get; set;
    }

    [DataField("MCOM_NUM_AUX")]
    public string Auxiliar {
      get; set;
    }

    [DataField("MCOM_SECTOR")]
    public string Sector {
      get; set;
    }

    [DataField("MCOM_MONEDA", ConvertFrom = typeof(decimal))]
    public int IdMoneda {
      get; set;
    }

    [DataField("MCOM_DISPONIB", ConvertFrom = typeof(decimal))]
    public int Disponibilidad {
      get; set;
    }

    [DataField("MCOM_IMPORTE", ConvertFrom = typeof(decimal))]
    public decimal Importe {
      get; set;
    }

    [DataField("MCOM_CVE_MOV", ConvertFrom = typeof(decimal))]
    public int TipoMovimiento {
      get; set;
    }

    [DataField("MCOM_CONCEPTO")]
    public int CuentaConcepto {
      get; set;
    }

  }  // class MovimientoSistemaLegado

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
