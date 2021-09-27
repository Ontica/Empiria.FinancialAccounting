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

    static internal FinancialReportDto Map(FinancialReportBreakdown financialReportBreakdown) {
      return new FinancialReportDto {
        Command = financialReportBreakdown.Command,
        Columns = financialReportBreakdown.DataColumns(),
        Entries = MapBreakdownEntries(financialReportBreakdown.Entries)
      };
    }


    #endregion Public mappers

    #region Helpers

    static private FixedList<DynamicFinancialReportEntryDto> MapBreakdownEntries(FixedList<FinancialReportBreakdownEntry> list) {
      var mappedItems = list.Select((x) => MapBreakdownEntry(x));

      return new FixedList<DynamicFinancialReportEntryDto>(mappedItems);
    }


    static private FinancialReportBreakdownEntryDto MapBreakdownEntry(FinancialReportBreakdownEntry entry) {
      dynamic o = new FinancialReportBreakdownEntryDto {
        UID = entry.GroupingRuleItem.UID,
        ItemCode = entry.GroupingRuleItem.Code,
        ItemName = entry.GroupingRuleItem.Name,
        SubledgerAccount = entry.GroupingRuleItem.SubledgerAccountNumber,
        SectorCode = entry.GroupingRuleItem.SectorCode,
        Operator = Convert.ToString((char) entry.GroupingRuleItem.Operator),
        GroupingRuleUID = entry.GroupingRuleItem.GroupingRule.UID,
      };

      o.SetTotalField(FinancialReportTotalField.DomesticCurrencyTotal, entry.DomesticCurrencyTotal);
      o.SetTotalField(FinancialReportTotalField.ForeignCurrencyTotal, entry.ForeignCurrencyTotal);
      o.SetTotalField(FinancialReportTotalField.Total, entry.Total);

      return o;
    }

    #endregion Helpers

  } // class FinancialReportBreakdownMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
