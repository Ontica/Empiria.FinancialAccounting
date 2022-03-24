/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Empiria Plain Object                    *
*  Type     : VouchersByAccountEntry                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for vouchers by account.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.Reporting {

  public interface IVouchersByAccountEntry {

  }

  /// <summary>Represents an entry for vouchers by account.</summary>
  public class AccountStatementEntry : IVouchersByAccountEntry {


    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get; internal set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get;  internal set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public int StandardAccountId {
      get; internal set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; internal set;
    }


    [DataField("ID_TRANSACCION", ConvertFrom = typeof(decimal))]
    public int VoucherId {
      get; internal set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get; internal set;
    } = Participant.Empty;


    [DataField("ID_AUTORIZADA_POR", ConvertFrom = typeof(long))]
    public Participant AuthorizedBy {
      get; internal set;
    } = Participant.Empty;


    [DataField("ID_MOVIMIENTO", ConvertFrom = typeof(decimal))]
    public int VoucherEntryId {
      get; internal set;
    }



    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; internal set;
    }


    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string AccountName {
      get; internal set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; internal set;
    }


    [DataField("NUMERO_TRANSACCION")]
    public string VoucherNumber {
      get; internal set;
    }


    [DataField("NATURALEZA")]
    public string DebtorCreditor {
      get; internal set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get; internal set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get; internal set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string Concept {
      get; internal set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get;  internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get; internal set;
    }


    public decimal CurrentBalance {
      get; internal set;
    }


    public bool IsCurrentBalance {
      get; internal set;
    } = false;

    public TrialBalanceItemType ItemType {
      get; internal set;
    } = TrialBalanceItemType.Entry;

  } // class VouchersByAccountEntry

} // namespace Empiria.FinancialAccounting.Reporting
