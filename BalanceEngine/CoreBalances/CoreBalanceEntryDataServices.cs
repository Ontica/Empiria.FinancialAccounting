/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : CoreBalanceEntryDataServices               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for core balance entries.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Data;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Provides data read methods for core balance entries.</summary>
  static internal class CoreBalanceEntryDataServices {

    #region Public methods

    static internal FixedList<CoreBalanceEntry> GetBalanceEntries(TrialBalanceQuery query) {
      Assertion.Require(query, nameof(query));

      BalancesSqlClauses sqlClauses = BalancesSqlClauses.BuildFrom(query);

      FixedList<CoreBalanceEntry> coreBalances = GetBalanceEntries(sqlClauses).ToFixedList();

      FixedList<CoreBalanceEntry> allAccountsInCatalog =
        GetBalancesWithFlattenedAccounts(coreBalances, query);

      return new FixedList<CoreBalanceEntry>(allAccountsInCatalog);
    }

    #endregion Public methods

    #region Helpers

    static private FixedList<CoreBalanceEntry> CombineCoreBalancesWithFlattenedAccounts(
                                                FixedList<CoreBalanceEntry> coreBalances, 
                                                FixedList<CoreBalanceEntry> flattenedAccountBalances) {
      var returnedList = new List<CoreBalanceEntry>(coreBalances);

      foreach (var account in flattenedAccountBalances) {

        var existAccount = returnedList.Where(x => x.Account.Number == account.Account.Number &&
                                              x.Sector.Code == account.Sector.Code &&
                                              x.Currency.Code == account.Currency.Code &&
                                              x.Ledger.Number == account.Ledger.Number &&
                                              x.DebtorCreditor == account.DebtorCreditor).ToList();
        if (existAccount.Count == 0) {
          returnedList.Add(account);
        }
      }

      return returnedList.ToFixedList();
    }


    static private FixedList<CoreBalanceEntry> ConvertFlattenedAccountsIntoBalances(
                                                          FixedList<FlatAccountDto> flattenedAccounts,
                                                          FixedList<Ledger> ledgers) {
      List<CoreBalanceEntry> convertedAccounts = new List<CoreBalanceEntry>();

      foreach (var account in flattenedAccounts) {

        convertedAccounts.AddRange(ledgers.Select((x) =>
                                    new CoreBalanceEntry().MapFromFlattenedAccount(
                                      account, (Ledger) x)));
      }
      return convertedAccounts.ToFixedList();
    }


    static private List<CoreBalanceEntry> GetBalanceEntries(BalancesSqlClauses clauses) {
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

      return DataReader.GetPlainObjectList<CoreBalanceEntry>(operation);
    }


    static private FixedList<CoreBalanceEntry> GetBalancesWithFlattenedAccounts(
                                                        FixedList<CoreBalanceEntry> coreBalances,
                                                        TrialBalanceQuery query) {

      if (query.BalancesType == BalancesType.AllAccountsInCatalog) {

        var accountsChartUseCase = AccountsChartUseCases.UseCaseInteractor();
        var accountsChartQuery = BalanceDataServiceClauses.GetAccountsChartQueryDto(query);
        var flattenedAccounts = accountsChartUseCase.GetFlattenedAccounts(accountsChartQuery);

        var ledgers = GetLedgersFromCoreBalances(coreBalances, query);

        FixedList<CoreBalanceEntry> flattenedAccountBalances = ConvertFlattenedAccountsIntoBalances(
                                                                flattenedAccounts, ledgers);

        return CombineCoreBalancesWithFlattenedAccounts(coreBalances, flattenedAccountBalances);
      } else {
        return new FixedList<CoreBalanceEntry>(coreBalances);
      }
    }


    static private FixedList<Ledger> GetLedgersFromCoreBalances(FixedList<CoreBalanceEntry> coreBalances,
                                                                TrialBalanceQuery query) {

      List<Ledger> ledgers = new List<Ledger>();

      if (coreBalances.Count == 0 || query.Consolidated) {

        ledgers.Add(Ledger.Parse(-1));
      } else {

        foreach (var balance in coreBalances) {
          var existLedger = ledgers.FindAll(x => x.UID == balance.Ledger.UID);

          if (existLedger.Count == 0) {
            ledgers.Add(balance.Ledger);
          }
        }
      }
      return ledgers.ToFixedList();
    }

    #endregion Helpers

  } // class CoreBalanceEntryDataServices

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
