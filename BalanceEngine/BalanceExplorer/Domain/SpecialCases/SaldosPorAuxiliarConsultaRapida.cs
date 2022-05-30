/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorAuxiliarConsultaRapida            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar de consulta rápida.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar de consulta rápida.</summary>
  internal class SaldosPorAuxiliarConsultaRapida {

    private readonly BalancesQuery _query;

    public SaldosPorAuxiliarConsultaRapida(BalancesQuery query) {
      _query = query;
    }

    internal Balances Build() {
      var helper = new BalanceHelper(_query);

      FixedList<BalanceEntry> balanceEntries = helper.GetBalanceEntries();

      balanceEntries = helper.GetSummaryToParentEntries(balanceEntries);

      EmpiriaHashTable<BalanceEntry> subledgerAccounts = GetSubledgerAccounts(balanceEntries);

      List<BalanceEntry> orderingBalance = OrderBySubledgerAccounts(subledgerAccounts);

      FixedList<BalanceEntry> returnedEntries = CombineSubledgerAccountsWithBalanceEntries(
                                                            orderingBalance, balanceEntries);

      var balancesToReturn = new FixedList<BalanceEntry>(returnedEntries);

      return new Balances(_query, balancesToReturn);
    }


    private FixedList<BalanceEntry> CombineSubledgerAccountsWithBalanceEntries(
                                    List<BalanceEntry> orderingBalance,
                                    FixedList<BalanceEntry> balanceEntries) {
      var returnedEntries = new List<BalanceEntry>();

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

    private List<BalanceEntry> OrderBySubledgerAccounts(
                                    EmpiriaHashTable<BalanceEntry> subledgerAccounts) {

      var returnedCombineOrdering = new List<BalanceEntry>();

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


    #region Helper methods

    private EmpiriaHashTable<BalanceEntry> GetSubledgerAccounts(FixedList<BalanceEntry> balance) {

      var subledgerAccountList = balance.Where(a => a.SubledgerAccountId > 0).ToList();

      var subledgerAccountListHashTable = new EmpiriaHashTable<BalanceEntry>();

      foreach (var entry in subledgerAccountList) {
        string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                      $"{entry.Account.Number}||{entry.Sector.Code}||" +
                      $"{entry.SubledgerAccountId}";

        subledgerAccountListHashTable.Insert(hash, entry);
      }

      return GenerateSubledgerAccount(subledgerAccountListHashTable);
    }

    private EmpiriaHashTable<BalanceEntry> GenerateSubledgerAccount(
                        EmpiriaHashTable<BalanceEntry> subledgerAccountListHash) {
      var helper = new BalanceHelper(_query);

      var returnedEntries = new EmpiriaHashTable<BalanceEntry>();

      foreach (var entry in subledgerAccountListHash.ToFixedList()) {

        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;
        entry.DebtorCreditor = entry.Account.DebtorCreditor;

        helper.SummaryBySubledgerAccount(returnedEntries, entry, TrialBalanceItemType.Summary);
      }

      return returnedEntries;
    }

    #endregion


  } // class SaldosPorAuxiliarConsultaRapida

} // Empiria.FinancialAccounting.BalanceEngine.Domain.SpecialCases
