/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Service provider                      *
*  Type     : PaymentTransactionProcessor                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Processes a payment financial transaction and generates the corresponding voucher              *
*             movements based on the applicable accounting rules.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Json;

using Empiria.FinancialAccounting.Transactions;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Processes a payment financial transaction and generates the corresponding voucher
  /// movements based on the applicable accounting rules.</summary>
  internal sealed class PaymentTransactionProcessor {

    internal AccountingRuleProcessingResult Process(FinancialTransaction transaction) {

      Assertion.Require(transaction, nameof(transaction));

      var result = new PaymentProvisionProcessor().Process(transaction);

      if (result.HasPendingItems) {
        return result;
      }

      ProcessBills(transaction, result);

      return result;
    }


    private void ProcessBills(FinancialTransaction transaction,
                              AccountingRuleProcessingResult result) {

      var bills = transaction.Payload.GetFixedList<JsonObject>("bills");

      foreach (JsonObject bill in bills) {
        ProcessBill(transaction, bill, result);
      }
    }


    private void ProcessBill(FinancialTransaction transaction,
                             JsonObject bill,
                             AccountingRuleProcessingResult result) {

      string categoryCode = bill.Get<string>("billCategoryCode");

      string message =
          $"Accounting processing for bill category '{categoryCode}' is not implemented.";

      result.AddPendingItem(message);
    }

  }  // class PaymentTransactionProcessor

}  // namespace Empiria.FinancialAccounting.AccountingRules
