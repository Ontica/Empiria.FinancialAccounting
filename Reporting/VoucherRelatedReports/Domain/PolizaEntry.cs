/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : PolizaEntry                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a voucher.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {

  public interface IPolizaEntry {

  }

  /// <summary>Represents an entry for a voucher.</summary>
  public class PolizaEntry : IPolizaEntry {


    #region Constructors and parsers

    internal PolizaEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    public ItemType ItemType {
      get; internal set;
    } = ItemType.Entry;


    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get;
      internal set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get;
      internal set;
    } = Participant.Empty;


    [DataField("NUMERO_TRANSACCION")]
    public string Number {
      get;
      internal set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string Concept {
      get;
      internal set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get;
      internal set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get;
      internal set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get;
      internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get;
      internal set;
    }


    public int VouchersByLedger {
      get; internal set;
    }


    internal void Sum(PolizaEntry voucher) {
      this.Debit += voucher.Debit;
      this.Credit += voucher.Credit;
      this.VouchersByLedger += 1;
    }

  } // class PolizaEntry


} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
