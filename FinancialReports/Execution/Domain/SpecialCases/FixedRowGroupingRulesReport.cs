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

      FixedList<FixedRowFinancialReportEntry> reportEntries = CreateReportEntriesWithoutTotals(fixedRows);

      EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances = GetBalancesAsHashTable();

      ProcessEntries(reportEntries, balances);

      var convertedEntries = new FixedList<FinancialReportEntry>(reportEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_command, convertedEntries);
    }


    internal FinancialReport GetBreakdown(string reportRowUID) {
      FinancialReportRow row = GetReportBreakdownRow(reportRowUID);

      FixedRowFinancialReportEntry reportEntry = CreateReportEntryWithoutTotals(row);

      EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances = GetBalancesAsHashTable();

      FixedList<FinancialReportBreakdownEntry> breakdownEntries = GetBreakdownEntries(reportEntry);

      ProcessBreakdown(breakdownEntries, balances);

      // Add breakdown total row

      var convertedEntries = new FixedList<FinancialReportEntry>(breakdownEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_command, convertedEntries);
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
        totals = totals.Sum(balance, item.Qualification);
      }

      totals.Round();

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
      // groupingRuleItem.ExternalVariableCode
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
      groupingRuleTotals.TotalBalance = value.DomesticCurrencyValue + value.ForeignCurrencyValue;

      return groupingRuleTotals;
    }


    private void ProcessEntries(FixedList<FixedRowFinancialReportEntry> reportEntries,
                                EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances) {

      foreach (var reportEntry in reportEntries) {
        ReportEntryTotals groupingRuleTotals = ProcessGroupingRule(reportEntry.GroupingRule, balances);

        groupingRuleTotals.Round();

        SetTotalsFields(reportEntry, groupingRuleTotals);
      }
    }


    private ReportEntryTotals ProcessGroupingRule(GroupingRule groupingRule,
                                                  EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> balances) {
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


    private EmpiriaHashTable<FixedList<TwoColumnsTrialBalanceEntryDto>> GetBalancesAsHashTable() {
      var balances = GetBalances();

      var converted = new
                FixedList<TwoColumnsTrialBalanceEntryDto>(balances.Entries.Select(x => (TwoColumnsTrialBalanceEntryDto) x))
                                    .FindAll(x => x.ItemType == BalanceEngine.TrialBalanceItemType.Entry ||
                                                  x.ItemType == BalanceEngine.TrialBalanceItemType.Summary);


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


    private FixedList<FixedRowFinancialReportEntry> CreateReportEntriesWithoutTotals(FixedList<FinancialReportRow> rows) {
      var enumeration = rows.Select(x => CreateReportEntryWithoutTotals(x));

      return new FixedList<FixedRowFinancialReportEntry>(enumeration);
    }


    private FixedRowFinancialReportEntry CreateReportEntryWithoutTotals(FinancialReportRow row) {
      return new FixedRowFinancialReportEntry { Row = row, GroupingRule = row.GroupingRule };
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


    internal void Round() {
      this.DomesticCurrencyTotal = Math.Round(this.DomesticCurrencyTotal, 0);
      this.ForeignCurrencyTotal = Math.Round(this.ForeignCurrencyTotal, 0);
      this.TotalBalance = Math.Round(this.TotalBalance, 0);
    }


    internal ReportEntryTotals Substract(ReportEntryTotals total, string qualification) {
      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal - (total.DomesticCurrencyTotal + total.ForeignCurrencyTotal),
          TotalBalance = this.TotalBalance - total.TotalBalance
        };
      }
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - total.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - total.ForeignCurrencyTotal,
        TotalBalance = this.TotalBalance - total.TotalBalance
      };
    }


    internal ReportEntryTotals Substract(TwoColumnsTrialBalanceEntryDto balance, string qualification) {
      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal - (balance.DomesticBalance + balance.ForeignBalance),
          TotalBalance = this.TotalBalance - balance.TotalBalance
        };
      }
      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal - balance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal - balance.ForeignBalance,
        TotalBalance = this.TotalBalance - balance.TotalBalance
      };
    }


    internal ReportEntryTotals Sum(ReportEntryTotals total, string qualification) {
      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal + (total.DomesticCurrencyTotal + total.ForeignCurrencyTotal),
          TotalBalance = this.TotalBalance + total.TotalBalance
        };
      }

      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + total.DomesticCurrencyTotal,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + total.ForeignCurrencyTotal,
        TotalBalance = this.TotalBalance + total.TotalBalance
      };
    }

    internal ReportEntryTotals Sum(TwoColumnsTrialBalanceEntryDto balance, string qualification) {
      if (qualification == "MonedaExtranjera") {
        return new ReportEntryTotals {
          DomesticCurrencyTotal = this.DomesticCurrencyTotal,
          ForeignCurrencyTotal = this.ForeignCurrencyTotal + (balance.DomesticBalance + balance.ForeignBalance),
          TotalBalance = this.TotalBalance + balance.TotalBalance
        };
      }

      return new ReportEntryTotals {
        DomesticCurrencyTotal = this.DomesticCurrencyTotal + balance.DomesticBalance,
        ForeignCurrencyTotal = this.ForeignCurrencyTotal + balance.ForeignBalance,
        TotalBalance = this.TotalBalance + balance.TotalBalance
      };
    }

  }  // class ReportEntryTotals

}  // namespace Empiria.FinancialAccounting.FinancialReports
