/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FixedRowFinancialConceptsReport            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates a report with fixed rows linked to financial concepts (e.g. R01, R10, R12).          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a report with fixed rows linked to financial concepts (e.g. R01, R10, R12).</summary>
  internal class FixedRowFinancialConceptsReport {

    private readonly FinancialReportCommand _command;
    private readonly EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> _balances;

    #region Public methods

    internal FixedRowFinancialConceptsReport(FinancialReportCommand command) {
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
                                                                    FindAll(x => x.IntegrationEntry.Type == FinancialConceptEntryType.Account);

        ReportEntryTotals breakdownTotal = ProcessBreakdown(breakdownEntries);

        reportEntries.AddRange(breakdownEntries);

        breakdownTotal.CopyTotalsTo(reportEntry);

        reportEntries.Add(reportEntry);
      }

      return MapToFinancialReport(reportEntries.ToFixedList());
    }

    #endregion Public methods


    #region Private methods

    private ReportEntryTotals ProcessAccount(FinancialConceptEntry integrationEntry) {
      if (!_balances.ContainsKey(integrationEntry.AccountNumber)) {
        return CreateReportEntryTotalsObject();
      }

      FixedList<ITrialBalanceEntryDto> accountBalances = GetAccountBalances(integrationEntry);

      var totals = CreateReportEntryTotalsObject();

      foreach (var balance in accountBalances) {
        if (integrationEntry.CalculationRule == "SumDebitsAndSubstractCredits") {
          totals = totals.SumDebitsOrSubstractCredits(balance, integrationEntry.DataColumn);
        } else {
          totals = totals.Sum(balance, integrationEntry.DataColumn);
        }
      }

      if (FinancialReportType.RoundDecimals) {
        totals = totals.Round();
      }

      if (integrationEntry.Operator == OperatorType.AbsoluteValue) {
        totals = totals.AbsoluteValue();
      }

      return totals;
    }


    private ReportEntryTotals ProcessBreakdown(FixedList<FinancialReportBreakdownEntry> breakdown) {

      ReportEntryTotals granTotal = CreateReportEntryTotalsObject();

      foreach (var breakdownItem in breakdown) {

        ReportEntryTotals breakdownTotals = CalculateBreakdown(breakdownItem.IntegrationEntry);

        if (FinancialReportType.RoundDecimals) {
          breakdownTotals = breakdownTotals.Round();
        }

        breakdownTotals.CopyTotalsTo(breakdownItem);

        switch (breakdownItem.IntegrationEntry.Operator) {

          case OperatorType.Add:
            granTotal = granTotal.Sum(breakdownTotals, breakdownItem.IntegrationEntry.DataColumn);
            break;

          case OperatorType.Substract:
            granTotal = granTotal.Substract(breakdownTotals, breakdownItem.IntegrationEntry.DataColumn);
            break;

          case OperatorType.AbsoluteValue:
            granTotal = granTotal.Sum(breakdownTotals, breakdownItem.IntegrationEntry.DataColumn)
                                 .AbsoluteValue();
            break;

        } // switch

      } // foreach

      return granTotal;
    }


    private ReportEntryTotals CalculateBreakdown(FinancialConceptEntry integrationEntry) {

      switch (integrationEntry.Type) {

        case FinancialConceptEntryType.FinancialConceptReference:
          return ProcessFinancialConcept(integrationEntry.ReferencedFinancialConcept);

        case FinancialConceptEntryType.Account:
          return ProcessAccount(integrationEntry);

        case FinancialConceptEntryType.ExternalVariable:
          return ProcessFixedValue(integrationEntry);

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


    private ReportEntryTotals ProcessFixedValue(FinancialConceptEntry integrationEntry) {
      ExternalValue value = ExternalValue.GetValue(integrationEntry.ExternalVariableCode,
                                                   _command.ToDate);

      var totals = CreateReportEntryTotalsObject();

      return totals.Sum(value, integrationEntry.DataColumn);
    }


    private ReportEntryTotals ProcessFinancialConcept(FinancialConcept financialConcept) {
      Assertion.Assert(!financialConcept.IsEmptyInstance,
                       "Cannot process the empty FinancialConcept instance.");

      var totals = CreateReportEntryTotalsObject();

      foreach (var integrationItem in financialConcept.Integration) {

        switch (integrationItem.Type) {
          case FinancialConceptEntryType.FinancialConceptReference:

            totals = CalculateFinancialConceptTotals(integrationItem, totals);
            break;

          case FinancialConceptEntryType.Account:
            totals = CalculateAccountTotals(integrationItem, totals);
            break;

          case FinancialConceptEntryType.ExternalVariable:
            totals = CalculateExternalVariableTotals(integrationItem, totals);
            break;

        }

      }  // foreach

      return totals;
    }


    private ReportEntryTotals CalculateAccountTotals(FinancialConceptEntry integrationEntry,
                                                     ReportEntryTotals totals) {

      Assertion.Assert(integrationEntry.Type == FinancialConceptEntryType.Account,
                       "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessAccount(integrationEntry),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(ProcessAccount(integrationEntry),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessAccount(integrationEntry),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
      }

    }


    private ReportEntryTotals CalculateFinancialConceptTotals(FinancialConceptEntry integrationEntry,
                                                              ReportEntryTotals totals) {

      Assertion.Assert(integrationEntry.Type == FinancialConceptEntryType.FinancialConceptReference,
                      "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessFinancialConcept(integrationEntry.ReferencedFinancialConcept),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(ProcessFinancialConcept(integrationEntry.ReferencedFinancialConcept),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessFinancialConcept(integrationEntry.ReferencedFinancialConcept),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");

      }

    }


    private ReportEntryTotals CalculateExternalVariableTotals(FinancialConceptEntry integrationEntry,
                                                              ReportEntryTotals totals) {

      Assertion.Assert(integrationEntry.Type == FinancialConceptEntryType.ExternalVariable,
                       "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessFixedValue(integrationEntry),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(ProcessFixedValue(integrationEntry),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessFixedValue(integrationEntry),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
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


    private FixedList<ITrialBalanceEntryDto> GetAccountBalances(FinancialConceptEntry integrationEntry) {
      FixedList<ITrialBalanceEntryDto> balances = _balances[integrationEntry.AccountNumber];

      FixedList<ITrialBalanceEntryDto> filtered;

      if (integrationEntry.HasSector && integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == integrationEntry.SectorCode &&
                                         x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);

      } else if (integrationEntry.HasSector && !integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == integrationEntry.SectorCode &&
                                         x.SubledgerAccountNumber.Length == 0);

      } else if (!integrationEntry.HasSector && integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);
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

      var financialConcept = reportEntry.FinancialConcept;

      foreach (var integrationEntry in financialConcept.Integration) {
        breakdown.Add(new FinancialReportBreakdownEntry { IntegrationEntry = integrationEntry });
      }

      return breakdown.ToFixedList();
    }


    private FinancialReportRow GetReportBreakdownRow(string reportRowUID) {
      return FinancialReportType.GetRow(reportRowUID);
    }


    private FixedList<FinancialReportRow> GetReportRowsWithIntegrationAccounts() {
      FixedList<FinancialReportRow> rows = GetReportFixedRows();

      return rows.FindAll(x => !x.FinancialConcept.IsEmptyInstance &&
                                x.FinancialConcept.Integration.Contains(item => item.Type == FinancialConceptEntryType.Account));
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
