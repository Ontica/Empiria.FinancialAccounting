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

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Methods used to map financial reports data.</summary>
  static internal class FinancialReportMapper {

    #region Public mappers

    static internal FixedList<FinancialReportTypeDto> Map(FixedList<FinancialReportType> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<FinancialReportTypeDto>(mappedItems);
    }


    static internal FinancialReportDto Map(FinancialReport financialReport) {
      return new FinancialReportDto {
        Command = financialReport.Command,
        Columns = financialReport.FinancialReportType.DataColumns,
        Entries = MapEntries(financialReport)
      };
    }


    static internal FinancialReportDto MapBreakdown(FinancialReport breakdownReport) {
      return new FinancialReportDto {
        Command = breakdownReport.Command,
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
        ExportTo = new FixedList<ExportToDto>(ExportToMapper.Map(reportType.ExportTo))
      };
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapBreakdownEntries(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapBreakdownEntry(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private DynamicFinancialReportEntryDto MapBreakdownEntry(FinancialReportEntry entry) {
      if (entry is FinancialReportBreakdownEntry entry1) {
        return MapBreakdownEntry(entry1);

      } else if (entry is FixedRowFinancialReportEntry entry2) {
        return MapBreakdownEntry(entry2);

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    static private DynamicFinancialReportEntryDto MapBreakdownEntry(FinancialReportBreakdownEntry entry) {
      dynamic o = new FinancialReportBreakdownEntryDto {
        UID = entry.GroupingRuleItem.UID,
        ItemType = FinancialReportItemType.Entry,
        ItemCode = entry.GroupingRuleItem.Code,
        ItemName = entry.GroupingRuleItem.Name,
        SubledgerAccount = entry.GroupingRuleItem.SubledgerAccountNumber,
        SectorCode = entry.GroupingRuleItem.SectorCode,
        Operator = Convert.ToString((char) entry.GroupingRuleItem.Operator),
        GroupingRuleUID = entry.GroupingRuleItem.GroupingRule.UID,
      };

      SetTotalsFields(o, entry);

      return o;
    }

    static private DynamicFinancialReportEntryDto MapBreakdownEntry(FixedRowFinancialReportEntry entry) {
      dynamic o = new FinancialReportBreakdownEntryDto {
        UID = entry.Row.UID,
        Type = Rules.GroupingRuleItemType.Agrupation,
        GroupingRuleUID = entry.GroupingRule.UID,
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


    static private FixedList<DynamicFinancialReportEntryDto> MapEntries(FinancialReport financialReport) {
      FinancialReportType reportType = financialReport.Command.GetFinancialReportType();

      switch (reportType.DesignType) {

        case FinancialReportDesignType.FixedRows:
          return MapToFixedRowsReport(financialReport.Entries);

        case FinancialReportDesignType.AccountsIntegration:
          return MapToFixedRowsReportConceptsIntegration(financialReport.Entries);

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled financial report type {financialReport.Command.FinancialReportType}.");
      }
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapToFixedRowsReport(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReport((FixedRowFinancialReportEntry) x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportEntryDto MapToFixedRowsReport(FixedRowFinancialReportEntry entry) {
      bool hasGroupingRule = !entry.GroupingRule.IsEmptyInstance;

      dynamic o;

      if (hasGroupingRule) {
        o = new FinancialReportEntryDto {
          UID = entry.Row.UID,
          ConceptCode = entry.GroupingRule.Code,
          Concept = entry.GroupingRule.Concept,
          Level = entry.GroupingRule.Level,
          GroupingRuleUID = entry.GroupingRule.UID,
          AccountsChartName = entry.GroupingRule.RulesSet.AccountsChart.Name,
          RulesSetName = entry.GroupingRule.RulesSet.Name
        };
      } else {
        o = new FinancialReportEntryDto {
          UID = entry.Row.UID,
          ConceptCode = entry.Row.Code,
          Concept = entry.Row.Label,
          Level = 1,
          GroupingRuleUID = string.Empty,
          AccountsChartName = string.Empty,
          RulesSetName = String.Empty
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
      if (entry is FinancialReportBreakdownEntry entry1) {
        return MapIntegrationEntry(entry1);

      } else if (entry is FixedRowFinancialReportEntry entry2) {
        return MapIntegrationEntry(entry2);

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    static private DynamicFinancialReportEntryDto MapIntegrationEntry(FixedRowFinancialReportEntry entry) {
      dynamic o = new IntegrationReportEntryDto {
        UID = entry.Row.UID,
        Type = Rules.GroupingRuleItemType.Agrupation,
        GroupingRuleUID = entry.GroupingRule.UID,
        ConceptCode = entry.GroupingRule.Code,
        Concept = entry.GroupingRule.Concept,
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


    static private DynamicFinancialReportEntryDto MapIntegrationEntry(FinancialReportBreakdownEntry entry) {
      dynamic o = new IntegrationReportEntryDto {
        UID = entry.GroupingRuleItem.UID,
        Type = entry.GroupingRuleItem.Type,
        GroupingRuleUID = entry.GroupingRuleItem.GroupingRule.UID,
        ConceptCode = entry.GroupingRuleItem.GroupingRule.Code,
        Concept = entry.GroupingRuleItem.GroupingRule.Concept,
        ItemType = FinancialReportItemType.Entry,
        ItemCode = entry.GroupingRuleItem.Code,
        ItemName = entry.GroupingRuleItem.Name,
        SubledgerAccount = entry.GroupingRuleItem.SubledgerAccountNumber,
        SectorCode = entry.GroupingRuleItem.SectorCode,
        Operator = Convert.ToString((char) entry.GroupingRuleItem.Operator)
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
