/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : TrialBalanceHelper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build trial balances and related accounting information.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build trial balances and related accounting information.</summary>
  internal class TrialBalanceHelper {

    private readonly TrialBalanceQuery _query;

    internal TrialBalanceHelper(TrialBalanceQuery query) {
      _query = query;
    }


    internal void AssignLastChangeDatesToParentEntries(
                                      FixedList<TrialBalanceEntry> AccountEntries,
                                      FixedList<TrialBalanceEntry> parentAccounts) {

      foreach (var entry in AccountEntries) {
        SetLastChangeDateToAccountEntries(entry, parentAccounts);
        SetLastChangeDateToParentEntries(entry, parentAccounts);
      }
    }


    internal FixedList<TrialBalanceEntry> ConsolidateToTargetCurrency(
                                          FixedList<TrialBalanceEntry> trialBalance,
                                          BalancesPeriod period) {

      var targetCurrency = Currency.Parse(period.ValuateToCurrrencyUID);
      var accountEntriesConsolidated = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}";

        if (entry.Currency.Equals(targetCurrency)) {
          accountEntriesConsolidated.Insert(hash, entry);
        } else if (accountEntriesConsolidated.ContainsKey(hash)) {
          accountEntriesConsolidated[hash].Sum(entry);
        } else {
          entry.Currency = targetCurrency;
          accountEntriesConsolidated.Insert(hash, entry);
        }
      }

      return accountEntriesConsolidated.Values.ToFixedList();
    }


    internal void GenerateOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry,
                                           StandardAccount targetAccount, Sector targetSector,
                                           TrialBalanceItemType itemType, string hash) {

      TrialBalanceEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TrialBalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = targetSector,
          Account = targetAccount,
          ItemType = itemType,
          GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName,
          DebtorCreditor = entry.DebtorCreditor,
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent,
          LastChangeDate = entry.LastChangeDate
        };

        summaryEntry.Sum(entry);

        summaryEntries.Insert(hash, summaryEntry);

      } else {

        summaryEntry.Sum(entry);
      }
    }


    internal FixedList<TrialBalanceEntry> GetAccountEntries() {

      FixedList<TrialBalanceEntry> accountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      if (accountEntries.Count == 0) {
        return accountEntries;
      }

      if (_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation) {
        ValuateAccountEntriesToExchangeRate(accountEntries);

        if (_query.ConsolidateBalancesToTargetCurrency) {
          accountEntries = ConsolidateToTargetCurrency(accountEntries, _query.InitialPeriod);
        }
      }

      RoundDecimals(accountEntries);

      return accountEntries;

    }


    internal void GetAccountEntriesAndParentSector(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                        TrialBalanceEntry entry, StandardAccount currentParent) {

      if (!_query.WithSectorization) {
        SummaryByAccountEntry(summaryEntries, entry, currentParent, Sector.Empty);

      } else {

        var parentSector = entry.Sector.Parent;
        while (true) {

          SummaryByAccountEntry(summaryEntries, entry, currentParent, parentSector);

          if (parentSector.IsRoot) {
            break;
          } else {
            parentSector = parentSector.Parent;
          }
        }
      }
    }


    internal void GetEntriesAndParentSector(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                        TrialBalanceEntry entry, StandardAccount currentParent) {
      if (!_query.WithSectorization) {
        SummaryByEntry(summaryEntries, entry, currentParent, Sector.Empty,
                          TrialBalanceItemType.Summary);
      } else {
        var parentSector = entry.Sector.Parent;
        while (true) {
          SummaryByEntry(summaryEntries, entry, currentParent, parentSector,
                                          TrialBalanceItemType.Summary);
          if (parentSector.IsRoot) {
            break;
          } else {
            parentSector = parentSector.Parent;
          }
        }
      }
    }


    internal List<TrialBalanceEntry> GetEntriesMappedForSectorization(
                                    List<TrialBalanceEntry> entriesList) {

      var returnedEntriesMapped = new List<TrialBalanceEntry>();

      foreach (var entry in entriesList) {
        TrialBalanceEntry balanceEntry = entry.CreatePartialCopy();
        balanceEntry.LastChangeDate = entry.LastChangeDate;
        balanceEntry.AverageBalance = entry.AverageBalance;
        balanceEntry.SecondExchangeRate = entry.SecondExchangeRate;
        balanceEntry.DebtorCreditor = entry.DebtorCreditor;
        balanceEntry.SubledgerAccountIdParent = entry.SubledgerAccountIdParent;
        balanceEntry.SubledgerAccountNumber = entry.SubledgerAccountNumber;
        balanceEntry.SubledgerNumberOfDigits = entry.SubledgerNumberOfDigits;
        balanceEntry.IsParentPostingEntry = entry.IsParentPostingEntry;
        balanceEntry.HasParentPostingEntry = entry.HasParentPostingEntry;
        returnedEntriesMapped.Add(balanceEntry);
      }

      return returnedEntriesMapped;
    }


    internal void GetSummarySectorizedAccountByLevel(List<TrialBalanceEntry> summaryEntries) {

      var EntriesList = new List<TrialBalanceEntry>(summaryEntries);

      var summaryEntriesList = new List<TrialBalanceEntry>(summaryEntries);

      foreach (var entry in summaryEntriesList) {

        var entriesWithSummarySector = EntriesList
                                  .Where(a => a.Account.Number == entry.Account.Number &&
                                         a.Ledger.Number == entry.Ledger.Number &&
                                         a.Currency.Code == entry.Currency.Code)
                                  .ToList();

        if (entry.Level > 1 &&
             (entriesWithSummarySector.Count == 2 &&
              entry.ItemType == TrialBalanceItemType.Summary) ||
             (entry.ItemType == TrialBalanceItemType.Entry &&
             entriesWithSummarySector.Count == 2 && entry.Sector.Code != "00")) {

          var entryWithoutSector = entriesWithSummarySector.FirstOrDefault(a => a.Sector.Code == "00");
          summaryEntries.Remove(entryWithoutSector);
        }
      }

    }


    internal List<TrialBalanceEntry> GetSummaryAccountsAndSectorization(
                                      List<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var startTime = DateTime.Now;
      var returnedAccountEntries = new List<TrialBalanceEntry>(accountEntries);

      if (_query.UseNewSectorizationModel) {

        if (_query.WithSectorization) {
          GetAccountsWithSectorization(returnedAccountEntries);
          EmpiriaLog.Debug($"INNER GetSummaryEntriesWithSectorization(): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");
        }

        if (!_query.WithSectorization) {
          GetAccountsWithoutSectorization(returnedAccountEntries);
          EmpiriaLog.Debug($"INNER GetSummaryEntriesWithoutSectorization(): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");
        }

        if (_query.TrialBalanceType != TrialBalanceType.AnaliticoDeCuentas &&
            _query.TrialBalanceType != TrialBalanceType.Balanza) {

          GetSummarySectorizedAccountByLevel(returnedAccountEntries);
        }
      }

      return returnedAccountEntries;
    }


    internal void SetSummaryToParentEntries(IEnumerable<TrialBalanceEntry> accountEntries) {

      var returnedEntries = new List<TrialBalanceEntry>(accountEntries);

      foreach (var entry in accountEntries) {
        StandardAccount currentParent = entry.Account.GetParent();

        /// Search for a parent account entry, for cases when both the account
        /// and also his parent have entries for the given balance period.
        /// WasConvertedToSummary marks an account that was converted from
        /// posting to summary in the given period.
        var parentAccountEntry = returnedEntries.FirstOrDefault(a => a.Account.Number == currentParent.Number &&
                                                                a.Currency.Code == entry.Currency.Code &&
                                                                a.Ledger.Number == entry.Ledger.Number &&
                                                                a.Sector.Code == entry.Sector.Code &&
                                                                a.Account.DebtorCreditor == entry.Account.DebtorCreditor);
        if (parentAccountEntry != null) {
          entry.HasParentPostingEntry = true;
          parentAccountEntry.IsParentPostingEntry = true;
          parentAccountEntry.Sum(entry);
        }
      }
    }


    internal List<TrialBalanceEntry> GetCalculatedParentAccounts(
                                     FixedList<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);

      foreach (var entry in accountEntries) {

        StandardAccount currentParent;

        bool isCalculatedAccount = ValidateEntryToAssignCurrentParentAccount(
                                                      entry, out currentParent);

        if (!ValidateEntryForSummaryParentAccount(entry, isCalculatedAccount)) {
          continue;
        }

        GetSummaryToParentAccountEntry(parentAccounts, entry, currentParent);

      } // foreach

      AssignLastChangeDatesToParentEntries(accountEntries, parentAccounts.ToFixedList());

      return parentAccounts.ToFixedList().ToList();
    }


    internal void RestrictLevels(List<TrialBalanceEntry> entries) {
      if (_query.Level == 0) {
        return;
      }

      if (_query.DoNotReturnSubledgerAccounts) {

        entries.RemoveAll(x => x.Level <= _query.Level);

      } else if (_query.WithSubledgerAccount) {

        entries.RemoveAll(x => x.Level <= _query.Level);

      } else {

        throw Assertion.EnsureNoReachThisCode();
      }
    }


    internal void RoundDecimals(FixedList<TrialBalanceEntry> entries, int decimals = 2) {
      foreach (var entry in entries) {
        entry.InitialBalance = Math.Round(entry.InitialBalance, decimals);
        entry.Debit = Math.Round(entry.Debit, decimals);
        entry.Credit = Math.Round(entry.Credit, decimals);
        entry.CurrentBalance = Math.Round(entry.CurrentBalance, decimals);
      }
    }


    internal void SetSubledgerAccountInfoByEntry(List<TrialBalanceEntry> entries) {
      if (!_query.WithSubledgerAccount) {
        return;
      }

      foreach (var entry in entries) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        if (subledgerAccount.IsEmptyInstance) {
          continue;
        }
        entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ?
                                        subledgerAccount.Number : "";
        entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Length;
      }
    }


    internal void SummaryByAccountForOperationalReport(EmpiriaHashTable<TrialBalanceEntry> entries,
                                                       TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      if (entry.ItemType == TrialBalanceItemType.Summary && entry.Level == 1 && entry.HasSector) {
        entry.InitialBalance = 0;
        entry.Debit = 0;
        entry.Credit = 0;
        entry.CurrentBalance = 0;
      }
      entry.LastChangeDate = balanceEntry.LastChangeDate;

      TrialBalanceItemType itemType = TrialBalanceItemType.Entry;

      string hash = $"{entry.Account.Number}";

      GenerateOrIncreaseEntries(entries, entry, entry.Account,
                                entry.Sector, itemType, hash);
    }



    internal void SummaryByAccountEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                 TrialBalanceEntry entry,
                                 StandardAccount targetAccount, Sector targetSector) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}" +
                    $"||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      var balanceHelper = new TrialBalanceHelper(_query);
      balanceHelper.GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount,
                                              targetSector, TrialBalanceItemType.Summary, hash);
    }


    internal void SummaryByEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                 TrialBalanceEntry entry,
                                 StandardAccount targetAccount, Sector targetSector,
                                 TrialBalanceItemType itemType) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      if (_query.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas ||
          _query.TrialBalanceType == TrialBalanceType.Balanza ||
          (_query.TrialBalanceType == TrialBalanceType.BalanzaEnColumnasPorMoneda &&
           _query.UseNewSectorizationModel)) {

        hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}" +
               $"||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      }
      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    internal void SummaryEntryBySectorization(EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                             TrialBalanceEntry entry, StandardAccount currentParent) {
      if (!_query.UseNewSectorizationModel || !_query.WithSectorization) {
        return;
      }

      if (!currentParent.HasParent || !entry.HasSector) {
        return;
      }

      SummaryByEntry(parentAccounts, entry, currentParent, entry.Sector.Parent,
                     TrialBalanceItemType.Summary);
    }


    internal bool ValidateEntryForSummaryParentAccount(TrialBalanceEntry entry, bool isCalculatedAccount) {

      if (!isCalculatedAccount) {
        return false;
      }

      if (entry.HasParentPostingEntry) {
        return false;
      }

      return true;
    }


    internal bool ValidateEntryToAssignCurrentParentAccount(TrialBalanceEntry entry,
                                                      out StandardAccount currentParent) {
      entry.DebtorCreditor = entry.Account.DebtorCreditor;
      entry.SubledgerAccountNumber = SubledgerAccount.Parse(entry.SubledgerAccountId).Number ?? "";

      if (entry.Account.NotHasParent || _query.WithSubledgerAccount ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
        currentParent = entry.Account;

      } else if (_query.DoNotReturnSubledgerAccounts && entry.Account.HasParent) {
        currentParent = entry.Account.GetParent();

      } else if (_query.DoNotReturnSubledgerAccounts && entry.Account.NotHasParent) {
        currentParent = entry.Account;
        return false;

      } else {
        throw Assertion.EnsureNoReachThisCode();
      }
      return true;
    }


    internal void ValidateSectorizationForSummaryParentEntry(
                  EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                  TrialBalanceEntry entry, StandardAccount currentParent) {

      if (!_query.UseNewSectorizationModel || !_query.WithSectorization) {
        return;
      }

      if (!currentParent.HasParent || !entry.HasSector) {
        return;
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);
      trialBalanceHelper.SummaryByAccountEntry(parentAccounts, entry, currentParent, entry.Sector.Parent);
    }


    internal void ValuateAccountEntriesToExchangeRate(FixedList<TrialBalanceEntry> entries) {

      FixedList<ExchangeRate> exchangeRates = GetExchangeRateListForDate();

      foreach (var entry in entries.Where(a => a.Currency.Code != "01")) {

        var exchangeRate = exchangeRates.FirstOrDefault(
                            a => a.ToCurrency.Code == entry.Currency.Code &&
                            a.FromCurrency.Code == _query.InitialPeriod.ValuateToCurrrencyUID);

        // ToDo: URGENT This require must be checked before any state change
        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} con la fecha proporcionada.");

        ClausesToExchangeRate(entry, exchangeRate);
      }
    }


    #region Private methods


    private void AddOrSumAccountsWithSectorization(List<TrialBalanceEntry> accountEntries,
                                                    EmpiriaHashTable<TrialBalanceEntry> hashEntries) {

      var checkAccountEntries = new List<TrialBalanceEntry>(accountEntries);
      foreach (var entry in checkAccountEntries) {

        var accountEntry = accountEntries.FirstOrDefault(a => a.Account.Number == entry.Account.Number &&
                                                               a.Ledger.Number == entry.Ledger.Number &&
                                                               a.Currency.Code == entry.Currency.Code &&
                                                               a.Sector.Code == "00");

        var sectorParent = entry.Sector.Parent;

        if (accountEntry != null && sectorParent.Code != "00" && entry.Level > 1) {
          accountEntry.Sum(entry);

        } else if (entry.Level > 1 && (sectorParent.Code != "00" ||
             (entry.ItemType == TrialBalanceItemType.Entry &&
              entry.HasSector))) {

          SummaryByEntry(hashEntries, entry, entry.Account, Sector.Empty, entry.ItemType);
        }
      }
    }


    private void AddOrSumAccountsWithoutSectorization(List<TrialBalanceEntry> accountEntries,
                                                      EmpiriaHashTable<TrialBalanceEntry> hashEntries) {

      var checkSummaryEntries = new List<TrialBalanceEntry>(accountEntries);
      foreach (var entry in checkSummaryEntries) {

        var sectorParent = entry.Sector.Parent;
        var returnedEntry = accountEntries.FirstOrDefault(a => a.Account.Number == entry.Account.Number &&
                                                            a.Ledger.Number == entry.Ledger.Number &&
                                                            a.Currency.Code == entry.Currency.Code &&
                                                            a.Sector.Code == "00");

        if (returnedEntry != null && sectorParent.Code != "00" &&
            entry.HasSector && entry.Level > 1) {

          returnedEntry.Sum(entry);
        } else if (entry.HasSector && entry.Level > 1) {

          SummaryByEntry(hashEntries, entry, entry.Account, Sector.Empty, TrialBalanceItemType.Summary);
        }
      }
    }


    private void AddOrSumHashEntryIntoAccountsWithoutSectorization(
                              List<TrialBalanceEntry> accountEntries,
                              EmpiriaHashTable<TrialBalanceEntry> hashEntries) {

      foreach (var hashEntry in hashEntries.Values.ToList()) {

        var entry = accountEntries.FirstOrDefault(
                                    a => a.Account.Number == hashEntry.Account.Number &&
                                         a.Ledger.Number == hashEntry.Ledger.Number &&
                                         a.Currency.Code == hashEntry.Currency.Code &&
                                         a.Sector.Code == hashEntry.Sector.Code && a.Sector.Code == "00");
        if (entry == null) {
          accountEntries.Add(hashEntry);

        } else {
          hashEntry.Sum(entry);
          accountEntries.Remove(entry);
          accountEntries.Add(hashEntry);

        }
      }
    }


    private void ClausesToExchangeRate(TrialBalanceEntry entry, ExchangeRate exchangeRate) {

      if (_query.IsOperationalReport && !_query.ConsolidateBalancesToTargetCurrency) {
        entry.ExchangeRate = exchangeRate.Value;

      } else {
        entry.MultiplyBy(exchangeRate.Value);

      }

    }


    private FixedList<ExchangeRate> GetExchangeRateListForDate() {
      if (_query.InitialPeriod.UseDefaultValuation) {
        _query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
        _query.InitialPeriod.ValuateToCurrrencyUID = "01";
        _query.InitialPeriod.ExchangeRateDate = _query.InitialPeriod.ToDate;
      }

      var exchangeRateType = ExchangeRateType.Parse(_query.InitialPeriod.ExchangeRateTypeUID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType,
                                                                   _query.InitialPeriod.ExchangeRateDate);
      return exchangeRates;

    }


    private void GetAccountsWithoutSectorization(
                                    List<TrialBalanceEntry> accountEntries) {

      var hashEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      AddOrSumAccountsWithoutSectorization(accountEntries, hashEntries);
      AddOrSumHashEntryIntoAccountsWithoutSectorization(accountEntries, hashEntries);
    }


    private void GetAccountsWithSectorization(
                                    List<TrialBalanceEntry> accountEntries) {

      var hashEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      AddOrSumAccountsWithSectorization(accountEntries, hashEntries);

      accountEntries.AddRange(hashEntries.ToFixedList().ToList());
    }


    private void GetSummaryToParentAccountEntry(EmpiriaHashTable<TrialBalanceEntry> parentAccounts,
                                                TrialBalanceEntry entry, StandardAccount currentParent) {

      while (true) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        if (entry.Level > 1) {
          SummaryByEntry(parentAccounts, entry, currentParent,
                          entry.Sector, TrialBalanceItemType.Summary);

          SummaryEntryBySectorization(parentAccounts, entry, currentParent);
        }

        if (!currentParent.HasParent && entry.HasSector) {
          GetEntriesAndParentSector(parentAccounts, entry, currentParent);
          break;

        } else if (!currentParent.HasParent) {
          break;

        } else {
          currentParent = currentParent.GetParent();
        }

      } // while
    }


    internal List<TrialBalanceEntry> OrderingParentsAndAccountEntries(
                                     List<TrialBalanceEntry> entries) {

      if (_query.WithSubledgerAccount) {

        return entries.OrderBy(a => a.Ledger.Number)
                     .ThenBy(a => a.Currency.Code)
                     .ThenByDescending(a => a.Account.DebtorCreditor)
                     .ThenBy(a => a.Account.Number)
                     .ThenBy(a => a.Sector.Code)
                     .ThenBy(a => a.SubledgerAccountNumber.Length)
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


    private void SetLastChangeDateToParentEntries(TrialBalanceEntry entry,
                                                  FixedList<TrialBalanceEntry> parentAccounts) {
      StandardAccount currentParentAccount = entry.Account.GetParent();

      while (true) {
        SetLastChangeDateToEntryWithSector(entry, parentAccounts, currentParentAccount);

        if (currentParentAccount.NotHasParent) {

          SetLastChangeDateToEntryWithoutSector(entry, parentAccounts, currentParentAccount);
          break;

        } else {
          currentParentAccount = currentParentAccount.GetParent();
        }

      } // while
    }


    private void SetLastChangeDateToEntryWithSector(TrialBalanceEntry entry,
                                                    FixedList<TrialBalanceEntry> parentAccounts,
                                                    StandardAccount currentParentAccount) {

      var parentToChange = parentAccounts.FirstOrDefault(
                          a => a.Account.Number == currentParentAccount.Number &&
                          a.Currency.Code == entry.Currency.Code &&
                          a.Sector.Code == entry.Sector.Code &&
                          entry.LastChangeDate > a.LastChangeDate);

      if (parentToChange != null) {
        parentToChange.LastChangeDate = entry.LastChangeDate;
      }
    }


    private void SetLastChangeDateToEntryWithoutSector(TrialBalanceEntry entry,
                                                       FixedList<TrialBalanceEntry> parentAccounts,
                                                       StandardAccount currentParentAccount) {

      var parentToChangeWithoutSector = parentAccounts.FirstOrDefault(
                                    a => a.Account.Number == currentParentAccount.Number &&
                                    a.Currency.Code == entry.Currency.Code &&
                                    a.Sector.Code == "00" &&
                                    entry.LastChangeDate > a.LastChangeDate);

      if (parentToChangeWithoutSector != null) {
        parentToChangeWithoutSector.LastChangeDate = entry.LastChangeDate;
      }
    }


    private void SetLastChangeDateToAccountEntries(TrialBalanceEntry entry,
                                                   FixedList<TrialBalanceEntry> parentAccounts) {

      var filtered = parentAccounts.Where(a => a.Account.Number == entry.Account.Number &&
                                               a.Currency.Code == entry.Currency.Code &&
                                               a.Sector.Code == entry.Sector.Code &&
                                               entry.LastChangeDate > a.LastChangeDate);

      foreach (var parentToChange in filtered) {
        parentToChange.LastChangeDate = entry.LastChangeDate;
      }
    }


    #endregion Private methods

  }  // class TrialBalanceHelper

}  // namespace Empiria.FinancialAccounting.BalanceEngine
