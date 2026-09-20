
/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services            Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Transactions.dll       Pattern   : Data Service                            *
*  Type     : FinancialTransactionData                    License   : Please read LICENSE.txt file           *
*                                                                                                            *
*  Summary  : Data access services for financial transactions.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

namespace Empiria.FinancialAccounting.Transactions.Data {

  /// <summary>Data access services for financial transactions.</summary>
  static internal class FinancialTransactionData {

    #region Data writing

    static internal void Write(FinancialTransaction o) {
      Assertion.Require(o, nameof(o));

      var op = DataOperation.Parse("write_cof_financial_txn",
                                  o.Id, o.UID, o.TransactionType.Id,
                                  o.TransactionReferenceId, o.TraceableEntityReferenceId,
                                  o.Description, o.Source.Id, o.Payload.ToString(),
                                  o.ApplicationDate, o.RecordingTime,
                                  o.ExtData.ToString(), o.Keywords,
                                  o.ProcessingTime, o.PostingTime,
                                  (char) o.Status);

      DataWriter.Execute(op);
    }

    #endregion Data writing

  }  // class FinancialTransactionData

}  // namespace Empiria.FinancialAccounting.Transactions.Data
