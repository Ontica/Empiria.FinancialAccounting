/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Mapper class                            *
*  Type     : GroupingRulesMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial accounting grouping rules.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Rules.Adapters {

  /// <summary>Mapping methods for financial accounting grouping rules.</summary>
  static public class GroupingRulesMapper {

    static internal FixedList<GroupingRuleDto> Map(FixedList<GroupingRule> rules) {
      return new FixedList<GroupingRuleDto>(rules.Select<GroupingRuleDto>(x => Map(x)));
    }

    static internal GroupingRuleDto Map(GroupingRule rule) {
      return new GroupingRuleDto {
        UID = rule.UID,
        Code = rule.Code,
        Concept = rule.Concept,
        Position = rule.Position,
        Level = rule.Level,
        ParentUID = rule.Parent.UID,
      };
    }

  }  // class GroupingRulesMapper

}  // namespace Empiria.FinancialAccounting.Rules.Adapters
