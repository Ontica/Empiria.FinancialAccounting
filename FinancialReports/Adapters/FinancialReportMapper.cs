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
      switch (command.FinancialReportType) {

        case FinancialReportType.R01:
          return MapToR01(list);

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled financial report type {command.FinancialReportType}.");
      }
    }


    static private FixedList<FinancialReportEntryDto> MapToR01(FixedList<FinancialReportEntry> list) {
      var mappedItems = list.Select((x) => MapToR01(x));

      return new FixedList<FinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportEntryDto MapToR01(FinancialReportEntry entry) {
      return new FinancialReportEntryDto {
         ConceptCode = entry.GroupingRule.Code,
         Concept = entry.GroupingRule.Concept,
         DomesticCurrencyTotal = 1000,
         ForeignCurrencyTotal = 2500,
         Total = 3500,
         GroupingRuleUID = entry.GroupingRule.UID,
      };
    }

    #endregion Helpers

  } // class FinancialReportMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
