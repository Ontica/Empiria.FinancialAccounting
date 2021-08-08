/* Empiria Financial *****************************************************************************************
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
  public class VoucherEntry {

    #region Constructors and parsers

    protected VoucherEntry() {
      // Required by Empiria Framework.
    }

    internal VoucherEntry(VoucherEntryFields fields) {
      Assertion.AssertObject(fields, "fields");

      LoadFields(fields);
    }


    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_MOVIMIENTO")]
    public long Id {
      get;
      private set;
    }


    [DataField("ID_TRANSACCION", ConvertFrom = typeof(long))]
    public int VoucherId {
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
    public SubsidiaryAccount SubledgerAccount {
      get;
      private set;
    }


    [DataField("ID_MOVIMIENTO_REFERENCIA", ConvertFrom = typeof(long))]
    public int ReferenceEntryId {
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
    public string AvailabilityCode {
      get;
      private set;
    }


    [DataField("NUMERO_VERIFICACION")]
    public string VerificationNumber {
      get;
      internal set;
    }


    [DataField("TIPO_MOVIMIENTO", Default = VoucherEntryType.Debit)]
    public VoucherEntryType VoucherEntryType {
      get;
      internal set;
    }


    [DataField("FECHA_MOVIMIENTO", Default = "ExecutionServer.DateMinValue")]
    public DateTime Date {
      get;
      internal set;
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
    public decimal BaseCurrrencyAmount {
      get;
      private set;
    }

    [DataField("PROTEGIDO", ConvertFrom = typeof(int))]
    public bool Protected {
      get;
      private set;
    }


    public Voucher Voucher {
      get {
        return Voucher.Parse(this.VoucherId);
      }
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
        return BaseCurrrencyAmount / Amount;
      }
    }


    public bool HasSubledgerAccount {
      get {
        return !this.SubledgerAccount.IsEmptyInstance;
      }
    }

    #endregion Public properties

    #region Methods

    internal void Delete() {
      VoucherData.DeleteVoucherEntry(this);
    }


    private void LoadFields(VoucherEntryFields fields) {
      this.VoucherId = fields.GetVoucher().Id;
      this.LedgerAccount = fields.GetLedgerAccount();
      this.Sector = fields.GetSector();
      this.SubledgerAccount = fields.GetSubledgerAccount();
      this.ReferenceEntryId = fields.ReferenceEntryId;
      this.ResponsibilityArea = fields.GetResponsibilityArea();
      this.BudgetConcept = fields.BudgetConcept;
      this.AvailabilityCode = fields.AvailabilityCode;
      this.VerificationNumber = fields.VerificationNumber;
      this.VoucherEntryType = fields.VoucherEntryType;
      this.Date = fields.Date;
      this.Concept = fields.Concept;
      this.Currency = fields.GetCurrency();
      this.Amount = fields.Amount;
      if (fields.UsesBaseCurrency()) {
        this.BaseCurrrencyAmount = fields.Amount;
      } else {
        this.BaseCurrrencyAmount = fields.BaseCurrencyAmount;
      }
      this.Protected = fields.Protected;
    }


    internal void Save() {
      if (this.Id == 0) {
        this.Id = VoucherData.NextVoucherEntryId();
      }
      VoucherData.WriteVoucherEntry(this);
    }

    #endregion Methods

  }  // class VoucherEntry

}  // namespace Empiria.FinancialAccounting.Vouchers
