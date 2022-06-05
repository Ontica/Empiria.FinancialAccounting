/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : SaldosPorAuxiliarBuilder                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar para el explorador de saldos.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar para el explorador de saldos.</summary>
  internal class SaldosPorAuxiliarBuilder {

    private readonly BalanceExplorerQuery _query;

    public SaldosPorAuxiliarBuilder(BalanceExplorerQuery query) {
      _query = query;
    }


    internal BalanceExplorerResult Build() {
      var helper = new BalanceExplorerHelper(_query);

      FixedList<BalanceExplorerEntry> balanceEntries = helper.GetBalanceExplorerEntries();

      balanceEntries = helper.GetSummaryToParentEntries(balanceEntries);

      EmpiriaHashTable<BalanceExplorerEntry> subledgerAccounts = GetSubledgerAccounts(balanceEntries);

      List<BalanceExplorerEntry> orderingBalance = OrderBySubledgerAccounts(subledgerAccounts);

      FixedList<BalanceExplorerEntry> returnedEntries = CombineSubledgerAccountsWithBalanceEntries(
                                                            orderingBalance, balanceEntries);

      var balancesToReturn = new FixedList<BalanceExplorerEntry>(returnedEntries);

      return new BalanceExplorerResult(_query, balancesToReturn);
    }

    #region Private methods

    private FixedList<BalanceExplorerEntry> CombineSubledgerAccountsWithBalanceEntries(
                                    List<BalanceExplorerEntry> orderingBalance,
                                    FixedList<BalanceExplorerEntry> balanceEntries) {
      var returnedEntries = new List<BalanceExplorerEntry>();

      foreach (var entry in orderingBalance) {
        var summaryAccounts = balanceEntries.Where(
                      a => a.SubledgerAccountId == entry.SubledgerAccountIdParent &&
                      a.Ledger.Number == entry.Ledger.Number &&
                      a.Currency.Code == entry.Currency.Code &&
                      a.ItemType == TrialBalanceItemType.Entry).ToList();

        foreach (var summary in summaryAccounts) {
          entry.LastChangeDate = summary.LastChangeDate > entry.LastChangeDate ?
                                 summary.LastChangeDate : entry.LastChangeDate;
          summary.SubledgerAccountId = 0;
          summary.SubledgerAccountNumber = entry.SubledgerAccountNumber;
          summary.SubledgerAccountName = entry.SubledgerAccountName;
        }

        returnedEntries.Add(entry);

        if (summaryAccounts.Count > 0) {
          returnedEntries.AddRange(summaryAccounts);
        }
      }

      return returnedEntries.ToFixedList();
    }

    private List<BalanceExplorerEntry> OrderBySubledgerAccounts(
                                    EmpiriaHashTable<BalanceExplorerEntry> subledgerAccounts) {

      var returnedCombineOrdering = new List<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccounts.ToFixedList()) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (subledgerAccount != null) {
          entry.SubledgerAccountNumber = subledgerAccount.Number;
          entry.SubledgerAccountName = subledgerAccount.Name;
          entry.GroupName = subledgerAccount.Name;
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Count();
          entry.SubledgerAccountId = entry.SubledgerAccountIdParent;
        }
        returnedCombineOrdering.Add(entry);
      }
      return returnedCombineOrdering.OrderBy(a => a.Currency.Code)
                                    .ThenBy(a => a.SubledgerNumberOfDigits)
                                    .ThenBy(a => a.SubledgerAccountNumber)
                                    .ToList();
    }

    #endregion Private methods

    #region Helpers

    private EmpiriaHashTable<BalanceExplorerEntry> GetSubledgerAccounts(FixedList<BalanceExplorerEntry> balance) {

      var subledgerAccountList = balance.Where(a => a.SubledgerAccountId > 0).ToList();

      var subledgerAccountListHashTable = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccountList) {
        string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                      $"{entry.Account.Number}||{entry.Sector.Code}||" +
                      $"{entry.SubledgerAccountId}";

        subledgerAccountListHashTable.Insert(hash, entry);
      }

      return GenerateSubledgerAccount(subledgerAccountListHashTable);
    }

    private EmpiriaHashTable<BalanceExplorerEntry> GenerateSubledgerAccount(
                        EmpiriaHashTable<BalanceExplorerEntry> subledgerAccountListHash) {
      var helper = new BalanceExplorerHelper(_query);

      var returnedEntries = new EmpiriaHashTable<BalanceExplorerEntry>();

      foreach (var entry in subledgerAccountListHash.ToFixedList()) {

        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;
        entry.DebtorCreditor = entry.Account.DebtorCreditor;

        helper.SummaryBySubledgerAccount(returnedEntries, entry, TrialBalanceItemType.Summary);
      }

      return returnedEntries;
    }

    #endregion Helpers

  } // class SaldosPorAuxiliarBuilder

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
