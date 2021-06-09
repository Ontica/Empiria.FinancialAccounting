/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : TrialBalanceHelper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build trial balances and related accounting information.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
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


    internal FixedList<TrialBalanceEntry> CombineSummaryAndPostingEntries(List<TrialBalanceEntry> summaryEntries,
                                                                          FixedList<TrialBalanceEntry> postingEntries) {
      var returnedEntries = new List<TrialBalanceEntry>(postingEntries);

      returnedEntries.AddRange(summaryEntries);

      returnedEntries = returnedEntries.OrderBy(a => a.Ledger.Number)
                                       .ThenBy(a => a.Currency.Code)
                                       .ThenByDescending(a => a.Account.DebtorCreditor)
                                       .ThenBy(a => a.Account.Number)
                                       .ThenBy(a => a.Sector.Code)
                                       .ThenBy(a => a.SubledgerAccountId)
                                       .ToList();

      return returnedEntries.ToFixedList();
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
        Account currentParent;

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

          SummaryByEntry(summaryEntries, entry, currentParent, entry.Sector,
                                         TrialBalanceItemType.BalanceSummary);

          if (!currentParent.HasParent && entry.Sector.Code != "00") {

            SummaryByEntry(summaryEntries, entry, currentParent, Sector.Empty,
                                           TrialBalanceItemType.BalanceSummary);

            SummaryByGroupEntries(summaryEntries, entry, Account.Empty, Sector.Empty,
                                           TrialBalanceItemType.BalanceTotalGroup);
            break;

          } else if (!currentParent.HasParent) {
            SummaryByGroupEntries(summaryEntries, entry, Account.Empty, Sector.Empty,
                                           TrialBalanceItemType.BalanceTotalGroup);
            break;

          } else {
            currentParent = currentParent.GetParent();
          }

        } // while

      } // foreach

      return summaryEntries.Values.ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryDebtorCreditor(List<TrialBalanceEntry> entries) {
      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries.Where(a => a.Level == 1 && a.Sector.Code == "00" &&
                                          a.ItemType == TrialBalanceItemType.BalanceSummary)) {

        if (entry.Account.DebtorCreditor == DebtorCreditorType.Deudora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry, Account.Empty,
                                       Sector.Empty, TrialBalanceItemType.BalanceTotalDeptor);
        }
        if (entry.Account.DebtorCreditor == DebtorCreditorType.Acreedora) {
          SummaryByDebtorCreditorEntries(totalSummaryDebtorCredtor, entry, Account.Empty,
                                       Sector.Empty, TrialBalanceItemType.BalanceTotalCreditor);
        }
      }

      entries.AddRange(totalSummaryDebtorCredtor.Values.ToList());

      return entries;
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryCurrency(List<TrialBalanceEntry> entries) {
      var totalSummaryCurrencies = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var debtorCreditorEntry in entries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalDeptor ||
                     a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        SummaryByCurrencyEntries(totalSummaryCurrencies, debtorCreditorEntry, Account.Empty,
                            Sector.Empty, TrialBalanceItemType.BalanceTotalCurrency);
      }

      entries.AddRange(totalSummaryCurrencies.Values.ToList());

      return entries;
    }


    internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries() {
      TrialBalanceCommandData commandData = _command.MapToTrialBalanceCommandData();

      return TrialBalanceDataService.GetTrialBalanceEntries(commandData);
    }


    internal FixedList<ITrialBalanceEntry> MergeAccountsIntoTwoColumnsByCurrency(FixedList<TrialBalanceEntry> trialBalance) {
      var targetCurrency = Currency.Parse(_command.ValuateToCurrrencyUID);

      var summaryEntries = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}";

        if (entry.Currency.Equals(targetCurrency)) {
          summaryEntries.Insert(hash, entry.MapToTwoColumnsBalanceEntry());
        } else if (summaryEntries.ContainsKey(hash)) {
          summaryEntries[hash].DomesticBalance = entry.InitialBalance;
          summaryEntries[hash].ForeignBalance = entry.CurrentBalance;
        } else {
          entry.Currency = targetCurrency;
          summaryEntries.Insert(hash, entry.MapToTwoColumnsBalanceEntry());
        }
      }

      return summaryEntries.Values.Select(x => (ITrialBalanceEntry) x)
                                  .ToList().ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> RestrictLevels(FixedList<TrialBalanceEntry> entries) {
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

      foreach (var entry in entries) {
        var exchangeRate = exchageRates.FirstOrDefault(a => a.FromCurrency.Code == _command.ValuateToCurrrencyUID &&
                                                            a.ToCurrency.Code == entry.Currency.Code);

        Assertion.AssertObject(exchangeRate, $"No hay tipo de cambio para la moneda {entry.Currency.FullName}.");

        entry.MultiplyBy(exchangeRate.Value);
      }
      return entries;
    }

    #region Private methods


    private void SummaryByCurrencyEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                   TrialBalanceEntry balanceEntry,
                                   Account targetAccount, Sector targetSector,
                                   TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapEntry(balanceEntry);

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = entry.InitialBalance > 0 ? entry.InitialBalance * -1 : entry.InitialBalance;
        entry.Debit = entry.Debit > 0 ? entry.Debit * -1 : entry.Debit;
        entry.Credit = entry.Credit > 0 ? entry.Credit * -1 : entry.Credit;
        entry.CurrentBalance = entry.CurrentBalance > 0 ? entry.CurrentBalance * -1 : entry.CurrentBalance;
      }

      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;

      string hash = $"{entry.GroupName}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByDebtorCreditorEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                              TrialBalanceEntry entry,
                                              Account targetAccount, Sector targetSector,
                                              TrialBalanceItemType itemType) {

      if (itemType == TrialBalanceItemType.BalanceTotalDeptor) {
        entry.GroupName = "TOTAL DEUDORAS " + entry.Currency.FullName;
      } else if (itemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.GroupName = "TOTAL ACREEDORAS " + entry.Currency.FullName;
      }

      string hash = $"{entry.GroupName}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                            TrialBalanceEntry entry,
                                            Account targetAccount, Sector targetSector,
                                            TrialBalanceItemType itemType) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                     TrialBalanceEntry balanceEntry,
                                     Account targetAccount, Sector targetSector,
                                     TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapEntry(balanceEntry);

      entry.GroupName = "TOTAL GRUPO";
      entry.GroupNumber = entry.Account.Number.Substring(0, 2) + "00";

      string hash = $"{entry.GroupNumber}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void GenerateOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry,
                                           Account targetAccount, Sector targetSector,
                                           TrialBalanceItemType itemType, string hash) {

      TrialBalanceEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new TrialBalanceEntry() {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = targetSector,
          Account = targetAccount,
          ItemType = itemType,
          GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName
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
