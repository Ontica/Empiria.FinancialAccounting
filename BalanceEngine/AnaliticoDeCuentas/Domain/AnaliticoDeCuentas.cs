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
      var balanceHelper = new TrialBalanceHelper(_command);

      var helper = new AnaliticoDeCuentasHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = balanceHelper.GetTrialBalanceEntries();

      postingEntries = balanceHelper.GetSummaryToParentEntries(postingEntries);

      postingEntries = balanceHelper.ValuateToExchangeRate(postingEntries, _command.InitialPeriod);

      postingEntries = balanceHelper.RoundTrialBalanceEntries(postingEntries);

      List<TrialBalanceEntry> summaryEntries = balanceHelper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> _postingEntries = balanceHelper.GetSummaryEntriesAndSectorization(
                                                postingEntries.ToList());

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              balanceHelper.GetSummaryEntriesAndSectorization(summaryEntries);

      helper.GetSummaryToSectorZeroForPesosAndUdis(_postingEntries, summaryEntriesAndSectorization);

      List<TrialBalanceEntry> balanceEntries = helper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, _postingEntries.ToFixedList());

      balanceEntries = RemoveCertainAccounts(balanceEntries);

      balanceEntries = balanceHelper.RestrictLevels(balanceEntries);

      FixedList<AnaliticoDeCuentasEntry> analyticEntries =
                                           helper.MergeTrialBalanceIntoAnalyticColumns(balanceEntries);

      analyticEntries = helper.CombineSubledgerAccountsWithSummaryEntries(
                                            analyticEntries, balanceEntries);

      FixedList<AnaliticoDeCuentasEntry> summaryGroupEntries =
                                            helper.GetTotalSummaryByGroup(analyticEntries);

      analyticEntries = helper.CombineSummaryGroupsAndEntries(
                                            analyticEntries, summaryGroupEntries);

      List<AnaliticoDeCuentasEntry> summaryTotalDeptorCreditorEntries =
                                      helper.GetTotalDeptorCreditorEntries(
                                        analyticEntries);
      analyticEntries = helper.CombineTotalDeptorCreditorAndEntries(
                                            analyticEntries.ToList(), summaryTotalDeptorCreditorEntries);

      List<AnaliticoDeCuentasEntry> summaryTotalReport =
                                    helper.GenerateTotalReport(summaryTotalDeptorCreditorEntries);

      analyticEntries = helper.CombineTotalConsolidatedAndEntries(
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
