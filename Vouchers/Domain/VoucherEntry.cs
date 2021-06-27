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


namespace Empiria.FinancialAccounting.Vouchers {

  public enum VoucherEntryType {
    Debit = 'D',

    Credit = 'H'
  }

  /// <summary>Represents an accounting voucher entry: a debit or credit movement.</summary>
  public class VoucherEntry : BaseObject {

    #region Constructors and parsers

    private VoucherEntry() {
      // Required by Empiria Framework.
    }


    static public VoucherEntry Parse(int id) {
      return BaseObject.ParseId<VoucherEntry>(id);
    }


    static public Voucher Empty => BaseObject.ParseEmpty<Voucher>();


    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_TRANSACCION", ConvertFrom = typeof(long))]
    public int TransactionId {
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
      get; internal set;
    }


    [DataField("TIPO_MOVIMIENTO", Default = VoucherEntryType.Debit)]
    public VoucherEntryType VoucherEntryType {
      get; internal set;
    }


    [DataField("FECHA_MOVIMIENTO")]
    public DateTime Date {
      get; internal set;
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

  }  // class VoucherEntry

}  // namespace Empiria.FinancialAccounting.Vouchers
