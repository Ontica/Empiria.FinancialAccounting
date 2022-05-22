/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Mapper class                            *
*  Type     : GroupingRulesMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial accounting grouping rules.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial accounting grouping rules.</summary>
  static public class GroupingRulesMapper {


    static internal FixedList<GroupingRuleDto> Map(FixedList<GroupingRule> rules) {
      return new FixedList<GroupingRuleDto>(rules.Select(x => Map(x)));
    }


    static internal FixedList<GroupingRuleItemDto> Map(FixedList<GroupingRuleItem> items) {
      return new FixedList<GroupingRuleItemDto>(items.Select(x => Map(x)));
    }


    static internal GroupingRuleDto Map(GroupingRule rule) {
      return new GroupingRuleDto {
        UID = rule.UID,
        Code = rule.Code,
        Concept = rule.Concept,
        Position = rule.Position,
        AccountsChartName = rule.Group.AccountsChart.Name,
        GroupName = rule.Group.Name
      };
    }


    static private GroupingRuleItemDto Map(GroupingRuleItem item) {
      return new GroupingRuleItemDto {
        UID = item.UID,
        Type = item.Type,
        ItemCode = item.Code,
        ItemName = item.Name,
        SubledgerAccount = item.SubledgerAccountNumber,
        SectorCode = item.SectorCode,
        Operator = Convert.ToString((char) item.Operator)
      };
    }

  }  // class GroupingRulesMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
