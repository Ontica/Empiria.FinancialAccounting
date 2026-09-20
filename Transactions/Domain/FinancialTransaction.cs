/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services            Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Transactions.dll       Pattern   : Information holder                      *
*  Type     : FinancialTransaction                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a financial transaction received from an external system.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Json;

using Empiria.Financial.Transactions.Adapters;

using Empiria.FinancialAccounting.Transactions.Data;

namespace Empiria.FinancialAccounting.Transactions {

  /// <summary>Holds information about a financial transaction received from an external system.</summary>
  public class FinancialTransaction : BaseObject {

    #region Constructors and parsers

    private FinancialTransaction() {
      // Required by Empiria Framework.
    }

    static public FinancialTransaction Parse(int id) => ParseId<FinancialTransaction>(id);

    static public FinancialTransaction Parse(string uid) => ParseKey<FinancialTransaction>(uid);

    static public FinancialTransaction Empty => ParseEmpty<FinancialTransaction>();

    static internal FinancialTransaction Create(FinancialTransactionFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      return new FinancialTransaction {
        TransactionType = FinancialTransactionType.ParseKey(fields.TransactionKey),
        TransactionReferenceId = fields.TransactionReferenceId,
        TraceableEntityReferenceId = fields.TraceableEntityReferenceId,
        Source = TransactionalSystem.ParseWithCode(fields.SourceCode),
        Description = fields.Description,
        Payload = fields.Payload,
        ApplicationDate = fields.ApplicationDate,
        RecordingTime = fields.RecordingTime,
        ProcessingTime = ExecutionServer.DateMaxValue,
        Status = FinancialTransactionStatus.Received,
      };
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("TXN_TYPE_ID")]
    public FinancialTransactionType TransactionType {
      get; private set;
    }


    [DataField("TXN_REFERENCE_ID")]
    public int TransactionReferenceId {
      get; private set;
    }


    [DataField("TXN_TRACEABLE_ENTITY_REF_ID")]
    public int TraceableEntityReferenceId {
      get; private set;
    }


    [DataField("TXN_DESCRIPTION")]
    public string Description {
      get; private set;
    }


    [DataField("TXN_SOURCE_ID")]
    public TransactionalSystem Source {
      get; private set;
    }


    [DataField("TXN_PAYLOAD")]
    public JsonObject Payload {
      get; private set;
    }


    [DataField("TXN_APPLICATION_DATE")]
    public DateTime ApplicationDate {
      get; private set;
    }


    [DataField("TXN_RECORDING_TIME")]
    public DateTime RecordingTime {
      get; private set;
    }


    [DataField("TXN_EXT_DATA")]
    internal protected JsonObject ExtData {
      get; private set;
    }


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(Description);
      }
    }


    [DataField("TXN_PROCESSING_TIME")]
    public DateTime ProcessingTime {
      get; private set;
    }


    [DataField("TXN_POSTING_TIME")]
    public DateTime PostingTime {
      get; private set;
    }


    [DataField("TXN_STATUS", Default = FinancialTransactionStatus.Received)]
    public FinancialTransactionStatus Status {
      get; private set;
    }

    #endregion Properties

    #region Methods

    protected override void OnSave() {
      if (IsNew) {
        PostingTime = DateTime.Now;
      }

      FinancialTransactionData.Write(this);
    }

    #endregion Methods

  }  // class FinancialTransaction

}  // namespace Empiria.FinancialAccounting.Transactions
