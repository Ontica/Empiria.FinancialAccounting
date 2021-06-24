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

      FixedList<TrialBalanceEntry> postingEntries = helper.GetTrialBalanceEntries();

      postingEntries = helper.ValuateToExchangeRate(postingEntries);

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(postingEntries);

      List<TrialBalanceEntry> trialBalance = helper.CombineSummaryAndPostingEntries(summaryEntries,
                                                                                    postingEntries);

      trialBalance = helper.RestrictLevels(trialBalance);

      FixedList<ITrialBalanceEntry> twoColumnsBalance = MergeAccountsIntoTwoColumnsByCurrency(trialBalance);

      return new TrialBalance(_command, twoColumnsBalance);
    }


    #region Helper methods


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

  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.BalanceEngine
