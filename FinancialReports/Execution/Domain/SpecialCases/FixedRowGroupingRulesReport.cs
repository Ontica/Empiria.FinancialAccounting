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
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a fixed row defined report which rows are linked to
  /// grouping rules (R01, R10, R12).</summary>
  internal class FixedRowGroupingRulesReport {

    private readonly FinancialReportCommand _command;
    private readonly EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> _balances;

    #region Public methods

    internal FixedRowGroupingRulesReport(FinancialReportCommand command) {
      _command = command;
      _balances = GetBalancesHashTable();
      this.FinancialReportType = _command.GetFinancialReportType();
    }

    public FinancialReportType FinancialReportType {
      get;
    }


    internal FinancialReport Generate() {
      FixedList<FinancialReportRow> fixedRows = GetReportFixedRows();

      FixedList<FixedRowFinancialReportEntry> reportEntries = CreateReportEntriesWithoutTotals(fixedRows);

      ProcessEntries(reportEntries);

      return MapToFinancialReport(reportEntries);
    }


    internal FinancialReport GenerateBreakdown(string reportRowUID) {
      FinancialReportRow row = GetReportBreakdownRow(reportRowUID);

      FixedRowFinancialReportEntry reportEntry = CreateReportEntryWithoutTotals(row);

      FixedList<FinancialReportBreakdownEntry> breakdownEntries = GetBreakdownEntries(reportEntry);

      ReportEntryTotals breakdownTotal = ProcessBreakdown(breakdownEntries);

      breakdownTotal.CopyTotalsTo(reportEntry);

      var reportEntries = new List<FinancialReportEntry>();

      reportEntries.AddRange(breakdownEntries);
      reportEntries.Add(reportEntry);

      return MapToFinancialReport(reportEntries.ToFixedList());
    }


    internal FinancialReport GenerateIntegration() {
      FixedList<FinancialReportRow> integrationRows = GetReportRowsWithIntegrationAccounts();

      var reportEntries = new List<FinancialReportEntry>();

      foreach (var row in integrationRows) {
        FixedRowFinancialReportEntry reportEntry = CreateReportEntryWithoutTotals(row);

        FixedList<FinancialReportBreakdownEntry> breakdownEntries = GetBreakdownEntries(reportEntry).
                                                                    FindAll(x => x.GroupingRuleItem.Type == GroupingRuleItemType.Account);

        ReportEntryTotals breakdownTotal = ProcessBreakdown(breakdownEntries);

        reportEntries.AddRange(breakdownEntries);

        breakdownTotal.CopyTotalsTo(reportEntry);

        reportEntries.Add(reportEntry);
      }

      return MapToFinancialReport(reportEntries.ToFixedList());
    }

    #endregion Public methods


    #region Private methods

    private ReportEntryTotals ProcessAccount(GroupingRuleItem groupingRule) {
      if (!_balances.ContainsKey(groupingRule.AccountNumber)) {
        return CreateReportEntryTotalsObject();
      }

      FixedList<ITrialBalanceEntryDto> accountBalances = GetAccountBalances(groupingRule);

      var totals = CreateReportEntryTotalsObject();

      foreach (var balance in accountBalances) {
        if (groupingRule.CalculationRule == "SumDebitsAndSubstractCredits") {
          totals = totals.SumDebitsOrSubstractCredits(balance, groupingRule.Qualification);
        } else {
          totals = totals.Sum(balance, groupingRule.Qualification);
        }
      }

      if (FinancialReportType.RoundDecimals) {
        totals = totals.Round();
      }

      if (groupingRule.Operator == OperatorType.AbsoluteValue) {
        totals = totals.AbsoluteValue();
      }

      return totals;
    }


    private ReportEntryTotals ProcessBreakdown(FixedList<FinancialReportBreakdownEntry> breakdown) {

      ReportEntryTotals granTotal = CreateReportEntryTotalsObject();

      foreach (var breakdownItem in breakdown) {

        ReportEntryTotals breakdownTotals = CalculateBreakdown(breakdownItem.GroupingRuleItem);

        if (FinancialReportType.RoundDecimals) {
          breakdownTotals = breakdownTotals.Round();
        }

        breakdownTotals.CopyTotalsTo(breakdownItem);

        switch (breakdownItem.GroupingRuleItem.Operator) {
          case OperatorType.Add:
            granTotal = granTotal.Sum(breakdownTotals, breakdownItem.GroupingRuleItem.Qualification);
            break;

          case OperatorType.Substract:
            granTotal = granTotal.Substract(breakdownTotals, breakdownItem.GroupingRuleItem.Qualification);
            break;

          case OperatorType.AbsoluteValue:
            granTotal = granTotal.Sum(breakdownTotals, breakdownItem.GroupingRuleItem.Qualification)
                                 .AbsoluteValue();
            break;

        } // switch

      } // foreach

      return granTotal;
    }


    private ReportEntryTotals CalculateBreakdown(GroupingRuleItem groupingRuleItem) {
      switch (groupingRuleItem.Type) {

        case GroupingRuleItemType.Agrupation:
          return ProcessFinancialConcept(groupingRuleItem.Reference);

        case GroupingRuleItemType.Account:
          return ProcessAccount(groupingRuleItem);

        case GroupingRuleItemType.ExternalVariable:
          return ProcessFixedValue(groupingRuleItem);

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }


    private void ProcessEntries(FixedList<FixedRowFinancialReportEntry> reportEntries) {

      foreach (var reportEntry in reportEntries) {

        if (reportEntry.FinancialConcept.IsEmptyInstance) {
          continue;
        }

        ReportEntryTotals totals = ProcessFinancialConcept(reportEntry.FinancialConcept);

        if (FinancialReportType.RoundDecimals) {
          totals = totals.Round();
        }

        totals.CopyTotalsTo(reportEntry);
      }
    }


    private ReportEntryTotals ProcessFixedValue(GroupingRuleItem groupingRuleItem) {
      ExternalValue value = ExternalValue.GetValue(groupingRuleItem.ExternalVariableCode,
                                                   _command.ToDate);

      var totals = CreateReportEntryTotalsObject();

      return totals.Sum(value, groupingRuleItem.Qualification);
    }


    private ReportEntryTotals ProcessFinancialConcept(FinancialConcept financialConcept) {
      Assertion.Assert(!financialConcept.IsEmptyInstance,
                       "Cannot process the empty FinancialConcept instance.");

      var totals = CreateReportEntryTotalsObject();

      foreach (var integrationItem in financialConcept.Integration) {

        switch (integrationItem.Type) {
          case GroupingRuleItemType.Agrupation:

            totals = CalculateAgrupationTotals(integrationItem, totals);
            break;

          case GroupingRuleItemType.Account:
            totals = CalculateAccountTotals(integrationItem, totals);
            break;

          case GroupingRuleItemType.ExternalVariable:
            totals = CalculateExternalVariableTotals(integrationItem, totals);
            break;

        }

      }  // foreach

      return totals;
    }


    private ReportEntryTotals CalculateAccountTotals(GroupingRuleItem groupingRuleItem, ReportEntryTotals totals) {
      Assertion.Assert(groupingRuleItem.Type == GroupingRuleItemType.Account, "Invalid groupingRuleItem.Type");

      switch (groupingRuleItem.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessAccount(groupingRuleItem),
                            groupingRuleItem.Qualification);

        case OperatorType.Substract:

          return totals.Substract(ProcessAccount(groupingRuleItem),
                                  groupingRuleItem.Qualification);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessAccount(groupingRuleItem),
                            groupingRuleItem.Qualification)
                       .AbsoluteValue();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled operator '{groupingRuleItem.Operator}'.");
      }

    }


    private ReportEntryTotals CalculateAgrupationTotals(GroupingRuleItem groupingRuleItem, ReportEntryTotals totals) {
      Assertion.Assert(groupingRuleItem.Type == GroupingRuleItemType.Agrupation, "Invalid groupingRuleItem.Type");

      switch (groupingRuleItem.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessFinancialConcept(groupingRuleItem.Reference),
                            groupingRuleItem.Qualification);

        case OperatorType.Substract:

          return totals.Substract(ProcessFinancialConcept(groupingRuleItem.Reference),
                                  groupingRuleItem.Qualification);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessFinancialConcept(groupingRuleItem.Reference),
                            groupingRuleItem.Qualification)
                       .AbsoluteValue();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled operator '{groupingRuleItem.Operator}'.");

      }

    }


    private ReportEntryTotals CalculateExternalVariableTotals(GroupingRuleItem groupingRuleItem, ReportEntryTotals totals) {
      Assertion.Assert(groupingRuleItem.Type == GroupingRuleItemType.ExternalVariable, "Invalid groupingRuleItem.Type");

      switch (groupingRuleItem.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessFixedValue(groupingRuleItem),
                            groupingRuleItem.Qualification);

        case OperatorType.Substract:

          return totals.Substract(ProcessFixedValue(groupingRuleItem),
                                  groupingRuleItem.Qualification);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessFixedValue(groupingRuleItem),
                            groupingRuleItem.Qualification)
                       .AbsoluteValue();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled operator '{groupingRuleItem.Operator}'.");
      }

    }

    #endregion Private methods

    #region Helpers

    private ReportEntryTotals CreateReportEntryTotalsObject() {
      switch (FinancialReportType.DataSource) {
        case FinancialReportDataSource.AnaliticoCuentas:
          return new AnaliticoCuentasReportEntryTotals();

        case FinancialReportDataSource.BalanzaEnColumnasPorMoneda:
          return new BalanzaEnColumnasPorMonedaReportEntryTotals();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled data source {FinancialReportType.DataSource}.");
      }
    }


    private FixedList<FixedRowFinancialReportEntry> CreateReportEntriesWithoutTotals(FixedList<FinancialReportRow> rows) {
      var converted = rows.Select(x => CreateReportEntryWithoutTotals(x));

      return new FixedList<FixedRowFinancialReportEntry>(converted);
    }


    private FixedRowFinancialReportEntry CreateReportEntryWithoutTotals(FinancialReportRow row) {
      return new FixedRowFinancialReportEntry {
        Row = row,
        FinancialConcept = row.FinancialConcept
      };
    }


    private FixedList<ITrialBalanceEntryDto> GetAccountBalances(GroupingRuleItem groupingRule) {
      FixedList<ITrialBalanceEntryDto> balances = _balances[groupingRule.AccountNumber];

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

      return filtered;
    }


    private FixedList<FinancialReportBreakdownEntry> GetBreakdownEntries(FixedRowFinancialReportEntry reportEntry) {
      var breakdown = new List<FinancialReportBreakdownEntry>();

      var groupingRule = reportEntry.FinancialConcept;

      foreach (var item in groupingRule.Integration) {
        breakdown.Add(new FinancialReportBreakdownEntry { GroupingRuleItem = item });
      }

      return breakdown.ToFixedList();
    }


    private FinancialReportRow GetReportBreakdownRow(string groupingRuleUID) {
      return FinancialReportType.GetRow(groupingRuleUID);
    }


    private FixedList<FinancialReportRow> GetReportRowsWithIntegrationAccounts() {
      FixedList<FinancialReportRow> rows = GetReportFixedRows();

      return rows.FindAll(x => !x.FinancialConcept.IsEmptyInstance &&
                                x.FinancialConcept.Integration.Contains(item => item.Type == GroupingRuleItemType.Account));
    }


    private FixedList<FinancialReportRow> GetReportFixedRows() {
      return FinancialReportType.GetRows();
    }


    private EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> GetBalancesHashTable() {
      var balancesProvider = new AccountBalancesProvider(_command);

      return balancesProvider.GetBalancesAsHashTable();
    }


    private FinancialReport MapToFinancialReport<T>(FixedList<T> reportEntries) where T : FinancialReportEntry {
      var convertedEntries = new FixedList<FinancialReportEntry>(reportEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_command, convertedEntries);
    }


    #endregion Helpers

  }  // class FixedRowGroupingRulesReport

}  // namespace Empiria.FinancialAccounting.FinancialReports
