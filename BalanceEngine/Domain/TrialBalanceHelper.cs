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

    private readonly TrialBalanceCommand _command;

    internal TrialBalanceHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(List<TrialBalanceEntry> summaryEntries,
                                                                     FixedList<TrialBalanceEntry> postingEntries) {
      var returnedEntries = new List<TrialBalanceEntry>(postingEntries);
      returnedEntries.AddRange(summaryEntries);

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ThenByDescending(a => a.Account.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineCurrencyTotalsAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      List<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var currencyEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        var listSummaryByCurrency = trialBalance.Where(a => a.Ledger.Id == currencyEntry.Ledger.Id &&
                                             a.Currency.Code == currencyEntry.Currency.Code).ToList();
        if (listSummaryByCurrency.Count > 0) {
          listSummaryByCurrency.Add(currencyEntry);
          returnedEntries.AddRange(listSummaryByCurrency);
        }
      }
      return OrderByLedgerAndCurrency(returnedEntries);
    }


    internal List<TrialBalanceEntry> CombineDebtorCreditorAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      List<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var debtorSummaryEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {

        var debtorsSummaryList = trialBalance.Where(a => a.Ledger.Id == debtorSummaryEntry.Ledger.Id &&
                                                  a.Currency.Code == debtorSummaryEntry.Currency.Code &&
                                                  a.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (debtorsSummaryList.Count > 0) {
          debtorsSummaryList.Add(debtorSummaryEntry);
          returnedEntries.AddRange(debtorsSummaryList);
        }
      }
      foreach (var creditorSummaryEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        var creditorsSummaryList = trialBalance.Where(a => a.Ledger.Id == creditorSummaryEntry.Ledger.Id &&
                                                  a.Currency.Code == creditorSummaryEntry.Currency.Code &&
                                                  a.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (creditorsSummaryList.Count > 0) {
          creditorsSummaryList.Add(creditorSummaryEntry);
          returnedEntries.AddRange(creditorsSummaryList);
        }
      }
      return OrderByLedgerAndCurrency(returnedEntries);
    }


    private List<TrialBalanceEntry> OrderByLedgerAndCurrency(List<TrialBalanceEntry> entries) {
      return entries.OrderBy(a => a.Ledger.Number)
                    .ThenBy(a => a.Currency.Code)
                    .ToList();
    }


    internal List<TrialBalanceEntry> CombineGroupEntriesAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      FixedList<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalGroupDebtorEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {
        var debtorEntries = trialBalance.Where(
                                  a => a.Account.GroupNumber == totalGroupDebtorEntry.GroupNumber &&
                                  a.Ledger.Id == totalGroupDebtorEntry.Ledger.Id &&
                                  a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        //if (debtorEntries.Count > 0) {
          debtorEntries.Add(totalGroupDebtorEntry);
          returnedEntries.AddRange(debtorEntries);
        //}
      }

      foreach (var creditorEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)) {
        var creditorEntries = trialBalance.Where(
                                  a => a.Account.GroupNumber == creditorEntry.GroupNumber &&
                                  a.Ledger.Id == creditorEntry.Ledger.Id &&
                                  a.Currency.Id == creditorEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        //if (creditorEntries.Count > 0) {
          creditorEntries.Add(creditorEntry);
          returnedEntries.AddRange(creditorEntries);
        //}
      }

      var c = returnedEntries.Count(x => x.ItemType == TrialBalanceItemType.BalanceEntry);
      var d = trialBalance.Count(x => x.ItemType == TrialBalanceItemType.BalanceEntry);

      var set = trialBalance.Except(returnedEntries).ToList();

      var s = "";
      foreach (var item in set) {
        s += $"{item.Account.Number} ({item.Currency.Code}), ";
      }

      Assertion.Assert(c == d, $"Has failed {c}, {d}, {set.Count}, {s}");

      return OrderByLedgerAndCurrency(returnedEntries);
    }


    internal List<TrialBalanceEntry> CombineTotalConsolidatedAndPostingEntries(List<TrialBalanceEntry> trialBalance,
                                                                               List<TrialBalanceEntry> summaryEntries) {
      var entries = new List<TrialBalanceEntry>(trialBalance);

      var consolidated = summaryEntries.FirstOrDefault(
                                  a => a.ItemType == TrialBalanceItemType.BalanceTotalConsolidated);

      if (consolidated != null) {
        entries.Add(consolidated);
      }

      return entries;
    }


    internal FixedList<TrialBalanceEntry> ConsolidateToTargetCurrency(FixedList<TrialBalanceEntry> trialBalance) {
      var targetCurrency = Currency.Parse(_command.ValuateToCurrrencyUID);

      var summaryEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}";

        if (entry.Currency.Equals(targetCurrency)) {
          summaryEntries.Insert(hash, entry);
        } else if (summaryEntries.ContainsKey(hash)) {
          summaryEntries[hash].Sum(entry);
        } else {
          entry.Currency = targetCurrency;
          summaryEntries.Insert(hash, entry);
        }
      }

      return summaryEntries.Values.ToList()
                                  .ToFixedList();
    }


    internal List<TrialBalanceEntry> GenerateSummaryEntries(FixedList<TrialBalanceEntry> entries) {
      var summaryEntries = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;

        StandardAccount currentParent;

        if (_command.ReturnSubledgerAccounts) {
          currentParent = entry.Account;

        } else if (_command.DoNotReturnSubledgerAccounts && entry.Account.HasParent) {
          currentParent = entry.Account.GetParent();

        } else if (_command.DoNotReturnSubledgerAccounts && entry.Account.NotHasParent) {
          continue;
        } else {
          throw Assertion.AssertNoReachThisCode();
        }

        while (true) {
          entry.DebtorCreditor = entry.Account.DebtorCreditor;
          entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

          SummaryByEntry(summaryEntries, entry, currentParent, entry.Sector,
                                         TrialBalanceItemType.BalanceSummary);

          if (!currentParent.HasParent && entry.HasSector) {

            SummaryByEntry(summaryEntries, entry, currentParent, Sector.Empty,
                                           TrialBalanceItemType.BalanceSummary);
            break;
          } else if (!currentParent.HasParent) {
            break;

          } else {
            currentParent = currentParent.GetParent();
          }

        } // while

      } // foreach

      return summaryEntries.Values.ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryDebtorCreditor(List<TrialBalanceEntry> postingEntries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TrialBalanceEntry>(postingEntries.Count);

      foreach (var entry in postingEntries) {

        if (entry.Account.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry, StandardAccount.Empty,
                                         Sector.Empty, TrialBalanceItemType.BalanceTotalDebtor);
        }
        if (entry.Account.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry, StandardAccount.Empty,
                                         Sector.Empty, TrialBalanceItemType.BalanceTotalCreditor);
        }
      }

      return totalSummaryDebtorCredtor.Values.ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryCurrency(List<TrialBalanceEntry> entries) {
      var totalSummaryCurrencies = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var debtorOrCreditorEntry in entries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                     a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        SummaryByCurrencyEntries(totalSummaryCurrencies, debtorOrCreditorEntry, StandardAccount.Empty,
                            Sector.Empty, TrialBalanceItemType.BalanceTotalCurrency);
      }

      entries.AddRange(totalSummaryCurrencies.Values.ToList());

      return entries;
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryConsolidated(List<TrialBalanceEntry> balanceEntries) {
      var totalSummaryConsolidated = new EmpiriaHashTable<TrialBalanceEntry>(balanceEntries.Count);

      foreach (var currencyEntry in balanceEntries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(currencyEntry);

        entry.GroupName = "TOTAL CONSOLIDADO";

        string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}";

        GenerateOrIncreaseEntries(totalSummaryConsolidated, entry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      balanceEntries.AddRange(totalSummaryConsolidated.Values.ToList());

      return balanceEntries;
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalSummaryGroups(FixedList<TrialBalanceEntry> entries) {

      var toReturnSummaryGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries) {
        SummaryByGroupEntries(toReturnSummaryGroupEntries, entry);
      }

      return toReturnSummaryGroupEntries.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetPostingEntries() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      if (_command.ValuateBalances) {
        postingEntries = helper.ValuateToExchangeRate(postingEntries);

        if (_command.ConsolidateBalancesToTargetCurrency) {
          postingEntries = helper.ConsolidateToTargetCurrency(postingEntries);
        }
      }
      return postingEntries;
    }


    internal List<TrialBalanceEntry> GetSummaryAndPostingEntries() {
      FixedList<TrialBalanceEntry> postingEntries = GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = GenerateSummaryEntries(postingEntries);

      return CombineSummaryAndPostingEntries(summaryEntries, postingEntries);
    }

    internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries() {
      if (_command.TrialBalanceType == TrialBalanceType.SaldosPorCuentaConDelegaciones) {
        _command.ShowCascadeBalances = true;
      }

      TrialBalanceCommandData commandData = _command.MapToTrialBalanceCommandData();

      return TrialBalanceDataService.GetTrialBalanceEntries(commandData);
    }


    internal List<TrialBalanceEntry> RestrictLevels(List<TrialBalanceEntry> entries) {
      if (_command.Level == 0) {
        return entries;
      }

      if (_command.DoNotReturnSubledgerAccounts) {
        return entries.FindAll(x => x.Level <= _command.Level);
      } else if (_command.ReturnSubledgerAccounts) {
        return entries.FindAll(x => x.Level <= _command.Level || x.LedgerAccountId != 0);
      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    internal FixedList<TrialBalanceEntry> ValuateToExchangeRate(FixedList<TrialBalanceEntry> entries) {
      var exchangeRateType = ExchangeRateType.Parse(_command.ExchangeRateTypeUID);

      FixedList<ExchangeRate> exchageRates = ExchangeRate.GetList(exchangeRateType, _command.ExchangeRateDate);

      foreach (var entry in entries.Where(a=>a.Currency.Code !="01" )) {
        var exchangeRate = exchageRates.FirstOrDefault(a => a.FromCurrency.Code == _command.ValuateToCurrrencyUID &&
                                                            a.ToCurrency.Code == entry.Currency.Code);

        Assertion.AssertObject(exchangeRate, $"No hay tipo de cambio para la moneda {entry.Currency.FullName}.");

        entry.MultiplyBy(exchangeRate.Value);
      }
      return entries;
    }

    #region Private methods


    internal void SummaryByAccount(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                   TrialBalanceEntry balanceEntry,
                                   StandardAccount targetAccount, Sector targetSector,
                                   TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      entry.Ledger = Ledger.Empty;
      entry.GroupName = "TOTAL " + entry.Account.Name.ToUpper();
      entry.InitialBalance = 0;
      entry.Debit = 0;
      entry.Credit = 0;
      entry.CurrentBalance = 0;

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByCurrencyEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                          TrialBalanceEntry balanceEntry,
                                          StandardAccount targetAccount, Sector targetSector,
                                          TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = -1 * entry.InitialBalance;
        entry.CurrentBalance = -1 * entry.CurrentBalance;
      }

      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;

      string hash = $"{entry.GroupName}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByDebtorCreditorEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                TrialBalanceEntry balanceEntry,
                                                StandardAccount targetAccount, Sector targetSector,
                                                TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      if (itemType == TrialBalanceItemType.BalanceTotalDebtor) {
        entry.GroupName = "TOTAL DEUDORAS " + entry.Currency.FullName;
      } else if (itemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.GroupName = "TOTAL ACREEDORAS " + entry.Currency.FullName;
      }

      string hash = $"{entry.GroupName}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    internal void SummaryByEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                 TrialBalanceEntry entry,
                                 StandardAccount targetAccount, Sector targetSector,
                                 TrialBalanceItemType itemType) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                       TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry groupEntry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      groupEntry.GroupName = $"TOTAL GRUPO {balanceEntry.Account.GroupNumber}";
      groupEntry.GroupNumber = $"{balanceEntry.Account.GroupNumber}";
      groupEntry.Account = StandardAccount.Empty;
      groupEntry.Sector = Sector.Empty;
      groupEntry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;

      string hash;

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Deudora) {
        hash = $"{balanceEntry.Ledger.Id}||{balanceEntry.Currency.Id}||{balanceEntry.Account.GroupNumber}||D";

        GenerateOrIncreaseEntries(summaryEntries, groupEntry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalGroupDebtor, hash);

      } else {
        hash = $"{balanceEntry.Ledger.Id}||{balanceEntry.Currency.Id}||{balanceEntry.Account.GroupNumber}||A";

        GenerateOrIncreaseEntries(summaryEntries, groupEntry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalGroupCreditor, hash);
      }
    }


    internal void SummaryBySubsidiaryEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry, TrialBalanceItemType itemType) {

      TrialBalanceEntry balanceEntry = TrialBalanceMapper.MapToTrialBalanceEntry(entry);

      balanceEntry.SubledgerAccountIdParent = entry.SubledgerAccountIdParent;

      string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, balanceEntry,
                                StandardAccount.Empty, Sector.Empty, itemType, hash);
    }


    private void GenerateOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
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
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent
        };
        summaryEntry.Sum(entry);

        summaryEntries.Insert(hash, summaryEntry);

      } else {
        summaryEntry.Sum(entry);
      }
    }

    #endregion Private methods

  }  // class TrialBalanceHelper

}  // namespace Empiria.FinancialAccounting.BalanceEngine
