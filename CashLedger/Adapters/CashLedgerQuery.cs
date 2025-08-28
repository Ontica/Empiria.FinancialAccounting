/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Query DTO                            *
*  Type     : CashLedgerQuery                               License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Input query DTO used to retrieve cash ledger transactions.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Input query DTO used to retrieve cash ledger transactions.</summary>
  public class CashLedgerQuery : BaseCashLedgerQuery {

    internal bool SearchEntries {
      get; set;
    }

  }  // class CashLedgerQuery

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
