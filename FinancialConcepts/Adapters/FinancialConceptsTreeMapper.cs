/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Pattern   : Data Transfer Object                    *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Mapper class                            *
*  Type     : FinancialConceptsTreeMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial concepts trees.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial concepts trees.</summary>
  static public class FinancialConceptsTreeMapper {


    static internal FixedList<FinancialConceptTreeNodeDto> Map(FixedList<FinancialConceptNode> nodes) {
      var list = new List<FinancialConceptTreeNodeDto>(nodes.Count * 2);

      foreach (var node in nodes) {
        list.Add(Map(node.FinancialConcept));
        list.AddRange(node.Children.Select(child => Map(child)));
      }
      return list.ToFixedList();
    }

    static private FinancialConceptTreeNodeDto Map(FinancialConceptEntry child) {
      return new FinancialConceptTreeNodeDto {
        ItemCode = child.Code,
        ItemName = child.Name,
        Operator = Convert.ToString((char) child.Operator),
        ParentCode = string.Empty,
        DataColumn = child.DataColumn,
        SubledgerAccount = child.SubledgerAccountNumber,
        SubledgerAccountName = child.SubledgerAccountName,
        SectorCode = child.SectorCode,
        CurrencyCode = child.CurrencyCode,
        UID = child.UID,
        Type = child.Type,
        Level = 2
      };
    }

    static private FinancialConceptTreeNodeDto Map(FinancialConcept financialConcept) {
      return new FinancialConceptTreeNodeDto {
        ItemCode = financialConcept.Code,
        ItemName = financialConcept.Name,
        UID = financialConcept.UID,
        Level = 1
      };
    }

  }  // class FinancialConceptsTreeMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
