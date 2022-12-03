/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : FinancialConceptsReport                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates a report from financial concepts (e.g. R01, R10, R12).                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.ExternalData;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generates a report from financial concepts (e.g. R01, R10, R12).</summary>
  internal class FinancialConceptsReport {

    private readonly FinancialReportQuery _buildQuery;
    private readonly EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> _balances;

    #region Public methods

    internal FinancialConceptsReport(FinancialReportQuery buildQuery) {
      _buildQuery = buildQuery;
      _balances = GetBalancesHashTable();

      FinancialReportType = _buildQuery.GetFinancialReportType();
    }


    public FinancialReportType FinancialReportType {
      get;
    }


    internal FinancialReport Generate() {
      FixedList<FinancialReportItemDefinition> rowsAndCells = FinancialReportType.GetRowsAndCells();

      FixedList<FinancialReportEntryResult> reportEntries = CreateReportEntriesWithoutTotals(rowsAndCells);

      FillEntries(reportEntries);

      CalculateColumns(reportEntries);

      return MapToFinancialReport(reportEntries);
    }


    internal FinancialReport GenerateBreakdown(string reportItemUID) {
      FinancialReportItemDefinition reportItem = FinancialReportType.GetRow(reportItemUID);

      FinancialReportEntryResult reportItemTotals = CreateReportEntryWithoutTotals(reportItem);

      FixedList<FinancialReportBreakdownEntry> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportItemTotals);

      ReportEntryTotals breakdownTotal = FillBreakdown(breakdownEntries);

      breakdownTotal.CopyTotalsTo(reportItemTotals);

      var reportEntries = new List<FinancialReportEntry>();

      reportEntries.AddRange(breakdownEntries);
      reportEntries.Add(reportItemTotals);

      CalculateColumns(reportEntries);

      return MapToFinancialReport(reportEntries.ToFixedList());
    }


    internal FinancialReport GenerateIntegration() {
      FixedList<FinancialReportItemDefinition> rowsAndCells = FinancialReportType.GetRowsAndCells();

      rowsAndCells = FilterItemsWithIntegrationAccounts(rowsAndCells);

      var reportEntries = new List<FinancialReportEntry>();

      foreach (var item in rowsAndCells) {
        FinancialReportEntryResult reportEntry = CreateReportEntryWithoutTotals(item);

        FixedList<FinancialReportBreakdownEntry> breakdownEntries = CreateBreakdownEntriesWithoutTotals(reportEntry);

        breakdownEntries = breakdownEntries.FindAll(x => x.IntegrationEntry.Type == FinancialConceptEntryType.Account);

        ReportEntryTotals breakdownTotal = FillBreakdown(breakdownEntries);

        reportEntries.AddRange(breakdownEntries);

        breakdownTotal.CopyTotalsTo(reportEntry);

        reportEntries.Add(reportEntry);
      }

      return MapToFinancialReport(reportEntries.ToFixedList());
    }

    #endregion Public methods

    #region Private methods

    private void CalculateColumns(IEnumerable<FinancialReportEntryResult> reportEntries) {
      var calculator = new FinancialReportCalculator(FinancialReportType);

      IEnumerable<FinancialReportEntry> castedEntries = reportEntries.Select(entry => (FinancialReportEntry) entry);

      var columnsToCalculate = FinancialReportType.DataColumns.FindAll(x => x.IsCalculated);

      calculator.CalculateColumns(columnsToCalculate, castedEntries);
    }


    private void CalculateColumns(IEnumerable<FinancialReportEntry> reportEntries) {
      var calculator = new FinancialReportCalculator(FinancialReportType);

      var columnsToCalculate = FinancialReportType.BreakdownColumns.FindAll(x => x.IsCalculated);

      calculator.CalculateColumns(columnsToCalculate, reportEntries);
    }


    private ReportEntryTotals ProcessAccount(FinancialConceptEntry integrationEntry) {
      if (!_balances.ContainsKey(integrationEntry.AccountNumber)) {
        return CreateReportEntryTotalsObject();
      }

      FixedList<ITrialBalanceEntryDto> accountBalances = GetAccountBalances(integrationEntry);

      var totals = CreateReportEntryTotalsObject();

      foreach (var balance in accountBalances) {

        if (integrationEntry.CalculationRule == "SaldoDeudorasMenosSaldoAcreedoras") {
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


    private ReportEntryTotals FillBreakdown(FixedList<FinancialReportBreakdownEntry> breakdown) {

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


    private FixedList<FinancialReportItemDefinition> FilterItemsWithIntegrationAccounts(FixedList<FinancialReportItemDefinition> list) {
      return list.FindAll(x => !x.FinancialConcept.IsEmptyInstance &&
                               x.FinancialConcept.Integration.Contains(item => item.Type == FinancialConceptEntryType.Account));
    }

    private ReportEntryTotals CalculateBreakdown(FinancialConceptEntry integrationEntry) {

      switch (integrationEntry.Type) {

        case FinancialConceptEntryType.FinancialConceptReference:
          return ProcessFinancialConcept(integrationEntry.ReferencedFinancialConcept);

        case FinancialConceptEntryType.Account:
          return ProcessAccount(integrationEntry);

        case FinancialConceptEntryType.ExternalVariable:
          return ProcessExternalVariable(integrationEntry);

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private void FillEntries(FixedList<FinancialReportEntryResult> reportEntries) {

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


    private ReportEntryTotals ProcessExternalVariable(FinancialConceptEntry integrationEntry) {
      var variable = ExternalVariable.TryParseWithCode(integrationEntry.ExternalVariableCode);

      ExternalValue value = ExternalValue.Empty;

      if (variable != null) {
        value = variable.GetValue(_buildQuery.ToDate);
      }

      var totals = CreateReportEntryTotalsObject();

      return totals.Sum(value, integrationEntry.DataColumn);
    }


    private ReportEntryTotals ProcessFinancialConcept(FinancialConcept financialConcept) {
      Assertion.Require(!financialConcept.IsEmptyInstance,
                        "Cannot process the empty FinancialConcept instance.");

      ReportEntryTotals totals = CreateReportEntryTotalsObject();

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

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.Account,
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
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
      }

    }


    private ReportEntryTotals CalculateFinancialConceptTotals(FinancialConceptEntry integrationEntry,
                                                              ReportEntryTotals totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.FinancialConceptReference,
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
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");

      }

    }


    private ReportEntryTotals CalculateExternalVariableTotals(FinancialConceptEntry integrationEntry,
                                                              ReportEntryTotals totals) {

      Assertion.Require(integrationEntry.Type == FinancialConceptEntryType.ExternalVariable,
                       "Invalid integrationEntry.Type");

      switch (integrationEntry.Operator) {

        case OperatorType.Add:

          return totals.Sum(ProcessExternalVariable(integrationEntry),
                            integrationEntry.DataColumn);

        case OperatorType.Substract:

          return totals.Substract(ProcessExternalVariable(integrationEntry),
                                  integrationEntry.DataColumn);

        case OperatorType.AbsoluteValue:

          return totals.Sum(ProcessExternalVariable(integrationEntry),
                            integrationEntry.DataColumn)
                       .AbsoluteValue();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled operator '{integrationEntry.Operator}'.");
      }

    }

    #endregion Private methods

    #region Helpers

    private ReportEntryTotals CreateReportEntryTotalsObject() {
      switch (FinancialReportType.DataSource) {

        case FinancialReportDataSource.AnaliticoCuentas:
          return new DynamicReportEntryTotals(FinancialReportType.DataColumns);

        case FinancialReportDataSource.BalanzaEnColumnasPorMoneda:
          return new BalanzaEnColumnasPorMonedaReportEntryTotals();

        case FinancialReportDataSource.BalanzaTradicional:
          return new DynamicReportEntryTotals(FinancialReportType.DataColumns);

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled data source {FinancialReportType.DataSource}.");
      }
    }


    private FixedList<FinancialReportEntryResult> CreateReportEntriesWithoutTotals(FixedList<FinancialReportItemDefinition> reportItemsDef) {
      return reportItemsDef.Select(x => CreateReportEntryWithoutTotals(x))
                           .ToFixedList();
    }


    private FinancialReportEntryResult CreateReportEntryWithoutTotals(FinancialReportItemDefinition reportItemDefinition) {
      return new FinancialReportEntryResult(reportItemDefinition);
    }


    private FixedList<FinancialReportBreakdownEntry> CreateBreakdownEntriesWithoutTotals(FinancialReportEntryResult reportEntry) {
      var integration = reportEntry.FinancialConcept.Integration;

      return integration.Select(x => new FinancialReportBreakdownEntry { IntegrationEntry = x })
                        .ToFixedList();
    }



    private FixedList<ITrialBalanceEntryDto> GetAccountBalances(FinancialConceptEntry integrationEntry) {
      FixedList<ITrialBalanceEntryDto> balances = _balances[integrationEntry.AccountNumber];

      FixedList<ITrialBalanceEntryDto> filtered;

      if (integrationEntry.HasSector && integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == integrationEntry.SectorCode &&
                                         x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);

      } else if (integrationEntry.HasSector && !integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == integrationEntry.SectorCode &&
                                         x.SubledgerAccountNumber.Length <= 1);

      } else if (!integrationEntry.HasSector && integrationEntry.HasSubledgerAccount) {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber == integrationEntry.SubledgerAccountNumber);
        }
      } else {
        filtered = balances.FindAll(x => x.SectorCode == "00" &&
                                         x.SubledgerAccountNumber.Length <= 1);
        if (filtered.Count == 0) {
          filtered = balances.FindAll(x => x.SectorCode != "00" &&
                                           x.SubledgerAccountNumber.Length <= 1);
        }
      }

      if (FinancialReportType.DataSource == FinancialReportDataSource.AnaliticoCuentas ||
          FinancialReportType.DataSource == FinancialReportDataSource.BalanzaTradicional) {

        return ConvertToDynamicTrialBalanceEntryDto(filtered);

      } else {

        return filtered;

      }
    }


    private EmpiriaHashTable<FixedList<ITrialBalanceEntryDto>> GetBalancesHashTable() {
      var balancesProvider = new AccountBalancesProvider(_buildQuery);

      return balancesProvider.GetBalancesAsHashTable();
    }


    private FinancialReport MapToFinancialReport<T>(FixedList<T> reportEntries) where T : FinancialReportEntry {
      var convertedEntries = new FixedList<FinancialReportEntry>(reportEntries.Select(x => (FinancialReportEntry) x));

      return new FinancialReport(_buildQuery, convertedEntries);
    }


    private FixedList<ITrialBalanceEntryDto> ConvertToDynamicTrialBalanceEntryDto(FixedList<ITrialBalanceEntryDto> sourceEntries) {
      var converter = new DynamicTrialBalanceEntryConverter();

      // Assertion.Require(sourceEntries.Count > 0, "There are no source entries ... ");

      FixedList<DynamicTrialBalanceEntryDto> convertedEntries = converter.Convert(sourceEntries);

      return convertedEntries.Select(entry => (ITrialBalanceEntryDto) entry)
                             .ToFixedList();
    }

    #endregion Helpers

  }  // class FinancialConceptsReport

}  // namespace Empiria.FinancialAccounting.FinancialReports
