/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Data Service                            *
*  Type     : GroupingRulesData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial accounting grouping rules.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Rules.Data {

  /// <summary>Data access layer for financial accounting grouping rules.</summary>
  static internal class GroupingRulesData {

    static internal FixedList<GroupingRule> GetGroupingRules(RulesSet rulesSet) {
      var sql = "SELECT * FROM COF_CONCEPTOS " +
                $"WHERE CONJUNTO_BASE = '{rulesSet.Code}' " +
                "ORDER BY POSICION";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<GroupingRule>(dataOperation);
    }

  }  // class GroupingRulesData

}  // namespace Empiria.FinancialAccounting.Rules.Data
