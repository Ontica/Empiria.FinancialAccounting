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

      List<TrialBalanceEntry> entriesWithLevels = trialBalance;//trialBalance.Where(a => a.Level > 1).ToList();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(entriesWithLevels.ToFixedList());

      summaryEntries = GetFirstLevelAccountsListByCurrency(trialBalance, summaryEntries);

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = GetLedgerAccountsList(summaryEntries);

      List<TrialBalanceEntry> orderingBalance = OrderingDollarizedBalance(ledgerAccounts.ToFixedList());

      //FixedList<TrialBalanceEntry> valuedEntries = helper.ValuateToExchangeRate(
      //                              orderingBalance.ToFixedList(), _command.InitialPeriod);

      FixedList<TrialBalanceEntry> valuedEntries = ValuateToExchangeRate(
                                    orderingBalance.ToFixedList(), _command.InitialPeriod);

      List<ValuedTrialBalanceEntry> mergeBalancesToValuedBalances =
                                    MergeTrialBalanceIntoValuedBalances(valuedEntries);

      List<ValuedTrialBalanceEntry> asignExchageRateAndTotalToBalances =
                                    GetExchangeRateByValuedEntry(mergeBalancesToValuedBalances);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                asignExchageRateAndTotalToBalances.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

    
    internal TrialBalance BuildBalanceInColumnsByCurrency() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      List<TrialBalanceEntry> entriesWithLevels = trialBalance.Where(a => a.Level > 1).ToList();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(entriesWithLevels.ToFixedList());

      summaryEntries = GetFirstLevelAccountsListByCurrency(trialBalance, summaryEntries);

      EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts = GetLedgerAccountsListByCurrency(summaryEntries);

      List<TrialBalanceByCurrencyEntry> mergeBalancesToBalanceByCurrency =
                                    MergeTrialBalanceIntoBalanceByCurrency(ledgerAccounts.ToFixedList());

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                mergeBalancesToBalanceByCurrency.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

    private List<TrialBalanceEntry> GetFirstLevelAccountsListByCurrency(
                                    List<TrialBalanceEntry> trialBalance,
                                    List<TrialBalanceEntry> summaryEntries) {
      var helper = new TrialBalanceHelper(_command);
      var firstLevelEntries = trialBalance.ToList();

      if (_command.TrialBalanceType != TrialBalanceType.BalanzaDolarizada) {
        firstLevelEntries = trialBalance.Where(a => a.Level == 1).ToList();
      } 
      
      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in firstLevelEntries) {
        helper.SummaryByEntry(hashAccountEntries, entry, entry.Account, Sector.Empty,
                                     TrialBalanceItemType.Summary);
      }
      if (hashAccountEntries.ToFixedList().Count > 0) {
        summaryEntries.AddRange(hashAccountEntries.ToFixedList().ToList());
      }

      return summaryEntries;
    }


    #region Private methods

    private EmpiriaHashTable<TrialBalanceEntry> GetAccountsByCurrency(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      if (_command.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) {

        returnedBalances = HashAccountsForValuedBalances(hashAccountEntries);

      } else if (_command.TrialBalanceType == TrialBalanceType.BalanzaEnColumnasPorMoneda) {

        returnedBalances = HashAccountsForBalancesByCurrency(hashAccountEntries);

      }

      return returnedBalances;
    }

    private List<ValuedTrialBalanceEntry> GetExchangeRateByValuedEntry(
                                          List<ValuedTrialBalanceEntry> mergeBalancesToToValuedBalances) {
      var returnedValuedBalances = new List<ValuedTrialBalanceEntry>();

      var headerAccounts = mergeBalancesToToValuedBalances
                          .Where(a => a.ItemType == TrialBalanceItemType.Summary).ToList();

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

      var ledgersList = trialBalance;//.Where(a => a.Level == 1 && a.Sector.Code == "00").ToList();

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in ledgersList) {
        TrialBalanceItemType itemType = entry.Currency.Code == "02" ?
                                        TrialBalanceItemType.Summary :
                                        TrialBalanceItemType.Entry;
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
                                        TrialBalanceItemType.Summary :
                                        TrialBalanceItemType.Entry;
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

      var secondaryAccounts = hashAccountEntries.ToFixedList().Where(a => a.Currency.Code != "01").ToList();
      foreach (var secondary in secondaryAccounts) {

        var existPrimaryAccount = returnedBalances.ToFixedList()
                    .Where(a => a.Account.Number == secondary.Account.Number && a.Currency.Code == "01")
                    .FirstOrDefault();

        if (existPrimaryAccount == null) {
          string hash = $"{secondary.Account.Number}||{secondary.Currency.Code}";
          returnedBalances.Insert(hash, secondary);
        }
      }

      return returnedBalances;
    }

    private EmpiriaHashTable<TrialBalanceEntry> HashAccountsForValuedBalances(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {
      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      HashAccountsWithDollarCurrency(returnedBalances, hashAccountEntries);

      HashAccountsWithoutDollarCurrency(returnedBalances, hashAccountEntries);

      return returnedBalances;
    }

    private void HashAccountsWithoutDollarCurrency(EmpiriaHashTable<TrialBalanceEntry> returnedBalances, 
                                                   EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {
      var helper = new TrialBalanceHelper(_command);
      var secondaryAccounts = hashAccountEntries.ToFixedList().Where(a => a.Currency.Code != "01" &&
                                                                          a.Currency.Code != "02").ToList();
      foreach (var secondary in secondaryAccounts) {
        var existPrimaryAccount = returnedBalances.ToFixedList()
                    .Where(a => a.Account.Number == secondary.Account.Number &&
                                a.Currency.Code == "02")
                    .FirstOrDefault();

        if (existPrimaryAccount == null) {
          TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(secondary);
          entry.Currency = Currency.Parse("02");
          entry.InitialBalance = 0;
          entry.Debit = 0;
          entry.Credit = 0;
          entry.CurrentBalance = 0;
          entry.LastChangeDate = secondary.LastChangeDate;
          entry.DebtorCreditor = secondary.DebtorCreditor;
          helper.SummaryByEntry(returnedBalances, entry, entry.Account,
                              Sector.Empty, TrialBalanceItemType.Summary);

          string hash = $"{secondary.Account.Number}||{secondary.Currency.Code}";
          returnedBalances.Insert(hash, secondary);
        }
      }
    }

    private void HashAccountsWithDollarCurrency(EmpiriaHashTable<TrialBalanceEntry> returnedBalances, 
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {
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
    }

    private void MergeDomesticAndForeignCurrenciesByAccount(List<TrialBalanceByCurrencyEntry> returnedBalance,
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


    private void MergeOnlyForeignCurrenciesByAccount(List<TrialBalanceByCurrencyEntry> returnedValuedBalance,
                                                     FixedList<TrialBalanceEntry> ledgerAccounts) {
      foreach (var ledger in ledgerAccounts) {
        ledger.ItemType = TrialBalanceItemType.Summary;

        var entry = returnedValuedBalance.Where(a => a.Account.Number == ledger.Account.Number)
                                         .FirstOrDefault();
        if (entry == null) {
          returnedValuedBalance.Add(ledger.MapToBalanceByCurrencyEntry());
        } else {
          if (ledger.Currency.Code == "01") {
            entry.DomesticBalance = ledger.CurrentBalance;
          }
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

      MergeDomesticAndForeignCurrenciesByAccount(returnedValuedBalance, ledgerAccounts);

      MergeOnlyForeignCurrenciesByAccount(returnedValuedBalance, ledgerAccounts);

      var returnedOrdering = returnedValuedBalance.OrderBy(a => a.Account.Number).ToList();

      return returnedOrdering;
    }


    private List<ValuedTrialBalanceEntry> MergeTrialBalanceIntoValuedBalances(
                                          FixedList<TrialBalanceEntry> getLedgerAccounts) {

      List<ValuedTrialBalanceEntry> returnedValuedBalance = new List<ValuedTrialBalanceEntry>();
      foreach (var entry in getLedgerAccounts) {
        returnedValuedBalance.Add(entry.MapToValuedBalanceEntry());
      }

      return returnedValuedBalance;
    }


    private List<TrialBalanceEntry> OrderingDollarizedBalance(
                                      FixedList<TrialBalanceEntry> trialBalanceEntries) {
      var orderingBalance = trialBalanceEntries.OrderBy(a => a.Account.Number).ToList();

      return orderingBalance;
    }


    private FixedList<TrialBalanceEntry> ValuateToExchangeRate(
                                          FixedList<TrialBalanceEntry> entries,
                                          TrialBalanceCommandPeriod commandPeriod) {

      commandPeriod.ExchangeRateTypeUID = "d6ea1b5f-2d1a-4882-a57c-3a4c94495bcd";
      commandPeriod.ValuateToCurrrencyUID = "01";
      commandPeriod.ExchangeRateDate = commandPeriod.ToDate;

      var exchangeRateType = ExchangeRateType.Parse(commandPeriod.ExchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, commandPeriod.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Code != "02")) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Code == commandPeriod.ValuateToCurrrencyUID &&
                                                              a.ToCurrency.Code == entry.Currency.Code);

        Assertion.AssertObject(exchangeRate, $"No hay tipo de cambio para la moneda {entry.Currency.FullName}.");

        entry.ExchangeRate = exchangeRate.Value;
      }
      return entries;
    }

    #endregion Private methods

  } // class BalanzaValorizada

} // namespace Empiria.FinancialAccounting.BalanceEngine
