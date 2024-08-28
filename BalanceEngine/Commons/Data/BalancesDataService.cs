/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalancesDataService                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for balances.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.Data;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for balances.</summary>
  static internal class BalancesDataService {


    #region Public methods


    static internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      var trialBalanceList = GetTrialBalanceEntries(sqlClauses);

      if (query.BalancesType == BalancesType.AllAccountsInCatalog) {

        var accountsChartQuery = GetAccountsChartQueryDto(query);
        //var accountsInCatalog = AccountsChartUseCases.GetAccounts(accountsChartQuery).Accounts;
        //return MergeBalancesAndAccounts(trialBalanceList, accountsInCatalog);
      }

      return new FixedList<TrialBalanceEntry>(trialBalanceList);
    }


    static internal FixedList<TrialBalanceEntry> GetTrialBalanceForBalancesExplorer(BalanceExplorerQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      return GetTrialBalanceEntries(sqlClauses).ToFixedList();
    }


    #endregion Public methods

    #region Private methods


    static private AccountsChartQueryDto GetAccountsChartQueryDto(
      TrialBalanceQuery query) {

      var accountsChartQuery = new AccountsChartQueryDto();
      accountsChartQuery.AccountsChart = AccountsChart.Parse(query.AccountsChartUID);
      accountsChartQuery.FromDate = query.InitialPeriod.FromDate;
      accountsChartQuery.ToDate = query.InitialPeriod.ToDate;

      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar ||
          query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        accountsChartQuery.Accounts = AccountRangeConverter.GetAccountRanges(query.Accounts);

      } else {
        accountsChartQuery.Accounts = AccountRangeConverter.GetAccountRange(query.FromAccount,
                                                                            query.ToAccount);
      }

      return accountsChartQuery;
    }


    static private List<TrialBalanceEntry> GetTrialBalanceEntries(BalancesSqlClauses clauses) {
      var operation = DataOperation.Parse("@qryTrialBalance",
                            DataCommonMethods.FormatSqlDbDate(clauses.StoredInitialBalanceSet.BalancesDate),
                            DataCommonMethods.FormatSqlDbDate(clauses.FromDate),
                            DataCommonMethods.FormatSqlDbDate(clauses.ToDate),
                            clauses.StoredInitialBalanceSet.Id,
                            clauses.Fields,
                            clauses.Filters,
                            clauses.AccountFilters,
                            clauses.Where,
                            clauses.Ordering,
                            clauses.InitialFields,
                            clauses.InitialGrouping,
                            clauses.AccountsChart.Id,
                            clauses.AverageBalance
                            );

      return DataReader.GetPlainObjectList<TrialBalanceEntry>(operation);
    }


    static private FixedList<TrialBalanceEntry> MergeBalancesAndAccounts(
      List<TrialBalanceEntry> trialBalanceList, FixedList<AccountDescriptorDto> accountsInCatalog) {

      return new FixedList<TrialBalanceEntry>();
    }


    #endregion Private methods


  } // class BalancesDataService

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
