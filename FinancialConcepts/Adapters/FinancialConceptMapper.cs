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

    internal static FixedList<FinancialConceptsGroupDto> Map(FixedList<FinancialConceptGroup> list) {
      return list.Select(group => Map(group))
                 .ToFixedList();
    }


    static internal FinancialConceptDto Map(FinancialConcept concept, bool withIntegration = true) {
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
        Integration = withIntegration ? Map(concept.Integration) : null,
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

    static internal FinancialConceptEntryDto Map(FinancialConceptEntry entry) {
      var dto = new FinancialConceptEntryDto {
        UID = entry.UID,
        Type = entry.Type,
        CalculationRule = entry.CalculationRule,
        DataColumn = entry.DataColumn,
        Operator = entry.Operator,
        Positioning = new ItemPositioning {
          Rule = PositioningRule.ByPositionValue,
          Position = entry.Position,
          OffsetUID = null
        },
      };

      if (entry.Type == FinancialConceptEntryType.Account) {

        dto.AccountNumber           = entry.AccountNumber;
        dto.SubledgerAccountNumber  = entry.SubledgerAccountNumber;
        dto.SectorCode              = entry.SectorCode;
        dto.CurrencyCode            = entry.CurrencyCode;

      } else if (entry.Type == FinancialConceptEntryType.FinancialConceptReference) {
        dto.ReferencedFinancialConcept = FinancialConceptMapper.Map(entry.ReferencedFinancialConcept, false);

      } else if (entry.Type == FinancialConceptEntryType.ExternalVariable) {
        dto.ExternalVariableCode = entry.ExternalVariableCode;
      }

      return dto;
    }


    static internal FixedList<FinancialConceptEntryDescriptorDto> Map(FixedList<FinancialConceptEntry> integration) {
      return integration.Select(entry => MapToDescriptor(entry))
                        .ToFixedList();
    }


    static internal FinancialConceptEntryDescriptorDto MapToDescriptor(FinancialConceptEntry entry) {
      return new FinancialConceptEntryDescriptorDto {
        UID               = entry.UID,
        Type              = entry.Type,
        ItemCode          = entry.Code,
        ItemName          = entry.Name,
        SubledgerAccount  = entry.SubledgerAccountNumber,
        SectorCode        = entry.SectorCode,
        Operator          = Convert.ToString((char) entry.Operator)
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
