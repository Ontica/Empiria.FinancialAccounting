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
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte Analítico de Cuentas.</summary>
  internal class AnaliticoDeCuentasBuilder {

    private readonly TrialBalanceQuery _query;

    internal AnaliticoDeCuentasBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal FixedList<AnaliticoDeCuentasEntry> Build() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      return Build(baseAccountEntries);
    }


    internal FixedList<AnaliticoDeCuentasEntry> Build(FixedList<TrialBalanceEntry> baseAccountEntries) {

      if (baseAccountEntries.Count == 0) {
        return new FixedList<AnaliticoDeCuentasEntry>();
      }

      FixedList<TrialBalanceEntry> saldosValorizados = SaldosDeCuentasValorizados(baseAccountEntries);

      var balanceHelper = new TrialBalanceHelper(_query);
      var analiticoHelper = new AnaliticoDeCuentasHelper(_query);

      List<TrialBalanceEntry> parentAccounts = analiticoHelper.GetCalculatedParentAccounts(saldosValorizados);

      List<TrialBalanceEntry> parentAndAccountEntries = analiticoHelper.CombineSummaryAndPostingEntries(
                                             parentAccounts, saldosValorizados.ToFixedList());

      List<TrialBalanceEntry> balanceEntries = balanceHelper.GetSummaryAccountsAndSectorization(
                                               parentAndAccountEntries.ToList());

      analiticoHelper.GetSummaryToSectorZeroForPesosAndUdis(balanceEntries);

      balanceEntries = RemoveUnneededAccounts(balanceEntries);

      balanceHelper.RestrictLevels(balanceEntries);

      List<AnaliticoDeCuentasEntry> analyticEntries =
                                        analiticoHelper.MergeTrialBalanceIntoAnalyticColumns(balanceEntries);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndSubledgerAccounts = 
        analiticoHelper.CombineSubledgerAccountsWithAnalyticEntries(analyticEntries, balanceEntries);

      List<AnaliticoDeCuentasEntry> totalsByGroup =
                                    analiticoHelper.GetTotalByGroup(analyticEntriesAndSubledgerAccounts);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndTotalsByGroup = 
        analiticoHelper.CombineTotalsByGroupAndAccountEntries(
                        analyticEntriesAndSubledgerAccounts, totalsByGroup);

      List<AnaliticoDeCuentasEntry> totalByDebtorsCreditors =
        analiticoHelper.GetTotalsByDebtorOrCreditorEntries(analyticEntriesAndSubledgerAccounts);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndTotalDebtorCreditor = 
        analiticoHelper.CombineTotalDebtorCreditorAndEntries(
                        analyticEntriesAndTotalsByGroup, totalByDebtorsCreditors);

      List<AnaliticoDeCuentasEntry> totalReport =
                                    analiticoHelper.GenerateTotalReport(totalByDebtorsCreditors);

      List<AnaliticoDeCuentasEntry> analyticReport = analiticoHelper.CombineTotalReportAndEntries(
                                            analyticEntriesAndTotalDebtorCreditor, totalReport);

      return analyticReport.ToFixedList();
    }


    private FixedList<TrialBalanceEntry> SaldosDeCuentasValorizados(FixedList<TrialBalanceEntry> baseAccountEntries) {
      var balanceHelper = new TrialBalanceHelper(_query);

      balanceHelper.SetSummaryToParentEntries(baseAccountEntries);

      balanceHelper.ValuateAccountEntriesToExchangeRate(baseAccountEntries);

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
