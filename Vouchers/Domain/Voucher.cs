/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Aggregate root                          *
*  Type     : Voucher                                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an accounting voucher.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Represents an accounting voucher.</summary>
  public class Voucher : BaseObject {

    private Lazy<FixedList<VoucherEntry>> _entries;

    #region Constructors and parsers

    private Voucher() {
      // Required by Empiria Framework.
    }

    internal Voucher(VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      this.LoadFields(fields);
      this.Number = "No actualizada";
      this.RecordingDate = DateTime.Today;
      this.IsOpened = true;
      this.ElaboratedBy = Participant.Current;
    }


    static public Voucher Parse(int id) {
      return BaseObject.ParseId<Voucher>(id);
    }

    static public Voucher Parse(string uid) {
      return BaseObject.ParseKey<Voucher>(uid);
    }

    static public FixedList<Voucher> GetList(string filter, string sort, int pageSize) {
      return VoucherData.GetVouchers(filter, sort, pageSize);
    }

    static public Voucher Empty => BaseObject.ParseEmpty<Voucher>();

    protected override void OnInitialize() {
      base.OnLoad();

      if (!this.IsEmptyInstance) {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => VoucherData.GetVoucherEntries(this));

      } else {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => new FixedList<VoucherEntry>());
      }
    }

    #endregion Constructors and parsers

    #region Public properties


    [DataField("NUMERO_TRANSACCION")]
    public string Number {
      get;
      private set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string Concept {
      get;
      private set;
    }


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get;
      private set;
    }


    [DataField("ID_TIPO_TRANSACCION", ConvertFrom = typeof(long))]
    public TransactionType TransactionType {
      get;
      private set;
    }


    [DataField("ID_TIPO_POLIZA", ConvertFrom = typeof(long))]
    public VoucherType VoucherType {
      get;
      private set;
    }


    [DataField("ID_FUENTE", ConvertFrom = typeof(long))]
    public FunctionalArea FunctionalArea {
      get;
      private set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get;
      private set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get;
      private set;
    }


    [DataField("ESTA_ABIERTA", ConvertFrom = typeof(int))]
    public bool IsOpened {
      get;
      private set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get;
      internal set;
    } = Participant.Empty;


    [DataField("ID_AUTORIZADA_POR", ConvertFrom = typeof(long))]
    public Participant AuthorizedBy {
      get;
      internal set;
    } = Participant.Empty;


    [DataField("ID_ENVIADA_DIARIO_POR", ConvertFrom = typeof(long))]
    public Participant ClosedBy {
      get;
      internal set;
    } = Participant.Empty;


    public FixedList<VoucherEntry> Entries {
      get {
        return _entries.Value;
      }
    }



    #endregion Public properties

    #region Methods


    internal void Delete() {
      VoucherData.DeleteVoucher(this);
    }


    private void LoadFields(VoucherFields fields) {
      this.Ledger = Ledger.Parse(fields.LedgerUID);
      this.AccountingDate = fields.AccountingDate;
      this.Concept = fields.Concept;
      this.VoucherType = VoucherType.Parse(fields.VoucherTypeUID);
      this.TransactionType = TransactionType.Parse(fields.TransactionTypeUID);
      this.FunctionalArea = FunctionalArea.Parse(fields.FunctionalAreaId);
    }


    protected override void OnSave() {
      VoucherData.WriteVoucher(this);
    }


    internal void Update(VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      this.Ledger = PatchField(fields.LedgerUID, this.Ledger);
      this.AccountingDate = PatchField(fields.AccountingDate, this.AccountingDate);
      this.Concept = PatchField(fields.Concept, this.Concept);
      this.TransactionType = PatchField(fields.TransactionTypeUID, this.TransactionType);
      this.VoucherType = PatchField(fields.VoucherTypeUID, this.VoucherType);
      this.FunctionalArea = PatchField(fields.FunctionalAreaId, this.FunctionalArea);
    }

    #endregion Methods

  }  // class Voucher

}  // namespace Empiria.FinancialAccounting.Vouchers
