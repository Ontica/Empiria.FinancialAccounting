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
  static internal class FinancialConceptMapper {

    static internal FixedList<FinancialConceptDto> Map(FixedList<FinancialConcept> list) {
      return new FixedList<FinancialConceptDto>(list.Select(financialConcept => Map(financialConcept)));
    }


    static internal FixedList<FinancialConceptEntryDto> Map(FixedList<FinancialConceptEntry> integration) {
      return new FixedList<FinancialConceptEntryDto>(integration.Select(entry => Map(entry)));
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


    static private FinancialConceptEntryDto Map(FinancialConceptEntry integrationEntry) {
      return new FinancialConceptEntryDto {
        UID = integrationEntry.UID,
        Type = integrationEntry.Type,
        ItemCode = integrationEntry.Code,
        ItemName = integrationEntry.Name,
        SubledgerAccount = integrationEntry.SubledgerAccountNumber,
        SectorCode = integrationEntry.SectorCode,
        Operator = Convert.ToString((char) integrationEntry.Operator)
      };
    }

  }  // class FinancialConceptMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
