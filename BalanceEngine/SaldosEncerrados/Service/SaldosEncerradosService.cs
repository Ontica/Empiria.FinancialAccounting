/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                                Component : Service Layer                        *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Service provider                     *
*  Type     : SaldosEncerradosService                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Main service to get balances information to create locked up balances report.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.Services;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Main service to get balances information to create locked up balances report.</summary>
  public class SaldosEncerradosService : Service {

    #region Constructors and parsers

    SaldosEncerradosQuery buildQuery;

    public SaldosEncerradosService(SaldosEncerradosQuery _query) {
      Assertion.Require(_query, nameof(_query));
      buildQuery = _query;
    }

    static public SaldosEncerradosService ServiceInteractor() {
      return Service.CreateInstance<SaldosEncerradosService>();
    }

    #endregion Constructors and parsers

    #region Methods

    public FixedList<SaldosEncerradosBaseEntryDto> Build() {

      FixedList<Account> accounts = GetAccountsChart();

      FixedList<TrialBalanceEntry> entries = BalancesByAccount(accounts).ToFixedList();

      FixedList<SaldosEncerradosEntryDto> mappedEntries =
        SaldosEncerradosMapper.MergeBalancesIntoLockedBalanceEntries(entries, accounts);

      FixedList<SaldosEncerradosBaseEntryDto> headers = GetHeaderByLedger(mappedEntries);

      return MergeHeadersAndEntries(headers, mappedEntries);
    }


    public FixedList<SaldosEncerradosEntryDto> BuildEntries() {

      FixedList<Account> accounts = GetAccountsChart();

      FixedList<TrialBalanceEntry> entries = BalancesByAccount(accounts).ToFixedList();

      FixedList<SaldosEncerradosEntryDto> mappedEntries =
        SaldosEncerradosMapper.MergeBalancesIntoLockedBalanceEntries(entries, accounts);

      return GetCancelableEntries(mappedEntries);
    }

    #endregion Methods

    #region Helpers

    private List<TrialBalanceEntry> BalancesByAccount(FixedList<Account> accounts) {

      if (accounts.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var returnedEntries = new List<TrialBalanceEntry>();
      var helper = new SaldosEncerradosHelper(buildQuery);

      accounts = accounts.FindAll(x => x.EndDate <= buildQuery.ToDate);

      FixedList<DateTime> changesDates = accounts.SelectDistinct(x => x.EndDate);

      foreach (var date in changesDates) {

        FixedList<Account> accountsByDate = accounts.FindAll(x => x.EndDate == date);

        List<TrialBalanceEntry> candidates = helper.GetAccountsBalances(accountsByDate, date);

        List<TrialBalanceEntry> candidatesWithRoleChange = FilterEntriesWithRoleChange(candidates,
                                                                                       accountsByDate);

        List<TrialBalanceEntry> currentBalances = helper.GetAccountsBalances(accountsByDate,
                                                                             buildQuery.ToDate);

        List<TrialBalanceEntry> lockedBalances = FilterEntriesWithBalance(candidatesWithRoleChange,
                                                                          currentBalances);

        returnedEntries.AddRange(lockedBalances);
      }


      return returnedEntries.OrderBy(a => a.Account.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountNumber).ToList();
    }


    private List<TrialBalanceEntry> FilterEntriesWithRoleChange(List<TrialBalanceEntry> entries,
                                                                FixedList<Account> accounts) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var account in accounts) {

        foreach (var entry in entries) {

          if (account.StandardAccount.Id == entry.Account.Id &&
              account.Role != entry.Account.Role) {
            returnedEntries.Add(entry);
          }
        }
      }

      return returnedEntries;
    }


    private List<TrialBalanceEntry> FilterEntriesWithBalance(List<TrialBalanceEntry> entriesWithLockedBalance,
                                                             List<TrialBalanceEntry> entriesToDate) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var entry in entriesWithLockedBalance) {

        var entryToDate = entriesToDate.Find(x => x.CurrentBalance != 0 &&
                                                  x.SubledgerAccountId == entry.SubledgerAccountId &&
                                                  x.Account.Number == entry.Account.Number &&
                                                  x.Ledger.Number == entry.Ledger.Number &&
                                                  x.Currency.Code == entry.Currency.Code &&
                                                  x.Sector.Code == entry.Sector.Code);

        if (entryToDate != null) {
          returnedEntries.Add(entry);
        }
      }

      return returnedEntries;
    }


    private FixedList<Account> GetAccountsChart() {

      var accountsChart = AccountsChart.Parse(buildQuery.AccountsChartUID);

      return SaldosEncerradosDataService.GetAccountsHistory(accountsChart,
                                          buildQuery.FromDate, buildQuery.ToDate);
    }


    private FixedList<SaldosEncerradosEntryDto> GetCancelableEntries(FixedList<SaldosEncerradosEntryDto> mappedEntries) {

      if (mappedEntries.Count == 0) {
        return new FixedList<SaldosEncerradosEntryDto>();
      }

      var cancelableEntries = mappedEntries.FindAll(x => x.IsCancelable).ToList();

      cancelableEntries = cancelableEntries.OrderBy(a => a.AccountNumber)
                       .ThenBy(a => a.CurrencyCode)
                       .ThenBy(a => a.SectorCode)
                       .ThenBy(a => a.SubledgerAccount).ToList();

      return cancelableEntries.ToFixedList();
    }


    private FixedList<SaldosEncerradosBaseEntryDto> GetHeaderByLedger(
                  FixedList<SaldosEncerradosEntryDto> mappedEntries) {

      var headers = new EmpiriaHashTable<SaldosEncerradosBaseEntryDto>();

      foreach (var entry in mappedEntries) {

        string hash = $"{entry.LedgerNumber}||{entry.RoleChangeDate}";

        SaldosEncerradosBaseEntryDto groupEntry;

        headers.TryGetValue(hash, out groupEntry);

        if (groupEntry == null) {
          groupEntry = entry.CreateGroupEntry();
          headers.Insert(hash, groupEntry);
        }
      }

      return headers.Values.OrderBy(a => a.LedgerNumber)
                           .ThenBy(a => a.RoleChangeDate).ToFixedList();
    }


    private FixedList<SaldosEncerradosBaseEntryDto> MergeHeadersAndEntries(
                            FixedList<SaldosEncerradosBaseEntryDto> headers,
                            FixedList<SaldosEncerradosEntryDto> mappedEntries) {

      var mergedEntries = new List<SaldosEncerradosBaseEntryDto>();

      foreach (var header in headers) {

        var entriesByHeader = mappedEntries.Where(a => a.RoleChangeDate == header.RoleChangeDate &&
                                                       a.LedgerNumber == header.LedgerNumber).ToList();

        mergedEntries.Add(header);
        mergedEntries.AddRange(entriesByHeader);
      }

      return mergedEntries.ToFixedList();
    }


    #endregion Helpers

  } // class SaldosEncerradosService

} // namespace Empiria.FinancialAccounting.BalanceEngine
