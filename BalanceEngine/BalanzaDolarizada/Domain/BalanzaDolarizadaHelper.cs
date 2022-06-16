/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanzaDolarizadaHelper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balanza dolarizada.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;
using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balanza dolarizada.</summary>
  internal class BalanzaDolarizadaHelper {

    private readonly TrialBalanceQuery _query;

    internal BalanzaDolarizadaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods

    internal List<TrialBalanceEntry> GetAccountList(
                                    FixedList<TrialBalanceEntry> trialBalance,
                                    List<TrialBalanceEntry> summaryEntries) {
      var helper = new TrialBalanceHelper(_query);

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        SummaryByEntry(hashAccountEntries, entry, entry.ItemType);
      }
      if (hashAccountEntries.ToFixedList().Count > 0) {
        summaryEntries.AddRange(hashAccountEntries.ToFixedList().ToList());
      }

      return summaryEntries;
    }


    internal List<BalanzaDolarizadaEntry> GetExchangeRateByValuedEntry(
                                          List<BalanzaDolarizadaEntry> mergeBalancesToToValuedBalances) {
      var returnedValuedBalances = new List<BalanzaDolarizadaEntry>();

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


    internal EmpiriaHashTable<TrialBalanceEntry> GetEntriesWithItemType(
                                                  List<TrialBalanceEntry> trialBalance) {

      var helper = new TrialBalanceHelper(_query);

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        TrialBalanceItemType itemType = entry.Currency.Code == "02" ?
                                        TrialBalanceItemType.Summary :
                                        TrialBalanceItemType.Entry;
        if (entry.Level == 1 && entry.Sector.Code != "00") {
          entry.InitialBalance = 0;
          entry.Debit = 0;
          entry.Credit = 0;
          entry.CurrentBalance = 0;
        }

        SummaryByEntry(hashAccountEntries, entry, itemType);
      }

      var hashReturnedEntries = GetAccountsForValuedBalances(hashAccountEntries);

      return hashReturnedEntries;
    }


    internal List<BalanzaDolarizadaEntry> MergeTrialBalanceIntoValuedBalances(
                                          FixedList<TrialBalanceEntry> getLedgerAccounts) {

      List<BalanzaDolarizadaEntry> returnedValuedBalance = new List<BalanzaDolarizadaEntry>();
      foreach (var entry in getLedgerAccounts) {
        returnedValuedBalance.Add(entry.MapToValuedBalanceEntry());
      }

      return returnedValuedBalance;
    }


    internal List<TrialBalanceEntry> OrderingDollarizedBalance(
                                      FixedList<TrialBalanceEntry> trialBalanceEntries) {
      var orderingBalance = trialBalanceEntries.OrderBy(a => a.Account.Number).ToList();

      return orderingBalance;
    }


    internal FixedList<TrialBalanceEntry> ValuateToExchangeRate(
                                          FixedList<TrialBalanceEntry> entries) {

      var exchangeRateType = ExchangeRateType.Dolarizacion;

      _query.InitialPeriod.ExchangeRateTypeUID = exchangeRateType.UID;
      _query.InitialPeriod.ValuateToCurrrencyUID = "01";
      _query.InitialPeriod.ExchangeRateDate = _query.InitialPeriod.ToDate;

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, _query.InitialPeriod.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Code != "02")) {
        var exchangeRate = exchangeRates.FirstOrDefault(
                            a => a.FromCurrency.Code == _query.InitialPeriod.ValuateToCurrrencyUID &&
                            a.ToCurrency.Code == entry.Currency.Code);

        // ToDo: URGENT This require must be checked before any state
        Assertion.Require(exchangeRate, $"No hay tipo de cambio para la moneda {entry.Currency.FullName}.");

        entry.ExchangeRate = exchangeRate.Value;
      }
      return entries;
    }

    #endregion Public methods


    #region Private methods


    private EmpiriaHashTable<TrialBalanceEntry> GetAccountsForValuedBalances(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {
      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      GetEntriesWithDollarCurrency(returnedBalances, hashAccountEntries);

      GetEntriesWithoutDollarCurrency(returnedBalances, hashAccountEntries);

      return returnedBalances;
    }


    private EmpiriaHashTable<BalanzaDolarizadaEntry> GetTotalByAccount(
                                                BalanzaDolarizadaEntry header, decimal totalEquivalence) {

      BalanzaDolarizadaEntry valuedEntry = TrialBalanceMapper.MapValuedTrialBalanceEntry(header);

      valuedEntry.GroupName = "TOTAL POR CUENTA";
      valuedEntry.TotalEquivalence = totalEquivalence;
      valuedEntry.ValuedExchangeRate = 0;
      valuedEntry.ItemType = TrialBalanceItemType.BalanceTotalCurrency;
      string hash = $"{valuedEntry.GroupName}||{valuedEntry.Account}";

      EmpiriaHashTable<BalanzaDolarizadaEntry> hashdEntry = new EmpiriaHashTable<BalanzaDolarizadaEntry>();

      hashdEntry.Insert(hash, valuedEntry);

      return hashdEntry;
    }


    private void GetEntriesWithoutDollarCurrency(EmpiriaHashTable<TrialBalanceEntry> returnedBalances,
                                                 EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var secondaryAccounts = hashAccountEntries.Values.Where(a => a.Currency.Code != "01" &&
                                                                   a.Currency.Code != "02");
      foreach (var secondary in secondaryAccounts) {
        var existPrimaryAccount = returnedBalances.Values
                                    .FirstOrDefault(a => a.Account.Number == secondary.Account.Number &&
                                     a.Currency.Code == "02");

        if (existPrimaryAccount == null) {
          TrialBalanceEntry entry = secondary.CreatePartialCopy();

          entry.Currency = Currency.Parse("02");
          entry.InitialBalance = 0;
          entry.Debit = 0;
          entry.Credit = 0;
          entry.CurrentBalance = 0;
          entry.LastChangeDate = secondary.LastChangeDate;
          entry.DebtorCreditor = secondary.DebtorCreditor;

          SummaryByEntry(returnedBalances, entry, TrialBalanceItemType.Summary);

          string hash = $"{secondary.Account.Number}||{secondary.Currency.Code}";
          returnedBalances.Insert(hash, secondary);
        }
      }
    }


    private void GetEntriesWithDollarCurrency(EmpiriaHashTable<TrialBalanceEntry> returnedBalances,
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var dollarEntries = hashAccountEntries.ToFixedList().Where(a => a.Currency.Code == "02").ToList();

      foreach (var header in dollarEntries) {

        var foreignEntries = hashAccountEntries.ToFixedList()
                                .Where(a => a.Currency.Code != "02" &&
                                            a.Account.Number == header.Account.Number)
                                .OrderBy(a => a.Currency.Code).ToList();

        string hash = $"{header.Account.Number}||{header.Currency.Code}||{header.ItemType}";
        returnedBalances.Insert(hash, header);

        foreach (var currencyAccount in foreignEntries) {
          hash = $"{currencyAccount.Account.Number}||{currencyAccount.Currency.Code}";
          returnedBalances.Insert(hash, currencyAccount);
        }
      }
    }


    private void GetOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry,
                                           StandardAccount targetAccount, Sector targetSector,
                                           TrialBalanceItemType itemType, string hash) {

      TrialBalanceEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TrialBalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = targetSector,
          Account = targetAccount,
          ItemType = itemType,
          GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName,
          DebtorCreditor = entry.DebtorCreditor,
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent,
          LastChangeDate = entry.LastChangeDate
        };

        summaryEntry.Sum(entry);

        summaryEntries.Insert(hash, summaryEntry);

      } else {

        summaryEntry.Sum(entry);
      }
    }


    private void SummaryByEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                 TrialBalanceEntry entry,
                                 TrialBalanceItemType itemType) {

      Sector targetSector = Sector.Empty;
      string hash = $"{entry.Account.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      if (_query.TrialBalanceType == TrialBalanceType.BalanzaEnColumnasPorMoneda &&
           _query.UseNewSectorizationModel) {

        hash = $"{entry.Account.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}||{entry.DebtorCreditor}";
      }
      GetOrIncreaseEntries(summaryEntries, entry, entry.Account, targetSector, itemType, hash);
    }


    #endregion Private methods

  } // class BalanzaDolarizadaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
