/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Interface                               *
*  Type     : IReconciliationRowReader                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Interface for all reconcilaction data row readers.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Json;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Interface for all reconcilaction data row readers.</summary>
  internal interface IReconciliationRowReader {

    string GetAccountNumber();

    decimal GetCredits();

    string GetCurrencyCode();

    decimal GetDebits();

    decimal GetEndBalance();

    JsonObject GetExtensionData();

    decimal GetInitialBalance();

    string GetLedger();

    string GetSectorCode();

    string GetSubledgerAccountNumber();

    string GetTransactionSlip();

    string GetUniqueKey();

  }  // interface IReconciliationRowReader

}  // namespace Empiria.FinancialAccounting.Reconciliation
