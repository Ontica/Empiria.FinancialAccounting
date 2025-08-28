/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalancesDataService                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds clauses used in balances data service.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Holds clauses used in balances data service.</summary>
  static internal class BalanceDataServiceClauses {


    #region Public methods

    static internal AccountsChartQueryDto GetAccountsChartQueryDto(TrialBalanceQuery query) {

      var accountsChartQuery = new AccountsChartQueryDto();
      accountsChartQuery.AccountsChart = AccountsChart.Parse(query.AccountsChartUID);
      accountsChartQuery.FromDate = query.InitialPeriod.FromDate;
      accountsChartQuery.ToDate = query.InitialPeriod.ToDate;
      accountsChartQuery.IncludeSummaryAccounts = false;

      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar ||
          query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        accountsChartQuery.Accounts = AccountRangeConverter.GetAccountRanges(query.Accounts);

      } else {
        accountsChartQuery.Accounts = AccountRangeConverter.GetAccountRange(query.FromAccount,
                                                                            query.ToAccount);
      }

      return accountsChartQuery;
    }


    static internal FixedList<TrialBalanceEntry> GetBalancesWithFlattenedAccounts(TrialBalanceQuery query,
                                                List<TrialBalanceEntry> trialBalanceList) {

      if (query.BalancesType == BalancesType.AllAccountsInCatalog) {

        var accountsChartUseCase = AccountsChartUseCases.UseCaseInteractor();
        var accountsChartQuery = GetAccountsChartQueryDto(query);
        
        var flattenedAccounts = accountsChartUseCase.GetFlattenedAccounts(accountsChartQuery);

        var ledgers = GetLedgersFromTrialBalance(trialBalanceList, query);

        FixedList<TrialBalanceEntry> flattenedAccountBalances = ConvertFlattenedAccountsIntoBalances(
                                                                flattenedAccounts, ledgers);

        return CombineBalanceEntriesAndFlattenedAccounts(trialBalanceList, flattenedAccountBalances);
      } else {
        return new FixedList<TrialBalanceEntry>(trialBalanceList);
      }
    }


    #endregion Public methods

    #region Private methods

    private static FixedList<TrialBalanceEntry> CombineBalanceEntriesAndFlattenedAccounts(
                                                List<TrialBalanceEntry> trialBalanceList,
                                                FixedList<TrialBalanceEntry> flattenedAccountBalances) {
      var returnedList = trialBalanceList.ToList();

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


    static private FixedList<TrialBalanceEntry> ConvertFlattenedAccountsIntoBalances(
                                                FixedList<FlatAccountDto> flattenedAccounts,
                                                FixedList<Ledger> ledgers) {

      List<TrialBalanceEntry> convertedAccounts = new List<TrialBalanceEntry>();
      
      foreach (var account in flattenedAccounts) {

        convertedAccounts.AddRange(ledgers.Select((x) =>
                                    new TrialBalanceEntry().MapFromFlatAccountToTrialBalanceEntry(
                                      account, (Ledger) x)));
      }
      return convertedAccounts.ToFixedList();
    }


    static private FixedList<Ledger> GetLedgersFromTrialBalance(List<TrialBalanceEntry> trialBalanceList,
                                                                TrialBalanceQuery query) {

      List<Ledger> ledgers = new List<Ledger>();

      if (trialBalanceList.Count == 0 || query.Consolidated) {

        ledgers.Add(Ledger.Parse(-1));
      } else {

        foreach (var balance in trialBalanceList) {
          var existLedger = ledgers.FindAll(x => x.UID == balance.Ledger.UID);

          if (existLedger.Count == 0) {
            ledgers.Add(balance.Ledger);
          }
        }
      }
      return ledgers.ToFixedList();
    }


    #endregion Private methods

  } // class BalanceDataServiceClauses

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
