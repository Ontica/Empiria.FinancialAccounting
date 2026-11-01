/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Data Access Layer                     *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Data services                         *
*  Type     : ClassificationRulesData                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Reads classification rule aggregates                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.FinancialAccounting.AccountingRules.Data {

  /// <summary>Reads classification rule aggregates</summary>
  static internal class ClassificationRulesData {

    static internal List<ClassificationRule> GetRules(ClassificationGuide guide) {
      var sql = "SELECT * FROM COF_ACCOUNTING_RULES " +
                $"WHERE ACR_GUIDE_ID = {guide.Id} " +
                "AND ACR_STATUS <> 'X' " +
                "ORDER BY ACR_PRIORITY, ACR_ID";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<ClassificationRule>(op);
    }


    static internal List<ClassificationEntry> GetEntries(ClassificationRule rule) {
      var sql = "SELECT * FROM COF_ACCOUNTING_RULE_ENTRIES " +
                $"WHERE ACR_ENTRY_RULE_ID = {rule.Id} " +
                "AND ACR_ENTRY_STATUS <> 'X' " +
                "ORDER BY ACR_ENTRY_POSITION, ACR_ENTRY_ID";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<ClassificationEntry>(op);
    }

  }  // class ClassificationRulesData

}  // namespace Empiria.FinancialAccounting.AccountingRules.Data
