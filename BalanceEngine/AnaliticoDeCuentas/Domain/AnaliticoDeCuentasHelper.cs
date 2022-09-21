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

    private readonly TrialBalanceQuery _query;

    internal AnaliticoDeCuentasHelper(TrialBalanceQuery query) {
      _query = query;
    }

    #region Public methods


    internal List<AnaliticoDeCuentasEntry> MergeSubledgerAccountsWithAnalyticEntries(
                                                  List<AnaliticoDeCuentasEntry> analyticEntries,
                                                  List<TrialBalanceEntry> balanceEntries) {
      if (analyticEntries.Count == 0 || balanceEntries.Count == 0) {
        return new List<AnaliticoDeCuentasEntry>();
      }
      if (!_query.WithSubledgerAccount) {
        return analyticEntries;
      }

      var targetCurrency = Currency.Parse(_query.InitialPeriod.ValuateToCurrrencyUID);
      var returnedEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var analyticEntry in analyticEntries) {
        var hashEntries = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

        MergeSubledgerAccountsWithAnaliticEntry(balanceEntries, targetCurrency, analyticEntry, hashEntries);

        var balanceEntriesList = hashEntries.Values.OrderBy(a => a.SubledgerAccountNumber.Length)
                                                   .ThenBy(a => a.SubledgerAccountNumber)
                                                   .ToList();
        returnedEntries.Add(analyticEntry);
        returnedEntries.AddRange(balanceEntriesList);
      }

      return returnedEntries;
    }


    internal List<AnaliticoDeCuentasEntry> GenerateTotalReport(
                                             List<AnaliticoDeCuentasEntry> balanceEntries) {
      if (balanceEntries.Count == 0) {
        return new List<AnaliticoDeCuentasEntry>();
      }

      var totalSummary = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

      foreach (var balanceEntry in balanceEntries) {
        AnaliticoDeCuentasEntry analyticEntry = AnaliticoDeCuentasMapper.CreatePartialCopy(balanceEntry);

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


    internal List<TrialBalanceEntry> GetCalculatedParentAccounts(
                                     FixedList<TrialBalanceEntry> accountEntries) {
     
      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);
      var trialBalanceHelper = new TrialBalanceHelper(_query);

      foreach (var entry in accountEntries) {

        StandardAccount currentParent;

        bool isCalculatedAccount = trialBalanceHelper.ValidateEntryToAssignCurrentParentAccount(
                                                      entry, out currentParent);

        if (!trialBalanceHelper.ValidateEntryForSummaryParentAccount(entry, isCalculatedAccount)) {
          continue;
        }

        GenerateOrIncreaseParentAccountEntries(parentAccounts, entry, currentParent);

      } // foreach

      trialBalanceHelper.AssignLastChangeDatesToParentEntries(accountEntries, parentAccounts.ToFixedList());

      return parentAccounts.ToFixedList().ToList();
    }


    internal void GetSummaryToSectorZeroForPesosAndUdis(
                    List<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return; 
      }

      if (!_query.UseNewSectorizationModel) {
        return;
      }

      SummaryToSectorZeroForPesosAndUdis(accountEntries);
    }


    internal List<AnaliticoDeCuentasEntry> GetTotalsByDebtorOrCreditorEntries(
                                           List<AnaliticoDeCuentasEntry> analyticEntries) {
      if (analyticEntries.Count == 0) {
        return new List<AnaliticoDeCuentasEntry>();
      }
      List<AnaliticoDeCuentasEntry> listEntries = GetFirstLevelEntriesToGroup(analyticEntries);
      var totalSummaryDebtorCredtor = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

      foreach (var entry in listEntries) {
        
        SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry);

      }

      return totalSummaryDebtorCredtor.Values.ToList();
    }


    internal List<AnaliticoDeCuentasEntry> GetTotalByGroup(List<AnaliticoDeCuentasEntry> analyticEntries) {

      if (analyticEntries.Count==0) {
        return new List<AnaliticoDeCuentasEntry>();
      }

      var totalsByGroup = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();
      List<AnaliticoDeCuentasEntry> listEntries = GetFirstLevelEntriesToGroup(analyticEntries);

      foreach (var entry in listEntries) {

        SummaryByGroupEntries(totalsByGroup, entry);
      }

      return totalsByGroup.Values.ToList();
    }


    internal List<AnaliticoDeCuentasEntry> MergeTrialBalanceIntoAnalyticColumns(
                                                  List<TrialBalanceEntry> balanceEntries) {

      if (balanceEntries.Count == 0) {
        return new List<AnaliticoDeCuentasEntry>();
      }

      IEnumerable<TrialBalanceEntry> accountEntries = GetFilteredAccountEntries(balanceEntries);

      var hashAnaliticoEntries = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

      ConvertIntoAnaliticoDeCuentasEntry(accountEntries, hashAnaliticoEntries);

      ICollection<AnaliticoDeCuentasEntry> analyticEntries = hashAnaliticoEntries.Values;

      MergeDomesticBalancesIntoSectorZero(analyticEntries, accountEntries);
      MergeForeignBalancesIntoSectorZero(analyticEntries, accountEntries);

      return analyticEntries.OrderBy(a => a.Ledger.Number)
                            .ThenByDescending(a => a.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToList();
    }


    #endregion Public methods


    #region Private methods


    private void BalanceEntryByCurrencyToSumTwoColumnEntry(AnaliticoDeCuentasEntry analyticEntry,
                                                           TrialBalanceEntry balanceEntry,
                                                           Currency currentCurrency) {

      if (balanceEntry.Level == 1 && (currentCurrency.Equals(Currency.MXN) || currentCurrency.Equals(Currency.UDI))) {
        analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

      } else if (balanceEntry.Sector.Code == "00" && currentCurrency.Equals(Currency.MXN)) {
        analyticEntry.DomesticBalance = balanceEntry.CurrentBalance;

      } else if (balanceEntry.Sector.Code != "00" &&
                  (currentCurrency.Equals(Currency.MXN) || currentCurrency.Equals(Currency.UDI))) {
        analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

      } else if (currentCurrency.Equals(Currency.UDI) && balanceEntry.IsSummaryForAnalytics) {
        analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

      } else if (!_query.UseNewSectorizationModel && currentCurrency.Equals(Currency.UDI)) {
        analyticEntry.DomesticBalance += balanceEntry.CurrentBalance;

      }
    }


    private void ConvertIntoAnaliticoDeCuentasEntry(IEnumerable<TrialBalanceEntry> accountEntries,
                                   EmpiriaHashTable<AnaliticoDeCuentasEntry> hashAnaliticoEntries) {

      var targetCurrency = Currency.Parse(_query.InitialPeriod.ValuateToCurrrencyUID);

      foreach (var entry in accountEntries) {
        if (entry.CurrentBalance != 0) {
          string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                        $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

          Currency currentCurrency = entry.Currency;
          MergeEntriesIntoTwoColumns(hashAnaliticoEntries, entry, hash, currentCurrency);

        }
      }
    }


    private void GenerateOrIncreaseParentAccountEntries(EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                                        TrialBalanceEntry entry,
                                                        StandardAccount currentParent) {

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      while (true) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        if (entry.Level > 1) {
          trialBalanceHelper.SummaryByAccountEntry(parentAccounts, entry, currentParent, entry.Sector);

          trialBalanceHelper.ValidateSectorizationForSummaryParentEntry(
                             parentAccounts, entry, currentParent);
        }

        if (!currentParent.HasParent && entry.HasSector) {

          trialBalanceHelper.GetAccountEntriesAndParentSector(parentAccounts, entry, currentParent);
          break;

        } else if (!currentParent.HasParent) {

          if (_query.WithSubledgerAccount && !entry.Account.HasParent) {

            trialBalanceHelper.SummaryByAccountEntry(parentAccounts, entry, currentParent,
                                                      Sector.Empty);

          }
          break;

        } else {
          currentParent = currentParent.GetParent();
        }

      } // while
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
                          EmpiriaHashTable<AnaliticoDeCuentasEntry> hashAnaliticoEntries,
                          TrialBalanceEntry entry, string hash,
                          Currency currentCurrency) {

      AnaliticoDeCuentasEntry analiticoEntry;

      hashAnaliticoEntries.TryGetValue(hash, out analiticoEntry);

      if (analiticoEntry == null) {
        hashAnaliticoEntries.Insert(hash, entry.MapToAnalyticBalanceEntry());
        SumTwoColumnEntry(hashAnaliticoEntries[hash], entry, currentCurrency);

      } else {
        SumTwoColumnEntry(analiticoEntry, entry, currentCurrency);

      }
    }


    private IEnumerable<TrialBalanceEntry> GetFilteredAccountEntries(List<TrialBalanceEntry> balanceEntries) {

      var accountEntries = new List<TrialBalanceEntry>(balanceEntries);

      if (_query.WithSubledgerAccount) {
        accountEntries = balanceEntries.Where(a => a.SubledgerAccountId == 0 &&
                                        a.ItemType == TrialBalanceItemType.Summary).ToList();

      } else {
        accountEntries = balanceEntries.Where(a => a.SubledgerAccountNumber.Length <= 1).ToList();
      }
      return accountEntries;
    }


    private List<AnaliticoDeCuentasEntry> GetFirstLevelEntriesToGroup(
                                          List<AnaliticoDeCuentasEntry> analyticEntries) {

      var listEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var entry in analyticEntries.Where(a => a.Level == 1)) {

        var entryWithoutSector = analyticEntries.FirstOrDefault(
                                    a => a.Account.Number == entry.Account.Number &&
                                    a.Ledger.Number == entry.Ledger.Number &&
                                    a.DebtorCreditor == entry.DebtorCreditor && a.NotHasSector);

        if (entryWithoutSector != null) {

          var existInList = listEntries.FirstOrDefault(a =>
                       a.Account.Number == entryWithoutSector.Account.Number &&
                       a.Ledger.Number == entryWithoutSector.Ledger.Number &&
                       a.DebtorCreditor == entryWithoutSector.DebtorCreditor &&
                       a.NotHasSector);

          if (existInList == null) {
            listEntries.Add(entryWithoutSector);
          }

        } else {

          listEntries.Add(entry);
        } 
      }

      return listEntries;
    }


    private void MergeDomesticBalancesIntoSectorZero(IEnumerable<AnaliticoDeCuentasEntry> analyticEntries,
                                                     IEnumerable<TrialBalanceEntry> accountEntries) {
      if (!_query.UseNewSectorizationModel) {
        return;
      }

      foreach (var entry in analyticEntries.Where(a => a.Sector.Code == "00" && a.DomesticBalance == 0)) {

        var accountsWithDomesticCurrency = accountEntries.Where(
              a => a.Account.Number == entry.Account.Number && a.Ledger.Number == entry.Ledger.Number &&
              a.Sector.Code != "00" && a.DebtorCreditor == entry.DebtorCreditor &&
              (a.Currency == Currency.MXN || a.Currency == Currency.UDI)).ToList();

        if (accountsWithDomesticCurrency.Count > 0) {

          entry.ResetBalances();
          
          foreach (var foreignEntry in accountsWithDomesticCurrency) {
            SumTwoColumnEntry(entry, foreignEntry, foreignEntry.Currency);
          }
        }
      }
    }


    private void MergeEntriesIntoTwoColumns(EmpiriaHashTable<AnaliticoDeCuentasEntry> hashAnaliticoEntries,
                                            TrialBalanceEntry entry, string hash, Currency currentCurrency) {

      var targetCurrency = Currency.Parse(_query.InitialPeriod.ValuateToCurrrencyUID);

      if (entry.Currency.Equals(targetCurrency)) {

        GenerateOrIncreaseTwoColumnEntry(hashAnaliticoEntries, entry, hash, currentCurrency);

      } else if (hashAnaliticoEntries.ContainsKey(hash)) {

        SumTwoColumnEntry(hashAnaliticoEntries[hash], entry, currentCurrency);

      } else {

        entry.Currency = targetCurrency;
        GenerateOrIncreaseTwoColumnEntry(hashAnaliticoEntries, entry, hash, currentCurrency);
      }
    }


    private void MergeForeignBalancesIntoSectorZero(IEnumerable<AnaliticoDeCuentasEntry> analyticEntries,
                                                    IEnumerable<TrialBalanceEntry> accountEntries) {

      if (!_query.UseNewSectorizationModel) {
        return;
      }

      foreach (var entry in analyticEntries.Where(a => a.Sector.Code == "00" && a.Level > 1)) {

        var entriesWithForeignCurrency = accountEntries.Where(
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


    private void MergeSubledgerAccountsWithAnaliticEntry(List<TrialBalanceEntry> balanceEntries,
                                                  Currency targetCurrency,
                                                  AnaliticoDeCuentasEntry analyticEntry,
                                                  EmpiriaHashTable<AnaliticoDeCuentasEntry> hashEntries) {

      var accountEntries = balanceEntries.Where(a => a.ItemType == TrialBalanceItemType.Entry &&
                                                a.Ledger.Number == analyticEntry.Ledger.Number &&
                                                a.Account.Number == analyticEntry.Account.Number &&
                                                a.Sector.Code == analyticEntry.Sector.Code &&
                                                a.SubledgerAccountIdParent > 0 &&
                                                a.SubledgerAccountNumber != "")
                                          .ToList();

      foreach (var entry in accountEntries) {

        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                      $"{entry.Ledger.Id}||{entry.DebtorCreditor}||{entry.SubledgerAccountIdParent}";

        Currency currentCurrency = entry.Currency;
        MergeEntriesIntoTwoColumns(hashEntries, entry, hash, currentCurrency);
      }
    }


    private void SumBalanceEntryIntoAnalyticEntry(AnaliticoDeCuentasEntry analyticEntry,
                                                TrialBalanceEntry balanceEntry) {

      analyticEntry.InitialBalance += balanceEntry.InitialBalance;
      analyticEntry.Debit += balanceEntry.Debit;
      analyticEntry.Credit += balanceEntry.Credit;
      analyticEntry.TotalBalance = analyticEntry.DomesticBalance + analyticEntry.ForeignBalance;
      analyticEntry.AverageBalance += balanceEntry.AverageBalance;
      analyticEntry.LastChangeDate = balanceEntry.LastChangeDate > analyticEntry.LastChangeDate ?
                                          balanceEntry.LastChangeDate : analyticEntry.LastChangeDate;
    }


    private void SummaryByDebtorCreditorEntries(
                  EmpiriaHashTable<AnaliticoDeCuentasEntry> summaryEntries,
                  AnaliticoDeCuentasEntry balanceEntry) {

      AnaliticoDeCuentasEntry entry = AnaliticoDeCuentasMapper.CreatePartialCopy(balanceEntry);

      entry.Account = StandardAccount.Empty;
      entry.GroupName = "TOTAL DEUDORAS";

      TrialBalanceItemType itemType = TrialBalanceItemType.BalanceTotalDebtor;

      if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {

        entry.GroupName = "TOTAL ACREEDORAS";
        itemType = TrialBalanceItemType.BalanceTotalCreditor;

      } 

      string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalBalance(summaryEntries, entry, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<AnaliticoDeCuentasEntry> summaryEntries,
                                                 AnaliticoDeCuentasEntry balanceEntry) {

      AnaliticoDeCuentasEntry entry = AnaliticoDeCuentasMapper.CreatePartialCopy(balanceEntry);

      entry.GroupName = $"TOTAL GRUPO {entry.Account.GroupNumber}";
      entry.GroupNumber = $"{entry.Account.GroupNumber}";

      TrialBalanceItemType itemType = TrialBalanceItemType.BalanceTotalGroupDebtor;

      if (entry.DebtorCreditor == DebtorCreditorType.Acreedora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupCreditor;
      }

      string hash = $"{entry.GroupNumber}||{Sector.Empty.Code}||{entry.Currency.Id}||" +
                    $"{entry.Ledger.Id}||{entry.DebtorCreditor}";

      GenerateOrIncreaseTotalBalance(summaryEntries, entry, itemType, hash);
    }


    private void SummaryToSectorZeroForPesosAndUdis(List<TrialBalanceEntry> entries) {

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

        SummaryBalancesByEntry(entry, entryUdis);
      }
    }


    private void SummaryBalancesByEntry(TrialBalanceEntry entry, TrialBalanceEntry entryUdis) {
      
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


    private void SumTwoColumnEntry(AnaliticoDeCuentasEntry analyticEntry,
                                   TrialBalanceEntry balanceEntry,
                                   Currency currentCurrency) {

      if (balanceEntry.DebtorCreditor != analyticEntry.DebtorCreditor) {
        return;
      }

      var targetCurrency = Currency.Parse(_query.InitialPeriod.ValuateToCurrrencyUID);

      if (currentCurrency != targetCurrency && !balanceEntry.Currency.Equals(Currency.UDI) &&
          !currentCurrency.Equals(Currency.UDI)) {

        analyticEntry.ForeignBalance += balanceEntry.CurrentBalance;

      } else {

        BalanceEntryByCurrencyToSumTwoColumnEntry(analyticEntry, balanceEntry, currentCurrency);
      }

      SumBalanceEntryIntoAnalyticEntry(analyticEntry, balanceEntry);

      if (_query.UseNewSectorizationModel) {
        balanceEntry.Currency = currentCurrency;
      }

    }


    #endregion Private methods


  } // class AccountAnalyticsHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
