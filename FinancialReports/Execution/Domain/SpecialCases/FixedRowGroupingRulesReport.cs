/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FixedRowGroupingRulesReport                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates a fixed row defined report which rows are linked to grouping rules (R01, R10, R12).  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a fixed row defined report which rows are linked to
  /// grouping rules (R01, R10, R12).</summary>
  internal class FixedRowGroupingRulesReport {

    private readonly FinancialReportCommand _command;

    #region Public methods

    internal FixedRowGroupingRulesReport(FinancialReportCommand command) {
      _command = command;
    }


    internal FinancialReport Generate() {
      FixedList<FinancialReportRow> fixedRows = GetReportFixedRows();

      FixedList<FixedRowFinancialReportEntry> reportEntries = CreateReportEntriesWithoutTotals(fixedRows);

      var balancesProvider = new AccountBalancesProvider(_command);

      EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> balances = balancesProvider.GetBalancesAsHashTable();

      ProcessEntries(reportEntries, balances);

      var convertedEntries = new FixedList<FinancialReportEntry>(reportEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_command, convertedEntries);
    }


    internal FinancialReport GetBreakdown(string reportRowUID) {
      FinancialReportRow row = GetReportBreakdownRow(reportRowUID);

      FixedRowFinancialReportEntry reportEntry = CreateReportEntryWithoutTotals(row);

      var balancesProvider = new AccountBalancesProvider(_command);

      EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> balances = balancesProvider.GetBalancesAsHashTable();

      FixedList<FinancialReportBreakdownEntry> breakdownEntries = GetBreakdownEntries(reportEntry);

      ProcessBreakdown(breakdownEntries, balances);

      // Add breakdown total row

      var convertedEntries = new FixedList<FinancialReportEntry>(breakdownEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_command, convertedEntries);
    }


    #endregion Public methods


    #region Private methods


    private ReportEntryTotals ProcessAccount(GroupingRuleItem groupingRule,
                                             FixedList<ITrialBalanceEntryDto> balances) {
      FixedList<ITrialBalanceEntryDto> filtered;

      if (groupingRule.HasSector && groupingRule.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == groupingRule.SectorCode &&
                                         x.SubledgerAccountNumber == groupingRule.SubledgerAccountNumber);

      } else if (groupingRule.HasSector && !groupingRule.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == groupingRule.SectorCode &&
                                         x.SubledgerAccountNumber.Length == 0);

      } else if (!groupingRule.HasSector && groupingRule.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber == groupingRule.SubledgerAccountNumber);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber == groupingRule.SubledgerAccountNumber);
        }
      } else {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber.Length == 0);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber.Length == 0);
        }
      }

      var totals = new ReportEntryTotals();

      foreach (var balance in filtered) {
        totals = totals.Sum(balance, groupingRule.Qualification);
      }

      totals.Round();

      return totals;
    }


    private void ProcessBreakdown(FixedList<FinancialReportBreakdownEntry> breakdown,
                                  EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> balances) {
      foreach (var breakdownItem in breakdown) {

        ReportEntryTotals groupingRuleTotals;

        if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.Agrupation) {

          groupingRuleTotals = ProcessGroupingRule(breakdownItem.GroupingRuleItem.Reference,
                                                   balances);

        } else if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.Account &&
                   balances.ContainsKey(breakdownItem.GroupingRuleItem.AccountNumber)) {

          groupingRuleTotals = ProcessAccount(breakdownItem.GroupingRuleItem,
                                              balances[breakdownItem.GroupingRuleItem.AccountNumber]);

        } else if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.Account) {

          groupingRuleTotals = new ReportEntryTotals();

        } else if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.FixedValue) {

          groupingRuleTotals = GetFixedValue(breakdownItem.GroupingRuleItem);

        } else {
          throw Assertion.AssertNoReachThisCode();
        }

        groupingRuleTotals.Round();

        SetTotalsFields(breakdownItem, groupingRuleTotals);
      }
    }


    private ReportEntryTotals GetFixedValue(GroupingRuleItem groupingRuleItem) {
      ExternalValue value = ExternalValue.GetValue(groupingRuleItem.ExternalVariableCode,
                                                   _command.Date);

      var groupingRuleTotals = new ReportEntryTotals();

      if (groupingRuleItem.Qualification == "MonedaNacional") {

        groupingRuleTotals.DomesticCurrencyTotal = value.DomesticCurrencyValue + value.ForeignCurrencyValue;

      } else if (groupingRuleItem.Qualification == "MonedaExtranjera") {

        groupingRuleTotals.ForeignCurrencyTotal = value.DomesticCurrencyValue + value.ForeignCurrencyValue;

      } else {

        groupingRuleTotals.DomesticCurrencyTotal = value.DomesticCurrencyValue;
        groupingRuleTotals.ForeignCurrencyTotal = value.ForeignCurrencyValue;
      }

      return groupingRuleTotals;
    }


    private void ProcessEntries(FixedList<FixedRowFinancialReportEntry> reportEntries,
                                EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> balances) {

      foreach (var reportEntry in reportEntries) {
        ReportEntryTotals groupingRuleTotals = ProcessGroupingRule(reportEntry.GroupingRule, balances);

        groupingRuleTotals.Round();

        SetTotalsFields(reportEntry, groupingRuleTotals);
      }
    }




    private ReportEntryTotals ProcessGroupingRule(GroupingRule groupingRule,
                                                  EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> balances) {
      var totals = new ReportEntryTotals();

      foreach (var groupingRuleItem in groupingRule.Items) {
        if (groupingRuleItem.Type == GroupingRuleItemType.Agrupation &&
            groupingRuleItem.Operator == OperatorType.Add) {

          totals = totals.Sum(ProcessGroupingRule(groupingRuleItem.Reference, balances),
                              groupingRuleItem.Qualification);

        } else if (groupingRuleItem.Type == GroupingRuleItemType.Agrupation &&
                   groupingRuleItem.Operator == OperatorType.Substract) {

          totals = totals.Substract(ProcessGroupingRule(groupingRuleItem.Reference, balances),
                                    groupingRuleItem.Qualification);

        } else if (groupingRuleItem.Type == GroupingRuleItemType.Account &&
                   balances.ContainsKey(groupingRuleItem.AccountNumber) &&
                   groupingRuleItem.Operator == OperatorType.Add) {

          totals = totals.Sum(ProcessAccount(groupingRuleItem, balances[groupingRuleItem.AccountNumber]),
                              groupingRuleItem.Qualification);

        } else if (groupingRuleItem.Type == GroupingRuleItemType.Account &&
                   balances.ContainsKey(groupingRuleItem.AccountNumber) &&
                   groupingRuleItem.Operator == OperatorType.Substract) {

          totals = totals.Substract(ProcessAccount(groupingRuleItem, balances[groupingRuleItem.AccountNumber]),
                                    groupingRuleItem.Qualification);

        } else if (groupingRuleItem.Type == GroupingRuleItemType.FixedValue &&
                   groupingRuleItem.Operator == OperatorType.Add) {

          totals = totals.Sum(GetFixedValue(groupingRuleItem), groupingRuleItem.Qualification);

        }
      }

      return totals;
    }


    #endregion Private methods

    #region Helpers

    private FixedList<FinancialReportBreakdownEntry> GetBreakdownEntries(FixedRowFinancialReportEntry reportEntry) {
      var breakdown = new List<FinancialReportBreakdownEntry>();

      var groupingRule = reportEntry.GroupingRule;

      foreach (var item in groupingRule.Items) {
        breakdown.Add(new FinancialReportBreakdownEntry { GroupingRuleItem = item });
      }

      return breakdown.ToFixedList();
    }


    private FinancialReportRow GetReportBreakdownRow(string groupingRuleUID) {
      return _command.GetFinancialReportType()
                     .GetRow(groupingRuleUID);
    }


    private FixedList<FixedRowFinancialReportEntry> CreateReportEntriesWithoutTotals(FixedList<FinancialReportRow> rows) {
      var converted = rows.Select(x => CreateReportEntryWithoutTotals(x));

      return new FixedList<FixedRowFinancialReportEntry>(converted);
    }


    private FixedRowFinancialReportEntry CreateReportEntryWithoutTotals(FinancialReportRow row) {
      return new FixedRowFinancialReportEntry {
        Row = row,
        GroupingRule = row.GroupingRule
      };
    }


    private FixedList<FinancialReportRow> GetReportFixedRows() {
      return _command.GetFinancialReportType()
                     .GetRows();
    }


    private void SetTotalsFields(FinancialReportEntry reportEntry, ReportEntryTotals groupingRuleTotals) {
      reportEntry.SetTotalField(FinancialReportTotalField.DomesticCurrencyTotal,
                                groupingRuleTotals.DomesticCurrencyTotal);
      reportEntry.SetTotalField(FinancialReportTotalField.ForeignCurrencyTotal,
                                groupingRuleTotals.ForeignCurrencyTotal);
      reportEntry.SetTotalField(FinancialReportTotalField.Total,
                                groupingRuleTotals.TotalBalance);
    }

    #endregion Helpers

  }  // class FixedRowGroupingRulesReport

}  // namespace Empiria.FinancialAccounting.FinancialReports
