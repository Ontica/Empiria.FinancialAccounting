/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanzaColumnasMonedaHelper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balanza en columnas por moneda.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Helper methods to build balanza en columnas por moneda.</summary>
  internal class BalanzaColumnasMonedaHelper {


    private readonly TrialBalanceQuery Query;

    internal BalanzaColumnasMonedaHelper(TrialBalanceQuery query) {
      Query = query;
    }


    #region Public methods



    internal void CombineAccountEntriesAndDebtorAccounts(List<TrialBalanceEntry> accountEntries,
                                                         List<TrialBalanceEntry> debtorAccounts) {

      if (accountEntries.Count == 0) {
        return;
      }
      var summaryAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in accountEntries) {
        SummaryByAccountEntry(summaryAccountEntries, entry, entry.ItemType);
      }

      debtorAccounts.AddRange(summaryAccountEntries.ToFixedList());
    }
    
    internal FixedList<TrialBalanceEntry> GetAccountEntriesByCurrency(
                                                List<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return new FixedList<TrialBalanceEntry>();
      }

      var helper = new TrialBalanceHelper(Query);

      var filteredAccountList = accountEntries.FindAll(a => (a.Level == 1 && a.Sector.Code == "00") ||
                                                            (a.Level > 1));

      var hashAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in filteredAccountList) {

        SummaryByAccountEntry(hashAccountEntries, entry, entry.ItemType);
      }

      return GetAccountEntriesByDomesticAndForeignCurrencies(hashAccountEntries).ToFixedList();
    }


    internal List<TrialBalanceEntry> GetSumFromCreditorToDebtorAccounts(
                                      List<TrialBalanceEntry> parentAccounts) {

      if (parentAccounts.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var returnedAccounts = new List<TrialBalanceEntry>(parentAccounts);

      foreach (var debtor in parentAccounts.Where(a => a.DebtorCreditor == DebtorCreditorType.Deudora)) {

        var creditor = parentAccounts.FirstOrDefault(a => a.Account.Number == debtor.Account.Number &&
                                                          a.Currency.Equals(debtor.Currency) &&
                                                          a.Sector.Code == debtor.Sector.Code &&
                                                          a.DebtorCreditor == DebtorCreditorType.Acreedora);
        if (creditor != null) {
          SumCreditorToDebtorAccount(debtor, creditor);
          returnedAccounts.Remove(creditor);
        }
      }

      return returnedAccounts;
    }

    
    internal void GetTotalValorizedByAccount(
      List<BalanzaColumnasMonedaEntry> balanceByCurrency) {

      foreach (var entry in balanceByCurrency) {
        entry.SumToTotalValorized();
      }
    }


    internal List<BalanzaColumnasMonedaEntry> MergeTrialBalanceIntoBalanceByCurrency(
                                          FixedList<TrialBalanceEntry> accountEntriesByCurrency) {

      if (accountEntriesByCurrency.Count == 0) {
        return new List<BalanzaColumnasMonedaEntry>();
      }

      var returnedValuedBalance = new List<BalanzaColumnasMonedaEntry>();

      foreach (var entry in accountEntriesByCurrency.Where(a => a.Currency.Equals(Currency.MXN))) {
        returnedValuedBalance.Add(
          entry.MapToBalanceByCurrencyEntry(Query.InitialPeriod.FromDate, Query.InitialPeriod.ToDate));
      }

      MergeDomesticWithForeignAccountEntries(returnedValuedBalance, accountEntriesByCurrency);

      MergeForeignWithReturnedAccountEntries(returnedValuedBalance, accountEntriesByCurrency);

      var returnedOrdering = returnedValuedBalance.OrderBy(a => a.Account.Number).ToList();

      return returnedOrdering;
    }


    internal void ValuateEntriesToExchangeRate(
      FixedList<TrialBalanceEntry> entries) {

      var isValorizedBalance = Query.InitialPeriod.ValuateToCurrrencyUID != string.Empty ||
                               Query.UseDefaultValuation ? true : false;

      FixedList<ExchangeRate> exchangeRates = GetExchangeRateList(isValorizedBalance);
      
      foreach (var entry in entries.Where(a => a.Currency.Distinct(Currency.MXN))) {

        var exchangeRate = exchangeRates.Find(
                            a => a.ToCurrency.Equals(entry.Currency) &&
                            a.FromCurrency.Code == Query.InitialPeriod.ValuateToCurrrencyUID);

        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} " +
                                        $"en la fecha {Query.InitialPeriod.ToDate}.");

        GetValorizedEntries(entry, exchangeRate, isValorizedBalance);
      }
    }


    #endregion Public methods


    #region Private methods


    private EmpiriaHashTable<TrialBalanceEntry> GetAccountEntriesByDomesticAndForeignCurrencies(
                                                EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      GetDomesticAccountEntries(returnedBalances, hashAccountEntries);

      GetForeignAccountEntries(returnedBalances, hashAccountEntries);

      return returnedBalances;
    }


    private void GetDomesticAccountEntries(EmpiriaHashTable<TrialBalanceEntry> returnedBalances,
                                           EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var domesticAccounts = hashAccountEntries.Values.Where(a => a.Currency.Equals(Currency.MXN)).ToList();

      foreach (var domestic in domesticAccounts) {

        var foreignCurrencyAccounts = hashAccountEntries.Values
                                .Where(a => a.Currency.Distinct(Currency.MXN) &&
                                            a.Account.Number == domestic.Account.Number)
                                .OrderBy(a => a.Currency.Code).ToList();

        string hash = $"{domestic.Account.Number}||{domestic.Currency.Code}||{domestic.ItemType}";
        returnedBalances.Insert(hash, domestic);

        foreach (var foreignCurrency in foreignCurrencyAccounts) {

          hash = $"{foreignCurrency.Account.Number}||{foreignCurrency.Currency.Code}";
          returnedBalances.Insert(hash, foreignCurrency);

        }
      }
    }


    private FixedList<ExchangeRate> GetExchangeRateList(bool isValorizedBalance) {

      if (Query.UseDefaultValuation || !isValorizedBalance) {
        Query.InitialPeriod.ValuateToCurrrencyUID = "01";
      }

      GetExchangeRateTypeUIDByTrialBalanceType();

      var exchangeRateType = ExchangeRateType.Parse(Query.InitialPeriod.ExchangeRateTypeUID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(
                                                exchangeRateType, Query.InitialPeriod.ToDate);
      return exchangeRates;
    }


    private void GetExchangeRateTypeUIDByTrialBalanceType() {
      
      if (Query.TrialBalanceType == TrialBalanceType.BalanzaDiferenciaDiariaPorMoneda) {
        
        Query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.BalanzaDiaria.UID;

      } else {

        var lastDayInMonth = DateTime.DaysInMonth(
        Query.InitialPeriod.ToDate.Year, Query.InitialPeriod.ToDate.Month);

        if (Query.InitialPeriod.ToDate.Day == lastDayInMonth) {
          Query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
        } else {
          Query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.Diario.UID;
        }
      }
    }


    private void GetForeignAccountEntries(EmpiriaHashTable<TrialBalanceEntry> returnedBalances,
                                          EmpiriaHashTable<TrialBalanceEntry> hashAccountEntries) {

      var foreignAccounts = hashAccountEntries.Values.Where(a => a.Currency.Distinct(Currency.MXN)).ToList();

      foreach (var foreignAccount in foreignAccounts) {

        var existDomesticAccount = returnedBalances.Values
                          .Where(a => a.Currency.Equals(Currency.MXN) &&
                                      a.Account.Number == foreignAccount.Account.Number)
                          .FirstOrDefault();

        if (existDomesticAccount == null) {

          string hash = $"{foreignAccount.Account.Number}||{foreignAccount.Currency.Code}";
          returnedBalances.Insert(hash, foreignAccount);

        }
      }
    }


    private void GetValorizedEntries(TrialBalanceEntry entry,
      ExchangeRate exchangeRate, bool isValorizedBalance) {

      entry.MultiplyByValorizedValue(exchangeRate.Value);

      if (isValorizedBalance) {
        entry.CurrentBalance = entry.ValorizedCurrentBalance;
      }
    }


    private void MergeDomesticWithForeignAccountEntries(List<BalanzaColumnasMonedaEntry> returnedBalance,
                                                            FixedList<TrialBalanceEntry> ledgerAccounts) {
      foreach (var entry in returnedBalance) {

        foreach (var ledger in ledgerAccounts.Where(a => a.Account.Number == entry.Account.Number)) {
          entry.FromDate = Query.InitialPeriod.FromDate;
          entry.ToDate = Query.InitialPeriod.ToDate;

          if (ledger.Currency.Equals(Currency.USD)) {
            entry.DollarBalance = ledger.CurrentBalance;
            entry.ValorizedDollarBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForDollar = ledger.ExchangeRate;
          }
          if (ledger.Currency.Equals(Currency.YEN)) {
            entry.YenBalance = ledger.CurrentBalance;
            entry.ValorizedYenBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForYen = ledger.ExchangeRate;
          }
          if (ledger.Currency.Equals(Currency.EUR)) {
            entry.EuroBalance = ledger.CurrentBalance;
            entry.ValorizedEuroBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForEuro = ledger.ExchangeRate;
          }
          if (ledger.Currency.Equals(Currency.UDI)) {
            entry.UdisBalance = ledger.CurrentBalance;
            entry.ValorizedUdisBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForUdi = ledger.ExchangeRate;
          }
        }
      }
    }


    private void MergeForeignWithReturnedAccountEntries(
                  List<BalanzaColumnasMonedaEntry> returnedValuedBalance,
                  FixedList<TrialBalanceEntry> ledgerAccounts) {

      foreach (var ledger in ledgerAccounts) {
        ledger.ItemType = TrialBalanceItemType.Summary;

        var entry = returnedValuedBalance.Where(a => a.Account.Number == ledger.Account.Number)
                                         .FirstOrDefault();
        if (entry == null) {

          returnedValuedBalance.Add(
            ledger.MapToBalanceByCurrencyEntry(Query.InitialPeriod.FromDate, Query.InitialPeriod.ToDate));

        } else {

          entry.FromDate = Query.InitialPeriod.FromDate;
          entry.ToDate = Query.InitialPeriod.ToDate;

          if (ledger.Currency.Equals(Currency.MXN)) {
            entry.DomesticBalance = ledger.CurrentBalance;
          }
          if (ledger.Currency.Equals(Currency.USD)) {
            entry.DollarBalance = ledger.CurrentBalance;
            entry.ValorizedDollarBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForDollar = ledger.ExchangeRate;
          }
          if (ledger.Currency.Equals(Currency.YEN)) {
            entry.YenBalance = ledger.CurrentBalance;
            entry.ValorizedYenBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForYen = ledger.ExchangeRate;
          }
          if (ledger.Currency.Equals(Currency.EUR)) {
            entry.EuroBalance = ledger.CurrentBalance;
            entry.ValorizedEuroBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForEuro = ledger.ExchangeRate;
          }
          if (ledger.Currency.Equals(Currency.UDI)) {
            entry.UdisBalance = ledger.CurrentBalance;
            entry.ValorizedUdisBalance = ledger.ValorizedCurrentBalance;
            entry.ExchangeRateForUdi = ledger.ExchangeRate;
          }
        }
      }
    }


    private void SumCreditorToDebtorAccount(TrialBalanceEntry debtor, TrialBalanceEntry creditor) {
      debtor.InitialBalance = debtor.InitialBalance - creditor.InitialBalance;
      debtor.Debit = debtor.Debit - creditor.Debit;
      debtor.Credit = debtor.Credit - creditor.Credit;
      debtor.CurrentBalance = debtor.CurrentBalance - creditor.CurrentBalance;
      debtor.ValorizedCurrentBalance = debtor.ValorizedCurrentBalance - creditor.ValorizedCurrentBalance;
    }


    private void SummaryByAccountEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                       TrialBalanceEntry entry,
                                       TrialBalanceItemType itemType) {

      Sector targetSector = Sector.Empty;

      string hash = $"{entry.Account.Number}||{targetSector.Code}||{entry.Currency.Id}" +
                    $"||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      if (!Query.UseNewSectorizationModel) {

        hash = $"{entry.Account.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";
      }

      var balanceHelper = new TrialBalanceHelper(Query);
      balanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, entry.Account,
                                              Sector.Empty, itemType, hash);
    }


    #endregion Private methods


  } // class BalanzaColumnasMonedaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
