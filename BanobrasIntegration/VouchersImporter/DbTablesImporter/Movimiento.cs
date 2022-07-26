/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Information Holder                   *
*  Type     : Movimiento                                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Información de un registro de la tabla MC_MOVIMIENTOS (Banobras) con movimientos de pólizas    *
*             por integrar (cargos o abonos).                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Información de un registro de la tabla MC_MOVIMIENTOS (Banobras) con
  /// movimientos de pólizas por integrar (cargos o abonos).</summary>
  internal class Movimiento {

    private readonly List<ToImportVoucherIssue> _issues = new List<ToImportVoucherIssue>();

    #region Properties

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


    [DataField("MCO_NO_OPERACION", ConvertFrom = typeof(long))]
    public int NumeroOperacion {
      get; private set;
    }


    [DataField("MCO_SISTEMA", ConvertFrom = typeof(long))]
    public int IdSistema {
      get; private set;
    }

    [DataField("MCO_TABLA_ORIGEN")]
    public string TablaOrigen {
      get; private set;
    }

    public Encabezado Encabezado {
      get; private set;
    }

    #endregion Properties

    #region Methods

    internal string GetVoucherUniqueID() {
      if (this.TipoContabilidad == 1 && IdSistema == 23) {
        return $"{this.TipoContabilidad}||{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
               $"{this.NumeroVolante}||{this.Descripcion}";
      } else {
        return $"{this.TipoContabilidad}||{this.IdSistema}||{this.FechaAfectacion.ToString("yyyy-MM-dd")}||" +
               $"{this.NumeroVolante}";
      }
    }


    private string GetEntryID() {
      return this.NumMovimiento.ToString();
    }


    internal StandardAccount GetStandardAccount() {
      try {
        var accountsChart = this.Encabezado.GetAccountsChart();

        string formatted = accountsChart.FormatAccountNumber(this.NumeroCuentaEstandar);

        StandardAccount stdAccount = accountsChart.TryGetStandardAccount(formatted);

        if (stdAccount != null) {
          return stdAccount;
        } else {
          AddError($"La cuenta '{formatted}' no existe en el catálogo de cuentas.");

          return StandardAccount.Empty;
        }
      } catch {
        AddError($"Ocurrió un problema al leer el catálogo de cuentas.");

        return StandardAccount.Empty;
      }
    }


    internal Sector GetSector() {
      this.ClaveSector = EmpiriaString.Clean(this.ClaveSector);

      if (this.ClaveSector.Length == 0 ||
          this.ClaveSector == "00" ||
          this.ClaveSector == "0") {
        return Sector.Empty;
      }

      try {
        return Sector.Parse(this.ClaveSector);

      } catch {
        AddError($"Ocurrió un problema al leer el sector {this.ClaveSector}.");

        return Sector.Empty;
      }
    }


    internal SubledgerAccount GetSubledgerAccount() {
      try {
        Ledger ledger = this.Encabezado.GetLedger();

        var formattedSubledgerAccountNo = GetSubledgerAccountNo();

        SubledgerAccount subledgerAccount = ledger.TryGetSubledgerAccount(formattedSubledgerAccountNo);

        if (subledgerAccount == null) {
          return SubledgerAccount.Empty;
        }

        if (subledgerAccount.Suspended) {
          AddError($"El auxiliar '{formattedSubledgerAccountNo}' ({subledgerAccount.Name}) está suspendido, " +
                   $"por lo que no permite operaciones de registro.");
        }

        return subledgerAccount;

      } catch {
        AddError($"Ocurrió un problema al leer el auxiliar. Tiene un formato no válido o pertenece a otra contabilidad.");

        return SubledgerAccount.Empty;
      }
    }


    internal string GetSubledgerAccountNo() {
      try {

        Ledger ledger = this.Encabezado.GetLedger();

        this.NumeroAuxiliar = ledger.FormatSubledgerAccount(this.NumeroAuxiliar);

        return this.NumeroAuxiliar;
      } catch {
        AddError($"Ocurrió un problema al leer el auxiliar {this.NumeroAuxiliar}. Tiene un formato no válido o pertenece a otra contabilidad.");

        return String.Empty;
      }
    }


    internal FunctionalArea GetResponsibilityArea() {
      try {

        return FunctionalArea.Parse(this.AreaMovimiento);

      } catch {
        AddError($"Ocurrió un problema al leer el área funcional {this.AreaMovimiento}.");

        return FunctionalArea.Empty;
      }
    }


    internal string GetBudgetConcept() {
      return EmpiriaString.Clean(this.ConceptoPresupuestal.ToString());
    }


    internal EventType GetEventType() {
      try {

        return EventType.Parse(this.ClaveDisponibilidad);

      } catch {
        AddError($"Ocurrió un problema al leer la clave de disponibilidad {this.ClaveDisponibilidad}.");

        return EventType.Empty;
      }
    }


    internal string GetVerificationNumber() {
      return EmpiriaString.Clean(this.NumeroOperacion.ToString());
    }


    internal VoucherEntryType GetVoucherEntryType() {
      if (this.TipoMovimiento == 1) {
        return VoucherEntryType.Debit;

      } else if (this.TipoMovimiento == 2) {
        return VoucherEntryType.Credit;

      } else {
        AddError($"Ocurrió un problema al leer el tipo de movimiento {this.TipoMovimiento}.");

        return VoucherEntryType.Debit;
      }
    }


    internal DateTime GetDate() {
      return ExecutionServer.DateMinValue;
    }


    internal string GetConcept() {
      return EmpiriaString.Clean(this.Descripcion);
    }


    internal Currency GetCurrency() {
      string claveMoneda = this.ClaveMoneda.ToString("00");

      try {
        return Currency.Parse(claveMoneda);
      } catch {

        AddError($"La moneda '{claveMoneda}' no existe en el catálogo de monedas.");

        return Currency.Empty;
      }
    }


    internal decimal GetAmount() {
      return Math.Round(this.Importe, 2);
    }


    internal decimal GetExchangeRate() {
      return Math.Round(this.TipoCambio, 6);
    }


    internal decimal GetBaseCurrencyAmount() {
      return this.GetAmount() * this.GetExchangeRate();
    }


    internal FixedList<ToImportVoucherIssue> GetIssues() {
      return _issues.ToFixedList();
    }


    private void AddError(string msg) {
      _issues.Add(new ToImportVoucherIssue(VoucherIssueType.Error,
                                           this.GetVoucherUniqueID(),
                                           this.GetEntryID(),
                                           msg));
    }


    internal bool GetProtected() {
      return true;
    }


    internal void SetEncabezado(Encabezado encabezado) {
      this.Encabezado = encabezado;
    }

    #endregion Methods

  }  // class Movimiento

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
