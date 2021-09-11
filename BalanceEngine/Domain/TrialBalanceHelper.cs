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
using System.Text.RegularExpressions;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build trial balances and related accounting information.</summary>
  internal class TrialBalanceHelper {

    private readonly TrialBalanceCommand _command;

    internal TrialBalanceHelper(TrialBalanceCommand command) {
      _command = command;
    }


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(
                                      List<TrialBalanceEntry> summaryEntries,
                                      FixedList<TrialBalanceEntry> postingEntries) {
      var returnedEntries = new List<TrialBalanceEntry>(postingEntries);

      if (_command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        foreach (var entry in summaryEntries.Where(a => a.SubledgerAccountIdParent > 0)) {
          returnedEntries.Add(entry);
        }

      } else {
        returnedEntries.AddRange(summaryEntries);
      }

      returnedEntries = OrderingTrialBalance(returnedEntries);

      return returnedEntries;
    }

    internal List<TrialBalanceEntry> CombineCurrencyTotalsAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      List<TrialBalanceEntry> summaryEntries) {
      var returnedEntries = new List<TrialBalanceEntry>();

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

    internal List<TrialBalanceEntry> TrialBalanceWithSubledgerAccounts(List<TrialBalanceEntry> trialBalance) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>(trialBalance);

      if (!_command.WithSubledgerAccount && _command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
        returnedEntries = returnedEntries.Where(a => a.SubledgerNumberOfDigits == 0
                                        //&& a.CurrentBalance != 0
                                        ).ToList();
      }

      return returnedEntries;
    }

    internal List<TrialBalanceEntry> CombineDebtorCreditorAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      List<TrialBalanceEntry> summaryEntries) {
      var returnedEntries = new List<TrialBalanceEntry>();

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
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalGroupDebtorEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {
        var debtorEntries = trialBalance.Where(
                                  a => a.Account.GroupNumber == totalGroupDebtorEntry.GroupNumber &&
                                  a.Ledger.Id == totalGroupDebtorEntry.Ledger.Id &&
                                  a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Deudora).ToList();
        debtorEntries.Add(totalGroupDebtorEntry);
        returnedEntries.AddRange(debtorEntries);
      }

      foreach (var creditorEntry in summaryEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)) {
        var creditorEntries = trialBalance.Where(
                                  a => a.Account.GroupNumber == creditorEntry.GroupNumber &&
                                  a.Ledger.Id == creditorEntry.Ledger.Id &&
                                  a.Currency.Id == creditorEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Acreedora).ToList();
        creditorEntries.Add(creditorEntry);
        returnedEntries.AddRange(creditorEntries);
      }

      return OrderByLedgerAndCurrency(returnedEntries);
    }


    internal List<TrialBalanceEntry> CombineTotalConsolidatedAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      List<TrialBalanceEntry> summaryEntries) {
      var entries = new List<TrialBalanceEntry>(trialBalance);

      var consolidated = summaryEntries.FirstOrDefault(
                                  a => a.ItemType == TrialBalanceItemType.BalanceTotalConsolidated);

      if (consolidated != null) {
        entries.Add(consolidated);
      }

      return entries;
    }


    internal List<TrialBalanceEntry> CombineTotalConsolidatedByLedgerAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      List<TrialBalanceEntry> totalConsolidatedByLedger) {
      if (totalConsolidatedByLedger.Count == 0) {
        return trialBalance;
      }
      var returnedEntries = new List<TrialBalanceEntry>();
      var totalConsolidated = new EmpiriaHashTable<TrialBalanceEntry>();
      foreach (var consolidatedByLedger in totalConsolidatedByLedger) {
        var listSummaryByLedger = trialBalance.Where(a => a.Ledger.Id == consolidatedByLedger.Ledger.Id).ToList();
        if (listSummaryByLedger.Count > 0) {
          listSummaryByLedger.Add(consolidatedByLedger);
          returnedEntries.AddRange(listSummaryByLedger);
        }
      }
      return returnedEntries;
    }


    internal FixedList<TrialBalanceEntry> ConsolidateToTargetCurrency(
                                          FixedList<TrialBalanceEntry> trialBalance,
                                          TrialBalanceCommandPeriod commandPeriod) {

      var targetCurrency = Currency.Parse(commandPeriod.ValuateToCurrrencyUID);

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


    internal List<TrialBalanceEntry> GenerateAverageDailyBalance(List<TrialBalanceEntry> trialBalance,
                                                            TrialBalanceCommandPeriod commandPeriod) {
      List<TrialBalanceEntry> averageBalances = new List<TrialBalanceEntry>(trialBalance);

      TimeSpan timeSpan = commandPeriod.ToDate - commandPeriod.FromDate;
      int numberOfDays = timeSpan.Days + 1;

      foreach (var entry in averageBalances) {
        entry.AverageBalance = entry.CurrentBalance / numberOfDays;
      }

      return averageBalances;
    }


    internal List<TrialBalanceEntry> GenerateSummaryEntries(FixedList<TrialBalanceEntry> entries) {
      var summaryEntries = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);
      var detailSummaryEntries = new List<TrialBalanceEntry>();
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var entry in entries) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountNumber = SubsidiaryAccount.Parse(entry.SubledgerAccountId).Number ?? "";
        StandardAccount currentParent;

        if ((entry.Account.NotHasParent) ||
            _command.WithSubledgerAccount ||
            _command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
          currentParent = entry.Account;

        } else if (_command.DoNotReturnSubledgerAccounts && entry.Account.HasParent) {
          currentParent = entry.Account.GetParent();

        } else if (_command.DoNotReturnSubledgerAccounts && entry.Account.NotHasParent) {
          continue;
        } else {
          throw Assertion.AssertNoReachThisCode();
        }

        int cont = 0;
        while (true) {

          entry.DebtorCreditor = entry.Account.DebtorCreditor;
          entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

          if (entry.Level > 1) {
            SummaryByEntry(summaryEntries, entry, currentParent, entry.Sector,
                                         TrialBalanceItemType.BalanceSummary);
          }

          cont++;
          if (cont == 1 && _command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
            GetDetailSummaryEntries(detailSummaryEntries, summaryEntries, currentParent, entry);
          }

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

      returnedEntries = AssignLastChangeDates(entries, summaryEntries);

      if (detailSummaryEntries.Count > 0 && _command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
        return detailSummaryEntries;
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryDebtorCreditor(
                                      List<TrialBalanceEntry> postingEntries) {

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


    internal List<TrialBalanceEntry> GenerateTotalSummaryConsolidated(
                                      List<TrialBalanceEntry> balanceEntries) {
      var totalSummaryConsolidated = new EmpiriaHashTable<TrialBalanceEntry>(balanceEntries.Count);

      foreach (var currencyEntry in balanceEntries.Where(
                a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

        TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(currencyEntry);

        entry.GroupName = "TOTAL CONSOLIDADO GENERAL";

        string hash;
        if (_command.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ||
             (_command.TrialBalanceType == TrialBalanceType.Balanza && _command.ShowCascadeBalances)) {
          if (_command.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada) {
            entry.GroupName = "TOTAL DEL REPORTE";
          }
          hash = $"{entry.GroupName}";
          entry.GroupNumber = "";
        } else {
          hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}";
        }

        GenerateOrIncreaseEntries(totalSummaryConsolidated, entry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalConsolidated, hash);
      }

      balanceEntries.AddRange(totalSummaryConsolidated.Values.ToList());

      return balanceEntries;
    }

    internal List<TrialBalanceEntry> GenerateTotalSummaryConsolidatedByLedger(
                                      List<TrialBalanceEntry> summaryCurrencies) {

      var summaryConsolidatedByLedger = new EmpiriaHashTable<TrialBalanceEntry>(summaryCurrencies.Count);
      List<TrialBalanceEntry> returnedListEntries = new List<TrialBalanceEntry>();
      if (_command.TrialBalanceType == TrialBalanceType.Balanza && _command.ShowCascadeBalances) {
        foreach (var currencyEntry in summaryCurrencies.Where(
                        a => a.ItemType == TrialBalanceItemType.BalanceTotalCurrency)) {

          TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(currencyEntry);

          entry.GroupName = $"TOTAL CONSOLIDADO {entry.Ledger.FullName}";
          entry.Currency = Currency.Empty;
          string hash = $"{entry.Ledger.Id}||{entry.GroupName}||{Sector.Empty.Code}";

          GenerateOrIncreaseEntries(summaryConsolidatedByLedger, entry, StandardAccount.Empty, Sector.Empty,
                                    TrialBalanceItemType.BalanceTotalConsolidatedByLedger, hash);
        }

        returnedListEntries.AddRange(summaryConsolidatedByLedger.Values.ToList());
      }
      return returnedListEntries.OrderBy(a => a.Ledger.Number).ToList();
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalSummaryGroups(FixedList<TrialBalanceEntry> entries) {

      var toReturnSummaryGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>(entries.Count);

      foreach (var entry in entries) {
        SummaryByGroupEntries(toReturnSummaryGroupEntries, entry);
      }

      return toReturnSummaryGroupEntries.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> GetPostingEntries() {
      
      FixedList<TrialBalanceEntry> postingEntries = GetTrialBalanceEntries(_command.InitialPeriod);

      if ((_command.ValuateBalances || _command.InitialPeriod.UseDefaultValuation) && 
          _command.TrialBalanceType != TrialBalanceType.BalanzaValorizadaEnDolares &&
          _command.TrialBalanceType != TrialBalanceType.BalanzaConsolidadaPorMoneda) {
        postingEntries = ValuateToExchangeRate(postingEntries, _command.InitialPeriod);

        if (_command.ConsolidateBalancesToTargetCurrency) {
          postingEntries = ConsolidateToTargetCurrency(postingEntries, _command.InitialPeriod);
        }
      }

      postingEntries = RoundTrialBalanceEntries(postingEntries);

      return postingEntries;
    }


    internal List<TrialBalanceEntry> GetSummaryAndPostingEntries() {
      FixedList<TrialBalanceEntry> postingEntries = GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = GenerateSummaryEntries(postingEntries);

      return CombineSummaryAndPostingEntries(summaryEntries, postingEntries);
    }

    internal FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceCommandPeriod commandPeriod) {

      if (_command.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada) {
        _command.ShowCascadeBalances = true;
      }

      TrialBalanceCommandData commandData = _command.MapToTrialBalanceCommandData(commandPeriod);

      return TrialBalanceDataService.GetTrialBalanceEntries(commandData);
    }


    internal List<TrialBalanceEntry> RestrictLevels(List<TrialBalanceEntry> entries) {
      if (_command.Level == 0) {
        return entries;
      }

      if (_command.DoNotReturnSubledgerAccounts) {
        return entries.FindAll(x => x.Level <= _command.Level);
      } else if (_command.WithSubledgerAccount) {
        return entries.FindAll(x => x.Level <= _command.Level);
      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    internal FixedList<TrialBalanceEntry> RoundTrialBalanceEntries(
                                          FixedList<TrialBalanceEntry> postingEntries) {
      FixedList<TrialBalanceEntry> roundedEntries = new FixedList<TrialBalanceEntry>(postingEntries);
      foreach (var posting in roundedEntries) {
        posting.InitialBalance = Math.Round(posting.InitialBalance, 2);
        posting.Debit = Math.Round(posting.Debit, 2);
        posting.Credit = Math.Round(posting.Credit, 2);
        posting.CurrentBalance = Math.Round(posting.CurrentBalance, 2);
      }
      return roundedEntries;
    }


    internal FixedList<TrialBalanceEntry> ValuateToExchangeRate(FixedList<TrialBalanceEntry> entries,
                                                                TrialBalanceCommandPeriod commandPeriod,
                                                                bool isSecondPeriod = false) {
      if (commandPeriod.UseDefaultValuation) {
        commandPeriod.ExchangeRateTypeUID = "96c617f6-8ed9-47f3-8d2d-f1240e446e1d";
        commandPeriod.ValuateToCurrrencyUID = "01";
        commandPeriod.ExchangeRateDate = commandPeriod.ToDate;
      }
      var exchangeRateType = ExchangeRateType.Parse(commandPeriod.ExchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, commandPeriod.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Code != "01")) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Code == commandPeriod.ValuateToCurrrencyUID &&
                                                              a.ToCurrency.Code == entry.Currency.Code);

        Assertion.AssertObject(exchangeRate, $"No hay tipo de cambio para la moneda {entry.Currency.FullName}.");

        if (_command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa) {
          if (isSecondPeriod) {
            entry.SecondExchangeRate = exchangeRate.Value;
          } else {
            entry.ExchangeRate = exchangeRate.Value;
          }
        } else if (_command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaEnDolares) {
          entry.ExchangeRate = exchangeRate.Value;
        } else {
          entry.MultiplyBy(exchangeRate.Value);
        }

      }
      return entries;
    }

    #region Private methods

    private List<TrialBalanceEntry> AssignLastChangeDates(
                                      FixedList<TrialBalanceEntry> entries,
                                      EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> summaryEntriesList =
                              new List<TrialBalanceEntry>(summaryEntries.ToFixedList().ToList());

      foreach (var entry in entries) {
        StandardAccount currentParent;
        currentParent = entry.Account.GetParent();

        LastChangeDateToSubsidiaryEntries(entry, summaryEntriesList);

        LastChangeDateToSummaryEntries(entry, currentParent, summaryEntriesList);

      }
      return summaryEntriesList.ToList();
    }


    private void GetDetailSummaryEntries(List<TrialBalanceEntry> detailSummaryEntries,
                                         EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                         StandardAccount currentParent, TrialBalanceEntry entry) {

      TrialBalanceEntry detailsEntry;
      string key = $"{currentParent.Number}||{entry.Sector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      summaryEntries.TryGetValue(key, out detailsEntry);
      if (detailsEntry != null) {
        var existEntry = detailSummaryEntries.FirstOrDefault(a => a.Ledger.Id == detailsEntry.Ledger.Id &&
                                                       a.Currency.Id == detailsEntry.Currency.Id &&
                                                       a.Account.Number == detailsEntry.Account.Number &&
                                                       a.Sector.Code == detailsEntry.Sector.Code);
        if (existEntry == null) {
          detailSummaryEntries.Add(detailsEntry);
        }
      }
    }


    private void LastChangeDateToSubsidiaryEntries(TrialBalanceEntry entry,
                                                    List<TrialBalanceEntry> summaryEntriesList) {
      var subsidiaryAccount = summaryEntriesList.Where(a => a.Account.Number == entry.Account.Number &&
                                                              a.Currency.Code == entry.Currency.Code &&
                                                              a.Sector.Code == entry.Sector.Code).ToList();
      foreach (var subsidiary in subsidiaryAccount) {
        if (entry.LastChangeDate > subsidiary.LastChangeDate) {
          subsidiary.LastChangeDate = entry.LastChangeDate;
        }
      }
    }


    private void LastChangeDateToSummaryEntries(TrialBalanceEntry entry, StandardAccount currentParent,
                                                  List<TrialBalanceEntry> summaryEntriesList) {
      while (true) {
        var summaryParent = summaryEntriesList.FirstOrDefault(
                                                a => a.Account.Number == currentParent.Number &&
                                                a.Currency.Code == entry.Currency.Code &&
                                                a.Sector.Code == entry.Sector.Code);

        if (summaryParent != null && entry.LastChangeDate > summaryParent.LastChangeDate) {
          summaryParent.LastChangeDate = entry.LastChangeDate;
        }

        if (!currentParent.HasParent) {
          var entryWithoutSector = summaryEntriesList.FirstOrDefault(
                                    a => a.Account.Number == currentParent.Number &&
                                    a.Currency.Code == entry.Currency.Code &&
                                    a.Sector.Code == "00");
          if (entryWithoutSector != null && entry.LastChangeDate > entryWithoutSector.LastChangeDate) {
            entryWithoutSector.LastChangeDate = entry.LastChangeDate;
          }
          break;
        } else {
          currentParent = currentParent.GetParent();
        }
      }
    }


    private List<TrialBalanceEntry> OrderingTrialBalance(List<TrialBalanceEntry> entries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      if (_command.WithSubledgerAccount && (_command.TrialBalanceType == TrialBalanceType.Balanza ||
          _command.TrialBalanceType == TrialBalanceType.SaldosPorCuenta)) {
        foreach (var entry in entries) {
          SubsidiaryAccount subledgerAccount = SubsidiaryAccount.Parse(entry.SubledgerAccountId);
          if (!subledgerAccount.IsEmptyInstance) {
            entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ? 
                                           subledgerAccount.Number : "";
            entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber != "" ?
                                            entry.SubledgerAccountNumber.Count() : 0;
          }
        }
        return returnedEntries = entries.Where(a => !a.SubledgerAccountNumber.Contains("undefined"))
                                  .OrderBy(a => a.Ledger.Number)
                                  .ThenBy(a => a.Currency.Code)
                                  .ThenByDescending(a => a.Account.DebtorCreditor)
                                  .ThenBy(a => a.Account.Number)
                                  .ThenBy(a => a.Sector.Code)
                                  .ThenBy(a => a.SubledgerNumberOfDigits)
                                  .ThenBy(a => a.SubledgerAccountNumber)
                                  .ToList();
      } else {
        return returnedEntries = entries.OrderBy(a => a.Ledger.Number)
                                  .ThenBy(a => a.Currency.Code)
                                  .ThenByDescending(a => a.Account.DebtorCreditor)
                                  .ThenBy(a => a.Account.Number)
                                  .ThenBy(a => a.Sector.Code)
                                  .ThenBy(a => a.SubledgerAccountNumber)
                                  .ToList();
      }
    }


    internal void SummaryByCurrencyEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                          TrialBalanceEntry balanceEntry,
                                          StandardAccount targetAccount, Sector targetSector,
                                          TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      if (_command.TrialBalanceType != TrialBalanceType.BalanzaConContabilidadesEnCascada &&
           entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = -1 * entry.InitialBalance;
        entry.CurrentBalance = -1 * entry.CurrentBalance;
      }

      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;
      string hash;
      if (_command.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada) {
        entry.GroupNumber = "";
        hash = $"{entry.GroupName}||{entry.Currency.Id}";
      } else {
        hash = $"{entry.GroupName}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";
      }
      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    internal void SummaryDebtorCreditorLedgersByAccount(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                TrialBalanceEntry balanceEntry,
                                                TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      if (itemType == TrialBalanceItemType.BalanceTotalDebtor) {
        entry.GroupName = "TOTAL DEUDORAS " + entry.Currency.FullName;
      } else if (itemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.GroupName = "TOTAL ACREEDORAS " + entry.Currency.FullName;
      }
      entry.Ledger = Ledger.Empty;
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;

      string hash = $"{entry.GroupName}||{entry.Currency.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, StandardAccount.Empty, Sector.Empty, itemType, hash);
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


    internal void SummaryBySubledgerEntry(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                 TrialBalanceEntry entry, TrialBalanceItemType itemType) {

      string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                        $"{entry.SubledgerAccountIdParent}||{Sector.Empty.Code}";

      GenerateOrIncreaseEntries(summaryEntries, entry, StandardAccount.Empty, Sector.Empty, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                       TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry groupEntry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      groupEntry.GroupName = $"TOTAL GRUPO {balanceEntry.Account.GroupNumber}";
      groupEntry.GroupNumber = balanceEntry.Account.GroupNumber;
      groupEntry.Account = StandardAccount.Empty;
      groupEntry.Sector = Sector.Empty;
      groupEntry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;

      string hash;

      if (groupEntry.DebtorCreditor == DebtorCreditorType.Deudora) {
        hash = $"{balanceEntry.Ledger.Id}||{balanceEntry.Currency.Id}||{balanceEntry.Account.GroupNumber}||D";

        GenerateOrIncreaseEntries(summaryEntries, groupEntry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalGroupDebtor, hash);

      } else {
        hash = $"{balanceEntry.Ledger.Id}||{balanceEntry.Currency.Id}||{balanceEntry.Account.GroupNumber}||A";

        GenerateOrIncreaseEntries(summaryEntries, groupEntry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalGroupCreditor, hash);
      }
    }


    internal void SummaryByLedgersGroupEntries(EmpiriaHashTable<TrialBalanceEntry> totalsListByGroupEntries,
                                                TrialBalanceEntry balanceEntry) {
      TrialBalanceEntry groupEntry = TrialBalanceMapper.MapToTrialBalanceEntry(balanceEntry);

      groupEntry.GroupName = $"SUMA DE DELEGACIONES";
      groupEntry.GroupNumber = balanceEntry.Account.Number;
      groupEntry.Account = balanceEntry.Account;
      groupEntry.Sector = balanceEntry.Sector;
      groupEntry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;
      groupEntry.Ledger = Ledger.Empty;

      TrialBalanceItemType itemType = TrialBalanceItemType.BalanceTotalGroupDebtor;

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {
        itemType = TrialBalanceItemType.BalanceTotalGroupCreditor;
      }

      string hash = $"{balanceEntry.Currency.Id}||{groupEntry.GroupNumber}||" +
                    $"{groupEntry.Sector.Code}||{groupEntry.DebtorCreditor}";

      GenerateOrIncreaseEntries(totalsListByGroupEntries, groupEntry, groupEntry.Account,
                                groupEntry.Sector, itemType, hash);
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
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent,
          LastChangeDate = entry.LastChangeDate
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
