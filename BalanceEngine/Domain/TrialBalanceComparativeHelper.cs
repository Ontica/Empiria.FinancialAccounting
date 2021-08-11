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
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build comparative balances.</summary>
  internal class TrialBalanceComparativeHelper {

    private readonly TrialBalanceCommand _command;

    internal TrialBalanceComparativeHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal List<TrialBalanceComparativeEntry> MergePeriodsIntoComparativeBalance(
                                      FixedList<TrialBalanceEntry> trialBalanceFirstPeriod,
                                      FixedList<TrialBalanceEntry> trialBalanceSecondPeriod) {

      List<TrialBalanceComparativeEntry> firstPeriod = MergeFirstPeriodToComparativeBalance(
                                                        trialBalanceFirstPeriod);
      List<TrialBalanceComparativeEntry> secondPeriod = MergeSecondPeriodToComparativeBalance(
                                                          trialBalanceSecondPeriod);

      List<TrialBalanceComparativeEntry> comparativeEntries = CombineFirstAndSecondPeriod(
                                                                firstPeriod, secondPeriod);

      CalculateVariationFields(comparativeEntries);

      AssingSubledgerAccountInfo(comparativeEntries);

      comparativeEntries = OrderingComparativeBalance(comparativeEntries);

      return comparativeEntries;
    }

    
    #region Private methods

    private void AssingSubledgerAccountInfo(List<TrialBalanceComparativeEntry> comparativeEntries) {
      List<TrialBalanceComparativeEntry> returnedEntries =
                                          new List<TrialBalanceComparativeEntry>(comparativeEntries);

      foreach (var entry in returnedEntries) {
        SubsidiaryAccount subledgerAccount = SubsidiaryAccount.Parse(entry.SubledgerAccountId);
        if (!subledgerAccount.IsEmptyInstance) {
          entry.SubledgerAccountName = subledgerAccount.Name;
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber != "" ?
                                          entry.SubledgerAccountNumber.Count() : 0;
        }
      }
    }

    private void CalculateVariationFields(List<TrialBalanceComparativeEntry> comparativeEntries) {
      List<TrialBalanceComparativeEntry> calculatedEntries = 
                                          new List<TrialBalanceComparativeEntry>(comparativeEntries);

      foreach (var entry in calculatedEntries) {
        entry.Variation = entry.SecondValorization - entry.FirstValorization;
        entry.VariationByER = (entry.FirstTotalBalance * entry.SecondExchangeRate) - entry.FirstValorization;
        entry.RealVariation = entry.Variation - entry.VariationByER;
      }
    }

    private List<TrialBalanceComparativeEntry> CombineFirstAndSecondPeriod(
                                      List<TrialBalanceComparativeEntry> firstPeriod,
                                      List<TrialBalanceComparativeEntry> secondPeriod) {
      List<TrialBalanceComparativeEntry> returnedEntries = new List<TrialBalanceComparativeEntry>();

      returnedEntries.AddRange(firstPeriod);

      foreach (var entry in returnedEntries) {

        var secondEntry = secondPeriod.FirstOrDefault(a => a.Ledger.Id == entry.Ledger.Id &&
                                                  a.Currency.Id == entry.Currency.Id &&
                                                  a.Sector.Id == entry.Sector.Id &&
                                                  a.Account.Number == entry.Account.Number &&
                                                  a.SubledgerAccountId == entry.SubledgerAccountId &&
                                                  a.DebtorCreditor == entry.DebtorCreditor);
        if (secondEntry != null) {
          MergeSecondPeriodToFirst(entry, secondEntry);
        }
      }

      return returnedEntries;
    }

    private List<TrialBalanceComparativeEntry> MergeFirstPeriodToComparativeBalance(
                                                FixedList<TrialBalanceEntry> trialBalanceFirstPeriod) {
      List<TrialBalanceComparativeEntry> returnedFirstPeriod = new List<TrialBalanceComparativeEntry>();

      foreach (var firstPeriod in trialBalanceFirstPeriod) {
        returnedFirstPeriod.Add(firstPeriod.MapToComparativeFirstPeriod());
      }

      return returnedFirstPeriod;
    }

    private List<TrialBalanceComparativeEntry> MergeSecondPeriodToComparativeBalance(
                                                FixedList<TrialBalanceEntry> trialBalanceSecondPeriod) {
      List<TrialBalanceComparativeEntry> returnedSecondPeriod = new List<TrialBalanceComparativeEntry>();

      foreach (var secondPeriod in trialBalanceSecondPeriod) {
        returnedSecondPeriod.Add(secondPeriod.MapToComparativeSecondPeriod());
      }

      return returnedSecondPeriod;
    }

    private void MergeSecondPeriodToFirst(TrialBalanceComparativeEntry entry,
                                          TrialBalanceComparativeEntry secondPeriod) {
      entry.Debit = secondPeriod.Debit;
      entry.Credit = secondPeriod.Credit;
      entry.SecondTotalBalance = secondPeriod.SecondTotalBalance;
      entry.SecondExchangeRate = secondPeriod.SecondExchangeRate;
      entry.SecondValorization = secondPeriod.SecondValorization;
    }

    private List<TrialBalanceComparativeEntry> OrderingComparativeBalance(
                                                List<TrialBalanceComparativeEntry> comparativeEntries) {
      List<TrialBalanceComparativeEntry> orderingEntries = 
                                          new List<TrialBalanceComparativeEntry>(comparativeEntries);

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

