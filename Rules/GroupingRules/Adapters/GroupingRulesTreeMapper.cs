/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Mapper class                            *
*  Type     : GroupingRulesTreeMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial accounting grouping rules as trees.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Rules.Adapters {

  /// <summary>Mapping methods for financial accounting grouping rules as trees</summary>
  static public class GroupingRulesTreeMapper {

    static internal FixedList<GroupingRulesTreeItemDto> MapFlat(FixedList<GroupingRulesTreeItem> items) {
      return new FixedList<GroupingRulesTreeItemDto>(items.Select(x => MapFlat(x)));
    }

    static private GroupingRulesTreeItemDto MapFlat(GroupingRulesTreeItem item) {
      return new GroupingRulesTreeItemDto {
        ItemCode = item.GroupingRuleItem.Code,
        ItemName = item.GroupingRuleItem.Name,
        Operator = Convert.ToString((char) item.GroupingRuleItem.Operator),
        ParentCode = item.Parent.GroupingRuleItem.IsEmptyInstance ?
                                            string.Empty: item.Parent.GroupingRuleItem.Code,
        Qualification = item.GroupingRuleItem.Qualification,
        SubledgerAccount = item.GroupingRuleItem.SubledgerAccountNumber,
        SubledgerAccountName = item.GroupingRuleItem.SubledgerAccountName,
        SectorCode = item.GroupingRuleItem.SectorCode,
        CurrencyCode = item.GroupingRuleItem.CurrencyCode,
        UID = item.GroupingRuleItem.UID,
        Type = item.GroupingRuleItem.Type,
        Level = item.Level
      };
    }

  }  // class GroupingRulesTreeMapper

}  // namespace Empiria.FinancialAccounting.Rules.Adapters
