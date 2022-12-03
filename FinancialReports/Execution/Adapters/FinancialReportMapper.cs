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
      if (entry is FinancialReportBreakdownEntry breakdownEntry) {
        return MapBreakdownEntry(breakdownEntry);

      } else if (entry is FinancialReportEntryResult totalEntry) {
        return MapBreakdownTotalEntry(totalEntry);

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    static private DynamicFinancialReportEntryDto MapBreakdownEntry(FinancialReportBreakdownEntry entry) {
      dynamic o = new FinancialReportBreakdownEntryDto {
        UID = entry.IntegrationEntry.UID,
        ItemType = FinancialReportItemType.Entry,
        ItemCode = entry.IntegrationEntry.Code,
        ItemName = entry.IntegrationEntry.Name,
        SubledgerAccount = entry.IntegrationEntry.SubledgerAccountNumber,
        SectorCode = entry.IntegrationEntry.SectorCode,
        Operator = Convert.ToString((char) entry.IntegrationEntry.Operator),
        FinancialConceptUID = entry.IntegrationEntry.FinancialConcept.UID,
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


    static private DynamicFinancialReportEntryDto MapFixedRowIntegrationBreakdownEntry(FinancialReportBreakdownEntry entry) {
      dynamic o = new IntegrationReportEntryDto {
        UID = entry.IntegrationEntry.UID,
        Type = entry.IntegrationEntry.Type,
        FinancialConceptUID = entry.IntegrationEntry.FinancialConcept.UID,
        ConceptCode = entry.IntegrationEntry.FinancialConcept.Code,
        Concept = entry.IntegrationEntry.FinancialConcept.Name,
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

      switch (reportType.DesignType) {

        case FinancialReportDesignType.FixedRows:
          return MapToFixedRowsReport(financialReport.Entries);

        case FinancialReportDesignType.AccountsIntegration:
          return MapToFixedRowsReportConceptsIntegration(financialReport.Entries);

        default:
          throw Assertion.EnsureNoReachThisCode(
                $"Unhandled financial report type '{financialReport.BuildQuery.FinancialReportType}'.");
      }
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapToFixedRowsReport(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReport((FinancialReportEntryResult) x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportEntryDto MapToFixedRowsReport(FinancialReportEntryResult entry) {
      bool hasFinancialConcept = !entry.FinancialConcept.IsEmptyInstance;

      dynamic o;

      if (hasFinancialConcept) {
        o = new FinancialReportEntryDto {
          UID = entry.UID,
          ConceptCode = entry.FinancialConcept.Code,
          Concept = entry.FinancialConcept.Name,
          Level = entry.FinancialConcept.Level,
          FinancialConceptUID = entry.FinancialConcept.UID,
          AccountsChartName = entry.FinancialConcept.Group.AccountsChart.Name,
          GroupName = entry.FinancialConcept.Group.Name
        };
      } else {
        o = new FinancialReportEntryDto {
          UID = entry.UID,
          ConceptCode = string.Empty,
          Concept = entry.Label,
          Level = 1,
          FinancialConceptUID = string.Empty,
          AccountsChartName = string.Empty,
          GroupName = String.Empty
        };
      }

      SetTotalsFields(o, entry);

      return o;
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapToFixedRowsReportConceptsIntegration(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReportConceptsIntegration(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private DynamicFinancialReportEntryDto MapToFixedRowsReportConceptsIntegration(FinancialReportEntry entry) {
      if (entry is FinancialReportBreakdownEntry breakdownEntry) {
        return MapFixedRowIntegrationBreakdownEntry(breakdownEntry);

      } else if (entry is FinancialReportEntryResult integrationTotalEntry) {
        return MapFixedRowIntegrationTotalEntry(integrationTotalEntry);

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
    }


    static private DynamicFinancialReportEntryDto MapFixedRowIntegrationTotalEntry(FinancialReportEntryResult entry) {
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

    static private void SetTotalsFields(DynamicFinancialReportEntryDto o, FinancialReportEntry entry) {
      var totalsColumns = entry.GetDynamicMemberNames();

      foreach (string column in totalsColumns) {
        o.SetTotalField(column, entry.GetTotalField(column));
      }
    }

    #endregion Helpers

  } // class FinancialReportMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
