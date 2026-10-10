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

using System;

using Empiria.Json;

using Empiria.FinancialAccounting.Transactions;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Processes a payment financial transaction and generates the corresponding voucher
  /// movements based on the applicable accounting rules.</summary>
  internal sealed class PaymentTransactionProcessor {

    internal AccountingRuleProcessingResult Process(FinancialTransaction transaction) {

      Assertion.Require(transaction, nameof(transaction));


      var provisionGuide = AccountingGuide.ParseWithCode("PROVISION_DE_PAGO");

      var result = new PaymentProvisionProcessor().Process(transaction, provisionGuide);

      if (result.HasPendingItems) {
        return result;
      }

      ProcessBills(transaction, result);

      return result;
    }

    #region Helpers

    private void AddMovement(AccountingRuleProcessingResult result,
                             VoucherMovementFields source,
                             string ledgerAccountNo,
                             string subledgerAccountNo,
                             decimal debitAmount,
                             decimal creditAmount) {

      result.AddMovement(new VoucherMovementFields {
        LedgerAccountNo = ledgerAccountNo,
        SubledgerAccountNo = subledgerAccountNo,
        ResponsibilityAreaCode = source.ResponsibilityAreaCode,
        BudgetAccountCode = source.BudgetAccountCode,
        BudgetControlNo = source.BudgetControlNo,
        CurrencyISOCode = source.CurrencyISOCode,
        ExchangeRate = source.ExchangeRate,
        Concept = source.Concept,
        DebitAmount = debitAmount,
        CreditAmount = creditAmount
      });
    }


    private decimal GetTransferredVatAmount(JsonObject bill) {

      var taxes = bill.GetFixedList<JsonObject>("taxes");

      decimal amount = 0m;

      foreach (JsonObject tax in taxes) {

        if (tax.Get<string>("taxCode") != "002") {
          continue;
        }

        if (tax.Get<string>("taxMethod") != "Traslado") {
          continue;
        }

        amount += tax.Get<decimal>("amount");
      }

      return amount;
    }


    private void ProcessBills(FinancialTransaction transaction,
                              AccountingRuleProcessingResult result) {

      var bills = transaction.Payload.GetFixedList<JsonObject>("bills");

      decimal invoiceVatAmount = 0m;
      decimal creditNoteAmount = 0m;
      decimal penaltyAmount = 0m;
      decimal creditNoteVatAmount = 0m;

      foreach (JsonObject bill in bills) {

        string categoryCode = bill.Get<string>("billCategoryCode");

        switch (categoryCode) {

          case "BILL-CFDI-FE":
            invoiceVatAmount += GetTransferredVatAmount(bill);
            break;

          case "BILL-CFDI-NC":
            creditNoteAmount += bill.Get<decimal>("subtotal");
            creditNoteVatAmount += GetTransferredVatAmount(bill);
            break;

          case "BILL-CFDI-PENALTY":
            penaltyAmount += bill.Get<decimal>("subtotal");
            creditNoteVatAmount += GetTransferredVatAmount(bill);
            break;

          default:
            result.AddPendingItem(
                $"Accounting processing for bill category '{categoryCode}' is not implemented.");
            break;
        }
      }

      if (result.HasPendingItems) {
        return;
      }

      invoiceVatAmount = Math.Round(invoiceVatAmount, 2);
      creditNoteVatAmount = Math.Round(creditNoteVatAmount, 2);

      ProcessPayment(transaction,
                     invoiceVatAmount,
                     creditNoteAmount,
                     penaltyAmount,
                     creditNoteVatAmount,
                     result);
    }


    private void ProcessPayment(FinancialTransaction transaction,
                                decimal invoiceVatAmount,
                                decimal creditNoteAmount,
                                decimal penaltyAmount,
                                decimal creditNoteVatAmount,
                                AccountingRuleProcessingResult result) {

      var guide = AccountingGuide.ParseWithCode(transaction.TransactionType.Key);

      AccountingGuideAccounts accounts = guide.Accounts;


      var provisionMovements =
          result.Movements.FindAll(x =>
              x.LedgerAccountNo.StartsWith("2.07.04.07.") &&
              x.CreditAmount > 0);

      if (provisionMovements.Count != 1) {
        result.AddPendingItem(
            "Payment processing supports exactly one provision liability movement.");

        return;
      }


      FixedList<VoucherMovementFields> expenseMovements =
        result.Movements.FindAll(x => x.LedgerAccountNo.StartsWith("6.") &&
                                      x.DebitAmount > 0);

      if (expenseMovements.Count != 1) {
        result.AddPendingItem(
            "Payment processing supports exactly one expense movement.");

        return;
      }


      VoucherMovementFields provision = provisionMovements[0];
      VoucherMovementFields expense = expenseMovements[0];

      decimal netBaseAmount = provision.CreditAmount;

      decimal adjustmentBaseAmount =
          creditNoteAmount + penaltyAmount;

      decimal grossBaseAmount =
          netBaseAmount + adjustmentBaseAmount;

      decimal liabilityAmount =
          grossBaseAmount + invoiceVatAmount;

      decimal total =
          transaction.Payload.Get<decimal>("total");


      decimal expectedTotal =
          liabilityAmount -
          creditNoteAmount -
          penaltyAmount -
          creditNoteVatAmount;


      if (expectedTotal != total) {
        result.AddPendingItem(
            $"Payment amounts do not match. " +
            $"Net base={netBaseAmount}, " +
            $"Gross base={grossBaseAmount}, " +
            $"Invoice VAT={invoiceVatAmount}, " +
            $"Credit note={creditNoteAmount}, " +
            $"Penalty={penaltyAmount}, " +
            $"Credit note VAT={creditNoteVatAmount}, " +
            $"Liability={liabilityAmount}, " +
            $"Expected total={expectedTotal}, " +
            $"Total={total}.");

        return;
      }


      string paymentMethodCode =
          transaction.Payload.Get<string>("paymentMethodCode");

      if (paymentMethodCode != "40") {
        result.AddPendingItem(
            $"Payment method '{paymentMethodCode}' is not supported yet.");

        return;
      }


      string realLiabilityAccount =
          provision.LedgerAccountNo.Replace("2.07.04.07.",
                                            "2.07.04.08.");


      //
      // Reconstruct original gross expense/provision.
      //

      if (adjustmentBaseAmount > 0) {

        AddMovement(result, expense,
                    expense.LedgerAccountNo,
                    expense.SubledgerAccountNo,
                    adjustmentBaseAmount, 0);

        AddMovement(result, provision,
                    provision.LedgerAccountNo,
                    provision.SubledgerAccountNo,
                    0, adjustmentBaseAmount);
      }


      //
      // Cancel provision liability.
      //

      AddMovement(result, provision,
                  provision.LedgerAccountNo,
                  provision.SubledgerAccountNo,
                  grossBaseAmount, 0);


      //
      // Invoice VAT pending accreditation.
      //

      if (invoiceVatAmount > 0) {
        AddMovement(result, provision,
                    accounts.VatPendingAccount,
                    accounts.VatSubledgerAccount,
                    invoiceVatAmount, 0);
      }


      //
      // Build real liability.
      //

      AddMovement(result, provision,
                  realLiabilityAccount,
                  provision.SubledgerAccountNo,
                  0, liabilityAmount);


      //
      // Liquidate real liability.
      //

      AddMovement(result, provision,
                  realLiabilityAccount,
                  provision.SubledgerAccountNo,
                  liabilityAmount, 0);


      //
      // Move invoice VAT from pending to creditable.
      //

      if (invoiceVatAmount > 0) {

        AddMovement(result, provision,
                    accounts.VatCreditableAccount,
                    accounts.VatSubledgerAccount,
                    invoiceVatAmount, 0);

        AddMovement(result, provision,
                    accounts.VatPendingAccount,
                    accounts.VatSubledgerAccount,
                    0, invoiceVatAmount);
      }


      //
      // Normal credit note / discount.
      //

      if (creditNoteAmount > 0) {
        AddMovement(result, expense,
                    expense.LedgerAccountNo,
                    expense.SubledgerAccountNo,
                    0, creditNoteAmount);
      }


      //
      // Penalty.
      //

      if (penaltyAmount > 0) {
        AddMovement(result, provision,
                    accounts.PenaltyIncomeAccount,
                    string.Empty,
                    0, penaltyAmount);
      }


      //
      // VAT associated with credit notes and penalties.
      //

      if (creditNoteVatAmount > 0) {
        AddMovement(result, provision,
                    accounts.CreditNoteVatAccount,
                    accounts.CreditNoteVatSubledgerAccount,
                    0, creditNoteVatAmount);
      }


      //
      // Actual payment flow.
      //

      AddMovement(result, provision,
                  "9.02.15",
                  string.Empty,
                  0, total);
    }

    #endregion Helpers

  }  // class PaymentTransactionProcessor

}  // namespace Empiria.FinancialAccounting.AccountingRules
