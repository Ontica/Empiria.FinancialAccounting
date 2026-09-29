/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Data Access Layer                     *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Data services                         *
*  Type     : AccountingRulesData                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Data access layer for accounting rules.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.FinancialAccounting.AccountingRules.Data {

  /// <summary>Data access layer for accounting rules.</summary>
  static internal class AccountingRulesData {

    static internal List<AccountingRule> GetAccountingRules(AccountingGuide guide) {
      var sql = "SELECT * FROM COF_ACCOUNTING_RULES " +
                $"WHERE ACR_GUIDE_ID = {guide.Id} " +
                "AND ACR_STATUS <> 'X' " +
                "ORDER BY ACR_PRIORITY";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<AccountingRule>(op);
    }


    static internal List<AccountingRuleEntry> GetAccountingRuleEntries(AccountingRule rule) {
      var sql = "SELECT * FROM COF_ACCOUNTING_RULE_ENTRIES " +
                $"WHERE ACR_ENTRY_RULE_ID = {rule.Id} " +
                "AND ACR_ENTRY_STATUS <> 'X' " +
                "ORDER BY ACR_ENTRY_RULE_ID";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<AccountingRuleEntry>(op);
    }

  }  // class AccountingRulesData

}  // namespace Empiria.FinancialAccounting.AccountingRules.Data
