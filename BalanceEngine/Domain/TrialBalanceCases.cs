/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceCases                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  internal class TrialBalanceCases {

    private readonly TrialBalanceCommand _command;

    public TrialBalanceCases(TrialBalanceCommand command) {
      _command = command;
    }

    internal TrialBalance BuildAnaliticoDeCuentas() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      postingEntries = helper.ValuateToExchangeRate(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> twoColumnsBalance = MergeAccountsIntoTwoColumnsByCurrency(trialBalance);

      return new TrialBalance(_command, twoColumnsBalance);
    }


    internal TrialBalance BuildSaldos() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      FixedList<ITrialBalanceEntry> returnBalance = postingEntries.Select(x => (ITrialBalanceEntry) x)
                                                                  .ToList().ToFixedList();

      return new TrialBalance(_command, returnBalance);
    }


    internal List<TrialBalanceEntry> BuildSaldosPorAuxiliar() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = GetSummaryAndPostingEntries();

      List<TrialBalanceEntry> summarySubsidiaryEntries = BalancesBySubsidiaryAccounts(trialBalance);

      trialBalance = CombineTotalSubsidiaryEntriesWithSummaryAccounts(summarySubsidiaryEntries);

      return helper.RestrictLevels(trialBalance);
    }


    internal List<TrialBalanceEntry> BuildSaldosPorCuentaConDelegaciones() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = GetSummaryAndPostingEntries();

      List<TrialBalanceEntry> summaryByAccountAndDelegations = GenerateTotalByAccountAndDelegations(trialBalance);

      trialBalance = CombineAccountsAndLedgers(summaryByAccountAndDelegations);

      return helper.RestrictLevels(trialBalance);
    }


    internal List<TrialBalanceEntry> BuildBalanzaTradicional() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries, postingEntries);

      FixedList<TrialBalanceEntry> summaryGroupEntries = helper.GenerateTotalSummaryGroup(postingEntries);

      trialBalance = helper.CombineGroupEntriesAndPostingEntries(trialBalance, summaryGroupEntries);

      List<TrialBalanceEntry> summaryTotalDebtorCreditorEntries =
                              helper.GenerateTotalSummaryDebtorCreditor(postingEntries.ToList());

      trialBalance = helper.CombineDebtorCreditorAndPostingEntries(
                            trialBalance, summaryTotalDebtorCreditorEntries);

      List<TrialBalanceEntry> summaryTotalCurrencies = helper.GenerateTotalSummaryCurrency(
                                                         summaryTotalDebtorCreditorEntries);

      trialBalance = helper.CombineCurrencyTotalsAndPostingEntries(trialBalance, summaryTotalCurrencies);

      List<TrialBalanceEntry> summaryTrialBalanceConsolidated = helper.GenerateTotalSummaryConsolidated(
                                                                       summaryTotalCurrencies);

      trialBalance = helper.CombineTotalConsolidatedAndPostingEntries(trialBalance, summaryTrialBalanceConsolidated);

      return helper.RestrictLevels(trialBalance);
    }


    #region Helper methods


    internal List<TrialBalanceEntry> BalancesBySubsidiaryAccounts(List<TrialBalanceEntry> trialBalance) {
      List<TrialBalanceEntry> returnedSubsidiaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in trialBalance) {
        if (entry.SubledgerAccountId > 0) {

          returnedSubsidiaryEntries.Add(entry);
        }
      }

      returnedSubsidiaryEntries = returnedSubsidiaryEntries.OrderBy(a => a.Ledger.Number)
                                                           .ThenBy(a => a.Currency.Code)
                                                           .ThenBy(a => a.Account.Number)
                                                           .ThenBy(a => a.Sector.Code)
                                                           .ToList();

      returnedSubsidiaryEntries = CombineSubsidiaryEntriesAndSummaryAccounts(returnedSubsidiaryEntries);

      return returnedSubsidiaryEntries;
    }



    internal List<TrialBalanceEntry> CombineSubsidiaryEntriesAndSummaryAccounts(
                                    List<TrialBalanceEntry> subsidiaryEntries) {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      foreach (var returned in subsidiaryEntries) {

        List<TrialBalanceEntry> summaryEntries = new List<TrialBalanceEntry>();
        var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

        StandardAccount currentParent;
        currentParent = returned.Account;

        while (true) {
          var existParent = returnedEntries.FirstOrDefault(a => a.Ledger.Number == returned.Ledger.Number &&
                            a.Currency.Code == returned.Currency.Code && a.SubledgerAccountId == 0 &&
                            a.SubledgerAccountIdParent == returned.SubledgerAccountId &&
                            a.Account.Number == currentParent.Number && a.Sector.Code == returned.Sector.Code);

          if (existParent == null) {
            helper.SummaryByEntry(summaryParentEntries, returned, currentParent, returned.Sector,
                                  TrialBalanceItemType.BalanceSummary);

            if (!currentParent.HasParent && returned.HasSector && returned.SubledgerAccountId > 0) {

              var entryWithoutSector = returnedEntries.FirstOrDefault(
                                               a => a.Ledger.Number == returned.Ledger.Number &&
                                               a.Currency.Code == returned.Currency.Code &&
                                               a.SubledgerAccountIdParent == returned.SubledgerAccountId &&
                                               a.Account.Number == currentParent.Number &&
                                               a.NotHasSector);

              if (entryWithoutSector == null) {
                helper.SummaryByEntry(summaryParentEntries, returned, currentParent, Sector.Empty,
                                     TrialBalanceItemType.BalanceSummary);
              } else {
                entryWithoutSector.InitialBalance += returned.InitialBalance;
                entryWithoutSector.Debit += returned.Debit;
                entryWithoutSector.Credit += returned.Credit;
                entryWithoutSector.CurrentBalance += returned.CurrentBalance;
              }
              break;
            } else if (!currentParent.HasParent) {
              break;
            } else {
              currentParent = currentParent.GetParent();
            }
          } else {
            existParent.InitialBalance += returned.InitialBalance;
            existParent.Debit += returned.Debit;
            existParent.Credit += returned.Credit;
            existParent.CurrentBalance += returned.CurrentBalance;

            if (!currentParent.HasParent) {
              var existTotalBySubledger = returnedEntries.FirstOrDefault(
                                          a => a.SubledgerAccountIdParent == returned.SubledgerAccountId &&
                                          a.Currency.Code == returned.Currency.Code &&
                                          a.Account.Number == currentParent.Number &&
                                          a.NotHasSector);

              if (existTotalBySubledger != null) {
                existTotalBySubledger.InitialBalance += returned.InitialBalance;
                existTotalBySubledger.Debit += returned.Debit;
                existTotalBySubledger.Credit += returned.Credit;
                existTotalBySubledger.CurrentBalance += returned.CurrentBalance;
              }
              break;
            } else {
              currentParent = currentParent.GetParent();
            }
          }

        } // while

        summaryEntries.AddRange(summaryParentEntries.Values.ToList());
        returnedEntries.AddRange(summaryEntries);

      } // foreach

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ThenByDescending(a => a.SubledgerAccountIdParent)
                            .ThenBy(a => a.Account.Number).ThenBy(a => a.SubledgerAccountId)
                            .ThenBy(a => a.Sector.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineAccountsAndLedgers(List<TrialBalanceEntry> summaryLedgersList) {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();
      var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in summaryLedgersList) {

        var existAccount = returnedEntries.FirstOrDefault(a => a.Ledger == Ledger.Empty &&
                                                          a.Currency.Code == entry.Currency.Code &&
                                                          a.Account.Number == entry.Account.Number &&
                                                          a.NotHasSector &&
                                                          a.GroupName == "TOTAL DE LA CUENTA"
                                                         );

        if (existAccount == null) {
          helper.SummaryByAccount(summaryParentEntries, entry, entry.Account, Sector.Empty,
                                  TrialBalanceItemType.BalanceSummary);
        }
      }
      returnedEntries.AddRange(summaryParentEntries.Values.ToList());
      returnedEntries.AddRange(summaryLedgersList);
      returnedEntries = returnedEntries.OrderBy(a => a.Currency.Code)
                                       .ThenBy(a => a.Account.Number)
                                       .ToList();
      return returnedEntries;
    }

    internal List<TrialBalanceEntry> CombineTotalSubsidiaryEntriesWithSummaryAccounts(
                                            List<TrialBalanceEntry> summaryEntries) {

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();

      var totaBySubsidiaryAccountList = summaryEntries.Where(a => a.Level == 1 && a.NotHasSector).ToList();

      foreach (var entry in totaBySubsidiaryAccountList.OrderBy(a => a.Currency.Code)) {

        entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        var summaryAccounts = summaryEntries.Where(
                               a => a.SubledgerAccountIdParent == entry.SubledgerAccountIdParent &&
                                    a.Ledger.Number == entry.Ledger.Number &&
                                    a.Currency.Code == entry.Currency.Code).ToList();

        if (summaryAccounts.Count > 0) {
          returnedEntries.AddRange(summaryAccounts);
        }
      }

      return returnedEntries;
    }

    private FixedList<TrialBalanceEntry> GetPostingEntries() {
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


    private List<TrialBalanceEntry> GetSummaryAndPostingEntries() {
      var helper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = GetPostingEntries();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries, postingEntries);

      return helper.RestrictLevels(trialBalance);
    }

    private List<TrialBalanceEntry> GenerateTotalByAccountAndDelegations(
                                   List<TrialBalanceEntry> trialBalance) {

      List<TrialBalanceEntry> summaryLedgersList = new List<TrialBalanceEntry>();
      List<TrialBalanceEntry> ledgersGroupList = trialBalance.Where(
                                                  a => a.HasSector && a.Level == 1).ToList();

      foreach (var ledgerGroup in ledgersGroupList) {

        var existLedger = summaryLedgersList.FirstOrDefault(a => a.Ledger.Number == ledgerGroup.Ledger.Number &&
                                                                 a.Currency.Code == ledgerGroup.Currency.Code &&
                                                                 a.Account.Number == ledgerGroup.Account.Number);

        var ledgersById = ledgersGroupList.Where(a => a.Ledger.Number == ledgerGroup.Ledger.Number &&
                                                 a.Currency.Code == ledgerGroup.Currency.Code &&
                                                 a.Account.Number == ledgerGroup.Account.Number).ToList();

        if (existLedger == null) {
          ledgerGroup.GroupName = ledgerGroup.Ledger.Name;
          ledgerGroup.Sector = Sector.Empty;
          ledgerGroup.ItemType = TrialBalanceItemType.BalanceEntry;
          summaryLedgersList.Add(ledgerGroup);
        } else {
          foreach (var ledger in ledgersById) {
            existLedger.Sum(ledger);
          }
        }

      } // foreach

      return summaryLedgersList;
    }


    private FixedList<ITrialBalanceEntry> MergeAccountsIntoTwoColumnsByCurrency(List<TrialBalanceEntry> trialBalance) {
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


    #endregion Helper methods

  }  // class TrialBalanceCases

}  // namespace Empiria.FinancialAccounting.BalanceEngine
