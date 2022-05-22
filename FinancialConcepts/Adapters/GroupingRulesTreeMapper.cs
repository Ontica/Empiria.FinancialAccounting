/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Pattern   : Data Transfer Object                    *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Mapper class                            *
*  Type     : GroupingRulesTreeMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial accounting grouping rules as trees.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial accounting grouping rules as trees</summary>
  static public class GroupingRulesTreeMapper {

    static internal FixedList<GroupingRulesTreeItemDto> MapFlat(FixedList<GroupingRulesTreeItem> items) {
      return new FixedList<GroupingRulesTreeItemDto>(items.Select(x => MapFlat(x)));
    }

    static private GroupingRulesTreeItemDto MapFlat(GroupingRulesTreeItem item) {
      return new GroupingRulesTreeItemDto {
        ItemCode = item.IntegrationEntry.Code,
        ItemName = item.IntegrationEntry.Name,
        Operator = Convert.ToString((char) item.IntegrationEntry.Operator),
        ParentCode = item.Parent.IntegrationEntry.IsEmptyInstance ?
                                            string.Empty: item.Parent.IntegrationEntry.Code,
        Qualification = item.IntegrationEntry.Qualification,
        SubledgerAccount = item.IntegrationEntry.SubledgerAccountNumber,
        SubledgerAccountName = item.IntegrationEntry.SubledgerAccountName,
        SectorCode = item.IntegrationEntry.SectorCode,
        CurrencyCode = item.IntegrationEntry.CurrencyCode,
        UID = item.IntegrationEntry.UID,
        Type = item.IntegrationEntry.Type,
        Level = item.Level
      };
    }

  }  // class GroupingRulesTreeMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
