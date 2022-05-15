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
  internal class AnaliticoDeCuentasHelper {

    private readonly TrialBalanceCommand _command;

    internal AnaliticoDeCuentasHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal FixedList<AnaliticoDeCuentasEntry> CombineSummaryGroupsAndEntries(
                                                  FixedList<AnaliticoDeCuentasEntry> entries,
                                                  FixedList<AnaliticoDeCuentasEntry> totalByGroup) {
      List<AnaliticoDeCuentasEntry> returnedEntries = new List<AnaliticoDeCuentasEntry>();

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


    internal FixedList<AnaliticoDeCuentasEntry> CombineSubledgerAccountsWithSummaryEntries(
                                                  FixedList<AnaliticoDeCuentasEntry> analyticEntries,
                                                  List<TrialBalanceEntry> trialBalance) {
      if (!_command.WithSubledgerAccount) {
        return analyticEntries;
      }

      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);
      var returnedEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var analyticEntry in analyticEntries) {

        var balanceEntries = trialBalance.Where(a => a.ItemType == TrialBalanceItemType.Entry &&
                                                a.Ledger.Number == analyticEntry.Ledger.Number &&
                                                a.Account.Number == analyticEntry.Account.Number &&
                                                a.Sector.Code == analyticEntry.Sector.Code &&
                                                a.SubledgerAccountIdParent > 0 &&
                                                a.SubledgerAccountNumber != "")
                                          .ToList();

        var hashEntries = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

        foreach (var entry in balanceEntries) {

          string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                        $"{entry.Ledger.Id}||{entry.DebtorCreditor}||{entry.SubledgerAccountIdParent}";

          Currency currentCurrency = entry.Currency;
          MergeEntriesIntoTwoColumns(hashEntries, entry, hash, currentCurrency);
        }

        var balanceEntriesList = hashEntries.Values.OrderBy(a => a.SubledgerNumberOfDigits)
                                                   .ThenBy(a => a.SubledgerAccountNumber)
                                                   .ToList();
        returnedEntries.Add(analyticEntry);
        returnedEntries.AddRange(balanceEntriesList);
      }

      return returnedEntries.ToFixedList();
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


    internal FixedList<AnaliticoDeCuentasEntry> CombineTotalConsolidatedAndEntries(
                                     FixedList<AnaliticoDeCuentasEntry> analyticEntries,
                                     List<AnaliticoDeCuentasEntry> totalsByLedgerList) {
      if (totalsByLedgerList.Count == 0 || analyticEntries.Count == 0) {
        return analyticEntries;
      }

      var returnedEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var totalByLedger in totalsByLedgerList) {
        var entriesByLedger = analyticEntries.Where(a => a.Currency == totalByLedger.Currency &&
                                                         a.Ledger == totalByLedger.Ledger)
                                             .ToList();
        if (entriesByLedger.Count > 0) {
          entriesByLedger.Add(totalByLedger);
          returnedEntries.AddRange(entriesByLedger);
        }
      }
      return returnedEntries.ToFixedList();
    }


    internal FixedList<AnaliticoDeCuentasEntry> CombineTotalDebtorCreditorAndEntries
                            (List<AnaliticoDeCuentasEntry> entries,
                             List<AnaliticoDeCuentasEntry> totalByDebtorCreditorEntries) {
      List<AnaliticoDeCuentasEntry> returnedEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var debtorEntry in totalByDebtorCreditorEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {

        var listSummaryDebtors = entries.Where(a => a.Ledger.Id == debtorEntry.Ledger.Id &&
                                               a.Currency.Code == debtorEntry.Currency.Code &&
                                               a.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (listSummaryDebtors.Count > 0) {
          listSummaryDebtors.Add(debtorEntry);
          returnedEntries.AddRange(listSummaryDebtors);
        }
      }
      foreach (var creditorEntry in totalByDebtorCreditorEntries
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


    internal FixedList<AnaliticoDeCuentasEntry> GenerateAverageBalance(
                                                    FixedList<AnaliticoDeCuentasEntry> analyticEntries,
                                                    BalanceEngineCommandPeriod commandPeriod) {
      if (!_command.WithAverageBalance) {
        return analyticEntries;
      }

      FixedList<AnaliticoDeCuentasEntry> returnedBalances =
                                      new FixedList<AnaliticoDeCuentasEntry>(analyticEntries);

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

      return returnedBalances;
    }


    internal List<AnaliticoDeCuentasEntry> GenerateTotalReport(
                                             List<AnaliticoDeCuentasEntry> balanceEntries) {
      var totalSummary = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

      foreach (var balanceEntry in balanceEntries) {
        AnaliticoDeCuentasEntry analyticEntry = TrialBalanceMapper.MapToAnalyticBalanceEntry(balanceEntry);

        if (balanceEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          analyticEntry.DomesticBalance = -1 * analyticEntry.DomesticBalance;
          analyticEntry.ForeignBalance = -1 * analyticEntry.ForeignBalance;
          analyticEntry.TotalBalance = -1 * analyticEntry.TotalBalance;
        }
        analyticEntry.Account = StandardAccount.Empty;
        analyticEntry.Sector = Sector.Empty;
        analyticEntry.GroupName = "TOTAL DEL REPORTE";

        string hash = $"{analyticEntry.GroupName}||{Sector.Empty.Code}||{analyticEntry.Currency.Id}||{analyticEntry.Ledger.Id}";

        GenerateOrIncreaseTotalBalance(totalSummary, analyticEntry,
                                                    TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      return totalSummary.Values.ToList();
    }


    internal void GetSummaryToSectorZeroForPesosAndUdis(
                    List<TrialBalanceEntry> postingEntries,
                    List<TrialBalanceEntry> summaryEntries) {

      SummaryToSectorZeroForPesosAndUdis(postingEntries);
      SummaryToSectorZeroForPesosAndUdis(summaryEntries);
    }


    internal List<AnaliticoDeCuentasEntry> GetTotalDebtorCreditorEntries(
                                                  FixedList<AnaliticoDeCuentasEntry> entries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<AnaliticoDeCuentasEntry>(entries.Count);
      var summaryEntries = entries.Where(a => a.ItemType != TrialBalanceItemType.BalanceTotalGroupDebtor &&
                                              a.ItemType != TrialBalanceItemType.BalanceTotalGroupCreditor)
                                  .ToFixedList();

      List<AnaliticoDeCuentasEntry> listEntries = GetFirstLevelEntriesToGroup(summaryEntries);

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


    internal FixedList<AnaliticoDeCuentasEntry> GetTotalSummaryByGroup(
                                                   FixedList<AnaliticoDeCuentasEntry> entries) {

      var totalsByGroup = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

      List<AnaliticoDeCuentasEntry> listEntries = GetFirstLevelEntriesToGroup(entries);

      foreach (var entry in listEntries) {
        if (entry.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByGroupEntries(totalsByGroup, entry,
                                          TrialBalanceItemType.BalanceTotalGroupDebtor);
        } else if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByGroupEntries(totalsByGroup, entry,
                                          TrialBalanceItemType.BalanceTotalGroupCreditor);
        }
      }
      return totalsByGroup.ToFixedList();
    }


    internal FixedList<AnaliticoDeCuentasEntry> MergeTrialBalanceIntoAnalyticColumns(
                                                  List<TrialBalanceEntry> trialBalance) {

      var hashSummaryEntries = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();
      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);

      IEnumerable<TrialBalanceEntry> entryList;

      if (_command.WithSubledgerAccount) {
        entryList = trialBalance.Where(a => a.SubledgerAccountId == 0 &&
                                            a.ItemType == TrialBalanceItemType.Summary);
      } else {
        entryList = trialBalance.Where(a => a.SubledgerAccountNumber.Length <= 1);
      }

      foreach (var entry in entryList) {
        if (entry.CurrentBalance != 0) {
          string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                        $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

          Currency currentCurrency = entry.Currency;
          MergeEntriesIntoTwoColumns(hashSummaryEntries, entry, hash, currentCurrency);

        }
      }

      ICollection<AnaliticoDeCuentasEntry> analyticEntries = hashSummaryEntries.Values;

      MergeDomesticBalancesIntoSectorZero(analyticEntries, entryList);
      MergeForeignBalancesIntoSectorZero(analyticEntries, entryList);

      return analyticEntries.OrderBy(a => a.Ledger.Number)
                            .ThenByDescending(a => a.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToFixedList();
    }


    #region Private methods

    private List<AnaliticoDeCuentasEntry> GetFirstLevelEntriesToGroup(
                                              FixedList<AnaliticoDeCuentasEntry> entries) {

      var listEntries = new List<AnaliticoDeCuentasEntry>();

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
          // no-op
        }
      }

      return listEntries;
    }


    private void GenerateOrIncreaseTotalBalance(
                  EmpiriaHashTable<AnaliticoDeCuentasEntry> summaryEntries,
                  AnaliticoDeCuentasEntry entry,
                  TrialBalanceItemType itemType, string hash) {

      AnaliticoDeCuentasEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry != null) {
        summaryEntry.Sum(entry);
        return;
      }

      summaryEntry = new AnaliticoDeCuentasEntry {
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
    }


    private void GenerateOrIncreaseTwoColumnEntry(
                          EmpiriaHashTable<AnaliticoDeCuentasEntry> summaryEntries,
                          TrialBalanceEntry entry, string hash,
                          Currency currentCurrency) {

      AnaliticoDeCuentasEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {
        summaryEntries.Insert(hash, entry.MapToAnalyticBalanceEntry());
        SumTwoColumnEntry(summaryEntries[hash], entry, currentCurrency);

      } else {
        SumTwoColumnEntry(summaryEntry, entry, currentCurrency);

      }
    }


    private List<TrialBalanceEntry> GetSubledgerAccountInfo(List<TrialBalanceEntry> entriesList) {
      if (!_command.WithSubledgerAccount) {
        return entriesList;
      }

      var returnedEntries = new List<TrialBalanceEntry>(entriesList);

      foreach (var entry in entriesList) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        if (!subledgerAccount.IsEmptyInstance) {
          entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ?
                                          subledgerAccount.Number : "";
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber != "" ?
                                          entry.SubledgerAccountNumber.Count() : 0;
        }
      }

      return returnedEntries;
    }


    private void MergeEntriesIntoTwoColumns(EmpiriaHashTable<AnaliticoDeCuentasEntry> hashEntries,
                                              TrialBalanceEntry entry, string hash, Currency currentCurrency) {
      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);

      if (entry.Currency.Equals(targetCurrency)) {

        GenerateOrIncreaseTwoColumnEntry(hashEntries, entry, hash, currentCurrency);

      } else if (hashEntries.ContainsKey(hash)) {

        SumTwoColumnEntry(hashEntries[hash], entry, currentCurrency);

      } else {

        entry.Currency = targetCurrency;
        GenerateOrIncreaseTwoColumnEntry(hashEntries, entry, hash, currentCurrency);
      }
    }


    private void MergeDomesticBalancesIntoSectorZero(IEnumerable<AnaliticoDeCuentasEntry> analyticEntries,
                                                     IEnumerable<TrialBalanceEntry> summaryEntries) {
      if (!_command.UseNewSectorizationModel) {
        return;
      }

      foreach (var entry in analyticEntries.Where(a => a.Sector.Code == "00" && a.DomesticBalance == 0)) {
        var balancesWithDomesticCurrency = summaryEntries.Where(
              a => a.Account.Number == entry.Account.Number && a.Ledger.Number == entry.Ledger.Number &&
              a.Sector.Code != "00" && a.DebtorCreditor == entry.DebtorCreditor &&
              (a.Currency == Currency.MXN || a.Currency == Currency.UDI)).ToList();

        if (balancesWithDomesticCurrency.Count > 0) {
          entry.DomesticBalance = 0;
          entry.InitialBalance = 0;
          entry.Debit = 0;
          entry.Credit = 0;
          entry.TotalBalance = 0;
          entry.AverageBalance = 0;

          foreach (var foreignEntry in balancesWithDomesticCurrency) {
            SumTwoColumnEntry(entry, foreignEntry, foreignEntry.Currency);
          }
        }
      }
    }


    private void MergeForeignBalancesIntoSectorZero(IEnumerable<AnaliticoDeCuentasEntry> analyticEntries,
                                                    IEnumerable<TrialBalanceEntry> summaryEntries) {

      if (!_command.UseNewSectorizationModel) {
        return;
      }

      foreach (var entry in analyticEntries.Where(a => a.Sector.Code == "00" && a.Level > 1)) {

        var entriesWithForeignCurrency = summaryEntries.Where(
              a => a.Account.Number == entry.Account.Number && a.Ledger.Number == entry.Ledger.Number &&
              a.Sector.Code != "00" && a.DebtorCreditor == entry.DebtorCreditor &&
              a.Currency != Currency.MXN && a.Currency != Currency.UDI).ToList();

        if (entriesWithForeignCurrency.Count > 0) {

          entry.ForeignBalance = 0;
          foreach (var foreignEntry in entriesWithForeignCurrency) {
            SumTwoColumnEntry(entry, foreignEntry, foreignEntry.Currency);
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


    private void SumTwoColumnEntry(AnaliticoDeCuentasEntry analyticEntry,
                                   TrialBalanceEntry balanceEntry,
                                   Currency currentCurrency) {

      if (balanceEntry.DebtorCreditor != analyticEntry.DebtorCreditor) {
        return;
      }

      var targetCurrency = Currency.Parse(_command.InitialPeriod.ValuateToCurrrencyUID);

      if (currentCurrency != targetCurrency && !balanceEntry.Currency.Equals(Currency.UDI) &&
          !currentCurrency.Equals(Currency.UDI)) {

        analyticEntry.ForeignBalance += balanceEntry.CurrentBalance;

      } else {

        if (balanceEntry.Level == 1 && (currentCurrency.Equals(Currency.MXN) || currentCurrency.Equals(Currency.UDI))) {
          analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

        } else if (balanceEntry.Sector.Code == "00" && currentCurrency.Equals(Currency.MXN)) {
          analyticEntry.DomesticBalance = balanceEntry.CurrentBalance;

        } else if (balanceEntry.Sector.Code != "00" &&
                    (currentCurrency.Equals(Currency.MXN) || currentCurrency.Equals(Currency.UDI))) {
          analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

        } else if (currentCurrency.Equals(Currency.UDI) && balanceEntry.IsSummaryForAnalytics) {
          analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

        } else if (!_command.UseNewSectorizationModel && currentCurrency.Equals(Currency.UDI)) {
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


    private void SummaryByDebtorCreditorEntries(
                  EmpiriaHashTable<AnaliticoDeCuentasEntry> summaryEntries,
                  AnaliticoDeCuentasEntry balanceEntry, TrialBalanceItemType itemType) {

      AnaliticoDeCuentasEntry entry = TrialBalanceMapper.MapToAnalyticBalanceEntry(balanceEntry);

      if (itemType == TrialBalanceItemType.BalanceTotalDebtor) {
        entry.GroupName = "TOTAL DEUDORAS";
      } else if (itemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.GroupName = "TOTAL ACREEDORAS";
      }
      entry.Account = StandardAccount.Empty;
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;

      string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalBalance(summaryEntries, entry, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<AnaliticoDeCuentasEntry> summaryEntries,
                                                 AnaliticoDeCuentasEntry balanceEntry,
                                                 TrialBalanceItemType itemType) {

      AnaliticoDeCuentasEntry entry = TrialBalanceMapper.MapToAnalyticBalanceEntry(balanceEntry);

      entry.GroupName = $"TOTAL GRUPO {entry.Account.GroupNumber}";
      entry.GroupNumber = $"{entry.Account.GroupNumber}";

      string hash = $"{entry.GroupNumber}||{Sector.Empty.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalBalance(summaryEntries, entry, itemType, hash);
    }


    private void SummaryToSectorZeroForPesosAndUdis(List<TrialBalanceEntry> entries) {

      if (!_command.UseNewSectorizationModel) {
        return;
      }

      var entriesWithUdisCurrency = entries.Where(a => a.Level > 1 &&
                                                  a.Currency.Code == "44" &&
                                                  a.Sector.Code == "00")
                                            .ToList();

      foreach (var entryUdis in entriesWithUdisCurrency) {
        var entry = entries.FirstOrDefault(a => a.Account.Number == entryUdis.Account.Number &&
                                            a.Ledger.Number == entryUdis.Ledger.Number &&
                                            a.Currency.Code == "01" &&
                                            a.Sector.Code == "00" &&
                                            a.DebtorCreditor == entryUdis.DebtorCreditor);

        if (entry != null) {
          entry.InitialBalance += entryUdis.InitialBalance;
          entry.Debit += entryUdis.Debit;
          entry.Credit += entryUdis.Credit;
          entry.CurrentBalance += entryUdis.CurrentBalance;
          entry.AverageBalance += entryUdis.AverageBalance;
        } else {
          entryUdis.IsSummaryForAnalytics = true;
        }
      }
    }

    #endregion Private methods


  } // class AccountAnalyticsHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
