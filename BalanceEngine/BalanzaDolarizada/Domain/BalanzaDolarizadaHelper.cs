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

    private readonly TrialBalanceQuery Query;

    internal BalanzaDolarizadaHelper(TrialBalanceQuery query) {
      Query = query;
    }


    #region Public methods


    internal List<TrialBalanceEntry> AssignItemTypeToAccountEntries(
                                     List<TrialBalanceEntry> parentAccountEntries) {

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in parentAccountEntries) {

        TrialBalanceItemType itemType = entry.Currency.Equals(Currency.USD) ?
                                        TrialBalanceItemType.Summary :
                                        TrialBalanceItemType.Entry;

        ResetBalancesByEntry(entry);
        SummaryByAccountEntry(hashAccountEntries, entry, itemType);
      }

      return hashAccountEntries.Values.ToList();
    }


    internal void GetAccountList(FixedList<TrialBalanceEntry> accountEntries,
                                 List<TrialBalanceEntry> parentAccountEntries) {

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries) {
        SummaryByAccountEntry(hashAccountEntries, entry, entry.ItemType);
      }

      parentAccountEntries.AddRange(hashAccountEntries.Values.ToList());

    }


    internal List<TrialBalanceEntry> GetDollarizedExchangeRate(
                                          List<TrialBalanceEntry> accountEntries) {

      var exchangeRateType = ExchangeRateType.Dolarizacion;

      Query.InitialPeriod.ExchangeRateTypeUID = exchangeRateType.UID;
      Query.InitialPeriod.ValuateToCurrrencyUID = "01";
      Query.InitialPeriod.ExchangeRateDate = Query.InitialPeriod.ToDate;

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType,
                                                                   Query.InitialPeriod.ExchangeRateDate);

      foreach (var entry in accountEntries.Where(a => a.Currency.Code != "02")) {
        var exchangeRate = exchangeRates.FirstOrDefault(
                            a => a.FromCurrency.Code == Query.InitialPeriod.ValuateToCurrrencyUID &&
                                 a.ToCurrency.Equals(entry.Currency));

        // ToDo: URGENT This require must be checked before any state
        Assertion.Require(exchangeRate, $"No hay tipo de cambio para la moneda {entry.Currency.FullName}.");

        entry.ExchangeRate = exchangeRate.Value;
      }
      return accountEntries;
    }


    internal List<BalanzaDolarizadaEntry> GetExchangeRateByAccountEntry(
                                          List<BalanzaDolarizadaEntry> balanzaDolarizada) {
      if (balanzaDolarizada.Count == 0) {
        return balanzaDolarizada;
      }

      var returnedValuedBalances = new List<BalanzaDolarizadaEntry>();
      var accountsWithDollarCurrency = balanzaDolarizada.Where(
                                       a => a.ItemType == TrialBalanceItemType.Summary).ToList();

      foreach (var dollarAccount in accountsWithDollarCurrency) {

        returnedValuedBalances.Add(dollarAccount);

        decimal totalEquivalence = dollarAccount.TotalEquivalence;

        totalEquivalence = GetExchangeRateByForeignAccount(returnedValuedBalances, balanzaDolarizada,
                                                           dollarAccount, totalEquivalence);

        var totalByAccount = GetTotalByAccount(dollarAccount, totalEquivalence);

        if (totalByAccount.Values.Count > 0) {

          returnedValuedBalances.Add(totalByAccount.ToFixedList().FirstOrDefault());
        }
      }
      return returnedValuedBalances;
    }


    private decimal GetExchangeRateByForeignAccount(List<BalanzaDolarizadaEntry> returnedValuedBalances,
                                                    List<BalanzaDolarizadaEntry> balanzaDolarizada,
                                                    BalanzaDolarizadaEntry dollarAccount,
                                                    decimal totalEquivalence) {

      var foreignAccounts = balanzaDolarizada
                               .Where(a => a.Account.Number == dollarAccount.Account.Number &&
                                           a.Currency.Code != dollarAccount.Currency.Code).ToList();

      foreach (var foreignAccount in foreignAccounts) {

        foreignAccount.ValuedExchangeRate = foreignAccount.ExchangeRate / dollarAccount.ExchangeRate;
        foreignAccount.TotalEquivalence = foreignAccount.TotalBalance * foreignAccount.ValuedExchangeRate;

        returnedValuedBalances.Add(foreignAccount);
        totalEquivalence += foreignAccount.TotalEquivalence;
      }
      return totalEquivalence;
    }


    public List<TrialBalanceEntry> GetForeignAccountEntries(
                                   List<TrialBalanceEntry> accountEntries) {

      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      GetAccountEntriesWithDollarCurrency(returnedBalances, accountEntries);

      GetForeignAccountEntriesExceptDollarCurrency(returnedBalances, accountEntries);

      return returnedBalances.Values.OrderBy(a => a.Account.Number).ToList();
    }


    internal List<BalanzaDolarizadaEntry> MergeAccountsIntoBalanzaDolarizada(
                                          List<TrialBalanceEntry> valuedAccountEntries) {
      if (valuedAccountEntries.Count == 0) {
        return new List<BalanzaDolarizadaEntry>();
      }

      var returnedValuedBalance = new List<BalanzaDolarizadaEntry>();

      foreach (var entry in valuedAccountEntries) {
        returnedValuedBalance.Add(entry.MapToValuedBalanceEntry());
      }

      return returnedValuedBalance;
    }


    private void ResetBalancesByEntry(TrialBalanceEntry entry) {

      if (entry.Level == 1 && entry.Sector.Code != "00") {
        entry.InitialBalance = 0;
        entry.Debit = 0;
        entry.Credit = 0;
        entry.CurrentBalance = 0;
      }

    }


    #endregion Public methods


    #region Private methods


    private void GetAccountEntriesWithDollarCurrency(EmpiriaHashTable<TrialBalanceEntry> returnedBalances,
                                                     List<TrialBalanceEntry> hashAccountEntries) {

      var dollarEntries = hashAccountEntries.ToFixedList().Where(a => a.Currency.Equals(Currency.USD)).ToList();

      foreach (var header in dollarEntries) {

        var foreignEntries = hashAccountEntries.ToFixedList()
                                .Where(a => a.Currency.Distinct(Currency.USD) &&
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


    private void GetForeignAccountEntriesExceptDollarCurrency(
                            EmpiriaHashTable<TrialBalanceEntry> returnedBalances,
                            List<TrialBalanceEntry> accountEntries) {

      var secondaryAccounts = accountEntries.Where(a => a.Currency.Code != "01" &&
                                                                   a.Currency.Code != "02");
      foreach (var secondary in secondaryAccounts) {
        var existPrimaryAccount = returnedBalances.Values
                                    .FirstOrDefault(a => a.Account.Number == secondary.Account.Number &&
                                                         a.Currency.Equals(Currency.USD));

        if (existPrimaryAccount == null) {
          TrialBalanceEntry entry = secondary.CreatePartialCopy();

          entry.Currency = Currency.USD;
          entry.InitialBalance = 0;
          entry.Debit = 0;
          entry.Credit = 0;
          entry.CurrentBalance = 0;
          entry.LastChangeDate = secondary.LastChangeDate;
          entry.DebtorCreditor = secondary.DebtorCreditor;

          SummaryByAccountEntry(returnedBalances, entry, TrialBalanceItemType.Summary);

          string hash = $"{secondary.Account.Number}||{secondary.Currency.Code}";
          returnedBalances.Insert(hash, secondary);
        }
      }
    }


    private EmpiriaHashTable<BalanzaDolarizadaEntry> GetTotalByAccount(
                                                BalanzaDolarizadaEntry header, decimal totalEquivalence) {

      BalanzaDolarizadaEntry valuedEntry = BalanzaDolarizadaMapper.BalanzaDolarizadaPartialCopy(header);

      valuedEntry.GroupName = "TOTAL POR CUENTA";
      valuedEntry.TotalEquivalence = totalEquivalence;
      valuedEntry.ValuedExchangeRate = 0;
      valuedEntry.ItemType = TrialBalanceItemType.BalanceTotalCurrency;
      string hash = $"{valuedEntry.GroupName}||{valuedEntry.Account}";

      EmpiriaHashTable<BalanzaDolarizadaEntry> hashdEntry = new EmpiriaHashTable<BalanzaDolarizadaEntry>();

      hashdEntry.Insert(hash, valuedEntry);

      return hashdEntry;
    }


    private void SummaryByAccountEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                       TrialBalanceEntry entry,
                                       TrialBalanceItemType itemType) {

      string hash = $"{entry.Account.Number}||{Sector.Empty.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      var trialBalanceHelper = new TrialBalanceHelper(Query);
      trialBalanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, entry.Account,
                                                   Sector.Empty, itemType, hash);
    }


    #endregion Private methods

  } // class BalanzaDolarizadaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
