/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Mapper class                            *
*  Type     : FinancialReportBreakdownMapper             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map financial reports breakdown data.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Methods used to map financial reports breakdown data.</summary>
  static internal class FinancialReportBreakdownMapper {

    #region Public mappers

    static internal FinancialReportBreakdownDto Map(FinancialReportBreakdown financialReportBreakdown) {
      return new FinancialReportBreakdownDto {
        Command = financialReportBreakdown.Command,
        Columns = financialReportBreakdown.DataColumns(),
        Entries = MapBreakdownEntries(financialReportBreakdown.Entries)
      };
    }


    #endregion Public mappers

    #region Helpers

    static private FixedList<FinancialReportBreakdownEntryDto> MapBreakdownEntries(FixedList<FinancialReportBreakdownEntry> list) {
      var mappedItems = list.Select((x) => MapBreakdownEntry(x));

      return new FixedList<FinancialReportBreakdownEntryDto>(mappedItems);
    }


    static private FinancialReportBreakdownEntryDto MapBreakdownEntry(FinancialReportBreakdownEntry entry) {
      return new FinancialReportBreakdownEntryDto {
        UID = entry.GroupingRuleItem.UID,
        ItemCode = entry.GroupingRuleItem.Code,
        ItemName = entry.GroupingRuleItem.Name,
        SubledgerAccount = entry.GroupingRuleItem.SubledgerAccountNumber,
        SectorCode = entry.GroupingRuleItem.SectorCode,
        Operator = Convert.ToString((char) entry.GroupingRuleItem.Operator),
        DomesticCurrencyTotal = entry.DomesticCurrencyTotal,
        ForeignCurrencyTotal = entry.ForeignCurrencyTotal,
        Total = entry.Total,
        GroupingRuleUID = entry.GroupingRuleItem.GroupingRule.UID,
      };
    }


    #endregion Helpers

  } // class FinancialReportBreakdownMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
