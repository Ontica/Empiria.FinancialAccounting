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
  static internal class ExternalValuesData {

    static internal FixedList<GroupingRule> GetGroupingRules(RulesSet rulesSet) {
      var sql = "SELECT * FROM COF_CONCEPTOS " +
                $"WHERE ID_GRUPO = {rulesSet.Id} " +
                "ORDER BY POSICION";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<GroupingRule>(dataOperation);
    }

    static internal FixedList<GroupingRuleItem> GetGroupingRulesItems(RulesSet rulesSet) {
      var sql = "SELECT COF_CONCEPTOS_INTEGRACION.* " +
                "FROM COF_CONCEPTOS_INTEGRACION " +
               $"WHERE ID_GRUPO = {rulesSet.Id} " +
                "ORDER BY POSICION";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<GroupingRuleItem>(dataOperation);
    }

    static internal ExternalValue GetValue(string externalVariableCode, DateTime date) {
      var sql = "SELECT * FROM COF_CONCEPTOS_VALORES " +
               $"WHERE CLAVE_VARIABLE = '{externalVariableCode}' " +
               $"AND FECHA = '{CommonMethods.FormatSqlDate(date)}'";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetObject<ExternalValue>(dataOperation, ExternalValue.Empty);
    }

  }  // class GroupingRulesData

}  // namespace Empiria.FinancialAccounting.Rules.Data
