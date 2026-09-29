/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder                    *
*  Type     : AccountingRuleContext                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds a financial transaction and one of its associated entries.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Json;

using Empiria.FinancialAccounting.Transactions;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Holds a financial transaction and one of its associated entries.</summary>
  public class AccountingRuleContext {

    internal AccountingRuleContext(FinancialTransaction transaction, JsonObject entry) {

      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(entry, nameof(entry));

      Transaction = transaction;
      Entry = entry;
    }


    internal FinancialTransaction Transaction {
      get;
    }


    internal JsonObject Entry {
      get;
    }

  }  // class AccountingRuleContext

}  // namespace Empiria.FinancialAccounting.AccountingRules
