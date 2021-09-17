/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : AnaliticoDeCuentas                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte Analítico de Cuentas.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte Analítico de Cuentas.</summary>
  internal class AnaliticoDeCuentas {

    private readonly TrialBalanceCommand _command;

    public AnaliticoDeCuentas(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);
      
      var twoColumnsHelper = new TwoCurrenciesBalanceHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries(_command.InitialPeriod);

      postingEntries = helper.ValuateToExchangeRate(postingEntries, _command.InitialPeriod);

      postingEntries = helper.RoundTrialBalanceEntries(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      trialBalance = RemoveCertainAccounts(trialBalance);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<TwoCurrenciesBalanceEntry> twoColumnsEntries =
                                            twoColumnsHelper.MergeSummaryEntriesIntoTwoColumns(trialBalance);

      twoColumnsEntries = twoColumnsHelper.CombineSubledgerAccountsWithSummaryEntries(
                                            twoColumnsEntries, trialBalance);

      FixedList<TwoCurrenciesBalanceEntry> summaryGroupEntries =
                                            twoColumnsHelper.GetTotalSummaryGroup(twoColumnsEntries);

      twoColumnsEntries = twoColumnsHelper.CombineGroupEntriesAndTwoColumnsEntries(
                                            twoColumnsEntries, summaryGroupEntries);

      List<TwoCurrenciesBalanceEntry> summaryTotalDeptorCreditorEntries =
                                      twoColumnsHelper.GetTotalDeptorCreditorTwoColumnsEntries(
                                        twoColumnsEntries);
      twoColumnsEntries = twoColumnsHelper.CombineTotalDeptorCreditorAndTwoColumnsEntries(
                                            twoColumnsEntries.ToList(), summaryTotalDeptorCreditorEntries);

      List<TwoCurrenciesBalanceEntry> summaryTwoColumnsBalanceTotal =
                                    twoColumnsHelper.GenerateTotalSummary(summaryTotalDeptorCreditorEntries);
      twoColumnsEntries = twoColumnsHelper.CombineTotalConsolidatedAndPostingEntries(
                                            twoColumnsEntries, summaryTwoColumnsBalanceTotal);

      twoColumnsEntries = twoColumnsHelper.GenerateAverageTwoColumnsBalance(
                                            twoColumnsEntries, _command.InitialPeriod);

      FixedList<ITrialBalanceEntry> twoColumnsBalance = twoColumnsEntries.Select(x => (ITrialBalanceEntry) x)
                                  .ToList().ToFixedList();

      return new TrialBalance(_command, twoColumnsBalance);
    }


    internal TrialBalance BuildAnaliticalBySubsidiaryAccount() {
      var helper = new TrialBalanceHelper(_command);
      var saldosHelper = new SaldosPorAuxiliar(_command);
      var twoColumnsHelper = new TwoCurrenciesBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      EmpiriaHashTable<TrialBalanceEntry> summaryEntries = 
                                          saldosHelper.BalancesBySubsidiaryAccounts(trialBalance);

      List<TrialBalanceEntry> assignSubledgerNumber = AssignSubledgerAccountNumber(summaryEntries);

      trialBalance = helper.RestrictLevels(assignSubledgerNumber);

      FixedList<TwoCurrenciesBalanceEntry> mergeCurrenciesByAccount =
                                                  twoColumnsHelper.MergeSummaryEntriesIntoTwoColumns(trialBalance);

      List<TwoCurrenciesBalanceEntry> orderingEntries = OrderingBySubledgerAccountNumber(
                                                        mergeCurrenciesByAccount);

      var returnBalance = new FixedList<ITrialBalanceEntry>(orderingEntries
                                                            .Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

    private List<TwoCurrenciesBalanceEntry> OrderingBySubledgerAccountNumber(FixedList<TwoCurrenciesBalanceEntry> subledgerAccounts) {
      var returnedEntries = new List<TwoCurrenciesBalanceEntry>(subledgerAccounts);

      return returnedEntries.OrderBy(a => a.SubledgerNumberOfDigits)
                                    .ThenBy(a => a.SubledgerAccountNumber)
                                    .ToList();
    }

    private List<TrialBalanceEntry> AssignSubledgerAccountNumber(EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {
      var returnedEntries = new List<TrialBalanceEntry>();

      foreach (var entry in summaryEntries.ToFixedList()) {
        SubsidiaryAccount subsidiary = SubsidiaryAccount.Parse(entry.SubledgerAccountIdParent);
        if (subsidiary != null) {
          entry.SubledgerAccountNumber = subsidiary.Number;
          entry.GroupName = subsidiary.Name;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Count();
          entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        }
        returnedEntries.Add(entry);
      }
      
      return returnedEntries;
    }

    private List<TrialBalanceEntry> RemoveCertainAccounts(List<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedSummaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in summaryEntries) {
        if (MustRemoveAccount(entry.Account)) {
          continue;
        }
        returnedSummaryEntries.Add(entry);
      }
      return returnedSummaryEntries;
    }


    private bool MustRemoveAccount(StandardAccount account) {
      if (account.Number.EndsWith("-00")) {
        return true;
      }
      if (account.Number.StartsWith("1503")) {
        return true;
      }
      if (account.Number.StartsWith("50")) {
        return true;
      }
      if (account.Number.StartsWith("90")) {
        return true;
      }
      if (account.Number.StartsWith("91")) {
        return true;
      }
      if (account.Number.StartsWith("92")) {
        return true;
      }
      if (account.Number.StartsWith("93")) {
        return true;
      }
      if (account.Number.StartsWith("94")) {
        return true;
      }
      if (account.Number.StartsWith("95")) {
        return true;
      }
      if (account.Number.StartsWith("96")) {
        return true;
      }
      if (account.Number.StartsWith("97")) {
        return true;
      }
      return false;
    }

  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.BalanceEngine
