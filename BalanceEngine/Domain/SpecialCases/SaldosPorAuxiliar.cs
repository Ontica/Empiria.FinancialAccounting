﻿/* Empiria Financial *****************************************************************************************
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

      EmpiriaHashTable<TrialBalanceEntry> summaryEntries = BalancesBySubsidiaryAccounts(trialBalance);

      List<TrialBalanceEntry> orderingtTialBalance = OrderByAccountNumber(summaryEntries);

      trialBalance = CombineTotalAndSummaryEntries(orderingtTialBalance, trialBalance);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Helper methods


    private EmpiriaHashTable<TrialBalanceEntry> BalancesBySubsidiaryAccounts(
                                                List<TrialBalanceEntry> trialBalance) {

      var subsidiaryEntries = trialBalance.Where(a => a.SubledgerAccountId > 0).ToList();

      var hashSubsidiaryEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var item in subsidiaryEntries) {
        string hash = $"{item.Ledger.Number}||{item.Currency.Code}||" +
                      $"{item.Account.Number}||{item.Sector.Code}||" +
                      $"{item.SubledgerAccountId}";

        hashSubsidiaryEntries.Insert(hash, item);
      }

      EmpiriaHashTable<TrialBalanceEntry> returnedSubsidiaryEntries = GenerateEntries(hashSubsidiaryEntries);

      return returnedSubsidiaryEntries;
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_command);

      _command.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
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
            EmpiriaHashTable<TrialBalanceEntry> hashSubsidiaryEntries) {

      var helper = new TrialBalanceHelper(_command);

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in hashSubsidiaryEntries.ToFixedList()) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        helper.SummaryBySubledgerEntry(hashReturnedEntries, entry, TrialBalanceItemType.BalanceSummary);

      }

      return hashReturnedEntries;
    }


    private List<TrialBalanceEntry> OrderByAccountNumber(
                                    EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {

      var totaBySubsidiaryAccountList = summaryEntries.ToFixedList();

      var returnedCombineOrdering = new List<TrialBalanceEntry>();

      foreach (var entry in totaBySubsidiaryAccountList) {
        SubsidiaryAccount subsidiary = SubsidiaryAccount.Parse(entry.SubledgerAccountIdParent);
        if (subsidiary != null) {
          entry.SubledgerAccountNumber = subsidiary.Number;
          entry.GroupName = subsidiary.Name;
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
