﻿/* Empiria Financial *****************************************************************************************
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

    internal static FixedList<FinancialConceptsGroupDto> Map(FixedList<FinancialConceptGroup> list) {
      return list.Select(group => Map(group))
                 .ToFixedList();
    }


    static internal FinancialConceptDto Map(FinancialConcept concept) {
      return new FinancialConceptDto {
        UID = concept.UID,
        Code = concept.Code,
        Name = concept.Name,
        Position = concept.Position,
        Level = concept.Level,
        StartDate = concept.StartDate,
        EndDate = concept.EndDate,
        Group = concept.Group.MapToNamedEntity(),
        AccountsChart = concept.Group.AccountsChart.MapToNamedEntity(),
        Integration = Map(concept.Integration),
      };
    }


    static private FinancialConceptsGroupDto Map(FinancialConceptGroup group) {
      return new FinancialConceptsGroupDto {
        UID = group.UID,
        Name = group.Name,
        AccountsChart = group.AccountsChart.MapToNamedEntity(),
        StartDate = group.StartDate,
        EndDate = group.EndDate,
        CalculationRules = group.CalculationRules,
        DataColumns = group.DataColumns,
        ExternalVariablesSets = group.ExternalVariablesSets.MapToNamedEntityList()
      };
    }

    static internal FixedList<FinancialConceptDescriptorDto> Map(FixedList<FinancialConcept> list) {
      return list.Select(financialConcept => MapToDescriptor(financialConcept))
                 .ToFixedList();
    }


    static internal FixedList<FinancialConceptEntryDto> Map(FixedList<FinancialConceptEntry> integration) {
      return integration.Select(entry => Map(entry))
                        .ToFixedList();
    }


    static internal FinancialConceptEntryDto Map(FinancialConceptEntry integrationEntry) {
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


    static private FinancialConceptDescriptorDto MapToDescriptor(FinancialConcept concept) {
      return new FinancialConceptDescriptorDto {
        UID = concept.UID,
        Code = concept.Code,
        Name = concept.Name,
        Position = concept.Position,
        Level = concept.Level,
        AccountsChartName = concept.Group.AccountsChart.Name,
        GroupName = concept.Group.Name
      };
    }


  }  // class FinancialConceptMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters