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
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

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

      FixedList<FinancialReportEntry> reportEntries = CreateReportEntriesWithoutTotals(fixedRows);

      EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances = GetBalancesAsHashTable();

      ProcessEntries(reportEntries, balances);

      return new FinancialReport(_command, reportEntries);
    }


    internal FinancialReportBreakdown GetBreakdown(string reportRowUID) {
      FinancialReportRow row = GetReportBreakdownRow(reportRowUID);

      FinancialReportEntry reportEntry = CreateReportEntryWithoutTotals(row);

      EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances = GetBalancesAsHashTable();

      FixedList<FinancialReportBreakdownEntry> breakdownEntries = GetBreakdownEntries(reportEntry);

      ProcessBreakdown(breakdownEntries, balances);

      // Add breakdown total row

      return new FinancialReportBreakdown(_command, breakdownEntries);
    }


    #endregion Public methods


    #region Private methods


    private ReportEntryTotals ProcessAccount(GroupingRuleItem item,
                                            FixedList<TwoColumnsTrialBalanceEntryDto> balances) {
      FixedList<TwoColumnsTrialBalanceEntryDto> filtered;

      if (item.HasSector && item.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == item.SectorCode && x.SubledgerAccountNumber == item.SubledgerAccountNumber);

      } else if (item.HasSector && !item.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == item.SectorCode && x.SubledgerAccountNumber.Length == 0);

      } else if (!item.HasSector && item.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == "00" && x.SubledgerAccountNumber == item.SubledgerAccountNumber);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" && x.SubledgerAccountNumber == item.SubledgerAccountNumber);
        }
      } else {
        filtered = balances.FindAll(x => x.SectorCode == "00" && x.SubledgerAccountNumber.Length == 0);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" && x.SubledgerAccountNumber.Length == 0);
        }
      }

      var totals = new ReportEntryTotals();

      foreach (var balance in filtered) {
        totals = totals.Sum(balance);
      }

      return totals;
    }


    private void ProcessBreakdown(FixedList<FinancialReportBreakdownEntry> breakdown,
                                  EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances) {
      foreach (var breakdownItem in breakdown) {

        ReportEntryTotals groupingRuleTotals;

        if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.Agrupation) {
          groupingRuleTotals = ProcessGroupingRule(breakdownItem.GroupingRuleItem.Reference, balances);

        } else if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.Account &&
                   balances.ContainsKey(breakdownItem.GroupingRuleItem.AccountNumber)) {
          groupingRuleTotals = ProcessAccount(breakdownItem.GroupingRuleItem, balances[breakdownItem.GroupingRuleItem.AccountNumber]);

        } else if (breakdownItem.GroupingRuleItem.Type == GroupingRuleItemType.Account) {
          groupingRuleTotals = new ReportEntryTotals();

        } else {
          throw Assertion.AssertNoReachThisCode();
        }

        breakdownItem.DomesticCurrencyTotal = groupingRuleTotals.DomesticCurrencyTotal;
        breakdownItem.ForeignCurrencyTotal = groupingRuleTotals.ForeignCurrencyTotal;
        breakdownItem.Total = groupingRuleTotals.TotalBalance;
      }
    }


    private void ProcessEntries(FixedList<FinancialReportEntry> reportEntries,
                                EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances) {

      foreach (var reportEntry in reportEntries) {
        ReportEntryTotals groupingRuleTotals = ProcessGroupingRule(reportEntry.GroupingRule, balances);

        reportEntry.DomesticCurrencyTotal = groupingRuleTotals.DomesticCurrencyTotal;
        reportEntry.ForeignCurrencyTotal = groupingRuleTotals.ForeignCurrencyTotal;
        reportEntry.Total = groupingRuleTotals.TotalBalance;
      }
    }


    private ReportEntryTotals ProcessGroupingRule(GroupingRule groupingRule,
                                                  EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances) {
      var totals = new ReportEntryTotals();

      foreach (var groupingRuleItem in groupingRule.Items) {
        if (groupingRuleItem.Type == GroupingRuleItemType.Agrupation &&
            groupingRuleItem.Operator == OperatorType.Add) {
          totals = totals.Sum(ProcessGroupingRule(groupingRuleItem.Reference, balances));

        } else if (groupingRuleItem.Type == GroupingRuleItemType.Agrupation &&
                   groupingRuleItem.Operator == OperatorType.Substract) {
          totals = totals.Substract(ProcessGroupingRule(groupingRuleItem.Reference, balances));

        } else if (groupingRuleItem.Type == GroupingRuleItemType.Account &&
                    balances.ContainsKey(groupingRuleItem.AccountNumber) &&
                    groupingRuleItem.Operator == OperatorType.Add) {
          totals = totals.Sum(ProcessAccount(groupingRuleItem, balances[groupingRuleItem.AccountNumber]));

        } else if (groupingRuleItem.Type == GroupingRuleItemType.Account &&
                   balances.ContainsKey(groupingRuleItem.AccountNumber) &&
                   groupingRuleItem.Operator == OperatorType.Substract) {
          totals = totals.Substract(ProcessAccount(groupingRuleItem, balances[groupingRuleItem.AccountNumber]));

        }
      }

      return totals;
    }


    #endregion Private methods

    #region Helpers

    private FixedList<FinancialReportBreakdownEntry> GetBreakdownEntries(FinancialReportEntry reportEntry) {
      var breakdown = new List<FinancialReportBreakdownEntry>();

      var groupingRule = reportEntry.GroupingRule;

      foreach (var item in groupingRule.Items) {
        breakdown.Add(new FinancialReportBreakdownEntry { GroupingRuleItem = item });
      }

      return breakdown.ToFixedList();
    }


    private EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> GetBalancesAsHashTable() {
      var balances = GetBalances();

      var converted = new
                FixedList<TwoColumnsTrialBalanceEntryDto>(balances.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x))
                                    .FindAll(x => x.ItemType == BalanceEngine.TrialBalanceItemType.BalanceEntry ||
                                                  x.ItemType == BalanceEngine.TrialBalanceItemType.BalanceSummary);


      var accounts = converted.Select(x => x.StandardAccountNumber).Distinct().ToList();

      var hashTable = new EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>>(accounts.Count);

      foreach (string account in accounts) {
        hashTable.Insert(account, converted.FindAll(x => x.StandardAccountNumber == account));
      }

      return hashTable;
    }


    private FinancialReportRow GetReportBreakdownRow(string groupingRuleUID) {
      return _command.GetFinancialReportType().GetRow(groupingRuleUID);
    }


    private TrialBalanceDto GetBalances() {
      TrialBalanceCommand trialBalanceCommand = GetTrialBalanceCommand();

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        return usecases.BuildTrialBalance(trialBalanceCommand);
      }
    }


    private FixedList<FinancialReportEntry> CreateReportEntriesWithoutTotals(FixedList<FinancialReportRow> rows) {
      var enumeration = rows.Select(x => CreateReportEntryWithoutTotals(x));

      return new FixedList<FinancialReportEntry>(enumeration);
    }


    private FinancialReportEntry CreateReportEntryWithoutTotals(FinancialReportRow row) {
      return new FinancialReportEntry { Row = row, GroupingRule = row.GroupingRule };
    }


    private FixedList<FinancialReportRow> GetReportFixedRows() {
      return _command.GetFinancialReportType().GetRows();
    }


    private TrialBalanceCommand GetTrialBalanceCommand() {
      return new TrialBalanceCommand {
        AccountsChartUID = _command.AccountsChartUID,
        TrialBalanceType = BalanceEngine.TrialBalanceType.AnaliticoDeCuentas,
        UseDefaultValuation = true,
        ShowCascadeBalances = false,
        WithSubledgerAccount = true,
        BalancesType = BalanceEngine.BalancesType.WithCurrentBalanceOrMovements,
        ConsolidateBalancesToTargetCurrency = false,
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = new DateTime(_command.Date.Year, _command.Date.Month, 1),
          ToDate = _command.Date,
          UseDefaultValuation = true
        }
      };
    }

    #endregion Helpers

  }  // class FixedRowGroupingRulesReport


  internal class ReportEntryTotals {

    public decimal DomesticCurrencyTotal {
      get; internal set;
    }

    public decimal ForeignCurrencyTotal {
      get; internal set;
    }

    public decimal TotalBalance {
      get; internal set;
    }

    internal ReportEntryTotals Substract(ReportEntryTotals total) {
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - total.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - total.ForeignCurrencyTotal,
        TotalBalance = this.TotalBalance - total.TotalBalance
      };
    }


    internal ReportEntryTotals Substract(TwoColumnsTrialBalanceEntryDto balance) {
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - balance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - balance.ForeignBalance,
        TotalBalance = this.TotalBalance - balance.TotalBalance
      };
    }


    internal ReportEntryTotals Sum(ReportEntryTotals total) {
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + total.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + total.ForeignCurrencyTotal,
        TotalBalance = this.TotalBalance + total.TotalBalance
      };
    }

    internal ReportEntryTotals Sum(TwoColumnsTrialBalanceEntryDto balance) {
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + balance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + balance.ForeignBalance,
        TotalBalance = this.TotalBalance + balance.TotalBalance
      };
    }

  }  // class ReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
