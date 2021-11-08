/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorAuxiliar                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar.</summary>
  internal class SaldosPorAuxiliar {

    private readonly TrialBalanceCommand _command;

    public SaldosPorAuxiliar(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      EmpiriaHashTable<TrialBalanceEntry> summaryEntries = BalancesBySubledgerAccounts(trialBalance);

      List<TrialBalanceEntry> orderingtTialBalance = OrderByAccountNumber(summaryEntries);

      trialBalance = CombineTotalAndSummaryEntries(orderingtTialBalance, trialBalance);

      trialBalance = helper.GenerateAverageBalance(trialBalance);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_command);

      _command.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Helper methods


    internal EmpiriaHashTable<TrialBalanceEntry> BalancesBySubledgerAccounts(
                                                List<TrialBalanceEntry> trialBalance) {

      var subledgerAccountsEntries = trialBalance.Where(a => a.SubledgerAccountId > 0).ToList();

      var subledgerAccountsEntriesHashTable = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var item in subledgerAccountsEntries) {
        string hash = $"{item.Ledger.Number}||{item.Currency.Code}||" +
                      $"{item.Account.Number}||{item.Sector.Code}||" +
                      $"{item.SubledgerAccountId}";

        subledgerAccountsEntriesHashTable.Insert(hash, item);
      }

      return GenerateEntries(subledgerAccountsEntriesHashTable);
    }


    private List<TrialBalanceEntry> CombineTotalAndSummaryEntries(
                                    List<TrialBalanceEntry> orderingtTialBalance,
                                    List<TrialBalanceEntry> trialBalance) {

      var returnedOrdering = new List<TrialBalanceEntry>();

      foreach (var entry in orderingtTialBalance) {
        var summaryAccounts = trialBalance.Where(
                      a => a.SubledgerAccountId == entry.SubledgerAccountIdParent &&
                      a.Ledger.Number == entry.Ledger.Number &&
                      a.Currency.Code == entry.Currency.Code &&
                      a.ItemType == TrialBalanceItemType.BalanceEntry).ToList();

        foreach (var summary in summaryAccounts) {
          entry.LastChangeDate = summary.LastChangeDate > entry.LastChangeDate ?
                                 summary.LastChangeDate : entry.LastChangeDate;
          summary.SubledgerAccountId = 0;
        }

        returnedOrdering.Add(entry);

        if (summaryAccounts.Count > 0) {
          returnedOrdering.AddRange(summaryAccounts);
        }
      }

      return returnedOrdering;
    }


    private EmpiriaHashTable<TrialBalanceEntry> GenerateEntries(
                        EmpiriaHashTable<TrialBalanceEntry> subledgerAccountEntriesHashTable) {

      var helper = new TrialBalanceHelper(_command);

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in subledgerAccountEntriesHashTable.ToFixedList()) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        helper.SummaryBySubledgerEntry(hashReturnedEntries, entry, TrialBalanceItemType.BalanceSummary);
      }

      return hashReturnedEntries;
    }


    internal List<TrialBalanceEntry> OrderByAccountNumber(
                                    EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {

      var returnedCombineOrdering = new List<TrialBalanceEntry>();

      foreach (var entry in summaryEntries.ToFixedList()) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (subledgerAccount != null) {
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          entry.GroupName = subledgerAccount.Name;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Count();
          entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        }
        returnedCombineOrdering.Add(entry);
      }
      return returnedCombineOrdering.Where(a => !a.SubledgerAccountNumber.Contains("undefined"))
                                    .OrderBy(a => a.Currency.Code)
                                    .ThenBy(a => a.SubledgerNumberOfDigits)
                                    .ThenBy(a => a.SubledgerAccountNumber)
                                    .ToList();
    }


    #endregion Helper methods

  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine
