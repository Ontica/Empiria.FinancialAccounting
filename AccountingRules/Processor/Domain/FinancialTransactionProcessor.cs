/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Service provider                      *
*  Type     : FinancialTransactionProcessor                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Processes a financial transaction and generates the corresponding voucher                      *
*             movements based on the applicable accounting rules.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Linq;

using Empiria.Json;

using Empiria.FinancialAccounting.Transactions;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Processes a financial transaction and generates the corresponding voucher
  /// movements based on the applicable accounting rules.</summary>
  internal sealed class FinancialTransactionProcessor {

    internal AccountingRuleProcessingResult Process(FinancialTransaction transaction) {

      Assertion.Require(transaction, nameof(transaction));

      var guide = AccountingGuide.ParseWithCode(transaction.TransactionType.Key);

      var result = new AccountingRuleProcessingResult();

      FixedList<JsonObject> budgetEntries = GetBudgetEntries(transaction);

      foreach (JsonObject budgetEntry in budgetEntries) {

        var context = new AccountingRuleContext(transaction, budgetEntry);

        AccountingRule rule = FindRule(guide, context);

        if (rule == null) {
          RegisterPendingRule(context, result);
          continue;
        }

        ProcessRule(rule, context, result);
      }

      return result;
    }

    #region Helpers

    private VoucherMovementFields CreateMovement(AccountingRuleEntry ruleEntry,
                                                 AccountingRuleContext context) {

      decimal amount = ResolveAmount(ruleEntry.AmountSource, context);

      var movement = new VoucherMovementFields {

        LedgerAccountNo = ruleEntry.StandardAccount.Number,

        SubledgerAccountNo = ResolveString(ruleEntry.SubledgerAccountSource, context),

        ResponsibilityAreaCode = ResolveString(ruleEntry.ResponsibilityAreaSource, context),

        BudgetAccountCode = ResolveString(ruleEntry.SelectorValueSource, context),

        BudgetControlNo = ResolveString(ruleEntry.ControlNoSource, context),

        CurrencyISOCode = context.Transaction.Payload.Get<string>("currencyISOCode"),

        ExchangeRate = context.Transaction.Payload.Get<decimal>("exchangeRate"),

        Concept = context.Transaction.Description
      };

      if (ruleEntry.EntryType == VoucherEntryType.Debit) {
        movement.DebitAmount = amount;
      } else {
        movement.CreditAmount = amount;
      }

      return movement;
    }


    private AccountingRule FindRule(AccountingGuide guide,
                                    AccountingRuleContext context) {

      string budgetAccountCode = context.Entry.Get<string>("budgetAccountCode");

      return guide.Rules.Find(x => x.SelectorValues.Contains(budgetAccountCode));
    }


    private FixedList<JsonObject> GetBudgetEntries(FinancialTransaction transaction) {

      return transaction.Payload.GetFixedList<JsonObject>("entries");
    }


    private void ProcessRule(AccountingRule rule,
                             AccountingRuleContext context,
                             AccountingRuleProcessingResult result) {

      Assertion.Require(rule, nameof(rule));
      Assertion.Require(context, nameof(context));
      Assertion.Require(result, nameof(result));

      foreach (AccountingRuleEntry ruleEntry in rule.Entries.OrderBy(x => x.Position)) {

        VoucherMovementFields movement = CreateMovement(ruleEntry, context);

        result.AddMovement(movement);
      }
    }


    private void RegisterPendingRule(AccountingRuleContext context,
                                     AccountingRuleProcessingResult result) {

      string budgetAccountCode = context.Entry.Get<string>("budgetAccountCode");

      string message = $"No existe una regla contable para la partida presupuestal " +
                       $"'{budgetAccountCode}'. El movimiento queda pendiente de contabilizar.";

      EmpiriaLog.Info(message);

      result.AddPendingItem(message);
    }


    private decimal ResolveAmount(string source, AccountingRuleContext context) {

      switch (source) {

        case "Amount":
          return context.Entry.Get<decimal>("amount");

        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized accounting amount source '{source}'.");
      }
    }


    private string ResolveString(string source, AccountingRuleContext context) {

      if (string.IsNullOrWhiteSpace(source)) {
        return string.Empty;
      }

      switch (source) {

        case "BudgetAccountCode":
          return context.Entry.Get<string>("budgetAccountCode");

        case "BudgetControlNo":
          return context.Entry.Get<string>("budgetControlNo");

        case "AreaCode":
          return context.Entry.Get<string>("orgUnitCode");

        case "PayeeSubledgerAccountNo":
          return context.Transaction.Payload.Get<string>("payeeSubledgerAccountNo");

        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized accounting rule source '{source}'.");
      }
    }

    #endregion Helpers

  }  // class FinancialTransactionProcessor

}  // namespace Empiria.FinancialAccounting.AccountingRules
