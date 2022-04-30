/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data service                            *
*  Type     : ReconciliationData                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for reconciliation data.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reconciliation.Data {

  /// <summary>Data access layer for reconciliation data.</summary>
  static internal class ReconciliationData {

    static internal void AppendEntry(ReconciliationEntry o) {
      var op = DataOperation.Parse("apd_cof_dato_conciliacion",
              o.Id, o.UID, o.GetEmpiriaType().Id, o.Dataset.Id, o.UniqueKey,
              o.LedgerNumber, o.AccountNumber, o.CurrencyCode, o.SectorCode,
              o.SubledgerAccountNumber, o.TransactionSlip, o.ExtData.ToString(),
              o.InitialBalance, o.Debits, o.Credits, o.EndBalance,
              o.Position);


     DataWriter.Execute(op);
    }

  }  // ReconciliationData

} // namespace Empiria.FinancialAccounting.Reconciliation.Data
