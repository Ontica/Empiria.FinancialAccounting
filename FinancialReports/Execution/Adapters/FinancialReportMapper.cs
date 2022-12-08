/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Mapper class                            *
*  Type     : FinancialReportMapper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map financial reports data.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Methods used to map financial reports data.</summary>
  static internal class FinancialReportMapper {

    #region Public mappers

    static internal FixedList<FinancialReportTypeDto> Map(FixedList<FinancialReportType> list) {
      return list.Select((x) => Map(x))
                 .ToFixedList();
    }


    static internal FinancialReportDto Map(FinancialReport financialReport) {
      return new FinancialReportDto {
        Query = financialReport.BuildQuery,
        Columns = financialReport.FinancialReportType.OutputDataColumns,
        Entries = MapEntries(financialReport)
      };
    }


    static internal FinancialReportDto MapBreakdown(FinancialReport breakdownReport) {
      return new FinancialReportDto {
        Query = breakdownReport.BuildQuery,
        Columns = breakdownReport.FinancialReportType.BreakdownColumns,
        Entries = MapBreakdownEntries(breakdownReport.Entries)
      };
    }

    #endregion Public mappers

    #region Private mappers

    static private FinancialReportTypeDto Map(FinancialReportType reportType) {
      return new FinancialReportTypeDto() {
        UID = reportType.UID,
        Name = reportType.Name,
        ExportTo = ExportToMapper.Map(reportType.ExportTo)
      };
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapBreakdownEntries(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapBreakdownEntry(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private DynamicFinancialReportEntryDto MapBreakdownEntry(FinancialReportEntry entry) {
      if (entry is FinancialReportBreakdownResult breakdownEntry) {
        return MapBreakdownEntry(breakdownEntry);

      } else if (entry is FinancialReportEntryResult totalEntry) {
        return MapBreakdownTotalEntry(totalEntry);

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    static private DynamicFinancialReportEntryDto MapBreakdownEntry(FinancialReportBreakdownResult entry) {
      dynamic o = new FinancialReportBreakdownEntryDto {
        UID = entry.IntegrationEntry.UID,
        ItemType = FinancialReportItemType.Entry,
        ItemCode = entry.IntegrationEntry.Code,
        ItemName = entry.IntegrationEntry.Name,
        SubledgerAccount = entry.IntegrationEntry.SubledgerAccountNumber,
        SectorCode = entry.IntegrationEntry.SectorCode,
        Operator = Convert.ToString((char) entry.IntegrationEntry.Operator),
        FinancialConceptUID = entry.FinancialConcept.UID,
      };

      SetTotalsFields(o, entry);

      return o;
    }


    static private DynamicFinancialReportEntryDto MapBreakdownTotalEntry(FinancialReportEntryResult entry) {
      dynamic o = new FinancialReportBreakdownEntryDto {
        UID = entry.UID,
        Type = FinancialConceptEntryType.FinancialConceptReference,
        FinancialConceptUID = entry.FinancialConcept.UID,
        ItemType = FinancialReportItemType.Summary,
        ItemCode = "TOTAL",
        ItemName = string.Empty,
        SubledgerAccount = string.Empty,
        SectorCode = string.Empty,
        Operator = string.Empty
      };

      SetTotalsFields(o, entry);

      return o;
    }


    static private DynamicFinancialReportEntryDto MapIntegrationBreakdownEntry(FinancialReportBreakdownResult entry) {
      dynamic o = new IntegrationReportEntryDto {
        UID = entry.IntegrationEntry.UID,
        Type = entry.IntegrationEntry.Type,
        FinancialConceptUID = entry.FinancialConcept.UID,
        ConceptCode = entry.FinancialConcept.Code,
        Concept = entry.FinancialConcept.Name,
        ItemType = FinancialReportItemType.Entry,
        ItemCode = entry.IntegrationEntry.Code,
        ItemName = entry.IntegrationEntry.Name,
        SubledgerAccount = entry.IntegrationEntry.SubledgerAccountNumber,
        SectorCode = entry.IntegrationEntry.SectorCode,
        Operator = Convert.ToString((char) entry.IntegrationEntry.Operator)
      };

      SetTotalsFields(o, entry);

      return o;
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapEntries(FinancialReport financialReport) {
      FinancialReportType reportType = financialReport.BuildQuery.GetFinancialReportType();

      if (reportType.DesignType != FinancialReportDesignType.AccountsIntegration) {
        return MapReportEntries(financialReport.Entries);
      } else {
        return MapToReportConceptsIntegrationEntries(financialReport.Entries);
      }
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapReportEntries(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapReportEntry((FinancialReportEntryResult) x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportEntryDto MapReportEntry(FinancialReportEntryResult entry) {
      bool hasFinancialConcept = !entry.FinancialConcept.IsEmptyInstance;

      dynamic o;

      if (hasFinancialConcept) {
        o = new FinancialReportEntryDto {
          UID = entry.UID,
          ReportEntryType = GetReportEntryType(entry.Definition),
          ConceptCode = entry.FinancialConcept.Code,
          Concept = entry.FinancialConcept.Name,
          Level = entry.FinancialConcept.Level,
          FinancialConceptUID = entry.FinancialConcept.UID,
          AccountsChartName = entry.FinancialConcept.Group.AccountsChart.Name,
          GroupName = entry.FinancialConcept.Group.Name
        };

        SetTotalsFields(o, entry);

      } else {
        o = new FinancialReportEntryDto {
          UID = entry.UID,
          ReportEntryType = GetReportEntryType(entry.Definition),
          ConceptCode = string.Empty,
          Concept = entry.Label,
          Level = 1,
          FinancialConceptUID = string.Empty,
          AccountsChartName = string.Empty,
          GroupName = String.Empty
        };
      }

      return o;
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapToReportConceptsIntegrationEntries(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToReportConceptsIntegrationEntry(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private DynamicFinancialReportEntryDto MapToReportConceptsIntegrationEntry(FinancialReportEntry entry) {
      if (entry is FinancialReportBreakdownResult breakdownEntry) {
        return MapIntegrationBreakdownEntry(breakdownEntry);

      } else if (entry is FinancialReportEntryResult integrationTotalEntry) {
        return MapIntegrationTotalEntry(integrationTotalEntry);

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unhandled integration entry type '{entry.GetType().Name}'.");

      }
    }


    static private DynamicFinancialReportEntryDto MapIntegrationTotalEntry(FinancialReportEntryResult entry) {
      dynamic o = new IntegrationReportEntryDto {
        UID = entry.UID,
        Type = FinancialConceptEntryType.FinancialConceptReference,
        FinancialConceptUID = entry.FinancialConcept.UID,
        ConceptCode = entry.FinancialConcept.Code,
        Concept = entry.FinancialConcept.Name,
        ItemType = FinancialReportItemType.Summary,
        ItemCode = "TOTAL",
        ItemName = string.Empty,
        SubledgerAccount = string.Empty,
        SectorCode = string.Empty,
        Operator = string.Empty
      };

      SetTotalsFields(o, entry);

      return o;
    }


    #endregion Private mappers

    #region Helpers


    static private FinancialReportEntryType GetReportEntryType(FinancialReportItemDefinition reportItem) {
      if (reportItem is FinancialReportRow) {
        return FinancialReportEntryType.Row;

      } else if (reportItem is FinancialReportCell) {
        return FinancialReportEntryType.Cell;

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unhandled reportItem entry type '{reportItem.GetType().Name}'.");
      }
    }


    static private void SetTotalsFields(DynamicFinancialReportEntryDto o, FinancialReportEntry entry) {
      var totalsColumns = entry.GetDynamicMemberNames();

      foreach (string column in totalsColumns) {
        o.SetTotalField(column, entry.GetTotalField(column));
      }
    }

    #endregion Helpers

  } // class FinancialReportMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
