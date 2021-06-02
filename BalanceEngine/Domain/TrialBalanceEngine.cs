/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceEngine                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to retrieve a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum TrialBalanceType {

    Traditional,

    Valued

  }


  public enum BalancesType {

    AllAccounts,

    WithCurrentBalance,

    WithCurrenBalanceOrMovements,

    WithMovements

  }


  /// <summary>Provides services to retrieve a trial balance.</summary>
  internal class TrialBalanceEngine {


    internal TrialBalanceEngine(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      this.Command = command;
    }


    public TrialBalanceCommand Command {
      get;
    }


    internal TrialBalance BuildTrialBalance() {
      TrialBalanceCommandData commandData = this.Command.MapToTrialBalanceCommandData();

      FixedList<TrialBalanceEntry> postingEntries = TrialBalanceDataService.GetTrialBalanceEntries(commandData);

      List<TrialBalanceEntry> summaryEntries = GenerateSummaryEntries(postingEntries);

      FixedList<TrialBalanceEntry> trialBalance = CombineSummaryAndPostingEntries(summaryEntries, postingEntries);

      trialBalance = RestrictLevels(trialBalance);

      return new TrialBalance(Command, trialBalance);
    }


    #region Private methods

    private FixedList<TrialBalanceEntry> CombineSummaryAndPostingEntries(List<TrialBalanceEntry> summaryEntries,
                                                                     FixedList<TrialBalanceEntry> postingEntries) {
      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>(postingEntries);

      foreach (var item in summaryEntries) {
        returnedEntries.Add(item);
      }

      returnedEntries = returnedEntries.OrderBy(a => a.Ledger.Number)
                                       .ThenBy(a => a.Currency.Code)
                                       .ThenBy(a => a.Account.Number)
                                       .ThenBy(a => a.Sector.Code)
                                       .ToList();

      return returnedEntries.ToFixedList();
    }


    private FixedList<TrialBalanceEntry> RestrictLevels(FixedList<TrialBalanceEntry> entries) {
      if (Command.Level > 0) {
        return entries.FindAll(x => x.Level <= Command.Level);
      } else {
        return entries;
      }
    }


    private List<TrialBalanceEntry> GenerateSummaryEntries(FixedList<TrialBalanceEntry> entries) {
      List<TrialBalanceEntry> summaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in entries) {
        if (!entry.Account.HasParent) {
          continue;
        }

        Account currentParent = entry.Account.GetParent();

        while (true) {

          var parentSummaryEntry = summaryEntries.Where(a => a.Ledger.Equals(entry.Ledger) &&
                                                             a.Account.Number == currentParent.Number &&
                                                             a.Sector.Code == entry.Sector.Code &&
                                                             a.Currency.Id == entry.Currency.Id).FirstOrDefault();

          if (parentSummaryEntry != null) {

            parentSummaryEntry.InitialBalance += entry.InitialBalance;
            parentSummaryEntry.Debit += entry.Debit;
            parentSummaryEntry.Credit += entry.Credit;
            parentSummaryEntry.CurrentBalance += entry.CurrentBalance;

          } else {

            parentSummaryEntry = new TrialBalanceEntry() {
              Ledger = entry.Ledger,
              Currency = entry.Currency,
              Sector = entry.Sector,
              Account = currentParent,
              InitialBalance = entry.InitialBalance,
              Debit = entry.Debit,
              Credit = entry.Credit,
              CurrentBalance = entry.CurrentBalance,
              ItemType = "BalanceSummary"
            };

            summaryEntries.Add(parentSummaryEntry);

          }

          if (!currentParent.HasParent) {
            break;
          } else {
            currentParent = currentParent.GetParent();
          }

        } // while

      } // foreach

      return summaryEntries;
    }


    #endregion Private methods

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
