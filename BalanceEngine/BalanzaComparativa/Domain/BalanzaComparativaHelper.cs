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

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build comparative balances.</summary>
  internal class BalanzaComparativaHelper {

    private readonly TrialBalanceQuery _query;

    internal BalanzaComparativaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    internal void GetAverageBalance(List<TrialBalanceEntry> trialBalance) {
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


    internal List<BalanzaComparativaEntry> MergePeriodsIntoComparativeBalance(
                                      FixedList<TrialBalanceEntry> trialBalance) {

      List<BalanzaComparativaEntry> comparativeEntries = new List<BalanzaComparativaEntry>();

      foreach (var entry in trialBalance) {
        comparativeEntries.Add(entry.MapToComparativeBalanceEntry());
      }

      CalculateVariationFields(comparativeEntries);

      AssingSubledgerAccountInfo(comparativeEntries);

      comparativeEntries = OrderingComparativeBalance(comparativeEntries);

      return comparativeEntries;
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

