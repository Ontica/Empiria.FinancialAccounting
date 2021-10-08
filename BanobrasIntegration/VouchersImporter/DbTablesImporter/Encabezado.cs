/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Information Holder                   *
*  Type     : Encabezado                                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Información de un registro de la tabla MC_ENCABEZADOS (Banobras) con pólizas por integrar.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Contacts;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Información de un registro de la tabla MC_ENCABEZADOS (Banobras) con pólizas por integrar.</summary>
  internal class Encabezado {

    [DataField("ENC_TIPO_CONT", ConvertFrom = typeof(long))]
    public int TipoContabilidad {
      get; private set;
    }


    [DataField("ENC_FECHA_VOL")]
    public DateTime FechaAfectacion {
      get; private set;
    }


    [DataField("ENC_NUM_VOL", ConvertFrom = typeof(long))]
    public int NumeroVolante {
      get; private set;
    }


    [DataField("ENC_AREA_CAP")]
    public string Contabilidad {
      get; private set;
    }


    [DataField("ENC_DESCRIP")]
    public string Concepto {
      get; private set;
    }


    [DataField("ENC_TOT_CARGOS")]
    public decimal TotalCargos {
      get; private set;
    }


    [DataField("ENC_TOT_ABONOS")]
    public decimal TotalAbonos {
      get; private set;
    }


    //[DataField("ENC_FIDEICOMISO")]
    //public int NumFideicomiso {
    //  get; private set;
    //}


    [DataField("ENC_FECHA_CAP")]
    public DateTime FechaCaptura {
      get; private set;
    }


    [DataField("ENC_USUARIO")]
    public string Usuario {
      get; private set;
    }


    [DataField("ENC_SISTEMA", ConvertFrom = typeof(long))]
    public int IdSistema {
      get; private set;
    }


    [DataField("ENC_TIPO_POLIZA", ConvertFrom = typeof(long))]
    public int TipoPoliza {
      get; private set;
    }


    //[DataField("ENC_NO_OPERACION", ConvertFrom = typeof(long))]
    //public int NumOperacion {
    //  get; private set;
    //}


    //[DataField("ENC_STATUS", ConvertFrom = typeof(long))]
    //public int Status {
    //  get; private set;
    //}


    //[DataField("ENC_GRUPO_CTL", ConvertFrom = typeof(long))]
    //public int GrupoControl {
    //  get; private set;
    //}


    internal string GetImportationSet() {
      return $"Sistema Num {this.IdSistema}";
    }


    internal string GetUniqueID() {
      return $"{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
             $"{this.NumeroVolante}||{this.Concepto}";
    }


    internal Ledger GetLedger() {
      throw new NotImplementedException();
    }


    internal string GetConcept() {
      return this.Concepto;
    }


    internal DateTime GetAccountingDate() {
      throw new NotImplementedException();
    }


    internal DateTime GetRecordingDate() {
      throw new NotImplementedException();
    }


    internal VoucherType GetVoucherType() {
      throw new NotImplementedException();
    }


    internal TransactionType GetTransactionType() {
      throw new NotImplementedException();
    }


    internal FunctionalArea GetFunctionalArea() {
      throw new NotImplementedException();
    }


    internal Contact GetElaboratedBy() {
      throw new NotImplementedException();
    }

    internal FixedList<ToImportVoucherIssue> GetIssues() {
      throw new NotImplementedException();
    }

  }  // class Encabezado

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
