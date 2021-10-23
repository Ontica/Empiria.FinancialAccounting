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
    public int ClaveDisponibilidad {
      get; private set;
    }


    [DataField("MCO_CONCEPTO", ConvertFrom = typeof(long))]
    public int ConceptoPresupuestal {
      get; private set;
    }


    [DataField("MCO_SISTEMA", ConvertFrom = typeof(long))]
    public int IdSistema {
      get; private set;
    }


    public Encabezado Encabezado {
      get; private set;
    }


    private string _voucherUniqueID;

    internal string GetVoucherUniqueID() {
      if (_voucherUniqueID == null) {
        _voucherUniqueID = $"{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
                           $"{this.NumeroVolante}||{this.Descripcion}";
      }
      return _voucherUniqueID;
    }


    internal StandardAccount GetStandardAccount() {
      string formatted = this.NumeroCuentaEstandar;

      try {
        var accountsChart = this.Encabezado.GetAccountsChart();

        formatted = accountsChart.FormatAccountNumber(this.NumeroCuentaEstandar);

        StandardAccount stdAccount = accountsChart.TryGetStandardAccount(formatted);

        if (stdAccount != null) {
          return stdAccount;
        } else {
          EmpiriaLog.Info($"La cuenta '{formatted}' no existe en el catálogo de cuentas. (Sistema {this.IdSistema})");
          return StandardAccount.Empty;
        }
      } catch {
        EmpiriaLog.Info($"Ocurrió un problema al leer el catálogo de cuentas. (Sistema {this.IdSistema})");
        return StandardAccount.Empty;
      }
    }


    internal Sector GetSector() {
      this.ClaveSector = EmpiriaString.TrimAll(this.ClaveSector);

      if (this.ClaveSector.Length == 0 ||
          this.ClaveSector == "00" ||
          this.ClaveSector == "0") {
        return Sector.Empty;
      }

      return Sector.Parse(this.ClaveSector);
    }


    internal LedgerAccount GetLedgerAccount() {
      Ledger ledger = this.Encabezado.GetLedger();

      StandardAccount standardAccount = this.GetStandardAccount();

      LedgerAccount ledgerAccount = ledger.TryGetAccount(standardAccount);

      if (ledgerAccount != null) {
        return ledgerAccount;
      } else {
        return LedgerAccount.Empty;
      }
    }


    internal SubsidiaryAccount GetSubledgerAccount() {
      Ledger ledger = this.Encabezado.GetLedger();

      var formattedAccountNo = GetSubledgerAccountNo();

      SubsidiaryAccount subledgerAccount = ledger.TryGetSubledgerAccount(formattedAccountNo);

      if (subledgerAccount != null) {
        return subledgerAccount;
      } else {
        return SubsidiaryAccount.Empty;
      }
    }


    internal string GetSubledgerAccountNo() {
      Ledger ledger = this.Encabezado.GetLedger();

      this.NumeroAuxiliar = ledger.FormatSubledgerAccount(this.NumeroAuxiliar);

      return this.NumeroAuxiliar;
    }


    internal FunctionalArea GetResponsibilityArea() {
      return FunctionalArea.Parse(this.AreaMovimiento);
    }


    internal string GetBudgetConcept() {
      return this.ConceptoPresupuestal.ToString();
    }


    internal EventType GetEventType() {
      return EventType.Parse(this.ClaveDisponibilidad);
    }


    internal string GetVerificationNumber() {
      return String.Empty;
    }


    internal VoucherEntryType GetVoucherEntryType() {
      if (this.TipoMovimiento == 1) {
        return VoucherEntryType.Debit;
      } else if (this.TipoMovimiento == 2) {
        return VoucherEntryType.Credit;
      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    internal DateTime GetDate() {
      return ExecutionServer.DateMinValue;
    }


    internal string GetConcept() {
      return EmpiriaString.TrimAll(this.Descripcion);
    }


    internal Currency GetCurrency() {
      string claveMoneda = this.ClaveMoneda.ToString("00");

      try {
        return Currency.Parse(claveMoneda);
      } catch {
        EmpiriaLog.Info($"La moneda '{claveMoneda}' no existe en el catálogo de monedas. (Sistema {this.IdSistema})");

        return Currency.Empty;
      }
    }


    internal decimal GetAmount() {
      return this.Importe;
    }


    internal decimal GetExchangeRate() {
      return this.TipoCambio;
    }


    internal decimal GetBaseCurrencyAmount() {
      return this.Importe * this.TipoCambio;
    }


    internal FixedList<ToImportVoucherIssue> GetIssues() {
      return new FixedList<ToImportVoucherIssue>();
    }


    internal bool GetProtected() {
      return true;
    }


    internal void SetEncabezado(Encabezado encabezado) {
      this.Encabezado = encabezado;
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
