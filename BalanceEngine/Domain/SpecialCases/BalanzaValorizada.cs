/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaValorizada                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza valorizada en dolares.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza valorizada en dolares.</summary>
  internal class BalanzaValorizada {

    private readonly TrialBalanceCommand _command;

    public BalanzaValorizada(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(trialBalance.ToFixedList());

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = GetLedgerAccountsList(summaryEntries);

      FixedList<TrialBalanceEntry> valuedEntries = helper.ValuateToExchangeRate(
                                    ledgerAccounts.ToFixedList(), _command.InitialPeriod);

      List<ValuedTrialBalanceEntry> mergeBalancesToValuedBalances =
                                    MergeTrialBalanceIntoValuedBalances(valuedEntries);

      List<ValuedTrialBalanceEntry> asignExchageRateAndTotalToBalances =
                                    GetExchangeRateByValuedEntry(mergeBalancesToValuedBalances);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                asignExchageRateAndTotalToBalances.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildBalanceConsolidatedByCurrency() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(trialBalance.ToFixedList());

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = GetLedgerAccountsListByCurrency(summaryEntries);

      List<TrialBalanceByCurrencyEntry> mergeBalancesToBalanceByCurrency =
                                    MergeTrialBalanceIntoBalanceByCurrency(ledgerAccounts.ToFixedList());

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                mergeBalancesToBalanceByCurrency.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Private methods

    private EmpiriaHashTable<TrialBalanceEntry> GetAccountsByCurrency(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      if (_command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaEnDolares) {

        returnedBalances = HashAccountsForValuedBalances(hashAccountEntries);

      } else if (_command.TrialBalanceType == TrialBalanceType.BalanzaConsolidadaPorMoneda) {

        returnedBalances = HashAccountsForBalancesByCurrency(hashAccountEntries);

      }

      return returnedBalances;
    }

    private List<ValuedTrialBalanceEntry> GetExchangeRateByValuedEntry(
                                          List<ValuedTrialBalanceEntry> mergeBalancesToToValuedBalances) {

      var returnedValuedBalances = new List<ValuedTrialBalanceEntry>();

      var headerAccounts = mergeBalancesToToValuedBalances
                          .Where(a => a.ItemType == TrialBalanceItemType.BalanceSummary).ToList();

      foreach (var header in headerAccounts) {
        returnedValuedBalances.Add(header);
        var foreignAccounts = mergeBalancesToToValuedBalances
                              .Where(a => a.Account.Number == header.Account.Number &&
                                          a.Currency.Code != header.Currency.Code).ToList();

        decimal totalEquivalence = header.TotalEquivalence;

        foreach (var foreign in foreignAccounts) {
          foreign.ValuedExchangeRate = foreign.ExchangeRate / header.ExchangeRate;
          foreign.TotalEquivalence = foreign.TotalBalance * foreign.ValuedExchangeRate;

          returnedValuedBalances.Add(foreign);
          totalEquivalence += foreign.TotalEquivalence;
        }
        var totalByAccount = GetTotalByAccount(header, totalEquivalence);
        if (totalByAccount.Values.Count > 0) {
          returnedValuedBalances.Add(totalByAccount.ToFixedList().FirstOrDefault());
        }
      }
      return returnedValuedBalances;
    }

    private EmpiriaHashTable<TrialBalanceEntry> GetLedgerAccountsList(List<TrialBalanceEntry> trialBalance) {

      var helper = new TrialBalanceHelper(_command);

      var ledgersList = trialBalance.Where(a => a.Level == 1 && a.Sector.Code == "00").ToList();

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in ledgersList) {
        TrialBalanceItemType itemType = entry.Currency.Code == "02" ?
                                        TrialBalanceItemType.BalanceSummary :
                                        TrialBalanceItemType.BalanceEntry;
        helper.SummaryByEntry(hashAccountEntries, entry, entry.Account,
                              Sector.Empty, itemType);
      }

      var hashReturnedEntries = GetAccountsByCurrency(hashAccountEntries);

      return hashReturnedEntries;
    }

    private EmpiriaHashTable<TrialBalanceEntry> GetLedgerAccountsListByCurrency(
                                                List<TrialBalanceEntry> summaryEntries) {
      var helper = new TrialBalanceHelper(_command);

      var ledgersList = summaryEntries.Where(a => a.Level == 1 && a.Sector.Code == "00").ToList();

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in ledgersList) {
        TrialBalanceItemType itemType = entry.Currency.Code == "01" ?
                                        TrialBalanceItemType.BalanceSummary :
                                        TrialBalanceItemType.BalanceEntry;
        helper.SummaryByEntry(hashAccountEntries, entry, entry.Account,
                              Sector.Empty, itemType);
      }

      var hashReturnedEntries = GetAccountsByCurrency(hashAccountEntries);

      return hashReturnedEntries;
    }

    private EmpiriaHashTable<ValuedTrialBalanceEntry> GetTotalByAccount(
                                                ValuedTrialBalanceEntry header, decimal totalEquivalence) {

      ValuedTrialBalanceEntry valuedEntry = TrialBalanceMapper.MapValuedTrialBalanceEntry(header);

      valuedEntry.GroupName = "TOTAL POR CUENTA";
      valuedEntry.TotalEquivalence = totalEquivalence;
      valuedEntry.ValuedExchangeRate = 0;
      valuedEntry.ItemType = TrialBalanceItemType.BalanceTotalCurrency;
      string hash = $"{valuedEntry.GroupName}||{valuedEntry.Account}";

      EmpiriaHashTable<ValuedTrialBalanceEntry> hashdEntry = new EmpiriaHashTable<ValuedTrialBalanceEntry>();

      hashdEntry.Insert(hash, valuedEntry);

      return hashdEntry;
    }

    private EmpiriaHashTable<TrialBalanceEntry> HashAccountsForBalancesByCurrency(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();
      var headerAccounts = hashAccountEntries.ToFixedList().Where(a => a.Currency.Code == "01").ToList();

      foreach (var header in headerAccounts) {
        var foreignCurrencies = hashAccountEntries.ToFixedList()
                                .Where(a => a.Currency.Code != "01" &&
                                            a.Account.Number == header.Account.Number)
                                .OrderBy(a => a.Currency.Code).ToList();

        string hash = $"{header.Account.Number}||{header.Currency.Code}||{header.ItemType}";
        returnedBalances.Insert(hash, header);

        foreach (var currencyAccount in foreignCurrencies) {
          hash = $"{currencyAccount.Account.Number}||{currencyAccount.Currency.Code}";
          returnedBalances.Insert(hash, currencyAccount);
        }
      }

      return returnedBalances;
    }

    private EmpiriaHashTable<TrialBalanceEntry> HashAccountsForValuedBalances(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();
      var headerAccounts = hashAccountEntries.ToFixedList().Where(a => a.Currency.Code == "02").ToList();

      foreach (var header in headerAccounts) {
        var foreignCurrencies = hashAccountEntries.ToFixedList()
                                .Where(a => a.Currency.Code != "02" &&
                                            a.Account.Number == header.Account.Number)
                                .OrderBy(a => a.Currency.Code).ToList();

        string hash = $"{header.Account.Number}||{header.Currency.Code}||{header.ItemType}";
        returnedBalances.Insert(hash, header);

        foreach (var currencyAccount in foreignCurrencies) {
          hash = $"{currencyAccount.Account.Number}||{currencyAccount.Currency.Code}";
          returnedBalances.Insert(hash, currencyAccount);
        }
      }

      return returnedBalances;
    }

    private void MergeForeignCurrenciesByAccount(List<TrialBalanceByCurrencyEntry> returnedBalance,
                                                 FixedList<TrialBalanceEntry> ledgerAccounts) {
      foreach (var entry in returnedBalance) {
        foreach (var ledger in ledgerAccounts.Where(a => a.Account.Number == entry.Account.Number)) {
          if (ledger.Currency.Code == "02") {
            entry.DollarBalance = ledger.CurrentBalance;
          }
          if (ledger.Currency.Code == "06") {
            entry.YenBalance = ledger.CurrentBalance;
          }
          if (ledger.Currency.Code == "27") {
            entry.EuroBalance = ledger.CurrentBalance;
          }
          if (ledger.Currency.Code == "44") {
            entry.UdisBalance = ledger.CurrentBalance;
          }
        }
      }
    }

    private List<TrialBalanceByCurrencyEntry> MergeTrialBalanceIntoBalanceByCurrency(
                                          FixedList<TrialBalanceEntry> ledgerAccounts) {

      List<TrialBalanceByCurrencyEntry> returnedValuedBalance = new List<TrialBalanceByCurrencyEntry>();
      foreach (var entry in ledgerAccounts.Where(a => a.Currency.Code == "01")) {
        returnedValuedBalance.Add(entry.MapToBalanceByCurrencyEntry());
      }

      MergeForeignCurrenciesByAccount(returnedValuedBalance, ledgerAccounts);

      return returnedValuedBalance;
    }

    private List<ValuedTrialBalanceEntry> MergeTrialBalanceIntoValuedBalances(
                                          FixedList<TrialBalanceEntry> getLedgerAccounts) {

      List<ValuedTrialBalanceEntry> returnedValuedBalance = new List<ValuedTrialBalanceEntry>();
      foreach (var entry in getLedgerAccounts) {
        returnedValuedBalance.Add(entry.MapToValuedBalanceEntry());
      }

      return returnedValuedBalance;
    }

    #endregion

  } // class BalanzaValorizada

} // namespace Empiria.FinancialAccounting.BalanceEngine
