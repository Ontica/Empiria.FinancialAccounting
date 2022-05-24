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

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial concepts trees.</summary>
  static public class FinancialConceptsTreeMapper {


    static internal FixedList<FinancialConceptEntryAsTreeNodeDto> Map(FixedList<FinancialConceptEntryAsTreeNode> nodes) {
      return new FixedList<FinancialConceptEntryAsTreeNodeDto>(nodes.Select(node => Map(node)));
    }


    static private FinancialConceptEntryAsTreeNodeDto Map(FinancialConceptEntryAsTreeNode node) {
      return new FinancialConceptEntryAsTreeNodeDto {
        ItemCode = node.IntegrationEntry.Code,
        ItemName = node.IntegrationEntry.Name,
        Operator = Convert.ToString((char) node.IntegrationEntry.Operator),
        ParentCode = node.ParentNode.IntegrationEntry.IsEmptyInstance ?
                                            string.Empty: node.ParentNode.IntegrationEntry.Code,
        Qualification = node.IntegrationEntry.Qualification,
        SubledgerAccount = node.IntegrationEntry.SubledgerAccountNumber,
        SubledgerAccountName = node.IntegrationEntry.SubledgerAccountName,
        SectorCode = node.IntegrationEntry.SectorCode,
        CurrencyCode = node.IntegrationEntry.CurrencyCode,
        UID = node.IntegrationEntry.UID,
        Type = node.IntegrationEntry.Type,
        Level = node.Level
      };
    }

  }  // class FinancialConceptsTreeMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
