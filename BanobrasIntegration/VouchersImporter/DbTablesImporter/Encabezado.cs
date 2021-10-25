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
    public int IdTipoPoliza {
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


    internal AccountsChart GetAccountsChart() {
      return AccountsChart.Parse(this.TipoContabilidad);
    }


    internal string GetImportationSet() {
      var system = TransactionalSystem.Get(x => x.SourceSystemId == this.IdSistema);

      return system.Name;
    }


    internal string GetUniqueID() {
      return $"{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
             $"{this.NumeroVolante}||{this.Concepto}";
    }


    internal Ledger GetLedger() {
      string ledgerNumber = this.AreaCaptura.Substring(1, 2);

      Ledger ledger = this.GetAccountsChart().TryGetLedger(ledgerNumber);

      return ledger ?? Ledger.Empty;
    }


    internal string GetConcept() {
      return EmpiriaString.TrimAll(this.Concepto);
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

      Assertion.AssertObject(rule, "rule");

      return rule.TargetVoucherType;
    }


    internal TransactionType GetTransactionType() {
      var system = TransactionalSystem.Get(x => x.SourceSystemId == this.IdSistema);

      TransactionalSystemRule rule = system.Rules.Find(x => x.SourceVoucherTypeId == this.IdTipoPoliza);

      Assertion.AssertObject(rule, "rule");

      return rule.TargetTransactionType;
    }


    internal FunctionalArea GetFunctionalArea() {
      var areaID = EmpiriaString.TrimAll(this.AreaCaptura);

      return FunctionalArea.Parse(areaID);
    }


    internal Participant GetElaboratedBy() {
      var userID = EmpiriaString.TrimAll(this.Usuario);

      try {
        var participant = Participant.TryParse(userID);

        if (participant != null) {
          return participant;

        } else {
          EmpiriaLog.Info($"No encontré la cuenta de usuario '{userID}'. Sistema ({this.IdSistema})");

          return Participant.Empty;
        }
      } catch {
        EmpiriaLog.Info($"Tuve problemas para encontrar la cuenta de usuario {userID}. Sistema ({this.IdSistema})");

        throw;
      }
    }


    internal FixedList<ToImportVoucherIssue> GetIssues() {
      return new FixedList<ToImportVoucherIssue>();
    }

  }  // class Encabezado

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
