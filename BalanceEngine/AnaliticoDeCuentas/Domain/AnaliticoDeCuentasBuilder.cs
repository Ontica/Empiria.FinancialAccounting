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

      List<TrialBalanceEntry> balanceEntries = SummariesAndAccountEntries(saldosValorizados);

      var helper = new AnaliticoDeCuentasHelper(_query);
      var utility = new AnaliticoDeCuentasUtility(_query);

      List<AnaliticoDeCuentasEntry> analyticEntries =
                                        helper.MergeTrialBalanceIntoAnalyticColumns(balanceEntries);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndSubledgerAccounts = 
        helper.MergeSubledgerAccountsWithAnalyticEntries(analyticEntries, balanceEntries);

      List<AnaliticoDeCuentasEntry> totalsByGroup =
                                    helper.GetTotalByGroup(analyticEntriesAndSubledgerAccounts);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndTotalsByGroup = 
        utility.CombineTotalsByGroupAndAccountEntries(
                        analyticEntriesAndSubledgerAccounts, totalsByGroup);

      List<AnaliticoDeCuentasEntry> totalByDebtorsCreditors =
        helper.GetTotalsByDebtorOrCreditorEntries(analyticEntriesAndSubledgerAccounts);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndTotalDebtorCreditor =
        utility.CombineTotalDebtorCreditorAndEntries(
                        analyticEntriesAndTotalsByGroup, totalByDebtorsCreditors);

      List<AnaliticoDeCuentasEntry> totalReport =
                                    helper.GenerateTotalReport(totalByDebtorsCreditors);

      List<AnaliticoDeCuentasEntry> analyticReport = utility.CombineTotalReportAndEntries(
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


    private List<TrialBalanceEntry> SummariesAndAccountEntries(FixedList<TrialBalanceEntry> saldosValorizados) {

      var helper = new AnaliticoDeCuentasHelper(_query);

      List<TrialBalanceEntry> parentAccounts = helper.GetCalculatedParentAccounts(saldosValorizados);

      var utility = new AnaliticoDeCuentasUtility(_query);

      List<TrialBalanceEntry> parentAndAccountEntries = utility.CombineSummaryAndPostingEntries(
                                             parentAccounts, saldosValorizados.ToFixedList());

      var balanceHelper = new TrialBalanceHelper(_query);

      List<TrialBalanceEntry> balanceEntries = balanceHelper.GetSummaryAccountsAndSectorization(
                                               parentAndAccountEntries.ToList());

      helper.GetSummaryToSectorZeroForPesosAndUdis(balanceEntries);

      balanceEntries = utility.RemoveUnneededAccounts(balanceEntries);

      balanceHelper.RestrictLevels(balanceEntries);

      return balanceEntries;
    }


  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.AnaliticoDeCuentasBuilder
