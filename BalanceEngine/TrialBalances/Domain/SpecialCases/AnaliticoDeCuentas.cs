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

      var analyticHelper = new AnalyticHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries(_command.InitialPeriod);

      postingEntries = helper.GetSummaryToParentEntries(postingEntries);

      postingEntries = helper.ValuateToExchangeRate(postingEntries, _command.InitialPeriod);

      postingEntries = helper.RoundTrialBalanceEntries(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> _postingEntries = helper.GetSummaryEntriesAndSectorization(
                                                postingEntries.ToList());

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              helper.GetSummaryEntriesAndSectorization(summaryEntries);

      analyticHelper.GetSummaryToSectorZeroForPesosAndUdis(_postingEntries, summaryEntriesAndSectorization);

      List<TrialBalanceEntry> trialBalance = analyticHelper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, _postingEntries.ToFixedList());

      trialBalance = RemoveCertainAccounts(trialBalance);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<AnalyticBalanceEntry> analyticEntries =
                                           analyticHelper.MergeTrialBalanceIntoAnalyticColumns(trialBalance);

      analyticEntries = analyticHelper.CombineSubledgerAccountsWithSummaryEntries(
                                            analyticEntries, trialBalance);

      FixedList<AnalyticBalanceEntry> summaryGroupEntries =
                                            analyticHelper.GetTotalSummaryByGroup(analyticEntries);

      analyticEntries = analyticHelper.CombineSummaryGroupsAndEntries(
                                            analyticEntries, summaryGroupEntries);

      List<AnalyticBalanceEntry> summaryTotalDeptorCreditorEntries =
                                      analyticHelper.GetTotalDeptorCreditorEntries(
                                        analyticEntries);
      analyticEntries = analyticHelper.CombineTotalDeptorCreditorAndEntries(
                                            analyticEntries.ToList(), summaryTotalDeptorCreditorEntries);

      List<AnalyticBalanceEntry> summaryTotalReport =
                                    analyticHelper.GenerateTotalReport(summaryTotalDeptorCreditorEntries);

      analyticEntries = analyticHelper.CombineTotalConsolidatedAndEntries(
                                            analyticEntries, summaryTotalReport);

      //analyticEntries = analyticHelper.GenerateAverageBalance(
      //                                      analyticEntries, _command.InitialPeriod);

      FixedList<ITrialBalanceEntry> analyticBalance = analyticEntries.Select(x => (ITrialBalanceEntry) x)
                                  .ToList().ToFixedList();

      //var ensureIsValid = new EnsureBalanceValidations(_command);
      //ensureIsValid.EnsureIsValid(twoColumnsBalance, postingEntries);

      return new TrialBalance(_command, analyticBalance);
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
