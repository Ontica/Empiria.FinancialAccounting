/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : AccountComparerBuilder                        License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides the services that is used to generate account comparer.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters;
using Empiria.FinancialAccounting.Reporting.AccountComparer.Domain;

namespace Empiria.FinancialAccounting.Reporting.AccountsComparer.Domain {

  /// <summary>Provides the services that is used to generate account comparer.</summary>
  internal class AccountComparerBuilder : IReportBuilder {

    #region Public methods


    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery trialBalanceQuery = this.MapToTrialBalanceQuery(buildQuery);

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceQuery);

        List<AccountComparerEntry> comparerEntries = BuildComparerEntries(trialBalance.Entries, buildQuery);

        return AccountComparerMapper.MapToReportDataDto(buildQuery, comparerEntries);
      }
    }


    private List<AccountComparerEntry> BuildComparerEntries(FixedList<ITrialBalanceEntryDto> balanceEntries,
                                                            ReportBuilderQuery buildQuery) {
      var helper = new AccountComparerHelper(buildQuery);

      var group = AccountsList.Parse(buildQuery.OutputType);

      var entries = balanceEntries.Select(a => (BalanzaTradicionalEntryDto) a);

      List<AccountComparerEntry> comparerEntries = helper.GetComparerEntries(group, entries);

      List<AccountComparerEntry> totalByCurrency = helper.GetTotalByCurrency(comparerEntries);

      List<AccountComparerEntry> returnedComparerEntries = helper.CombineComparerEntriesWithTotals(
                                                           comparerEntries, totalByCurrency);
        
      return returnedComparerEntries;
    }


    #endregion Public methods

    #region Private methods


    private TrialBalanceQuery MapToTrialBalanceQuery(ReportBuilderQuery buildQuery) {

      return new TrialBalanceQuery {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.Parse(buildQuery.AccountsChartUID).UID,
        BalancesType = BalancesType.WithCurrentBalanceOrMovements,
        ShowCascadeBalances = false,
        WithAverageBalance = false,
        InitialPeriod = new BalancesPeriod {
          FromDate = new DateTime(buildQuery.ToDate.Year, buildQuery.ToDate.Month, 1),
          ToDate = buildQuery.ToDate
        },
        IsOperationalReport = true,
      };

    }


    #endregion Private methods

  } // class AccountComparerBuilder

} // namespace Empiria.FinancialAccounting.Reporting.AccountsComparer.Domain
