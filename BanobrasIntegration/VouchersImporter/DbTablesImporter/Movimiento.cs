/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Information Holder                   *
*  Type     : Movimiento                                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Información de un registro de la tabla MC_MOVIMIENTOS (Banobras) con movimientos de pólizas    *
*             por integrar (cargos o abonos).                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Información de un registro de la tabla MC_MOVIMIENTOS (Banobras) con
  /// movimientos de pólizas por integrar (cargos o abonos).</summary>
  internal class Movimiento {

    [DataField("MCO_TIPO_CONT", ConvertFrom = typeof(long))]
    public int TipoContabilidad {
      get; private set;
    }


    [DataField("MCO_FECHA_VOL")]
    public DateTime FechaAfectacion {
      get; private set;
    }


    [DataField("MCO_NUM_VOL", ConvertFrom = typeof(long))]
    public int NumeroVolante {
      get; private set;
    }


    [DataField("MCO_FOLIO", ConvertFrom = typeof(long))]
    public int NumMovimiento {
      get; private set;
    }


    [DataField("MCO_AREA")]
    public string AreaMovimiento {
      get; private set;
    }


    [DataField("MCO_REG_CONTABLE")]
    public string NumeroCuentaEstandar {
      get; private set;
    }


    [DataField("MCO_SECTOR")]
    public string ClaveSector {
      get; private set;
    }


    [DataField("MCO_NUM_AUX")]
    public string NumeroAuxiliar {
      get; private set;
    }


    [DataField("MCO_MONEDA", ConvertFrom = typeof(long))]
    public int ClaveMoneda {
      get; private set;
    }


    [DataField("MCO_T_CAMBIO")]
    public decimal TipoCambio {
      get; private set;
    }


    [DataField("MCO_CVE_MOV", ConvertFrom = typeof(long))]
    public int TipoMovimiento {
      get; private set;
    }


    [DataField("MCO_IMPORTE")]
    public decimal Importe {
      get; private set;
    }


    [DataField("MCO_DESCRIP")]
    public string Descripcion {
      get; private set;
    }


    [DataField("MCO_DISPONIB", ConvertFrom = typeof(long))]
    public int Disponibilidad {
      get; private set;
    }


    [DataField("MCO_CONCEPTO", ConvertFrom = typeof(long))]
    public int ConceptoPresupuestal {
      get; private set;
    }


    [DataField("MCO_SISTEMA", ConvertFrom = typeof(long))]
    public int Sistema {
      get; private set;
    }

    internal string GetVoucherUniqueID() {
      throw new NotImplementedException();
    }

    internal LedgerAccount GetLedgerAccount() {
      throw new NotImplementedException();
    }

    internal Sector GetSector() {
      throw new NotImplementedException();
    }

    internal SubsidiaryAccount GetSubledgerAccount() {
      throw new NotImplementedException();
    }

    internal FunctionalArea GetResponsibilityArea() {
      throw new NotImplementedException();
    }

    internal string GetBudgetConcept() {
      throw new NotImplementedException();
    }

    internal EventType GetEventType() {
      throw new NotImplementedException();
    }

    internal string GetVerificationNumber() {
      throw new NotImplementedException();
    }

    internal VoucherEntryType GetVoucherEntryType() {
      throw new NotImplementedException();
    }

    internal DateTime GetDate() {
      throw new NotImplementedException();
    }

    internal string GetConcept() {
      throw new NotImplementedException();
    }

    internal Currency GetCurrency() {
      throw new NotImplementedException();
    }

    internal decimal GetAmount() {
      throw new NotImplementedException();
    }

    internal decimal GetExchangeRate() {
      throw new NotImplementedException();
    }

    internal decimal GetBaseCurrencyAmount() {
      throw new NotImplementedException();
    }

    internal FixedList<ToImportVoucherIssue> GetIssues() {
      throw new NotImplementedException();
    }

    internal bool GetProtected() {
      throw new NotImplementedException();
    }


    //[DataField("MCO_NO_OPERACION", ConvertFrom = typeof(long))]
    //public int NumOperacion {
    //  get; private set;
    //}


    //[DataField("MCO_STATUS", ConvertFrom = typeof(long))]
    //public int Status {
    //  get; private set;
    //}


    //[DataField("MCO_FIDEICOMISO")]
    //public int NumFideicomiso {
    //  get; private set;
    //}


    //[DataField("MCO_STATUS_ANT", ConvertFrom = typeof(long))]
    //public int StatusAnterior {
    //  get; private set;
    //}


    //[DataField("MCO_GPO_CTLA", ConvertFrom = typeof(long))]
    //public int StatusAnterior {
    //  get; private set;
    //}


    //[DataField("MCO_GPO_CTB", ConvertFrom = typeof(long))]
    //public int StatusAnterior {
    //  get; private set;
    //}

  }  // class Movimiento

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
