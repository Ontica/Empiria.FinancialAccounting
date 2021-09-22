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
        Entries = Map(financialReport.Command, financialReport.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<FinancialReportEntryDto> Map(FinancialReportCommand command,
                                                          FixedList<FinancialReportEntry> list) {
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


    static private FixedList<FinancialReportEntryDto> MapToFixedRowsReport(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReport(x));

      return new FixedList<FinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportEntryDto MapToFixedRowsReport(FinancialReportEntry entry) {
      return new FinancialReportEntryDto {
         UID = entry.Row.UID,
         ConceptCode = entry.GroupingRule.Code,
         Concept = entry.GroupingRule.Concept,
         DomesticCurrencyTotal = entry.DomesticCurrencyTotal,
         ForeignCurrencyTotal = entry.ForeignCurrencyTotal,
         Total = entry.Total,
         GroupingRuleUID = entry.GroupingRule.UID,
      };
    }

    static private FixedList<FinancialReportEntryDto> MapToFixedRowsReportConceptsIntegration(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToFixedRowsReportConceptsIntegration(x));

      return new FixedList<FinancialReportEntryDto>(mappedItems);
    }

    static private FinancialReportEntryDto MapToFixedRowsReportConceptsIntegration(FinancialReportEntry entry) {
      return new FinancialReportEntryDto {
        UID = entry.Row.UID,
        ConceptCode = entry.GroupingRule.Code,
        Concept = entry.GroupingRule.Concept,
        DomesticCurrencyTotal = entry.DomesticCurrencyTotal,
        ForeignCurrencyTotal = entry.ForeignCurrencyTotal,
        Total = entry.Total,
        GroupingRuleUID = entry.GroupingRule.UID,
      };
    }

    #endregion Helpers

  } // class FinancialReportMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
