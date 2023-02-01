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

      List<TrialBalanceEntry> entries = BalancesByAccount(accounts);

      FixedList<SaldosEncerradosEntryDto> mappedEntries =
        SaldosEncerradosMapper.MergeBalancesIntoLockedUpBalanceEntries(entries, accounts);

      FixedList<SaldosEncerradosBaseEntryDto> headers = GetHeaderByLedger(mappedEntries);

      FixedList<SaldosEncerradosBaseEntryDto> headersAndEntries = MergeHeadersAndEntries(headers, mappedEntries);

      return headersAndEntries;
    }


    public FixedList<SaldosEncerradosEntryDto> BuildEntries() {

      FixedList<Account> accounts = GetAccountsChart();

      List<TrialBalanceEntry> entries = BalancesByAccount(accounts);

      FixedList<SaldosEncerradosEntryDto> mappedEntries =
        SaldosEncerradosMapper.MergeBalancesIntoLockedUpBalanceEntries(entries, accounts);

      FixedList<SaldosEncerradosEntryDto> entriesList = GetCancelableEntries(mappedEntries);

      return entriesList;
    }


    #endregion Public methods

    #region Private methods


    public FixedList<Account> GetAccountsChart() {

      var accountsChart = AccountsChart.Parse(buildQuery.AccountsChartUID);

      return SaldosEncerradosDataService.GetAccountsWithChanges(accountsChart,
                                          buildQuery.FromDate, buildQuery.ToDate);
    }


    private FixedList<SaldosEncerradosEntryDto> GetCancelableEntries(
            FixedList<SaldosEncerradosEntryDto> mappedEntries) {

      var returnedEntries = mappedEntries.FindAll(a => a.IsCancelable).ToList();
      
      returnedEntries.OrderBy(a => a.AccountNumber)
                     .ThenBy(a => a.CurrencyCode)
                     .ThenBy(a => a.SectorCode)
                     .ThenBy(a => a.SubledgerAccount).ToList();

      return returnedEntries.ToFixedList();
    }


    private List<TrialBalanceEntry> BalancesByAccount(
                FixedList<Account> accounts) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var account in accounts) {

        List<TrialBalanceEntry> entries = GetBalancesByAccount(account);
        returnedEntries.AddRange(entries);
      }

      return returnedEntries.OrderBy(a => a.Account.Number)
                     .ThenBy(a => a.Currency.Code)
                     .ThenBy(a => a.Sector.Code)
                     .ThenBy(a => a.SubledgerAccountNumber).ToList();
    }


    private List<TrialBalanceEntry> GetAccountEntries(
            FixedList<ITrialBalanceEntry> entries, Account account) {

      if (entries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var balanceEntries = entries.Select(x => (TrialBalanceEntry) x);

      if (account.Role != AccountRole.Control) {

        return balanceEntries.Where(a => a.Account.Number == account.Number &&
                                         a.ItemType == TrialBalanceItemType.Entry).ToList();
      }

      return balanceEntries.Where(a => a.Account.Number == account.Number).ToList();
    }


    private List<TrialBalanceEntry> GetBalancesByAccount(Account account) {

      using (var balancesUsecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceQuery trialBalanceQuery = GetTrialBalanceQueryClauses(account);

        var balanza = new BalanzaTradicionalBuilder(trialBalanceQuery);

        TrialBalance balances = balanza.Build();
        return GetAccountEntries(balances.Entries, account);
      }

    }


    static private FixedList<SaldosEncerradosBaseEntryDto> GetHeaderByLedger(
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


    private TrialBalanceQuery GetTrialBalanceQueryClauses(Account account) {

      string[] ledger = new string[] {};

      if (buildQuery.LedgerUID != "") {
        ledger = new string[1] { buildQuery.LedgerUID };
      }

      return new TrialBalanceQuery {
        AccountsChartUID = buildQuery.AccountsChartUID,
        InitialPeriod = {
          FromDate = buildQuery.FromDate,
          ToDate = buildQuery.ToDate
        },
        TrialBalanceType = TrialBalanceType.Balanza,
        BalancesType = BalancesType.WithCurrentBalance,
        FromAccount = account.Number,
        ToAccount = account.Number,
        Ledgers = ledger,
        IsOperationalReport = true,
        WithSubledgerAccount = account.Role == AccountRole.Control ? true : false,
      };
    }


    static private FixedList<SaldosEncerradosBaseEntryDto> MergeHeadersAndEntries(
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
