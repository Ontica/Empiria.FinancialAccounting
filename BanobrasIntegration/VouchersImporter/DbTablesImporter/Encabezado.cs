/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information Holder                   *
*  Type     : Encabezado                                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Información de un registro de la tabla MC_ENCABEZADOS (Banobras) con pólizas por integrar.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Información de un registro de la tabla MC_ENCABEZADOS (Banobras) con pólizas por integrar.</summary>
  internal class Encabezado {

    private readonly List<ToImportVoucherIssue> _headerIssues = new List<ToImportVoucherIssue>();

    #region Properties

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
    public string AreaCaptura {
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
    public int IdTipoPoliza {
      get; private set;
    }


    [DataField("ENC_TABLA_ORIGEN")]
    public string TablaOrigen {
      get; private set;
    }

    #endregion Properties


    #region Methods

    public void AddError(string msg) {
      _headerIssues.Add(new ToImportVoucherIssue(VoucherIssueType.Error,
                                                 this.GetImportationSet(),
                                                 msg));
    }

    internal AccountsChart GetAccountsChart() {
      return AccountsChart.Parse(this.TipoContabilidad);
    }


    internal string GetImportationSet() {
      return DbVouchersImporterDataService.GetImportationSetUID(this.IdSistema,
                                                                this.TipoContabilidad,
                                                                this.FechaAfectacion);
    }


    internal string GetUniqueID() {
      if (this.TipoContabilidad == 1 && IdSistema == 23) {
        return $"{this.TipoContabilidad}||{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
               $"{this.NumeroVolante}||{this.Concepto}";
      } else {
        return $"{this.TipoContabilidad}||{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
               $"{this.NumeroVolante}";
      }
    }


    internal Ledger GetLedger() {
      string ledgerNumber = this.AreaCaptura.Substring(1, 2);

      Ledger ledger = this.GetAccountsChart().TryGetLedger(ledgerNumber);

      return ledger ?? Ledger.Empty;
    }


    internal string GetConcept() {
      return EmpiriaString.Clean(this.Concepto);
    }


    internal DateTime GetAccountingDate() {
      return this.FechaAfectacion;
    }


    internal DateTime GetRecordingDate() {
      return this.FechaCaptura;
    }


    internal VoucherType GetVoucherType() {
      var system = TransactionalSystem.Get(x => x.SourceSystemId == this.IdSistema);

      TransactionalSystemRule rule = system.Rules.Find(x => x.SourceVoucherTypeId == this.IdTipoPoliza);

      Assertion.Require(rule, "rule");

      return rule.TargetVoucherType;
    }


    internal TransactionType GetTransactionType() {
      var system = TransactionalSystem.Get(x => x.SourceSystemId == this.IdSistema);

      TransactionalSystemRule rule = system.Rules.Find(x => x.SourceVoucherTypeId == this.IdTipoPoliza);

      Assertion.Require(rule, "rule");

      return rule.TargetTransactionType;
    }


    internal FunctionalArea GetFunctionalArea() {
      var areaID = EmpiriaString.TrimAll(this.AreaCaptura);

      FunctionalArea functionalArea = FunctionalArea.TryParse(areaID);

      if (functionalArea != null) {
        return functionalArea;
      }

      AddError($"El área funcional '{areaID}' no está registrada.");

      return FunctionalArea.Empty;
    }


    internal Participant GetElaboratedBy() {
      var userID = EmpiriaString.TrimAll(this.Usuario);

      var participant = Participant.TryParse(userID);

      if (participant != null) {
        return participant;
      }

      AddError($"No encontré la cuenta de usuario '{userID}'.");

      return Participant.Empty;
    }


    internal FixedList<ToImportVoucherIssue> GetIssues() {
      return _headerIssues.ToFixedList();
    }


    #endregion Methods

  }  // class Encabezado

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
