/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Domain Layer                            *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Information holder                      *
*  Type     : CashEntryExtended                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data for a cash transaction entry with its transaction data.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.CashLedger {

  /// <summary>Holds data for a cash transaction entry with its transaction data.</summary>
  internal class CashEntryExtended : CashEntry {

    #region Properties

    [DataField("ID_TRANSACCION")]
    public long TransactionId {
      get; private set;
    }


    [DataField("NUMERO_TRANSACCION")]
    public string TransactionNumber {
      get; private set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string TransactionConcept {
      get; private set;
    }


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    [DataField("ID_TIPO_TRANSACCION", ConvertFrom = typeof(long))]
    public TransactionType TransactionType {
      get; private set;
    }


    [DataField("ID_TIPO_POLIZA", ConvertFrom = typeof(long))]
    public VoucherType VoucherType {
      get; private set;
    }


    [DataField("ID_FUENTE", ConvertFrom = typeof(long))]
    public FunctionalArea TransactionFunctionalArea {
      get; private set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get; private set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get; private set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get; private set;
    } = Participant.Empty;

    #endregion Properties

  }  // class CashEntryExtended

}  // namespace Empiria.FinancialAccounting.CashLedger
