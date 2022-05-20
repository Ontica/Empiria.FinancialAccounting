/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : AnaliticoDeCuentasBuilder                  License   : Please read LICENSE.txt file            *
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
  internal class AnaliticoDeCuentasBuilder {

    private readonly TrialBalanceCommand _command;

    public AnaliticoDeCuentasBuilder(TrialBalanceCommand command) {
      _command = command;
    }


    internal FixedList<AnaliticoDeCuentasEntry> Build() {
      var balanceHelper = new TrialBalanceHelper(_command);

      FixedList<TrialBalanceEntry> baseAccountEntries = balanceHelper.ReadAccountEntriesFromDataService();

      return Build(baseAccountEntries);
    }


    internal FixedList<AnaliticoDeCuentasEntry> Build(FixedList<TrialBalanceEntry> baseAccountEntries) {
      FixedList<TrialBalanceEntry> balanzaValorizada = GetBalanzaTradicionalValorizada(baseAccountEntries);

      var balanceHelper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> summaryEntries = balanceHelper.GenerateSummaryEntries(balanzaValorizada);

      List<TrialBalanceEntry> postingEntries = balanceHelper.GetSummaryEntriesAndSectorization(
                                               balanzaValorizada.ToList());

      List<TrialBalanceEntry> summaryEntriesAndSectorization =
                              balanceHelper.GetSummaryEntriesAndSectorization(summaryEntries);

      var analiticoHelper = new AnaliticoDeCuentasHelper(_command);

      analiticoHelper.GetSummaryToSectorZeroForPesosAndUdis(postingEntries, summaryEntriesAndSectorization);

      List<TrialBalanceEntry> balanceEntries = analiticoHelper.CombineSummaryAndPostingEntries(
                                             summaryEntriesAndSectorization, postingEntries.ToFixedList());

      balanceEntries = RemoveUnneededAccounts(balanceEntries);

      balanceHelper.RestrictLevels(balanceEntries);

      FixedList<AnaliticoDeCuentasEntry> analyticEntries =
                                           analiticoHelper.MergeTrialBalanceIntoAnalyticColumns(balanceEntries);

      analyticEntries = analiticoHelper.CombineSubledgerAccountsWithSummaryEntries(
                                            analyticEntries, balanceEntries);

      FixedList<AnaliticoDeCuentasEntry> summaryGroupEntries =
                                            analiticoHelper.GetTotalSummaryByGroup(analyticEntries);

      analyticEntries = analiticoHelper.CombineSummaryGroupsAndEntries(
                                            analyticEntries, summaryGroupEntries);

      List<AnaliticoDeCuentasEntry> summaryTotalDebtorCreditorEntries =
                                      analiticoHelper.GetTotalDebtorCreditorEntries(
                                        analyticEntries);

      analyticEntries = analiticoHelper.CombineTotalDebtorCreditorAndEntries(
                                            analyticEntries.ToList(), summaryTotalDebtorCreditorEntries);

      List<AnaliticoDeCuentasEntry> summaryTotalReport =
                                    analiticoHelper.GenerateTotalReport(summaryTotalDebtorCreditorEntries);

      analyticEntries = analiticoHelper.CombineTotalConsolidatedAndEntries(
                                            analyticEntries, summaryTotalReport);

      return analyticEntries.ToFixedList();
    }


    private FixedList<TrialBalanceEntry> GetBalanzaTradicionalValorizada(FixedList<TrialBalanceEntry> baseAccountEntries) {
      var balanceHelper = new TrialBalanceHelper(_command);

      // balanceHelper.SummarizeParentPostingEntryAccounts(baseAccountEntries);

      baseAccountEntries = balanceHelper.GetSummaryToParentEntries(baseAccountEntries);

      // balanceHelper.ApplyExchangeRates(baseAccountEntries);

      baseAccountEntries = balanceHelper.ValuateToExchangeRate(baseAccountEntries, _command.InitialPeriod);

      // balanceHelper.RoundDecimals(baseAccountEntries);

      balanceHelper.RoundDecimals(baseAccountEntries);

      return baseAccountEntries;
    }


    private List<TrialBalanceEntry> RemoveUnneededAccounts(List<TrialBalanceEntry> summaryEntries) {
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

}  // namespace Empiria.FinancialAccounting.AnaliticoDeCuentasBuilder
