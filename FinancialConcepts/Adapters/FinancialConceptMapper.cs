/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Mapper class                            *
*  Type     : FinancialConceptMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial concepts.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial concepts.</summary>
  static public class FinancialConceptMapper {

    static internal FixedList<FinancialConceptDto> Map(FixedList<FinancialConcept> list) {
      return new FixedList<FinancialConceptDto>(list.Select(financialConcept => Map(financialConcept)));
    }


    static internal FixedList<GroupingRuleItemDto> Map(FixedList<GroupingRuleItem> list) {
      return new FixedList<GroupingRuleItemDto>(list.Select(x => Map(x)));
    }


    static internal FinancialConceptDto Map(FinancialConcept concept) {
      return new FinancialConceptDto {
        UID = concept.UID,
        Code = concept.Code,
        Concept = concept.Name,
        Position = concept.Position,
        AccountsChartName = concept.Group.AccountsChart.Name,
        GroupName = concept.Group.Name
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

  }  // class FinancialConceptMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
