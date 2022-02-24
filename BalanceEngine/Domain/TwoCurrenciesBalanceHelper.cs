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

      foreach (var debtorsGroup in totalByGroup
                    .Where(a => a.DebtorCreditor == DebtorCreditorType.Deudora)) {
        var debtorEntries = entries.Where(a => a.Account.GroupNumber == debtorsGroup.GroupNumber &&
                                          a.Ledger.Id == debtorsGroup.Ledger.Id &&
                                          a.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (debtorEntries.Count > 0) {
          debtorEntries.Add(debtorsGroup);
          returnedEntries.AddRange(debtorEntries);
        }
      }
      foreach (var creditorsGroup in totalByGroup
                    .Where(a => a.DebtorCreditor == DebtorCreditorType.Acreedora)) {
        var creditorEntries = entries.Where(a => a.Account.GroupNumber == creditorsGroup.GroupNumber &&
                                          a.Ledger.Id == creditorsGroup.Ledger.Id &&
                                          a.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (creditorEntries.Count > 0) {
          creditorEntries.Add(creditorsGroup);
          returnedEntries.AddRange(creditorEntries);
        }
      }
      return returnedEntries.ToFixedList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> CombineSubledgerAccountsWithSummaryEntries(
                                                  FixedList<TwoCurrenciesBalanceEntry> twoColumnsEntries,
                                                  List<TrialBalanceEntry> trialBalance) {
      if (_command.WithSubledgerAccount) {
        var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);
        var returnedEntries = new List<TwoCurrenciesBalanceEntry>();

        foreach (var summary in twoColumnsEntries) {
          var balanceEntries = trialBalance.Where(a => a.ItemType == TrialBalanceItemType.Entry &&
                                                  a.Account.Number == summary.Account.Number &&
                                                  a.Sector.Code == summary.Sector.Code &&
                                                  a.SubledgerAccountIdParent > 0 &&
                                                  a.SubledgerAccountNumber != "")
                                           .ToList();

          var hashBalanceEntries = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>();
          foreach (var entry in balanceEntries) {
            string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                        $"{entry.Ledger.Id}||{entry.DebtorCreditor}||{entry.SubledgerAccountIdParent}";
            Currency currentCurrency = entry.Currency;
            MergeAccountsIntoTwoColumns(hashBalanceEntries, entry, hash, currentCurrency);
          }
          var balanceEntriesList = hashBalanceEntries.Values.OrderBy(a => a.SubledgerNumberOfDigits)
                                                        .ThenBy(a => a.SubledgerAccountNumber).ToList();
          returnedEntries.Add(summary);
          returnedEntries.AddRange(balanceEntriesList);
        }
        return returnedEntries.ToFixedList();
      } else {
        return twoColumnsEntries;
      }

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
    }


    internal FixedList<TwoCurrenciesBalanceEntry> CombineTotalDeptorCreditorAndTwoColumnsEntries
                            (List<TwoCurrenciesBalanceEntry> entries,
                             List<TwoCurrenciesBalanceEntry> totalByDeptorCreditorEntries) {
      List<TwoCurrenciesBalanceEntry> returnedEntries = new List<TwoCurrenciesBalanceEntry>();

      foreach (var debtorEntry in totalByDeptorCreditorEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {

        var listSummaryDebtors = entries.Where(a => a.Ledger.Id == debtorEntry.Ledger.Id &&
                                               a.Currency.Code == debtorEntry.Currency.Code &&
                                               a.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (listSummaryDebtors.Count > 0) {
          listSummaryDebtors.Add(debtorEntry);
          returnedEntries.AddRange(listSummaryDebtors);
        }
      }
      foreach (var creditorEntry in totalByDeptorCreditorEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        var listSummaryCreditros = entries.Where(a => a.Ledger.Id == creditorEntry.Ledger.Id &&
                                                 a.Currency.Code == creditorEntry.Currency.Code &&
                                                 a.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (listSummaryCreditros.Count > 0) {
          listSummaryCreditros.Add(creditorEntry);
          returnedEntries.AddRange(listSummaryCreditros);
        }
      }
      return returnedEntries.ToFixedList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> GenerateAverageTwoColumnsBalance(
                                                    FixedList<TwoCurrenciesBalanceEntry> twoColumnsBalance,
                                                    TrialBalanceCommandPeriod commandPeriod) {
      FixedList<TwoCurrenciesBalanceEntry> returnedBalances =
                                            new FixedList<TwoCurrenciesBalanceEntry>(twoColumnsBalance);
      if (_command.WithAverageBalance) {
        //TimeSpan timeSpan = commandPeriod.ToDate - commandPeriod.FromDate;
        //int numberOfDays = timeSpan.Days + 1;

        //foreach (var entry in returnedBalances) {
        //  entry.AverageBalance = (entry.TotalBalance / numberOfDays) + entry.InitialBalance;
        //}

        foreach (var entry in returnedBalances.Where(a => a.ItemType == TrialBalanceItemType.Entry ||
                                                        a.ItemType == TrialBalanceItemType.Summary)) {
          decimal debtorCreditor = entry.DebtorCreditor == DebtorCreditorType.Deudora ?
                                   entry.Debit - entry.Credit : entry.Credit - entry.Debit;

          TimeSpan timeSpan = commandPeriod.ToDate - entry.LastChangeDate;
          int numberOfDays = timeSpan.Days + 1;

          entry.AverageBalance = ((numberOfDays * debtorCreditor) /
                                   _command.InitialPeriod.ToDate.Day) +
                                   entry.InitialBalance;
        }
      }

      return returnedBalances;
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
        entry.Account = StandardAccount.Empty;
        entry.GroupName = "TOTAL DEL REPORTE";

        string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

        GenerateOrIncreaseTotalTwoCurrenciesBalance(totalSummary, entry,
                                                    TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      balanceEntries.AddRange(totalSummary.Values.ToList());

      return balanceEntries;
    }


    internal List<TwoCurrenciesBalanceEntry> GetTotalDeptorCreditorTwoColumnsEntries(
                                                  FixedList<TwoCurrenciesBalanceEntry> entries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>(entries.Count);
      var listEntries = new List<TwoCurrenciesBalanceEntry>();
      var summaryEntries = entries.Where(a => a.ItemType != TrialBalanceItemType.BalanceTotalGroupDebtor &&
                                              a.ItemType != TrialBalanceItemType.BalanceTotalGroupCreditor)
                                  .ToList().ToFixedList();

      GenerateListSummaryGroupEntries(summaryEntries, listEntries);

      foreach (var entry in listEntries) {

        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry,
                                         TrialBalanceItemType.BalanceTotalDebtor);
        } else if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry,
                                         TrialBalanceItemType.BalanceTotalCreditor);
        }
      }

      return totalSummaryDebtorCredtor.Values.ToList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> GetTotalSummaryGroup(
                                                   FixedList<TwoCurrenciesBalanceEntry> entries) {

      var summaryByGroup = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>(entries.Count);

      var listEntries = new List<TwoCurrenciesBalanceEntry>();

      GenerateListSummaryGroupEntries(entries, listEntries);

      foreach (var entry in listEntries) {
        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByTwoColumnsGroupEntries(summaryByGroup, entry,
                                          TrialBalanceItemType.BalanceTotalGroupDebtor);
        } else if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByTwoColumnsGroupEntries(summaryByGroup, entry,
                                          TrialBalanceItemType.BalanceTotalGroupCreditor);
        }
      }
      return summaryByGroup.ToFixedList();
    }


    internal FixedList<TwoCurrenciesBalanceEntry> MergeSummaryEntriesIntoTwoColumns(
                                                  List<TrialBalanceEntry> trialBalance) {

      var hashSummaryEntries = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>();
      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);
      var summaryEntries = new List<TrialBalanceEntry>();
      if (_command.WithSubledgerAccount) {
        summaryEntries = trialBalance.Where(a => a.SubledgerAccountId == 0 &&
                                            a.ItemType == TrialBalanceItemType.Summary).ToList();
      } else {
        summaryEntries = trialBalance.Where(a => a.SubledgerAccountNumber.Length <= 1).ToList();
      }

      foreach (var entry in summaryEntries) {
        if (entry.CurrentBalance != 0) {
          string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                      $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

          Currency currentCurrency = entry.Currency;
          MergeAccountsIntoTwoColumns(hashSummaryEntries, entry, hash, currentCurrency);

        }
      }
      List<TwoCurrenciesBalanceEntry> twoColumnsEntries = hashSummaryEntries.Values.ToList();

      MergeDomesticBalancesIntoSectorZero(twoColumnsEntries, summaryEntries);
      MergeForeignBalancesIntoSectorZero(twoColumnsEntries, summaryEntries);

      return twoColumnsEntries.OrderBy(a => a.Ledger.Number)
                            .ThenByDescending(a => a.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToList().ToFixedList();
    }


    #region Private methods

    private void GenerateListSummaryGroupEntries(FixedList<TwoCurrenciesBalanceEntry> entries,
                                                  List<TwoCurrenciesBalanceEntry> listEntries) {
      var isSummary = entries.Where(a => a.Level == 1).ToList();

      foreach (var summary in isSummary) {
        var summaryWithoutSector = entries.FirstOrDefault(
                                    a => a.Account.Number == summary.Account.Number &&
                                    a.DebtorCreditor == summary.DebtorCreditor && a.NotHasSector);

        if (summaryWithoutSector != null &&
              listEntries.FirstOrDefault(a =>
                          a.Account.Number == summaryWithoutSector.Account.Number &&
                          a.NotHasSector &&
                          a.DebtorCreditor == summaryWithoutSector.DebtorCreditor) == null) {

          listEntries.Add(summaryWithoutSector);
        } else if (summaryWithoutSector == null) {
          listEntries.Add(summary);
        } else {
          continue;
        }
      }
    }


    private void GenerateOrIncreaseTotalTwoCurrenciesBalance(
                  EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                  TwoCurrenciesBalanceEntry entry,
                  TrialBalanceItemType itemType, string hash) {

      TwoCurrenciesBalanceEntry summaryEntry;
      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TwoCurrenciesBalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = entry.Sector,
          Account = entry.Account,
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


    private void GenerateOrIncreaseTwoCurrenciesBalanceEntry(
                          EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                          TrialBalanceEntry entry, string hash,
                          Currency currentCurrency) {

      TwoCurrenciesBalanceEntry summaryEntry;
      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {
        summaryEntries.Insert(hash, entry.MapToTwoColumnsBalanceEntry());
        SumTwoCurrenciesBalanceEntry(summaryEntries[hash], entry, currentCurrency);
      } else {
        SumTwoCurrenciesBalanceEntry(summaryEntry, entry, currentCurrency);
      }
    }


    private void MergeAccountsIntoTwoColumns(EmpiriaHashTable<TwoCurrenciesBalanceEntry> hashEntries,
                                              TrialBalanceEntry entry, string hash, Currency currentCurrency) {
      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);

      if (entry.Currency.Equals(targetCurrency)) {

        GenerateOrIncreaseTwoCurrenciesBalanceEntry(hashEntries, entry, hash, currentCurrency);

      } else if (hashEntries.ContainsKey(hash)) {

        SumTwoCurrenciesBalanceEntry(hashEntries[hash], entry, currentCurrency);

      } else {

        entry.Currency = targetCurrency;
        GenerateOrIncreaseTwoCurrenciesBalanceEntry(hashEntries, entry, hash,
                                                    currentCurrency);
      }
    }


    private void MergeDomesticBalancesIntoSectorZero(List<TwoCurrenciesBalanceEntry> twoColumnsEntries,
                                                      List<TrialBalanceEntry> summaryEntries) {
      if (_command.UseNewSectorizationModel) {
        foreach (var entry in twoColumnsEntries.Where(a => a.Sector.Code == "00" && a.DomesticBalance == 0)) {
          var balancesWithDomesticCurrency = summaryEntries.Where(
                a => a.Account.Number == entry.Account.Number && a.Ledger.Number == entry.Ledger.Number &&
                a.Sector.Code != "00" && a.DebtorCreditor == entry.DebtorCreditor &&
                (a.Currency.Code == "01" || a.Currency.Code == "44")).ToList();

          if (balancesWithDomesticCurrency.Count > 0) {
            entry.DomesticBalance = 0;
            entry.InitialBalance = 0;
            entry.Debit = 0;
            entry.Credit = 0;
            entry.TotalBalance = 0;
            entry.AverageBalance = 0;

            foreach (var foreignEntry in balancesWithDomesticCurrency) {
              SumTwoCurrenciesBalanceEntry(entry, foreignEntry, foreignEntry.Currency);
            }
          }
        }
      }
    }


    private void MergeForeignBalancesIntoSectorZero(List<TwoCurrenciesBalanceEntry> twoColumnsEntries,
                                                      List<TrialBalanceEntry> summaryEntries) {

      if (_command.UseNewSectorizationModel) {
        foreach (var entry in twoColumnsEntries.Where(a => a.Sector.Code == "00" && a.Level > 1)) {
          var balancesWithForeignCurrency = summaryEntries.Where(
                a => a.Account.Number == entry.Account.Number && a.Ledger.Number == entry.Ledger.Number &&
                a.Sector.Code != "00" && a.DebtorCreditor == entry.DebtorCreditor &&
                a.Currency.Code != "01" && a.Currency.Code != "44").ToList();

          if (balancesWithForeignCurrency.Count > 0) {
            entry.ForeignBalance = 0;
            foreach (var foreignEntry in balancesWithForeignCurrency) {
              SumTwoCurrenciesBalanceEntry(entry, foreignEntry, foreignEntry.Currency);
            }
          }
        }
      }
    }


    private void SumTwoCurrenciesBalanceEntry(TwoCurrenciesBalanceEntry twoCurrenciesEntry,
                                               TrialBalanceEntry entry,
                                               Currency currentCurrency) {
      if (entry.DebtorCreditor == twoCurrenciesEntry.DebtorCreditor) {
        var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);

        if (currentCurrency != targetCurrency && entry.Currency.Code != "44" && 
            currentCurrency.Code != "44" ) {

          twoCurrenciesEntry.ForeignBalance += entry.CurrentBalance;

        } else {
          if (entry.Level == 1 && (currentCurrency.Code == "01" || currentCurrency.Code == "44")) {
            twoCurrenciesEntry.DomesticBalance += entry.CurrentBalance;
          } else if (entry.Sector.Code == "00" && currentCurrency.Code == "01") {
            twoCurrenciesEntry.DomesticBalance = entry.CurrentBalance;
          } else if (entry.Sector.Code != "00" &&
                     (currentCurrency.Code == "01" || currentCurrency.Code == "44")) {
            twoCurrenciesEntry.DomesticBalance += entry.CurrentBalance;
          } else if (currentCurrency.Code == "44" && entry.IsSummaryForAnalytics) {
            twoCurrenciesEntry.DomesticBalance += entry.CurrentBalance;
          }
        }
        twoCurrenciesEntry.InitialBalance += entry.InitialBalance;
        twoCurrenciesEntry.Debit += entry.Debit;
        twoCurrenciesEntry.Credit += entry.Credit;
        twoCurrenciesEntry.TotalBalance = twoCurrenciesEntry.DomesticBalance + twoCurrenciesEntry.ForeignBalance;
        twoCurrenciesEntry.AverageBalance += entry.AverageBalance;
        twoCurrenciesEntry.LastChangeDate = entry.LastChangeDate > twoCurrenciesEntry.LastChangeDate ?
                                            entry.LastChangeDate : twoCurrenciesEntry.LastChangeDate;
        if (_command.UseNewSectorizationModel) {
          entry.Currency = currentCurrency;
        }
      }
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
      entry.Account = StandardAccount.Empty;
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;

      string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalTwoCurrenciesBalance(summaryEntries, entry, itemType, hash);
    }


    private void SummaryByTwoColumnsGroupEntries(EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                                                 TwoCurrenciesBalanceEntry balanceEntry,
                                                 TrialBalanceItemType itemType) {

      TwoCurrenciesBalanceEntry entry = TrialBalanceMapper.MapTwoCurrenciesBalance(balanceEntry);

      entry.GroupName = $"TOTAL GRUPO {entry.Account.GroupNumber}";
      entry.GroupNumber = $"{entry.Account.GroupNumber}";

      string hash = $"{entry.GroupNumber}||{Sector.Empty.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalTwoCurrenciesBalance(summaryEntries, entry, itemType, hash);
    }



    #endregion Private methods


  } // class TwoCurrenciesBalanceHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
