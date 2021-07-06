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

      FixedList<ITrialBalanceEntry> twoColumnsBalance = MergeAccountsIntoTwoColumns(trialBalance);

      return new TrialBalance(_command, twoColumnsBalance);
    }


    #region Helper methods

    internal void GenerateOrIncreaseTwoCurrenciesBalanceEntry(
                          EmpiriaHashTable<TwoCurrenciesBalanceEntry> summaryEntries,
                          TrialBalanceEntry entry, string hash, Currency targetCurrency,
                          Currency currentCurrency) {

      TwoCurrenciesBalanceEntry summaryEntry;
      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {
        summaryEntries.Insert(hash, entry.MapToTwoColumnsBalanceEntry());
        SumTwoCurrenciesBalanceEntry(summaryEntries[hash], entry, targetCurrency, currentCurrency);
      } else {
        SumTwoCurrenciesBalanceEntry(summaryEntry, entry, targetCurrency, currentCurrency);
      }
    }


    private FixedList<ITrialBalanceEntry> MergeAccountsIntoTwoColumns(List<TrialBalanceEntry> trialBalance) {
      var targetCurrency = Currency.Parse(_command.ValuateToCurrrencyUID);
      var summaryEntries = new EmpiriaHashTable<TwoCurrenciesBalanceEntry>();

      foreach (var entry in trialBalance) {
        string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||{entry.Ledger.Id}";
        Currency currentCurrency = entry.Currency;
        
        if (entry.Currency.Equals(targetCurrency)) {
          GenerateOrIncreaseTwoCurrenciesBalanceEntry(summaryEntries, entry, hash, 
                                                      targetCurrency, currentCurrency);
        } else if (summaryEntries.ContainsKey(hash)) {
          SumTwoCurrenciesBalanceEntry(summaryEntries[hash], entry, targetCurrency, currentCurrency);
        } else {
          entry.Currency = targetCurrency;
          GenerateOrIncreaseTwoCurrenciesBalanceEntry(summaryEntries, entry, hash,
                                                      targetCurrency, currentCurrency);
        }
      }

      return summaryEntries.Values.Select(x => (ITrialBalanceEntry) x)
                                  .ToList().ToFixedList();
    }


    internal void SumTwoCurrenciesBalanceEntry(TwoCurrenciesBalanceEntry twoCurrenciesEntry, 
                                               TrialBalanceEntry entry,
                                               Currency targetCurrency, Currency currentCurrency) {
      
      if (currentCurrency != targetCurrency &&
                  entry.Currency.Code != "44") {

        twoCurrenciesEntry.ForeignBalance += entry.CurrentBalance;
      } else {
        twoCurrenciesEntry.DomesticBalance += entry.CurrentBalance;
      }
    }

    #endregion Helper methods

  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.BalanceEngine
