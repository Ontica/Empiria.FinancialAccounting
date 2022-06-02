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


    internal List<TrialBalanceEntry> CombineGroupEntriesAndPostingEntries(
                                      List<TrialBalanceEntry> trialBalance,
                                      FixedList<TrialBalanceEntry> summaryGroupEntries) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var totalGroupDebtorEntry in summaryGroupEntries
                    .Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {

        var debtorEntries = trialBalance.Where(
                                  a => a.Account.GroupNumber == totalGroupDebtorEntry.GroupNumber &&
                                  a.Ledger.Id == totalGroupDebtorEntry.Ledger.Id &&
                                  a.Currency.Id == totalGroupDebtorEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == DebtorCreditorType.Deudora).ToList();

        debtorEntries.Add(totalGroupDebtorEntry);
        returnedEntries.AddRange(debtorEntries);
      }

      foreach (var creditorEntry in summaryGroupEntries
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


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(
                                      List<TrialBalanceEntry> summaryEntries,
                                      FixedList<TrialBalanceEntry> postingEntries) {
      var returnedEntries = new List<TrialBalanceEntry>(postingEntries);
      if (_query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        foreach (var entry in summaryEntries.Where(a => a.SubledgerAccountIdParent > 0)) {
          returnedEntries.Add(entry);
        }

      } else {
        returnedEntries.AddRange(summaryEntries);
      }

      SetSubledgerAccountInfo(returnedEntries);

      returnedEntries = OrderingTrialBalance(returnedEntries);

      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineTotalConsolidatedAndAccountEntries(
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


    internal List<TrialBalanceEntry> GenerateAverageDailyBalance(List<TrialBalanceEntry> trialBalance,
                                                                 BalancesPeriod period) {
      List<TrialBalanceEntry> averageBalances = new List<TrialBalanceEntry>(trialBalance);

      TimeSpan timeSpan = period.ToDate - period.FromDate;
      int numberOfDays = timeSpan.Days + 1;

      foreach (var entry in averageBalances) {
        entry.AverageBalance = entry.CurrentBalance / numberOfDays;
      }

      return averageBalances;
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


    internal FixedList<TrialBalanceEntry> GetPostingEntries() {

      FixedList<TrialBalanceEntry> accountEntries = ReadAccountEntriesFromDataService();

      if ((_query.ValuateBalances || _query.InitialPeriod.UseDefaultValuation) &&
          _query.TrialBalanceType != TrialBalanceType.BalanzaDolarizada &&
          _query.TrialBalanceType != TrialBalanceType.BalanzaEnColumnasPorMoneda) {
        accountEntries = ValuateToExchangeRate(accountEntries, _query.InitialPeriod);

        if (_query.ConsolidateBalancesToTargetCurrency) {
          accountEntries = ConsolidateToTargetCurrency(accountEntries, _query.InitialPeriod);
        }
      }

      RoundDecimals(accountEntries);

      return accountEntries;
    }


    internal void GetSummarySectorizedAccountByLevel(List<TrialBalanceEntry> summaryEntries) {

      var EntriesList = new List<TrialBalanceEntry>(summaryEntries);

      if (_query.UseNewSectorizationModel) {
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
    }


    internal List<TrialBalanceEntry> GetSummaryAccountEntriesAndSectorization(
                                      List<TrialBalanceEntry> entriesList) {

      var startTime = DateTime.Now;
      var returnedEntries = new List<TrialBalanceEntry>(entriesList);

      if (_query.UseNewSectorizationModel && _query.WithSectorization) {

        GetSummaryEntriesWithSectorization(returnedEntries);
        EmpiriaLog.Debug($"INNER GetSummaryEntriesWithSectorization(): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");

      } else if (_query.UseNewSectorizationModel && !_query.WithSectorization) {

        GetSummaryEntriesWithoutSectorization(returnedEntries);
        EmpiriaLog.Debug($"INNER GetSummaryEntriesWithoutSectorization(): {DateTime.Now.Subtract(startTime).TotalSeconds} seconds.");
      }
      if (_query.TrialBalanceType != TrialBalanceType.AnaliticoDeCuentas &&
          _query.TrialBalanceType != TrialBalanceType.Balanza) {

        GetSummarySectorizedAccountByLevel(returnedEntries);
      }

      return returnedEntries;
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


    internal List<TrialBalanceEntry> GetCalculatedParentAccounts(FixedList<TrialBalanceEntry> accountEntries) {
      var parentAccounts = new EmpiriaHashTable<TrialBalanceEntry>(accountEntries.Count);

      var detailSummaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in accountEntries) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountNumber = SubledgerAccount.Parse(entry.SubledgerAccountId).Number ?? "";
        StandardAccount currentParent;

        if ((entry.Account.NotHasParent) ||
            _query.WithSubledgerAccount ||
            _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
          currentParent = entry.Account;

        } else if (_query.DoNotReturnSubledgerAccounts && entry.Account.HasParent) {
          currentParent = entry.Account.GetParent();

        } else if (_query.DoNotReturnSubledgerAccounts && entry.Account.NotHasParent) {
          continue;
        } else {
          throw Assertion.EnsureNoReachThisCode();
        }

        if (entry.HasParentPostingEntry) {
          continue;
        }

        int cont = 0;

        while (true) {
          entry.DebtorCreditor = entry.Account.DebtorCreditor;
          entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

          if (entry.Level > 1) {
            SummaryByEntry(parentAccounts, entry, currentParent,
                            entry.Sector, TrialBalanceItemType.Summary);

            SummaryEntryBySectorization(parentAccounts, entry, currentParent);
          }

          cont++;

          if (cont == 1 && _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
            GetDetailSummaryEntries(detailSummaryEntries, parentAccounts, currentParent, entry);
          }
          if (!currentParent.HasParent && entry.HasSector) {
            GetEntriesAndParentSector(parentAccounts, entry, currentParent);
            break;

          } else if (!currentParent.HasParent) {
            if (_query.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas &&
                _query.WithSubledgerAccount && !entry.Account.HasParent) {
              SummaryByEntry(parentAccounts, entry, currentParent,
                              Sector.Empty, TrialBalanceItemType.Summary);
            }
            break;
          } else {
            currentParent = currentParent.GetParent();
          }

        } // while

      } // foreach

      AssignLastChangeDatesToSummaryEntries(accountEntries, parentAccounts);

      if (detailSummaryEntries.Count > 0 && _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
        return detailSummaryEntries;
      }
      return parentAccounts.ToFixedList().ToList();
    }


    internal List<TrialBalanceEntry> GenerateTotalSummaryDebtorCreditor(
                                      List<TrialBalanceEntry> postingEntries) {

      var totalSummaryDebtorCredtor = new EmpiriaHashTable<TrialBalanceEntry>(postingEntries.Count);

      foreach (var entry in postingEntries.Where(a => !a.HasParentPostingEntry)) {

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

        TrialBalanceEntry entry = currencyEntry.CreatePartialCopy();

        entry.GroupName = "TOTAL CONSOLIDADO GENERAL";

        string hash = $"{entry.GroupName}||{Sector.Empty.Code}||{entry.Ledger.Id}";

        if (_query.TrialBalanceType == TrialBalanceType.Balanza && _query.ShowCascadeBalances) {

          hash = $"{entry.GroupName}";
          entry.GroupNumber = "";
        }

        if (_query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta &&
            ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
             _query.ShowCascadeBalances)) {

          hash = $"{entry.GroupName}||{Sector.Empty.Code}";
        }


        GenerateOrIncreaseEntries(totalSummaryConsolidated, entry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalConsolidated, hash);
      } // foreach

      balanceEntries.AddRange(totalSummaryConsolidated.Values.ToList());

      return balanceEntries;
    }


    internal FixedList<TrialBalanceEntry> GenerateTotalSummaryGroups(FixedList<TrialBalanceEntry> entries) {

      var toReturnSummaryGroupEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in entries.Where(a => !a.HasParentPostingEntry)) {
        SummaryByGroupEntries(toReturnSummaryGroupEntries, entry);
      }

      return toReturnSummaryGroupEntries.ToFixedList();
    }


    internal FixedList<TrialBalanceEntry> ReadAccountEntriesFromDataService() {

      if (_query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada) {
        _query.ShowCascadeBalances = true;
      }

      return BalancesDataService.GetTrialBalanceEntries(_query);
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


    internal void SummaryByAccount(EmpiriaHashTable<TrialBalanceEntry> entries, TrialBalanceEntry balanceEntry) {

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


    internal void SummaryByCurrencyEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                          TrialBalanceEntry balanceEntry,
                                          StandardAccount targetAccount, Sector targetSector,
                                          TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.InitialBalance = -1 * entry.InitialBalance;
        entry.CurrentBalance = -1 * entry.CurrentBalance;
      }
      entry.GroupName = "TOTAL MONEDA " + entry.Currency.FullName;

      string hash = $"{entry.GroupName}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
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

        hash = $"{targetAccount.Number}||{targetSector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}||{entry.DebtorCreditor}";

      }
      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    internal List<TrialBalanceEntry> TrialBalanceWithSubledgerAccounts(List<TrialBalanceEntry> trialBalance) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>(trialBalance);

      if (!_query.WithSubledgerAccount && _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
        returnedEntries = returnedEntries.Where(a => a.SubledgerNumberOfDigits == 0).ToList();
      }
      if (_query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {
        returnedEntries = returnedEntries.Where(
                            a => a.ItemType != TrialBalanceItemType.BalanceTotalGroupDebtor &&
                                 a.ItemType != TrialBalanceItemType.BalanceTotalGroupCreditor)
                                         .ToList();
      }

      return returnedEntries;
    }


    internal void ValuateAccountEntriesToExchangeRate(FixedList<TrialBalanceEntry> entries) {

      if (_query.InitialPeriod.UseDefaultValuation) {
        _query.InitialPeriod.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
        _query.InitialPeriod.ValuateToCurrrencyUID = "01";
        _query.InitialPeriod.ExchangeRateDate = _query.InitialPeriod.ToDate;
      }

      var exchangeRateType = ExchangeRateType.Parse(_query.InitialPeriod.ExchangeRateTypeUID);
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType,
                                                                   _query.InitialPeriod.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Code != "01")) {

        var exchangeRate = exchangeRates.FirstOrDefault(
                            a => a.FromCurrency.Code == _query.InitialPeriod.ValuateToCurrrencyUID &&
                            a.ToCurrency.Code == entry.Currency.Code);

        // ToDo: URGENT This require must be checked before any state change
        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} con la fecha proporcionada.");

        entry.MultiplyBy(exchangeRate.Value);
      }
    }


    internal FixedList<TrialBalanceEntry> ValuateToExchangeRate(FixedList<TrialBalanceEntry> entries,
                                                                BalancesPeriod period) {
      if (period.UseDefaultValuation) {
        period.ExchangeRateTypeUID = ExchangeRateType.ValorizacionBanxico.UID;
        period.ValuateToCurrrencyUID = "01";
        period.ExchangeRateDate = period.ToDate;
      }

      var exchangeRateType = ExchangeRateType.Parse(period.ExchangeRateTypeUID);

      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(exchangeRateType, period.ExchangeRateDate);

      foreach (var entry in entries.Where(a => a.Currency.Code != "01")) {
        var exchangeRate = exchangeRates.FirstOrDefault(a => a.FromCurrency.Code == period.ValuateToCurrrencyUID &&
                                                             a.ToCurrency.Code == entry.Currency.Code);

        // ToDo: URGENT This require must be checked before any state
        Assertion.Require(exchangeRate, $"No se ha registrado el tipo de cambio para la " +
                                        $"moneda {entry.Currency.FullName} en la fecha proporcionada.");

        if (_query.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa) {
          if (period.IsSecondPeriod) {
            entry.SecondExchangeRate = exchangeRate.Value;
          } else {
            entry.ExchangeRate = exchangeRate.Value;
          }
        } else if ((_query.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) ||
                  (_query.IsOperationalReport && !_query.ConsolidateBalancesToTargetCurrency)) {
          entry.ExchangeRate = exchangeRate.Value;
        } else {
          entry.MultiplyBy(exchangeRate.Value);
        }

      }
      return entries;
    }

    #region Private methods

    internal void AssignLastChangeDatesToSummaryEntries(
                                      FixedList<TrialBalanceEntry> entries,
                                      EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {

      var summaryEntriesList = new List<TrialBalanceEntry>(summaryEntries.Values);

      foreach (var entry in entries) {
        SetLastChangeDateToSummaryEntries(entry, summaryEntriesList);

        SetLastChangeDateToParentEntries(entry, summaryEntriesList);
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


    private void GetDetailSummaryEntries(List<TrialBalanceEntry> detailSummaryEntries,
                                         EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                         StandardAccount currentParent, TrialBalanceEntry entry) {

      TrialBalanceEntry detailsEntry;
      string key = $"{currentParent.Number}||{entry.Sector.Code}||{entry.Currency.Id}||{entry.Ledger.Id}";

      summaryEntries.TryGetValue(key, out detailsEntry);

      if (detailsEntry != null) {
        var existEntry = detailSummaryEntries.FirstOrDefault(a =>
                                                       a.Account.Number == detailsEntry.Account.Number &&
                                                       a.Ledger.Id == detailsEntry.Ledger.Id &&
                                                       a.Currency.Id == detailsEntry.Currency.Id &&
                                                       a.Sector.Code == detailsEntry.Sector.Code);
        if (existEntry == null) {
          detailSummaryEntries.Add(detailsEntry);
        }
      }
    }


    private void SetSubledgerAccountInfo(List<TrialBalanceEntry> entries) {
      if (!_query.WithSubledgerAccount) {
        return;
      }
      if (_query.TrialBalanceType != TrialBalanceType.Balanza &&
          _query.TrialBalanceType != TrialBalanceType.SaldosPorCuenta) {
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


    private void GetSummaryEntriesWithoutSectorization(
                                    List<TrialBalanceEntry> returnedEntries) {

      var hashEntries = new EmpiriaHashTable<TrialBalanceEntry>();
      var checkSummaryEntries = new List<TrialBalanceEntry>(returnedEntries);

      foreach (var entry in checkSummaryEntries) {
        var sectorParent = entry.Sector.Parent;
        var returnedEntry = returnedEntries.FirstOrDefault(a => a.Account.Number == entry.Account.Number &&
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

      if (hashEntries.Count > 0) {
        foreach (var hashEntry in hashEntries.ToFixedList().ToList()) {
          var entry = returnedEntries.FirstOrDefault(
                                      a => a.Account.Number == hashEntry.Account.Number &&
                                           a.Ledger.Number == hashEntry.Ledger.Number &&
                                           a.Currency.Code == hashEntry.Currency.Code &&
                                           a.Sector.Code == hashEntry.Sector.Code && a.Sector.Code == "00");
          if (entry == null) {
            returnedEntries.Add(hashEntry);
          } else {
            hashEntry.Sum(entry);
            returnedEntries.Remove(entry);
            returnedEntries.Add(hashEntry);
          }
        }
      }
    }


    private void GetSummaryEntriesWithSectorization(
                                    List<TrialBalanceEntry> returnedEntries) {

      var hashEntries = new EmpiriaHashTable<TrialBalanceEntry>();
      var checkSummaryEntries = new List<TrialBalanceEntry>(returnedEntries);

      foreach (var entry in checkSummaryEntries) {
        var sectorParent = entry.Sector.Parent;
        var returnedEntry = returnedEntries.FirstOrDefault(a => a.Account.Number == entry.Account.Number &&
                                                               a.Ledger.Number == entry.Ledger.Number &&
                                                               a.Currency.Code == entry.Currency.Code &&
                                                               a.Sector.Code == "00");
        if (returnedEntry != null && sectorParent.Code != "00" && entry.Level > 1) {
          returnedEntry.Sum(entry);

        } else if (entry.Level > 1 && (sectorParent.Code != "00" ||
             (entry.ItemType == TrialBalanceItemType.Entry &&
              entry.HasSector))) {

          SummaryByEntry(hashEntries, entry, entry.Account, Sector.Empty, entry.ItemType);
        }
      }

      returnedEntries.AddRange(hashEntries.ToFixedList().ToList());
    }


    private void SetLastChangeDateToParentEntries(TrialBalanceEntry entry,
                                                  List<TrialBalanceEntry> summaryEntriesList) {
      StandardAccount currentParentAccount = entry.Account.GetParent();

      while (true) {
        var parentEntry = summaryEntriesList.FirstOrDefault(
                                                a => a.Account.Number == currentParentAccount.Number &&
                                                a.Currency.Code == entry.Currency.Code &&
                                                a.Sector.Code == entry.Sector.Code);

        if (parentEntry != null && entry.LastChangeDate > parentEntry.LastChangeDate) {
          parentEntry.LastChangeDate = entry.LastChangeDate;
        }

        if (!currentParentAccount.HasParent) {
          var entryWithoutSector = summaryEntriesList.FirstOrDefault(
                                    a => a.Account.Number == currentParentAccount.Number &&
                                    a.Currency.Code == entry.Currency.Code &&
                                    a.Sector.Code == "00");
          if (entryWithoutSector != null && entry.LastChangeDate > entryWithoutSector.LastChangeDate) {
            entryWithoutSector.LastChangeDate = entry.LastChangeDate;
          }
          break;
        } else {
          currentParentAccount = currentParentAccount.GetParent();
        }
      }
    }


    private List<TrialBalanceEntry> OrderByLedgerAndCurrency(List<TrialBalanceEntry> entries) {
      return entries.OrderBy(a => a.Ledger.Number)
                    .ThenBy(a => a.Currency.Code)
                    .ToList();
    }


    private List<TrialBalanceEntry> OrderingTrialBalance(List<TrialBalanceEntry> entries) {

      if (_query.WithSubledgerAccount && (_query.TrialBalanceType == TrialBalanceType.Balanza ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta)) {

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


    private void SetLastChangeDateToSummaryEntries(TrialBalanceEntry entry,
                                                   List<TrialBalanceEntry> summaryEntries) {

      var filtered = summaryEntries.Where(a => a.Account.Number == entry.Account.Number &&
                                               a.Currency.Code == entry.Currency.Code &&
                                               a.Sector.Code == entry.Sector.Code &&
                                               entry.LastChangeDate > a.LastChangeDate);

      foreach (var summaryToChange in filtered) {
        summaryToChange.LastChangeDate = entry.LastChangeDate;
      }
    }


    private void SummaryByDebtorCreditorEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                TrialBalanceEntry balanceEntry,
                                                StandardAccount targetAccount, Sector targetSector,
                                                TrialBalanceItemType itemType) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      if (itemType == TrialBalanceItemType.BalanceTotalDebtor) {
        entry.GroupName = "TOTAL DEUDORAS " + entry.Currency.FullName;
      } else if (itemType == TrialBalanceItemType.BalanceTotalCreditor) {
        entry.GroupName = "TOTAL ACREEDORAS " + entry.Currency.FullName;
      }

      string hash = string.Empty;
      if ((_query.TrialBalanceType == TrialBalanceType.Balanza ||
           _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) &&
          ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
             _query.ShowCascadeBalances)) {

        hash = $"{entry.Ledger.Id}||{entry.Currency.Id}||{entry.GroupName}";

      } else {
        hash = $"{entry.GroupName}||{entry.Currency.Id}";
      }
      GenerateOrIncreaseEntries(summaryEntries, entry, targetAccount, targetSector, itemType, hash);
    }


    private void SummaryByGroupEntries(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                       TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry groupEntry = balanceEntry.CreatePartialCopy();

      groupEntry.GroupName = $"TOTAL GRUPO {balanceEntry.Account.GroupNumber}";
      groupEntry.GroupNumber = balanceEntry.Account.GroupNumber;
      groupEntry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;
      groupEntry.Account = StandardAccount.Empty;
      groupEntry.Sector = Sector.Empty;

      string hash;

      if (balanceEntry.DebtorCreditor == DebtorCreditorType.Deudora) {

        if (_query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta &&
            ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
             _query.ShowCascadeBalances)) {
          hash = $"{balanceEntry.Ledger.Id}||{groupEntry.DebtorCreditor}||{groupEntry.Currency.Id}||{groupEntry.GroupNumber}";
        } else {
          hash = $"{groupEntry.DebtorCreditor}||{groupEntry.Currency.Id}||{groupEntry.GroupNumber}";
        }


        GenerateOrIncreaseEntries(summaryEntries, groupEntry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalGroupDebtor, hash);

      } else if (balanceEntry.DebtorCreditor == DebtorCreditorType.Acreedora) {

        if (_query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta &&
            ((_query.WithSubledgerAccount && _query.ShowCascadeBalances) ||
             _query.ShowCascadeBalances)) {
          hash = $"{balanceEntry.Ledger.Id}||{groupEntry.DebtorCreditor}||{groupEntry.Currency.Id}||{groupEntry.GroupNumber}";
        } else {
          hash = $"{groupEntry.DebtorCreditor}||{groupEntry.Currency.Id}||{groupEntry.GroupNumber}";
        }

        GenerateOrIncreaseEntries(summaryEntries, groupEntry, StandardAccount.Empty, Sector.Empty,
                                  TrialBalanceItemType.BalanceTotalGroupCreditor, hash);
      }
    }


    internal void SummaryEntryBySectorization(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                             TrialBalanceEntry entry, StandardAccount currentParent) {
      if (!_query.UseNewSectorizationModel || !_query.WithSectorization) {
        return;
      }

      if (!currentParent.HasParent || !entry.HasSector) {
        return;
      }

      SummaryByEntry(summaryEntries, entry, currentParent, entry.Sector.Parent,
                     TrialBalanceItemType.Summary);
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

    #endregion Private methods

  }  // class TrialBalanceHelper

}  // namespace Empiria.FinancialAccounting.BalanceEngine
