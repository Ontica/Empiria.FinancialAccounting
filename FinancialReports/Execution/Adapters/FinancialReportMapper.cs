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

    internal static FinancialReportDto Map(FinancialReport financialReport) {
      return new FinancialReportDto {
        Command = financialReport.Command,
        Columns = financialReport.DataColumns(),
        Entries = MapEntries(financialReport.Command, financialReport.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<DynamicFinancialReportEntryDto> MapEntries(FinancialReportCommand command,
                                                                        FixedList<FixedRowFinancialReportEntry> list) {
      FinancialReportType reportType = command.GetFinancialReportType();

      switch (reportType.DesignType) {

        case FinancialReportDesignType.FixedRows:
          return MapToFixedRowsReport(list);

        case FinancialReportDesignType.ConceptsIntegration:
          return MapToFixedRowsReportConceptsIntegration(list);

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled financial report type {command.FinancialReportType}.");
      }
    }


    static private FixedList<DynamicFinancialReportEntryDto> MapToFixedRowsReport(FixedList<FixedRowFinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReport(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportEntryDto MapToFixedRowsReport(FixedRowFinancialReportEntry entry) {
      dynamic o = new FinancialReportEntryDto {
         UID = entry.Row.UID,
         ConceptCode = entry.GroupingRule.Code,
         Concept = entry.GroupingRule.Concept,
         GroupingRuleUID = entry.GroupingRule.UID,
         AccountsChartName = entry.GroupingRule.RulesSet.AccountsChart.Name,
         RulesSetName = entry.GroupingRule.RulesSet.Name,
      };

      o.SetTotalField(FinancialReportTotalField.DomesticCurrencyTotal, entry.DomesticCurrencyTotal);
      o.SetTotalField(FinancialReportTotalField.ForeignCurrencyTotal, entry.ForeignCurrencyTotal);
      o.SetTotalField(FinancialReportTotalField.Total, entry.Total);

      return o;
    }

    static private FixedList<DynamicFinancialReportEntryDto> MapToFixedRowsReportConceptsIntegration(FixedList<FixedRowFinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReportConceptsIntegration(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }

    static private FinancialReportEntryDto MapToFixedRowsReportConceptsIntegration(FixedRowFinancialReportEntry entry) {
      dynamic o = new FinancialReportEntryDto {
        UID = entry.Row.UID,
        ConceptCode = entry.GroupingRule.Code,
        Concept = entry.GroupingRule.Concept,
        GroupingRuleUID = entry.GroupingRule.UID,
        AccountsChartName = entry.GroupingRule.RulesSet.AccountsChart.Name,
        RulesSetName = entry.GroupingRule.RulesSet.Name
      };

      o.SetTotalField(FinancialReportTotalField.DomesticCurrencyTotal, entry.DomesticCurrencyTotal);
      o.SetTotalField(FinancialReportTotalField.ForeignCurrencyTotal, entry.ForeignCurrencyTotal);
      o.SetTotalField(FinancialReportTotalField.Total, entry.Total);

      return o;
    }

    #endregion Helpers

  } // class FinancialReportMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
