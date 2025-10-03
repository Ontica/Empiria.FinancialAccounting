/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : TrialBalanceComparativeHelper              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build comparative balances.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build comparative balances.</summary>
  internal class BalanzaComparativaHelper {

    private readonly TrialBalanceQuery _query;

    internal BalanzaComparativaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    internal void GetAverageBalance(FixedList<TrialBalanceEntry> trialBalance) {
      var returnedEntries = new List<TrialBalanceEntry>(trialBalance);

      if (_query.WithAverageBalance) {

        foreach (var entry in returnedEntries) {

          decimal debtorCreditor = entry.DebtorCreditor == DebtorCreditorType.Deudora ?
                                   entry.Debit - entry.Credit : entry.Credit - entry.Debit;

          TimeSpan timeSpan = _query.FinalPeriod.ToDate - entry.LastChangeDate;
          int numberOfDays = timeSpan.Days + 1;

          entry.AverageBalance = ((numberOfDays * debtorCreditor) /
                                   _query.InitialPeriod.ToDate.Day) +
                                   entry.InitialBalance;
        }
      }
    }


    internal FixedList<TrialBalanceEntry> AccountEntriesValorized(
                                          FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<TrialBalanceEntry>();
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      if ((_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation)) {

        ValuateEntriesToExchangeRate(accountEntries, _query.InitialPeriod);

        if (_query.ConsolidateBalancesToTargetCurrency) {
          accountEntries = trialBalanceHelper.ConsolidateToTargetCurrency(
                                              accountEntries, _query.InitialPeriod);
        }
      }

      trialBalanceHelper.RoundDecimals(accountEntries);

      return accountEntries;
    }


    internal FixedList<TrialBalanceEntry> GetExchangeRateByPeriod(FixedList<TrialBalanceEntry> entries,
                                                                BalancesPeriod period) {

      if (period.UseDefaultValuation) {
        period.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
        period.ValuateToCurrrencyUID = "01";
        period.ExchangeRateDate = period.ToDate;
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      trialBalanceHelper.ValidateDateToExchangeRate();

      var exchangeRateType = ExchangeRateType.Parse(period.ExchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, period.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Distinct(Currency.MXN))) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Code == period.ValuateToCurrrencyUID &&
                                                             a.ToCurrency.Equals(entry.Currency));

        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para " +
                                        $"la cuenta {entry.Account.Number} con la " +
                                        $"moneda {entry.Currency.FullName} en la fecha proporcionada.");

        if (period.IsSecondPeriod) {
          entry.SecondExchangeRate = exchangeRate.Value;
        } else {
          entry.ExchangeRate = exchangeRate.Value;
        }

      }
      return entries;
    }


    internal void GetExchangeRateByPeriodV2(FixedList<TrialBalanceEntry> entries,
                                                                BalancesPeriod period) {

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      var exchangeRateFor = trialBalanceHelper.GetExchangeRateTypeForCurrencies(period);

      foreach (var entry in entries.Where(a => a.Currency.Distinct(Currency.MXN))) {

        var exchangeRate = exchangeRateFor.ExchangeRateList.Find(
                            a => a.ToCurrency.Equals(entry.Currency) &&
                                 a.FromCurrency.Code == exchangeRateFor.ValuateToCurrrencyUID);

        if (period.IsSecondPeriod) {
          entry.SecondExchangeRate = exchangeRate.Value;
        } else {
          entry.ExchangeRate = exchangeRate.Value;
        }
      }
    }


    internal FixedList<BalanzaComparativaEntry> MergePeriodsIntoComparativeBalance(
                                      FixedList<TrialBalanceEntry> entriesWithExchangeRate) {
      if (entriesWithExchangeRate.Count == 0) {
        return new FixedList<BalanzaComparativaEntry>();
      }

      List<BalanzaComparativaEntry> comparativeEntries = new List<BalanzaComparativaEntry>();

      foreach (var entry in entriesWithExchangeRate) {
        comparativeEntries.Add(entry.MapToComparativeBalanceEntry());
      }

      CalculateVariationFields(comparativeEntries);

      AssingSubledgerAccountInfo(comparativeEntries);

      comparativeEntries = OrderingComparativeBalance(comparativeEntries);

      return comparativeEntries.ToFixedList();
    }


    internal void ValuateEntriesToExchangeRate(FixedList<TrialBalanceEntry> accountEntries,
                                               BalancesPeriod period) {
      if (period.ToDate >= new DateTime(2025, 06, 01)) {

        GetExchangeRateByPeriodV2(accountEntries, period);

      } else {

        GetExchangeRateByPeriod(accountEntries, period);
      }
    }

    #region Private methods

    private void AssingSubledgerAccountInfo(List<BalanzaComparativaEntry> comparativeEntries) {
      List<BalanzaComparativaEntry> returnedEntries =
                                          new List<BalanzaComparativaEntry>(comparativeEntries);

      foreach (var entry in returnedEntries) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        if (!subledgerAccount.IsEmptyInstance) {
          entry.SubledgerAccountName = subledgerAccount.Name;
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Length;
        }
      }
    }


    private void CalculateVariationFields(List<BalanzaComparativaEntry> comparativeEntries) {
      List<BalanzaComparativaEntry> calculatedEntries =
                                          new List<BalanzaComparativaEntry>(comparativeEntries);

      foreach (var entry in calculatedEntries) {
        entry.Variation = entry.SecondValorization - entry.FirstValorization;
        entry.VariationByER = (entry.FirstTotalBalance * entry.SecondExchangeRate) - entry.FirstValorization;
        entry.RealVariation = entry.Variation - entry.VariationByER;
      }
    }


    private List<BalanzaComparativaEntry> OrderingComparativeBalance(
                                                List<BalanzaComparativaEntry> comparativeEntries) {
      List<BalanzaComparativaEntry> orderingEntries =
                                          new List<BalanzaComparativaEntry>(comparativeEntries);

      foreach (var entry in orderingEntries) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        if (!subledgerAccount.IsEmptyInstance) {
          entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ?
                                         subledgerAccount.Number : "";
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Length;
        }
      }
      return orderingEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ThenByDescending(a => a.Account.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerNumberOfDigits)
                            .ThenBy(a => a.SubledgerAccountNumber)
                            .ToList();
    }

    #endregion

  } // class TrialBalanceComparativeHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine

