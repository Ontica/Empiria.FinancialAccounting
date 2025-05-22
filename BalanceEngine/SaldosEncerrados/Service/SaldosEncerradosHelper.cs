/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : SaldosEncerradosHelper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build locked balances.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build locked balances.</summary>
  internal class SaldosEncerradosHelper {

    private readonly SaldosEncerradosQuery _query;

    public SaldosEncerradosHelper(SaldosEncerradosQuery query) {
      _query = query;
    }


    #region Public methods

    public List<TrialBalanceEntry> GetBalancesByAccount(Account account) {

      TrialBalanceQuery trialBalanceQuery = GetTrialBalanceQueryClauses(account);

      var balanza = new BalanzaTradicionalBuilder(trialBalanceQuery);

      TrialBalance balances = balanza.Build();

      return GetAccountEntries(balances.Entries, account);

    }

    #endregion Public methods


    #region Private methods


    private List<TrialBalanceEntry> GetAccountsByRules(Account account,
                                           IEnumerable<TrialBalanceEntry> entries) {

      List<TrialBalanceEntry> entriesByRole = GetEntriesByAccountAndSectorRole(account, entries);

      List<TrialBalanceEntry> entriesBySectorRules = GetEntriesBySectorRules(account, entriesByRole);

      List<TrialBalanceEntry> entriesByCurrencyRules = GetEntriesByCurrencyRules(account, entriesBySectorRules);

      return entriesByCurrencyRules;
    }


    private List<TrialBalanceEntry> GetAccountEntries(
            FixedList<ITrialBalanceEntry> entries, Account account) {

      if (entries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var balanceEntries = entries.Select(x => (TrialBalanceEntry) x);

      return GetAccountsByRules(account, balanceEntries);
    }


    private List<SectorRule> GetRemovedSectors(Account account) {

      var sectoresAntes = account.GetSectors(_query.FromDate);
      var sectoresDespues = account.GetSectors(_query.ToDate);
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

      var sectors = account.GetSectors(_query.FromDate);

      if (sectors.Count > 0) {
        return GetEntriesBySectorRole(sectors, account, entries);
      } else {
        return GetEntriesByAccountRole(account, entries);
      }
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


    private List<TrialBalanceEntry> GetEntriesByCurrencyRules(Account account,
                                    List<TrialBalanceEntry> entriesList) {

      var monedasEliminadas = GetRemovedCurrencies(account);

      List<TrialBalanceEntry> currencyEntries = GetEntriesByCurrencyRole(
                              monedasEliminadas.ToFixedList(), account, entriesList);

      foreach (var entry in currencyEntries) {

        var checkEntry = entriesList.Select(a => entry).First();

        if (checkEntry == null) {
          entriesList.Add(entry);
        }
      }

      return entriesList;
    }


    private List<TrialBalanceEntry> GetEntriesByCurrencyRole(FixedList<CurrencyRule> currenciesRules,
                                                             Account account, List<TrialBalanceEntry> entries) {

      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var currencyEntry in currenciesRules) {

        var entry = entries.Where(a => a.Account.Number == account.Number &&
                                       a.Currency.Equals(currencyEntry.Currency)).ToList();

        returnedEntries.AddRange(entry);
      }

      return returnedEntries;
    }


    private List<CurrencyRule> GetRemovedCurrencies(Account account) {

      var monedasAntes = account.GetCurrencies(_query.FromDate);
      var monedasDespues = account.GetCurrencies(_query.ToDate);
      var monedasEliminadas = new List<CurrencyRule>();

      foreach (var antes in monedasAntes) {

        var existe = monedasDespues.Find(a => a.Currency.Equals(antes.Currency));

        if (existe == null) {
          monedasEliminadas.Add(antes);
        }
      }

      return monedasEliminadas;
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


    private List<TrialBalanceEntry> GetEntriesBySectorRules(
            Account account, List<TrialBalanceEntry> entriesList) {

      var sectoresEliminados = GetRemovedSectors(account);

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


    private TrialBalanceQuery GetTrialBalanceQueryClauses(Account account) {

      string[] ledger = new string[] { };

      if (_query.LedgerUID != "") {
        ledger = new string[1] { _query.LedgerUID };
      }

      return new TrialBalanceQuery {
        AccountsChartUID = _query.AccountsChartUID,
        InitialPeriod = {
          FromDate = _query.FromDate,
          ToDate = _query.ToDate
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


    #endregion Private methods


  } // class SaldosEncerradosHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
