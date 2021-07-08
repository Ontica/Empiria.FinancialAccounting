/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : TwoCurrenciesBalanceHelper                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build two currencies balances.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build two currencies balances.</summary>
  internal class TwoCurrenciesBalanceHelper {

    private readonly TrialBalanceCommand _command;

    internal TwoCurrenciesBalanceHelper(TrialBalanceCommand command) {
      _command = command;  
    }


    internal FixedList<TwoCurrenciesBalanceEntry> CombineGroupEntriesAndTwoColumnsEntries(
                                                  FixedList<TwoCurrenciesBalanceEntry> entries,
                                                  FixedList<TwoCurrenciesBalanceEntry> totalByGroup) {

      List<TwoCurrenciesBalanceEntry> returnedEntries = new List<TwoCurrenciesBalanceEntry>();

      foreach (var group in totalByGroup
                    .Where(a => a.DebtorCreditor == DebtorCreditorType.Deudora)) {

        var debtorEntries = entries.Where(a => a.Account.GroupNumber == group.GroupNumber &&
                                          a.Ledger.Id == group.Ledger.Id &&
                                          a.Account.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (debtorEntries.Count > 0) {
          debtorEntries.Add(group);
          returnedEntries.AddRange(debtorEntries);
        }
      }

      foreach (var group in totalByGroup
                    .Where(a => a.DebtorCreditor == DebtorCreditorType.Acreedora)) {

        var creditorEntries = entries.Where(a => a.Account.GroupNumber == group.GroupNumber &&
                                          a.Ledger.Id == group.Ledger.Id &&
                                          a.Account.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (creditorEntries.Count > 0) {
          creditorEntries.Add(group);
          returnedEntries.AddRange(creditorEntries);
        }
      }

      return returnedEntries.ToFixedList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> CombineTotalDeptorCreditorAndTwoColumnsEntries
                            (List<TwoCurrenciesBalanceEntry> entries,
                             List<TwoCurrenciesBalanceEntry> totalByDeptorCreditorEntries) {

      List<TwoCurrenciesBalanceEntry> returnedEntries = new List<TwoCurrenciesBalanceEntry>();

      foreach (var debtorCreditorEntry in totalByDeptorCreditorEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {

        var listSummary = entries.Where(a => a.Ledger.Id == debtorCreditorEntry.Ledger.Id &&
                                        a.Currency.Code == debtorCreditorEntry.Currency.Code &&
                                        a.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(debtorCreditorEntry);
          returnedEntries.AddRange(listSummary);
        }
      }
      foreach (var debtorCreditorEntry in totalByDeptorCreditorEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        var listSummary = entries.Where(a => a.Ledger.Id == debtorCreditorEntry.Ledger.Id &&
                                        a.Currency.Code == debtorCreditorEntry.Currency.Code &&
                                        a.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(debtorCreditorEntry);
          returnedEntries.AddRange(listSummary);
        }
      }

      return returnedEntries.ToFixedList();
    }

    internal FixedList<TwoCurrenciesBalanceEntry> CombineTotalConsolidatedAndPostingEntries
                                    (FixedList<TwoCurrenciesBalanceEntry> twoColumnsEntries, 
                                     List<TwoCurrenciesBalanceEntry> summaryTwoColumnsBalanceTotal) {

      var returnedEntries = new List<TwoCurrenciesBalanceEntry>(twoColumnsEntries);

      var totalBalance = summaryTwoColumnsBalanceTotal.FirstOrDefault(
                                  a => a.ItemType == TrialBalanceItemType.BalanceTotalConsolidated);

      if (totalBalance != null) {
        returnedEntries.Add(totalBalance);
      }

      return returnedEntries.ToFixedList();
      ;
    }

    internal List<TwoCurrenciesBalanceEntry> GenerateTotalSummary(
                                             List<TwoCurrenciesBalanceEntry> balanceEntries) {

      var totalSummary = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>(balanceEntries.Count);

      foreach (var debtorCreditor in balanceEntries) {

        TwoCurrenciesBalanceEntry entry = TrialBalanceMapper.MapTwoCurrenciesBalance(debtorCreditor);

        if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
          entry.DomesticBalance = -1 * entry.DomesticBalance;
          entry.ForeignBalance = -1 * entry.ForeignBalance;
          entry.TotalBalance = -1 * entry.TotalBalance;
        }
        entry.GroupName = "TOTAL DEL REPORTE";

        string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

        GenerateOrIncreaseTotalTwoCurrenciesBalance(totalSummary, entry, 
                                                    StandardAccount.Empty, Sector.Empty, 
                                                    TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      balanceEntries.AddRange(totalSummary.Values.ToList());

      return balanceEntries;
    }


    internal List<TwoCurrenciesBalanceEntry> GetTotalDeptorCreditorTwoColumnsEntries(
                                                  FixedList<TwoCurrenciesBalanceEntry> entries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>(entries.Count);

      foreach (var entry in entries.Where(a => a.GroupNumber != "" && a.NotHasSector && a.Level == 1)) {

        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry,
                                         TrialBalanceItemType.BalanceTotalDebtor);
        }
        if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry,
                                         TrialBalanceItemType.BalanceTotalCreditor);
        }
      }

      return totalSummaryDebtorCredtor.Values.ToList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> GetTotalSummaryGroup(
                                                   FixedList<TwoCurrenciesBalanceEntry> entries) {

      var summaryByGroup = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>(entries.Count);

      foreach (var entry in entries.Where(a => a.NotHasSector && a.Level == 1)) {
        if (entry.Account.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByTwoColumnsGroupEntries(summaryByGroup, entry, 
                                          TrialBalanceItemType.BalanceTotalGroupDebtor);
        }

        if (entry.Account.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByTwoColumnsGroupEntries(summaryByGroup, entry, 
                                          TrialBalanceItemType.BalanceTotalGroupCreditor);
        }
      }
      return summaryByGroup.ToFixedList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> MergeAccountsIntoTwoColumns(List<TrialBalanceEntry> trialBalance) {
      var targetCurrency = Currency.Parse(_command.ValuateToCurrrencyUID);
      var summaryEntries = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}||{entry.Account.DebtorCreditor}";
        Currency currentCurrency = entry.Currency;

        if (entry.Currency.Equals(targetCurrency)) {
          GenerateOrIncreaseTwoCurrenciesBalanceEntry(summaryEntries, entry, hash,
                                                      targetCurrency, currentCurrency);
        } else if (summaryEntries.ContainsKey(hash)) {
          SumTwoCurrenciesBalanceEntry(summaryEntries[hash], entry, targetCurrency, currentCurrency);
        } else {
          entry.Currency = targetCurrency;
          GenerateOrIncreaseTwoCurrenciesBalanceEntry(summaryEntries, entry, hash,
                                                      targetCurrency, currentCurrency);
        }
      }

      List<TwoCurrenciesBalanceEntry> returnedEntries = summaryEntries.Values.ToList();

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenByDescending(a => a.Account.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToList().ToFixedList();
    }


    #region Private methods

    private void GenerateOrIncreaseTwoCurrenciesBalanceEntry(
                          EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                          TrialBalanceEntry entry, string hash, Currency targetCurrency,
                          Currency currentCurrency) {

      TwoCurrenciesBalanceEntry summaryEntry;
      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {
        summaryEntries.Insert(hash, entry.MapToTwoColumnsBalanceEntry());
        SumTwoCurrenciesBalanceEntry(summaryEntries[hash], entry, targetCurrency, currentCurrency);
      } else {
        SumTwoCurrenciesBalanceEntry(summaryEntry, entry, targetCurrency, currentCurrency);
      }
    }


    private void SumTwoCurrenciesBalanceEntry(TwoCurrenciesBalanceEntry twoCurrenciesEntry,
                                               TrialBalanceEntry entry,
                                               Currency targetCurrency, Currency currentCurrency) {

      if (currentCurrency != targetCurrency && entry.Currency.Code != "44") {
        twoCurrenciesEntry.ForeignBalance += entry.CurrentBalance;
      } else {
        twoCurrenciesEntry.DomesticBalance += entry.CurrentBalance;
      }
      twoCurrenciesEntry.TotalBalance = twoCurrenciesEntry.DomesticBalance + twoCurrenciesEntry.ForeignBalance;
    }


    private void SummaryByDebtorCreditorEntries(
                  EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                  TwoCurrenciesBalanceEntry balanceEntry, TrialBalanceItemType itemType) {

      TwoCurrenciesBalanceEntry entry = TrialBalanceMapper.MapTwoCurrenciesBalance(balanceEntry);

      if (itemType == TrialBalanceItemType.BalanceTotalDebtor) {
        entry.GroupName = "TOTAL DEUDORAS";
      } else if (itemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.GroupName = "TOTAL ACREEDORAS";
      }
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;

      string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalTwoCurrenciesBalance(summaryEntries, entry, StandardAccount.Empty, Sector.Empty, itemType, hash);
    }


    private void SummaryByTwoColumnsGroupEntries(EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                                                 TwoCurrenciesBalanceEntry balanceEntry,
                                                 TrialBalanceItemType itemType) {

      TwoCurrenciesBalanceEntry entry = TrialBalanceMapper.MapTwoCurrenciesBalance(balanceEntry);

      entry.GroupName = $"TOTAL GRUPO {entry.Account.GroupNumber}";
      entry.GroupNumber = $"{entry.Account.GroupNumber}";

      string hash = $"{entry.GroupNumber}||{Sector.Empty.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalTwoCurrenciesBalance(summaryEntries, entry, StandardAccount.Empty, Sector.Empty, itemType, hash);
    }


    private void GenerateOrIncreaseTotalTwoCurrenciesBalance(EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                                           TwoCurrenciesBalanceEntry entry,
                                           StandardAccount targetAccount, Sector targetSector,
                                           TrialBalanceItemType itemType, string hash) {

      TwoCurrenciesBalanceEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TwoCurrenciesBalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = targetSector,
          Account = targetAccount,
          ItemType = itemType,
          GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName,
          DebtorCreditor = entry.DebtorCreditor
        };

        summaryEntry.Sum(entry);
        summaryEntries.Insert(hash, summaryEntry);

      } else {
        summaryEntry.Sum(entry);
      }
    }

    #endregion Private methods


  } // class TwoCurrenciesBalanceHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
