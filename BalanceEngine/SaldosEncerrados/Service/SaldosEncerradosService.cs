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
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.UseCases;
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


    #region Public methods


    public FixedList<SaldosEncerradosBaseEntryDto> Build() {

      FixedList<Account> accounts = GetAccountsChart();

      FixedList<TrialBalanceEntry> entries = BalancesByAccount(accounts).ToFixedList();

      //FixedList<TrialBalanceEntry> filteredEntriesByAccounts = FilteredEntriesByAccounts(entries, accounts);

      FixedList<SaldosEncerradosEntryDto> mappedEntries =
        SaldosEncerradosMapper.MergeBalancesIntoLockedBalanceEntries(entries, accounts);

      FixedList<SaldosEncerradosBaseEntryDto> headers = GetHeaderByLedger(mappedEntries);

      FixedList<SaldosEncerradosBaseEntryDto> headersAndEntries = MergeHeadersAndEntries(headers,
                                                                                         mappedEntries);
      return headersAndEntries;
    }


    public FixedList<SaldosEncerradosEntryDto> BuildEntries() {

      FixedList<Account> accounts = GetAccountsChart();

      FixedList<TrialBalanceEntry> entries = BalancesByAccount(accounts).ToFixedList();

      //FixedList<TrialBalanceEntry> filteredEntriesByAccounts = FilteredEntriesByAccounts(entries, accounts);

      FixedList<SaldosEncerradosEntryDto> mappedEntries =
        SaldosEncerradosMapper.MergeBalancesIntoLockedBalanceEntries(entries, accounts);

      return GetCancelableEntries(mappedEntries);
    }


    #endregion Public methods

    #region Private methods


    private List<TrialBalanceEntry> BalancesByAccount(
                FixedList<Account> accounts) {

      if (accounts.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var returnedEntries = new List<TrialBalanceEntry>();
      var helper = new SaldosEncerradosHelper(buildQuery);

      foreach (var account in accounts) {
        
        List<TrialBalanceEntry> entries = helper.GetBalancesByAccount(account, account.EndDate);

        returnedEntries.AddRange(FilteredEntriesByAccounts(entries, account));
      }

      return returnedEntries.OrderBy(a => a.Account.Number)
                     .ThenBy(a => a.Currency.Code)
                     .ThenBy(a => a.Sector.Code)
                     .ThenBy(a => a.SubledgerAccountNumber).ToList();
    }


    private List<TrialBalanceEntry> FilteredEntriesByAccounts(List<TrialBalanceEntry> entries,
                                                                   Account account) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var entry in entries) {
        
        if (account.Role != entry.Account.Role) {
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


    private FixedList<SaldosEncerradosEntryDto> GetCancelableEntries(
            FixedList<SaldosEncerradosEntryDto> mappedEntries) {

      if (mappedEntries.Count == 0) {
        return new FixedList<SaldosEncerradosEntryDto>();
      }

      var cancelableEntries = mappedEntries.FindAll(a => a.IsCancelable).ToList();

      cancelableEntries.OrderBy(a => a.AccountNumber)
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


    #endregion Private methods

  } // class SaldosEncerradosService

} // namespace Empiria.FinancialAccounting.BalanceEngine
