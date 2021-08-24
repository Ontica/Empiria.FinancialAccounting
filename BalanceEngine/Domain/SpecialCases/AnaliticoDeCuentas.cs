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
      summaryEntries = twoColumnsHelper.CleanSummaryAccounts(summaryEntries);
      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<TwoCurrenciesBalanceEntry> twoColumnsEntries =
                                            twoColumnsHelper.MergeAccountsIntoTwoColumns(trialBalance);

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


  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.BalanceEngine
