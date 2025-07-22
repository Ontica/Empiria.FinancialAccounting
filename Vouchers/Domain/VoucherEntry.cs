﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Aggregate root                          *
*  Type     : VoucherEntry                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an accounting voucher entry: a debit or credit movement.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  public enum VoucherEntryType {

    Debit = 'D',

    Credit = 'H'

  }

  /// <summary>Represents an accounting voucher entry: a debit or credit movement.</summary>
  internal class VoucherEntry : IVoucherEntry {

    #region Constructors and parsers

    //protected VoucherEntry() {
    //  // Required by Empiria Framework.
    //}


    internal VoucherEntry(Voucher voucher) {
      Assertion.Require(voucher, "voucher");

      this.Voucher = voucher;
    }


    internal VoucherEntry(Voucher voucher, VoucherEntryFields fields) {
      Assertion.Require(voucher, "voucher");
      Assertion.Require(fields, "fields");

      this.Voucher = voucher;

      LoadFields(fields);
    }


    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_MOVIMIENTO")]
    public long Id {
      get;
      private set;
    }


    [DataField("ID_CUENTA", ConvertFrom = typeof(long))]
    public LedgerAccount LedgerAccount {
      get;
      private set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get;
      private set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(long))]
    public SubledgerAccount SubledgerAccount {
      get;
      private set;
    }


    [DataField("ID_MOVIMIENTO_REFERENCIA", ConvertFrom = typeof(long))]
    public int CashFlowAccountId {
      get;
      private set;
    }


    [DataField("ID_AREA_RESPONSABILIDAD", ConvertFrom = typeof(long))]
    public FunctionalArea ResponsibilityArea {
      get;
      private set;
    }


    [DataField("CLAVE_PRESUPUESTAL")]
    public string BudgetConcept {
      get;
      private set;
    }


    [DataField("CLAVE_DISPONIBILIDAD")]
    private string _claveDisponibilidad;

    public EventType EventType {
      get {
        if (String.IsNullOrWhiteSpace(_claveDisponibilidad) ||
            Convert.ToInt32(_claveDisponibilidad) == 0) {
          return EventType.Empty;
        }
        return EventType.Parse(Convert.ToInt32(_claveDisponibilidad));
      }
      private set {
        _claveDisponibilidad = value.Id.ToString();
      }
    }


    [DataField("NUMERO_VERIFICACION")]
    public string VerificationNumber {
      get;
      private set;
    }


    [DataField("TIPO_MOVIMIENTO", Default = VoucherEntryType.Debit)]
    public VoucherEntryType VoucherEntryType {
      get;
      private set;
    }


    [DataField("FECHA_MOVIMIENTO", Default = "ExecutionServer.DateMinValue")]
    public DateTime Date {
      get;
      private set;
    }

    public bool HasDate {
      get {
        return this.Date != ExecutionServer.DateMinValue;
      }
    }


    [DataField("CONCEPTO_MOVIMIENTO")]
    public string Concept {
      get;
      private set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public Currency Currency {
      get;
      private set;
    }


    [DataField("MONTO")]
    public decimal Amount {
      get;
      private set;
    }


    [DataField("MONTO_MONEDA_BASE")]
    public decimal BaseCurrencyAmount {
      get;
      private set;
    }

    [DataField("PROTEGIDO", ConvertFrom = typeof(int))]
    public bool Protected {
      get;
      private set;
    }


    public Voucher Voucher {
      get;
    }

    public decimal Debit {
      get {
        return this.VoucherEntryType == VoucherEntryType.Debit ? this.Amount : 0m;
      }
    }


    public decimal Credit {
      get {
        return this.VoucherEntryType == VoucherEntryType.Credit ? this.Amount : 0m;
      }
    }


    public decimal ExchangeRate {
      get {
        return BaseCurrencyAmount / Amount;
      }
    }


    public bool HasEventType {
      get {
        return this.EventType.Id > 0;
      }
    }

    public bool HasSubledgerAccount {
      get {
        return !this.SubledgerAccount.IsEmptyInstance;
      }
    }

    public bool HasSector {
      get {
        return !this.Sector.IsEmptyInstance;
      }
    }

    public SectorRule SectorRule {
      get {
        return this.LedgerAccount.SectorRules.Find(x => x.Sector.Equals(this.Sector) &&
                                                        x.AppliesOn(this.Voucher.AccountingDate));
      }
    }

    #endregion Public properties

    #region Methods

    internal VoucherEntry CreateCopy() {
      VoucherEntry copy = (VoucherEntry) this.MemberwiseClone();

      copy.Id = 0;

      return copy;
    }

    internal void Delete() {
      VoucherData.DeleteVoucherEntry(this);
    }


    private void EnsureIsValidAfterLoad() {
      Ledger ledger = Voucher.Ledger;

      Assertion.Require(LedgerAccount.Ledger.Equals(ledger),
           $"La cuenta de mayor con id {LedgerAccount.Id} no pertenece a la contabilidad {ledger.FullName}.");

      if (HasSubledgerAccount) {
        Assertion.Require(SubledgerAccount.BelongsTo(ledger),
              $"El auxiliar {SubledgerAccount.Number} no pertenece a la contabilidad {ledger.FullName}.");

        Assertion.Require(!SubledgerAccount.Suspended,
              $"El auxiliar {SubledgerAccount.Number} ({SubledgerAccount.Name}) está suspendido, por lo que no acepta movimientos.");
      }

      Assertion.Require(Amount > 0, "El importe del cargo o abono debe ser mayor a cero.");

      Assertion.Require(Math.Round(Amount, 2) == Amount, "El movimiento tiene un importe con más de dos decimales.");

      Assertion.Require(BaseCurrencyAmount > 0, "El importe en moneda base debe ser mayor a cero.");

      Assertion.Require(Math.Round(BaseCurrencyAmount, 8) == BaseCurrencyAmount,
          "El movimiento en moneda base tiene un importe con más de ocho decimales. El tipo de cambio debe estar incorrecto.");
    }


    Account IVoucherEntry.GetAccount(DateTime accountingDate) {
      return this.LedgerAccount.GetHistoric(accountingDate);
    }

    SubledgerAccount IVoucherEntry.GetSubledgerAccount() {
      return this.SubledgerAccount;
    }

    private void LoadFields(VoucherEntryFields fields) {
      this.LedgerAccount = fields.GetLedgerAccount();
      this.Sector = fields.Sector;
      this.SubledgerAccount = fields.GetSubledgerAccount();
      this.CashFlowAccountId = fields.CashFlowAccountId;
      this.ResponsibilityArea = fields.GetResponsibilityArea();
      this.BudgetConcept = EmpiriaString.TrimAll(fields.BudgetConcept);
      this.EventType = fields.GetEventType();
      this.VerificationNumber = EmpiriaString.TrimAll(fields.VerificationNumber);
      this.VoucherEntryType = fields.VoucherEntryType;
      this.Date = fields.Date;
      this.Concept = EmpiriaString.TrimAll(fields.Concept.ToUpperInvariant());
      this.Currency = fields.Currency;
      this.Amount = fields.Amount;

      if (fields.UsesBaseCurrency()) {
        this.BaseCurrencyAmount = fields.Amount;
      } else {
        this.BaseCurrencyAmount = fields.BaseCurrencyAmount;
      }

      this.Protected = fields.Protected;

      EnsureIsValidAfterLoad();
    }


    internal void Save() {
      if (this.Id == 0) {
        this.Id = VoucherData.NextVoucherEntryId();
      }
      VoucherData.WriteVoucherEntry(this);
    }


    internal void Update(VoucherEntryFields fields) {
      Assertion.Require(fields, "fields");

      LoadFields(fields);

      Save();
    }

    #endregion Methods

  }  // class VoucherEntry

}  // namespace Empiria.FinancialAccounting.Vouchers
