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
        SaldosEncerradosMapper.MergeBalancesIntoLockedBalanceEntries(entries, accounts);

      FixedList<SaldosEncerradosBaseEntryDto> headers = GetHeaderByLedger(mappedEntries);

      FixedList<SaldosEncerradosBaseEntryDto> headersAndEntries = MergeHeadersAndEntries(headers, mappedEntries);

      return headersAndEntries;
    }


    public FixedList<SaldosEncerradosEntryDto> BuildEntries() {

      FixedList<Account> accounts = GetAccountsChart();

      List<TrialBalanceEntry> entries = BalancesByAccount(accounts);

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

      foreach (var account in accounts) {

        List<TrialBalanceEntry> entries = GetBalancesByAccount(account);
        returnedEntries.AddRange(entries);

      }

      return returnedEntries.OrderBy(a => a.Account.Number)
                     .ThenBy(a => a.Currency.Code)
                     .ThenBy(a => a.Sector.Code)
                     .ThenBy(a => a.SubledgerAccountNumber).ToList();
    }


    private List<TrialBalanceEntry> GetEntriesByAccountRole(Account account,
                                      IEnumerable<TrialBalanceEntry> entries) {

      if (account.Role == AccountRole.Control) {

        return entries.Where(a => a.Account.Number == account.Number).ToList();

      } else {
        return entries.Where(a => a.Account.Number == account.Number &&
                        a.SubledgerAccountNumber.Length <= 1 &&
                        a.ItemType == TrialBalanceItemType.Entry).ToList();
      }
    }


    private List<TrialBalanceEntry> GetAccountsByRules(Account account,
                                           IEnumerable<TrialBalanceEntry> entries) {

      List<TrialBalanceEntry> entriesByRole = GetEntriesByAccountAndSectorRole(account, entries);

      List<TrialBalanceEntry> entriesBySectorRules = GetEntriesBySectorRules(account, entriesByRole);

      //GetEntriesByCurrencyRules(account, entriesByRole);

      return entriesBySectorRules;
    }


    private List<TrialBalanceEntry> GetEntriesBySectorRules(
            Account account, List<TrialBalanceEntry> entriesList) {

      var sectoresEliminados = GetDeletedSectors(account);

      List<TrialBalanceEntry> balanceEntries = GetEntriesBySectorRole(
                              sectoresEliminados.ToFixedList(), account, entriesList);

      foreach (var entry in balanceEntries) {
        
        var checkEntry = entriesList.Select(a => entry).First();

        if (checkEntry == null) {
          entriesList.Add(entry);
        }

      }

      return entriesList;
    }


    private List<SectorRule> GetDeletedSectors(Account account) {
      
      var sectoresAntes = account.GetSectors(buildQuery.FromDate);
      var sectoresDespues = account.GetSectors(buildQuery.ToDate);
      var sectoresEliminados = new List<SectorRule>();

      foreach (var antes in sectoresAntes) {
        var existe = sectoresDespues.Where(a => a.Sector.Code == antes.Sector.Code).FirstOrDefault();
        if (existe == null) {
          sectoresEliminados.Add(antes);
        }
      }

      return sectoresEliminados;
    }


    private List<TrialBalanceEntry> GetEntriesByAccountAndSectorRole(
            Account account, IEnumerable<TrialBalanceEntry> entries) {
      
      var sectors = account.GetSectors(buildQuery.FromDate);

      if (sectors.Count > 0) {
        return GetEntriesBySectorRole(sectors, account, entries);
      } else {
        return GetEntriesByAccountRole(account, entries);
      }
    }


    private List<TrialBalanceEntry> GetEntriesByCurrencyRules(Account account,
                                    IEnumerable<TrialBalanceEntry> entries) {

      var currencies = account.GetCurrencies(buildQuery.FromDate);
      var secondCurrencies = account.GetCurrencies(buildQuery.ToDate);
      
      return new List<TrialBalanceEntry>();
    }


    private FixedList<Account> GetAccountsChart() {

      var accountsChart = AccountsChart.Parse(buildQuery.AccountsChartUID);

      return SaldosEncerradosDataService.GetAccountsWithChanges(accountsChart,
                                          buildQuery.FromDate, buildQuery.ToDate);
    }


    private List<TrialBalanceEntry> GetAccountEntries(
            FixedList<ITrialBalanceEntry> entries, Account account) {

      if (entries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var balanceEntries = entries.Select(x => (TrialBalanceEntry) x);

      return GetAccountsByRules(account, balanceEntries);
    }


    private List<TrialBalanceEntry> GetBalancesByAccount(Account account) {

      TrialBalanceQuery trialBalanceQuery = GetTrialBalanceQueryClauses(account);

      var balanza = new BalanzaTradicionalBuilder(trialBalanceQuery);

      TrialBalance balances = balanza.Build();

      return GetAccountEntries(balances.Entries, account);

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


    private List<TrialBalanceEntry> GetEntriesBySectorRole(FixedList<SectorRule> sectors,
             Account account, IEnumerable<TrialBalanceEntry> entries) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var sector in sectors) {

        if (sector.SectorRole == AccountRole.Control) {

          var sectorRole = entries.Where(a => a.Account.Number == account.Number &&
          a.Sector.Code == sector.Sector.Code).ToList();

          returnedEntries.AddRange(sectorRole);
        } else {
          var sectorRole = entries.Where(a => a.Account.Number == account.Number &&
                          a.Sector.Code == sector.Sector.Code &&
                          a.ItemType == TrialBalanceItemType.Entry).ToList();

          returnedEntries.AddRange(sectorRole);
        }
      }

      return returnedEntries;

    }


    private TrialBalanceQuery GetTrialBalanceQueryClauses(Account account) {

      string[] ledger = new string[] { };

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
        WithSubledgerAccount = true //account.Role == AccountRole.Control ? true : false,
      };
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
