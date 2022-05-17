/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Service                            *
*  Type     : GroupingRulesData                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial accounting grouping rules.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts.Data {

  /// <summary>Data access layer for financial accounting grouping rules.</summary>
  static internal class GroupingRulesData {

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


    static internal void Write(GroupingRule o) {
      var op = DataOperation.Parse("write_cof_concepto",
                      o.Id, o.Code, o.Concept, o.Position,
                      o.StartDate, o.EndDate, o.UID, o.RulesSet.Id);


      DataWriter.Execute(op);
    }



    static internal void Write(GroupingRuleItem o) {
      var op = DataOperation.Parse("write_cof_concepto_integracion",
                      o.Id, o.UID, o.GroupingRule.Id, o.CalculationRule, o.Reference.Id,
                      o.AccountNumber, o.SubledgerAccountNumber, o.SectorCode, o.ExternalVariableCode,
                      (char) o.Operator, o.Qualification, o.IntegrationTypeId, o.Position,
                      o.RulesSet.Id, o.CurrencyCode, o.AccountsListId);

      DataWriter.Execute(op);
    }


  }  // class GroupingRulesData

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Data
