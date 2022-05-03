/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : AccountAnalyticsHelper                     License   : Please read LICENSE.txt file            *
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
  internal class AnalyticHelper {

    private readonly TrialBalanceCommand _command;

    internal AnalyticHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal FixedList<AnalyticBalanceEntry> CombineGroupEntriesAndTwoColumnsEntries(
                                                  FixedList<AnalyticBalanceEntry> entries,
                                                  FixedList<AnalyticBalanceEntry> totalByGroup) {
      List<AnalyticBalanceEntry> returnedEntries = new List<AnalyticBalanceEntry>();

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


    internal FixedList<AnalyticBalanceEntry> CombineSubledgerAccountsWithSummaryEntries(
                                                  FixedList<AnalyticBalanceEntry> analyticEntries,
                                                  List<TrialBalanceEntry> trialBalance) {
      if (_command.WithSubledgerAccount) {

        var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);
        var returnedEntries = new List<AnalyticBalanceEntry>();

        foreach (var analyticEntry in analyticEntries) {

          var balanceEntries = trialBalance.Where(a => a.ItemType == TrialBalanceItemType.Entry &&
                                                  a.Ledger.Number == analyticEntry.Ledger.Number &&
                                                  a.Account.Number == analyticEntry.Account.Number &&
                                                  a.Sector.Code == analyticEntry.Sector.Code &&
                                                  a.SubledgerAccountIdParent > 0 &&
                                                  a.SubledgerAccountNumber != "")
                                           .ToList();

          var hashEntries = new EmpiriaHashTable<AnalyticBalanceEntry>();
          
          foreach (var entry in balanceEntries) {

            string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                        $"{entry.Ledger.Id}||{entry.DebtorCreditor}||{entry.SubledgerAccountIdParent}";

            Currency currentCurrency = entry.Currency;
            MergeAccountsIntoTwoColumns(hashEntries, entry, hash, currentCurrency);
          }
          var balanceEntriesList = hashEntries.Values.OrderBy(a => a.SubledgerNumberOfDigits)
                                                        .ThenBy(a => a.SubledgerAccountNumber).ToList();
          returnedEntries.Add(analyticEntry);
          returnedEntries.AddRange(balanceEntriesList);
        }

        return returnedEntries.ToFixedList();

      } else {
        return analyticEntries;
      }

    }


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(
                                      List<TrialBalanceEntry> summaryEntries,
                                      FixedList<TrialBalanceEntry> postingEntries) {
      var returnedEntries = new List<TrialBalanceEntry>(postingEntries);
      returnedEntries.AddRange(summaryEntries);
      returnedEntries = GetSubledgerAccountInfo(returnedEntries);
      returnedEntries = OrderingEntries(returnedEntries);

      return returnedEntries;
    }


    internal FixedList<AnalyticBalanceEntry> CombineTotalConsolidatedAndPostingEntries
                                    (FixedList<AnalyticBalanceEntry> twoColumnsEntries,
                                     List<AnalyticBalanceEntry> totalsByLedgerList) {
      var returnedEntries = new List<AnalyticBalanceEntry>();

      if (totalsByLedgerList.Count > 0 && twoColumnsEntries.Count > 0) {
        foreach (var totalByLedger in totalsByLedgerList) {
          var entriesByLedger = twoColumnsEntries.Where(a => a.Currency.Code == totalByLedger.Currency.Code &&
                                                        a.Ledger.Number == totalByLedger.Ledger.Number)
                                                 .ToList();
          if (entriesByLedger.Count > 0) {
            entriesByLedger.Add(totalByLedger);
            returnedEntries.AddRange(entriesByLedger);
          }
        }
        return returnedEntries.ToFixedList();
      }
      return twoColumnsEntries;
    }


    internal FixedList<AnalyticBalanceEntry> CombineTotalDeptorCreditorAndTwoColumnsEntries
                            (List<AnalyticBalanceEntry> entries,
                             List<AnalyticBalanceEntry> totalByDeptorCreditorEntries) {
      List<AnalyticBalanceEntry> returnedEntries = new List<AnalyticBalanceEntry>();

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


    internal FixedList<AnalyticBalanceEntry> GenerateAverageTwoColumnsBalance(
                                                    FixedList<AnalyticBalanceEntry> twoColumnsBalance,
                                                    TrialBalanceCommandPeriod commandPeriod) {
      FixedList<AnalyticBalanceEntry> returnedBalances =
                                            new FixedList<AnalyticBalanceEntry>(twoColumnsBalance);
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



    internal List<AnalyticBalanceEntry> GenerateTotalSummary(
                                             List<AnalyticBalanceEntry> balanceEntries) {
      var totalSummary = new EmpiriaHashTable<AnalyticBalanceEntry>();

      foreach (var entry in balanceEntries) {
        AnalyticBalanceEntry columnsEntry = TrialBalanceMapper.MapTwoCurrenciesBalance(entry);

        if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          columnsEntry.DomesticBalance = -1 * columnsEntry.DomesticBalance;
          columnsEntry.ForeignBalance = -1 * columnsEntry.ForeignBalance;
          columnsEntry.TotalBalance = -1 * columnsEntry.TotalBalance;
        }
        columnsEntry.Account = StandardAccount.Empty;
        columnsEntry.Sector = Sector.Empty;
        columnsEntry.GroupName = "TOTAL DEL REPORTE";

        string hash = $"{columnsEntry.GroupName}||{Sector.Empty.Code}||{columnsEntry.Currency.Id}||{columnsEntry.Ledger.Id}";

        GenerateOrIncreaseTotalTwoCurrenciesBalance(totalSummary, columnsEntry,
                                                    TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      return totalSummary.Values.ToList();
    }


    internal List<AnalyticBalanceEntry> GetTotalDeptorCreditorTwoColumnsEntries(
                                                  FixedList<AnalyticBalanceEntry> entries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<AnalyticBalanceEntry>(entries.Count);
      var summaryEntries = entries.Where(a => a.ItemType != TrialBalanceItemType.BalanceTotalGroupDebtor &&
                                              a.ItemType != TrialBalanceItemType.BalanceTotalGroupCreditor)
                                  .ToList().ToFixedList();

      List<AnalyticBalanceEntry> listEntries = GenerateListSummaryGroupEntries(summaryEntries);

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


    internal FixedList<AnalyticBalanceEntry> GetTotalSummaryByGroup(
                                                   FixedList<AnalyticBalanceEntry> entries) {

      var totalsByGroup = new EmpiriaHashTable<AnalyticBalanceEntry>();

      List<AnalyticBalanceEntry> listEntries = GenerateListSummaryGroupEntries(entries);

      foreach (var entry in listEntries) {
        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByTwoColumnsGroupEntries(totalsByGroup, entry,
                                          TrialBalanceItemType.BalanceTotalGroupDebtor);
        } else if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByTwoColumnsGroupEntries(totalsByGroup, entry,
                                          TrialBalanceItemType.BalanceTotalGroupCreditor);
        }
      }
      return totalsByGroup.ToFixedList();
    }


    internal FixedList<AnalyticBalanceEntry> MergeEntriesIntoTwoColumns(
                                                  List<TrialBalanceEntry> trialBalance) {

      var hashSummaryEntries = new EmpiriaHashTable<AnalyticBalanceEntry>();
      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);
      var entryList = new List<TrialBalanceEntry>();
      if (_command.WithSubledgerAccount) {
        entryList = trialBalance.Where(a => a.SubledgerAccountId == 0 &&
                                            a.ItemType == TrialBalanceItemType.Summary).ToList();
      } else {
        entryList = trialBalance.Where(a => a.SubledgerAccountNumber.Length <= 1).ToList();
      }

      foreach (var entry in entryList) {
        if (entry.CurrentBalance != 0) {
          string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                      $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

          Currency currentCurrency = entry.Currency;
          MergeAccountsIntoTwoColumns(hashSummaryEntries, entry, hash, currentCurrency);

        }
      }
      List<AnalyticBalanceEntry> analyticEntries = hashSummaryEntries.Values.ToList();

      MergeDomesticBalancesIntoSectorZero(analyticEntries, entryList);
      MergeForeignBalancesIntoSectorZero(analyticEntries, entryList);

      return analyticEntries.OrderBy(a => a.Ledger.Number)
                            .ThenByDescending(a => a.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToList().ToFixedList();
    }


    #region Private methods

    private List<AnalyticBalanceEntry> GenerateListSummaryGroupEntries(
                                              FixedList<AnalyticBalanceEntry> entries) {
      
      var listEntries = new List<AnalyticBalanceEntry>();

      foreach (var entry in entries.Where(a => a.Level == 1)) {
        var entryWithoutSector = entries.FirstOrDefault(
                                    a => a.Account.Number == entry.Account.Number &&
                                    a.Ledger.Number == entry.Ledger.Number &&
                                    a.DebtorCreditor == entry.DebtorCreditor && a.NotHasSector);

        if (entryWithoutSector != null &&
              listEntries.FirstOrDefault(a =>
                          a.Account.Number == entryWithoutSector.Account.Number &&
                          a.Ledger.Number == entryWithoutSector.Ledger.Number &&
                          a.DebtorCreditor == entryWithoutSector.DebtorCreditor &&
                          a.NotHasSector) == null) {

          listEntries.Add(entryWithoutSector);

        } else if (entryWithoutSector == null) {

          listEntries.Add(entry);
        } else {
          continue;
        }
      }
      return listEntries;
    }


    private void GenerateOrIncreaseTotalTwoCurrenciesBalance(
                  EmpiriaHashTable<AnalyticBalanceEntry> summaryEntries,
                  AnalyticBalanceEntry entry,
                  TrialBalanceItemType itemType, string hash) {

      AnalyticBalanceEntry summaryEntry;
      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new AnalyticBalanceEntry {
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
                          EmpiriaHashTable<AnalyticBalanceEntry> summaryEntries,
                          TrialBalanceEntry entry, string hash,
                          Currency currentCurrency) {

      AnalyticBalanceEntry summaryEntry;
      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {
        summaryEntries.Insert(hash, entry.MapToTwoColumnsBalanceEntry());
        SumTwoCurrenciesBalanceEntry(summaryEntries[hash], entry, currentCurrency);
      } else {
        SumTwoCurrenciesBalanceEntry(summaryEntry, entry, currentCurrency);
      }
    }


    private List<TrialBalanceEntry> GetSubledgerAccountInfo(List<TrialBalanceEntry> entriesList) {
      var returnedEntries = new List<TrialBalanceEntry>(entriesList);

      if (_command.WithSubledgerAccount) {
        foreach (var entry in entriesList) {
          SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
          if (!subledgerAccount.IsEmptyInstance) {
            entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ?
                                           subledgerAccount.Number : "";
            entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber != "" ?
                                            entry.SubledgerAccountNumber.Count() : 0;
          }
        }
      }
      return returnedEntries;
    }


    private void MergeAccountsIntoTwoColumns(EmpiriaHashTable<AnalyticBalanceEntry> hashEntries,
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


    private void MergeDomesticBalancesIntoSectorZero(List<AnalyticBalanceEntry> analyticEntries,
                                                      List<TrialBalanceEntry> summaryEntries) {
      if (_command.UseNewSectorizationModel) {
        foreach (var entry in analyticEntries.Where(a => a.Sector.Code == "00" && a.DomesticBalance == 0)) {
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


    private void MergeForeignBalancesIntoSectorZero(List<AnalyticBalanceEntry> analyticEntries,
                                                      List<TrialBalanceEntry> summaryEntries) {

      if (_command.UseNewSectorizationModel) {

        foreach (var entry in analyticEntries.Where(a => a.Sector.Code == "00" && a.Level > 1)) {

          var entriesWithForeignCurrency = summaryEntries.Where(
                a => a.Account.Number == entry.Account.Number && a.Ledger.Number == entry.Ledger.Number &&
                a.Sector.Code != "00" && a.DebtorCreditor == entry.DebtorCreditor &&
                a.Currency.Code != "01" && a.Currency.Code != "44").ToList();

          if (entriesWithForeignCurrency.Count > 0) {

            entry.ForeignBalance = 0;
            foreach (var foreignEntry in entriesWithForeignCurrency) {
              SumTwoCurrenciesBalanceEntry(entry, foreignEntry, foreignEntry.Currency);
            }
          }
        }
      }
    }


    private List<TrialBalanceEntry> OrderingEntries(List<TrialBalanceEntry> entries) {

      if (_command.WithSubledgerAccount) {

        return entries.OrderBy(a => a.Ledger.Number)
                      .ThenBy(a => a.Currency.Code)
                      .ThenByDescending(a => a.Account.DebtorCreditor)
                      .ThenBy(a => a.Account.Number)
                      .ThenBy(a => a.Sector.Code)
                      .ThenBy(a => a.SubledgerNumberOfDigits)
                      .ThenBy(a => a.SubledgerAccountNumber)
                      .ToList();
      } else {
        return entries.OrderBy(a => a.Ledger.Number)
                      .ThenBy(a => a.Currency.Code)
                      .ThenByDescending(a => a.Account.DebtorCreditor)
                      .ThenBy(a => a.Account.Number)
                      .ThenBy(a => a.Sector.Code)
                      .ThenBy(a => a.SubledgerAccountNumber)
                      .ToList();
      }
    }


    private void SumTwoCurrenciesBalanceEntry(AnalyticBalanceEntry analyticEntry,
                                               TrialBalanceEntry balanceEntry,
                                               Currency currentCurrency) {

      if (balanceEntry.DebtorCreditor == analyticEntry.DebtorCreditor) {
        var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);

        if (currentCurrency != targetCurrency && balanceEntry.Currency.Code != "44" &&
            currentCurrency.Code != "44") {

          analyticEntry.ForeignBalance += balanceEntry.CurrentBalance;

        } else {
          if (balanceEntry.Level == 1 && (currentCurrency.Code == "01" || currentCurrency.Code == "44")) {
            analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;
          } else if (balanceEntry.Sector.Code == "00" && currentCurrency.Code == "01") {
            analyticEntry.DomesticBalance = balanceEntry.CurrentBalance;
          } else if (balanceEntry.Sector.Code != "00" &&
                     (currentCurrency.Code == "01" || currentCurrency.Code == "44")) {
            analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;
          } else if (currentCurrency.Code == "44" && balanceEntry.IsSummaryForAnalytics) {
            analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;
          } else if (!_command.UseNewSectorizationModel && currentCurrency.Code == "44") {
            analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;
          }
        }
        analyticEntry.InitialBalance += balanceEntry.InitialBalance;
        analyticEntry.Debit += balanceEntry.Debit;
        analyticEntry.Credit += balanceEntry.Credit;
        analyticEntry.TotalBalance = analyticEntry.DomesticBalance + analyticEntry.ForeignBalance;
        analyticEntry.AverageBalance += balanceEntry.AverageBalance;
        analyticEntry.LastChangeDate = balanceEntry.LastChangeDate > analyticEntry.LastChangeDate ?
                                            balanceEntry.LastChangeDate : analyticEntry.LastChangeDate;
        if (_command.UseNewSectorizationModel) {
          balanceEntry.Currency = currentCurrency;
        }
      }
    }


    private void SummaryByDebtorCreditorEntries(
                  EmpiriaHashTable<AnalyticBalanceEntry> summaryEntries,
                  AnalyticBalanceEntry balanceEntry, TrialBalanceItemType itemType) {

      AnalyticBalanceEntry entry = TrialBalanceMapper.MapTwoCurrenciesBalance(balanceEntry);

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


    private void SummaryByTwoColumnsGroupEntries(EmpiriaHashTable<AnalyticBalanceEntry> summaryEntries,
                                                 AnalyticBalanceEntry balanceEntry,
                                                 TrialBalanceItemType itemType) {

      AnalyticBalanceEntry entry = TrialBalanceMapper.MapTwoCurrenciesBalance(balanceEntry);

      entry.GroupName = $"TOTAL GRUPO {entry.Account.GroupNumber}";
      entry.GroupNumber = $"{entry.Account.GroupNumber}";

      string hash = $"{entry.GroupNumber}||{Sector.Empty.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalTwoCurrenciesBalance(summaryEntries, entry, itemType, hash);
    }



    #endregion Private methods


  } // class AccountAnalyticsHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
