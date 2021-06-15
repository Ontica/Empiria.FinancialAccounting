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
using System;

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

      returnedEntries = returnedEntries.OrderBy(a => a.Ledger.Number)
                                       .ThenBy(a => a.Currency.Code)
                                       .ThenByDescending(a => a.Account.DebtorCreditor)
                                       .ThenBy(a => a.Account.Number)
                                       .ThenBy(a => a.Sector.Code)
                                       .ThenBy(a => a.SubledgerAccountId)
                                       .ToList();

      return returnedEntries;
    }


    internal List<TrialBalanceEntry> BalancesBySubsidiaryAccounts(List<TrialBalanceEntry> trialBalance) {
      if (_command.TrialBalanceType != TrialBalanceType.SaldosPorAuxiliar) {
        return trialBalance;
      }

      List<TrialBalanceEntry> subsidiaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in trialBalance.Where(a=>a.DebtorCreditor == DebtorCreditorType.Deudora)) {
        if (entry.SubledgerAccountId > 0) {

          var returnedList = subsidiaryEntries.Where(a => a.SubledgerAccountId == entry.SubledgerAccountId &&
                                                            a.Ledger.Id == entry.Ledger.Id &&
                                                            a.Currency.Code == entry.Currency.Code &&
                                                            a.DebtorCreditor == DebtorCreditorType.Deudora)
                                              .ToList();
          if (returnedList.Count == 0) {
            subsidiaryEntries.Add(entry);
          }
        }
      }

      foreach (var entry in trialBalance.Where(a => a.DebtorCreditor == DebtorCreditorType.Acreedora)) {
        if (entry.SubledgerAccountId > 0) {

          var returnedList = subsidiaryEntries.Where(a => a.SubledgerAccountId == entry.SubledgerAccountId &&
                                                            a.Ledger.Id == entry.Ledger.Id &&
                                                            a.Currency.Code == entry.Currency.Code &&
                                                            a.DebtorCreditor == DebtorCreditorType.Acreedora)
                                              .ToList();
          if (returnedList.Count == 0) {
            subsidiaryEntries.Add(entry);
          }
        }
      }
      subsidiaryEntries = subsidiaryEntries.OrderBy(a => a.Ledger.Number)
                                           .ThenBy(a => a.Currency.Code)
                                           .ThenByDescending(a => a.Account.DebtorCreditor)
                                           .ThenBy(a => a.Account.Number)
                                           .ThenBy(a => a.Sector.Code)
                                           .ThenBy(a => a.SubledgerAccountId)
                                           .ToList();

      subsidiaryEntries = CombineSubsidiaryAccounts(trialBalance, subsidiaryEntries);

      return subsidiaryEntries;
    }

    internal List<TrialBalanceEntry> CombineSubsidiaryAccounts(List<TrialBalanceEntry> trialBalance, List<TrialBalanceEntry> subsidiaryEntries) {

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();
      
      foreach (var returned in subsidiaryEntries.Where(a => a.DebtorCreditor == DebtorCreditorType.Deudora)) {
        List<TrialBalanceEntry> debtorEntries = new List<TrialBalanceEntry>();

        debtorEntries = trialBalance.Where(a => a.Currency.Code == returned.Currency.Code &&
                                              a.SubledgerAccountId == 0 &&
                                              a.SubledgerAccountIdParent == returned.SubledgerAccountId &&
                                              a.DebtorCreditor == DebtorCreditorType.Deudora)
                                  .ToList();

        debtorEntries = debtorEntries.OrderBy(a => a.Ledger.Number)
                         .ThenBy(a => a.Currency.Code)
                         .ThenBy(a => a.Account.Number)
                         .ThenBy(a => a.Sector.Code)
                         .ToList();

        returnedEntries.Add(returned);
        if (debtorEntries.Count > 0) {
          returnedEntries.AddRange(debtorEntries);
        } else if (debtorEntries.Count == 0 && returned.Level > 1) {
          var summaryEntries = new EmpiriaHashTable<TrialBalanceEntry>();
          StandardAccount currentParent;
          currentParent = returned.Account;
          while (true) {
            SummaryByEntry(summaryEntries, returned, currentParent, returned.Sector,
                                           TrialBalanceItemType.BalanceSummary);
            if (!currentParent.HasParent && returned.Sector.Code != "00") {
              SummaryByEntry(summaryEntries, returned, currentParent, Sector.Empty,
                                             TrialBalanceItemType.BalanceSummary);
              break;
            } else if (!currentParent.HasParent) {
              break;
            } else {
              currentParent = currentParent.GetParent();
            }
          } // while
          debtorEntries = summaryEntries.Values.ToList();
          debtorEntries =debtorEntries.OrderBy(a => a.Ledger.Number)
                         .ThenBy(a => a.Currency.Code)
                         .ThenBy(a => a.Account.Number)
                         .ThenBy(a => a.Sector.Code)
                         .ToList();

          returnedEntries.AddRange(debtorEntries);
        }
      }

      foreach (var returned in subsidiaryEntries.Where(a => a.DebtorCreditor == DebtorCreditorType.Acreedora)) {
        List<TrialBalanceEntry> creditorEntries = new List<TrialBalanceEntry>();

        creditorEntries = trialBalance.Where(a => a.Currency.Code == returned.Currency.Code &&
                                              a.SubledgerAccountId == 0 &&
                                              a.SubledgerAccountIdParent == returned.SubledgerAccountId &&
                                              a.DebtorCreditor == DebtorCreditorType.Acreedora)
                                  .ToList();

        creditorEntries = creditorEntries.OrderBy(a => a.Ledger.Number)
                         .ThenBy(a => a.Currency.Code)
                         .ThenBy(a => a.Account.Number)
                         .ThenBy(a => a.Sector.Code)
                         .ToList();

        returnedEntries.Add(returned);
        if (creditorEntries.Count > 0) {
          returnedEntries.AddRange(creditorEntries);
        } else if (creditorEntries.Count == 0 && returned.Level > 1) {
          var summaryEntries = new EmpiriaHashTable<TrialBalanceEntry>();
          StandardAccount currentParent;
          currentParent = returned.Account;
          while (true) {
            SummaryByEntry(summaryEntries, returned, currentParent, returned.Sector,
                                           TrialBalanceItemType.BalanceSummary);
            if (!currentParent.HasParent && returned.Sector.Code != "00") {
              SummaryByEntry(summaryEntries, returned, currentParent, Sector.Empty,
                                             TrialBalanceItemType.BalanceSummary);
              break;
            } else if (!currentParent.HasParent) {
              break;
            } else {
              currentParent = currentParent.GetParent();
            }
          } // while
          creditorEntries = summaryEntries.Values.ToList();
          creditorEntries = creditorEntries.OrderBy(a => a.Ledger.Number)
                         .ThenBy(a => a.Currency.Code)
                         .ThenBy(a => a.Account.Number)
                         .ThenBy(a => a.Sector.Code)
                         .ToList();
          returnedEntries.AddRange(summaryEntries.Values.ToList());
        }
      }

      return returnedEntries;
    }

    internal List<TrialBalanceEntry> CombineCurrencyAndPostingEntries(List<TrialBalanceEntry> trialBalance,
                                                                    List<TrialBalanceEntry> summaryEntries) {

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var currencyEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        var listSummary = trialBalance.Where(a => a.Ledger.Id == currencyEntry.Ledger.Id &&
                                             a.Currency.Code == currencyEntry.Currency.Code).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(currencyEntry);
          returnedEntries.AddRange(listSummary);
        }
      }
      returnedEntries = returnedEntries.OrderBy(a => a.Ledger.Number).ThenBy(a => a.Currency.Code).ToList();

      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineDebtorCreditorAndPostingEntries(List<TrialBalanceEntry> trialBalance,
                                                                            List<TrialBalanceEntry> summaryEntries) {

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var deptorCreditorEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {

        var listSummary = trialBalance.Where(a => a.Ledger.Id == deptorCreditorEntry.Ledger.Id &&
                                                  a.Currency.Code == deptorCreditorEntry.Currency.Code &&
                                                  a.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(deptorCreditorEntry);
          returnedEntries.AddRange(listSummary);
        }
      }
      foreach (var deptorCreditorEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        var listSummary = trialBalance.Where(a => a.Ledger.Id == deptorCreditorEntry.Ledger.Id &&
                                                  a.Currency.Code == deptorCreditorEntry.Currency.Code &&
                                                  a.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(deptorCreditorEntry);
          returnedEntries.AddRange(listSummary);
        }
      }
      returnedEntries = returnedEntries.OrderBy(a => a.Ledger.Number).ThenBy(a => a.Currency.Code).ToList();

      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineGroupEntriesAndPostingEntries(List<TrialBalanceEntry> trialBalance,
                                                                          FixedList<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var groupEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {

        var listSummary = trialBalance.Where(
                                  a => a.Account.Number.Substring(0, 2) + "00" == groupEntry.GroupNumber &&
                                  a.Ledger.Id == groupEntry.Ledger.Id &&
                                  a.Currency.Code == groupEntry.Currency.Code &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(groupEntry);
          returnedEntries.AddRange(listSummary);
        }
      }

      foreach (var groupEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)) {

        var listSummary = trialBalance.Where(
                                  a => a.Account.Number.Substring(0, 2) + "00" == groupEntry.GroupNumber &&
                                  a.Ledger.Id == groupEntry.Ledger.Id &&
                                  a.Currency.Code == groupEntry.Currency.Code &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        if (listSummary.Count > 0) {
          listSummary.Add(groupEntry);
          returnedEntries.AddRange(listSummary);
        }
      }
      returnedEntries = returnedEntries.OrderBy(a => a.Ledger.Number).ThenBy(a => a.Currency.Code).ToList();

      return returnedEntries;
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

          if (!currentParent.HasParent && entry.Sector.Code != "00") {

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

      foreach (var debtorCreditorEntry in entries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalDebtor ||
                     a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {

        SummaryByCurrencyEntries(totalSummaryCurrencies, debtorCreditorEntry, StandardAccount.Empty,
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

        GenerateOrIncreaseEntries(totalSummaryConsolidated, entry, StandardAccount.Empty, Sector.Empty, TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      balanceEntries.AddRange(totalSummaryConsolidated.Values.ToList());

      return balanceEntries;
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalSummaryGroup(FixedList<TrialBalanceEntry> entries) {
      var totalSummaryGroup = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries) {

        if (entry.Account.DebtorCreditor == DebtorCreditorType.Deudora) {

          SummaryByGroupEntries(totalSummaryGroup, entry, StandardAccount.Empty,
                                Sector.Empty, TrialBalanceItemType.BalanceTotalGroupDebtor);

        }
        if (entry.Account.DebtorCreditor == DebtorCreditorType.Acreedora) {

          SummaryByGroupEntries(totalSummaryGroup, entry, StandardAccount.Empty,
                                Sector.Empty, TrialBalanceItemType.BalanceTotalGroupCreditor);

        }
      }

      return totalSummaryGroup.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries() {
      TrialBalanceCommandData commandData = _command.MapToTrialBalanceCommandData();

      return TrialBalanceDataService.GetTrialBalanceEntries(commandData);
    }


    internal FixedList<ITrialBalanceEntry> MergeAccountsIntoTwoColumnsByCurrency(List<TrialBalanceEntry> trialBalance) {
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


    private void SummaryByEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                            TrialBalanceEntry entry,
                                            StandardAccount targetAccount, Sector targetSector,
                                            TrialBalanceItemType itemType) {

      string hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                     TrialBalanceEntry balanceEntry,
                                     StandardAccount targetAccount, Sector targetSector,
                                     TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      entry.GroupName = $"TOTAL GRUPO {entry.Account.Number.Substring(0, 2)}00";
      entry.GroupNumber = $"{entry.Account.Number.Substring(0, 2)}00";
      entry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;

      string hash = $"{entry.GroupNumber}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void GenerateOrIncreaseEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                           TrialBalanceEntry entry,
                                           StandardAccount targetAccount, Sector targetSector,
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
