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
using Empiria.Contacts;

using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Represents an accounting voucher.</summary>
  public class Voucher : BaseObject {

    private Lazy<FixedList<VoucherEntry>> _entries;

    #region Constructors and parsers

    private Voucher() {
      // Required by Empiria Framework.
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


    protected override void OnLoad() {
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
      get; private set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get; private set;
    }


    [DataField("ESTA_ABIERTA", ConvertFrom = typeof(int))]
    public bool IsOpened {
      get; private set;
    }


    public FixedList<VoucherEntry> Entries {
      get {
        return _entries.Value;
      }
    }

    #endregion Public properties

  }  // class Voucher

}  // namespace Empiria.FinancialAccounting.Vouchers
