/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : AnaliticoDeCuentasUtility                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Utility methods to manage accounts information for analitico de cuentas.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Utility methods to manage accounts information for analitico de cuentas.</summary>
  internal class AnaliticoDeCuentasUtility {

    private readonly TrialBalanceQuery _query;

    internal AnaliticoDeCuentasUtility(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods


    internal List<TrialBalanceEntry> CombineSummaryAndPostingEntries(
                                      List<TrialBalanceEntry> parentAccounts,
                                      FixedList<TrialBalanceEntry> accountEntries) {
      if (accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var returnedEntries = new List<TrialBalanceEntry>(accountEntries);

      if (parentAccounts.Count > 0) {
        returnedEntries.AddRange(parentAccounts);
      }

      returnedEntries = GetSubledgerAccountInfo(returnedEntries);
      returnedEntries = OrderingEntries(returnedEntries);

      return returnedEntries;
    }


    internal List<AnaliticoDeCuentasEntry> CombineTotalsByGroupAndAccountEntries(
                                                  List<AnaliticoDeCuentasEntry> analyticEntries,
                                                  List<AnaliticoDeCuentasEntry> totalByGroup) {
      if (totalByGroup.Count == 0) {
        return analyticEntries;
      }

      List<AnaliticoDeCuentasEntry> returnedEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var debtorsGroup in totalByGroup) {
        var debtorEntries = analyticEntries.FindAll(a => a.Account.GroupNumber == debtorsGroup.GroupNumber &&
                                                         a.Ledger.Equals(debtorsGroup.Ledger) &&
                                                         a.DebtorCreditor == debtorsGroup.DebtorCreditor);
        if (debtorEntries.Count > 0) {
          debtorEntries.Add(debtorsGroup);
          returnedEntries.AddRange(debtorEntries);
        }
      }

      return returnedEntries;
    }


    internal List<AnaliticoDeCuentasEntry> CombineTotalDebtorCreditorAndEntries(
                                                List<AnaliticoDeCuentasEntry> analyticEntries,
                                                List<AnaliticoDeCuentasEntry> totalByDebtorsCreditors) {
      if (totalByDebtorsCreditors.Count == 0) {
        return analyticEntries;
      }

      List<AnaliticoDeCuentasEntry> returnedEntries = new List<AnaliticoDeCuentasEntry>();

      CombineEntriesWithTotalDebtors(returnedEntries, analyticEntries, totalByDebtorsCreditors);
      CombineEntriesWithTotalCreditors(returnedEntries, analyticEntries, totalByDebtorsCreditors);

      return returnedEntries;
    }


    internal List<AnaliticoDeCuentasEntry> CombineTotalReportAndEntries(
                                     List<AnaliticoDeCuentasEntry> analyticEntries,
                                     List<AnaliticoDeCuentasEntry> totalsByLedgerList) {
      if (totalsByLedgerList.Count == 0 || analyticEntries.Count == 0) {
        return analyticEntries;
      }

      var returnedEntries = new List<AnaliticoDeCuentasEntry>();

      foreach (var totalByLedger in totalsByLedgerList) {
        var entriesByLedger = analyticEntries.FindAll(a => a.Currency.Equals(totalByLedger.Currency) &&
                                                           a.Ledger.Equals(totalByLedger.Ledger));
        if (entriesByLedger.Count > 0) {
          entriesByLedger.Add(totalByLedger);
          returnedEntries.AddRange(entriesByLedger);
        }
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> RemoveUnneededAccounts(List<TrialBalanceEntry> summaryEntries) {
      List<TrialBalanceEntry> returnedSummaryEntries = new List<TrialBalanceEntry>();

      foreach (var entry in summaryEntries) {
        if (MustRemoveAccount(entry.Account)) {
          continue;
        }
        returnedSummaryEntries.Add(entry);
      }
      return returnedSummaryEntries;
    }


    #endregion Public methods

    #region Private methods


    private void CombineEntriesWithTotalCreditors(List<AnaliticoDeCuentasEntry> returnedEntries,
                                                  List<AnaliticoDeCuentasEntry> analyticEntries,
                                                  List<AnaliticoDeCuentasEntry> totalByDebtorsCreditors) {

      foreach (var totalCreditor in totalByDebtorsCreditors
                    .Where(a => a.DebtorCreditor == DebtorCreditorType.Acreedora)) {

        var creditorEntries = analyticEntries.FindAll(a => a.Ledger.Equals(totalCreditor.Ledger) &&
                                                           a.Currency.Equals(totalCreditor.Currency) &&
                                                           a.DebtorCreditor == totalCreditor.DebtorCreditor);
        creditorEntries.Add(totalCreditor);
        returnedEntries.AddRange(creditorEntries);
      }

    }


    private void CombineEntriesWithTotalDebtors(List<AnaliticoDeCuentasEntry> returnedEntries,
                                                List<AnaliticoDeCuentasEntry> analyticEntries,
                                                List<AnaliticoDeCuentasEntry> totalByDebtorsCreditors) {

      foreach (var totalDebtor in totalByDebtorsCreditors
                    .Where(a => a.DebtorCreditor == DebtorCreditorType.Deudora)) {

        var debtorEntries = analyticEntries.FindAll(a => a.Ledger.Equals(totalDebtor.Ledger) &&
                                                         a.Currency.Equals(totalDebtor.Currency) &&
                                                         a.DebtorCreditor == totalDebtor.DebtorCreditor);
        debtorEntries.Add(totalDebtor);
        returnedEntries.AddRange(debtorEntries);
      }

    }


    private List<TrialBalanceEntry> GetSubledgerAccountInfo(List<TrialBalanceEntry> entriesList) {
      if (!_query.WithSubledgerAccount) {
        return entriesList;
      }

      var returnedEntries = new List<TrialBalanceEntry>(entriesList);

      foreach (var entry in entriesList) {
        SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
        if (!subledgerAccount.IsEmptyInstance) {
          entry.SubledgerAccountNumber = subledgerAccount.Number != "0" ?
                                          subledgerAccount.Number : "";
          entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber.Length;
        }
      }

      return returnedEntries;
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


    private List<TrialBalanceEntry> OrderingEntries(List<TrialBalanceEntry> entries) {

      if (_query.WithSubledgerAccount) {

        return entries.OrderBy(a => a.Ledger.Number)
                      .ThenBy(a => a.Currency.Code)
                      .ThenByDescending(a => a.Account.DebtorCreditor)
                      .ThenBy(a => a.Account.Number)
                      .ThenBy(a => a.Sector.Code)
                      .ThenBy(a => a.SubledgerNumberOfDigits)
                      .ThenBy(a => a.SubledgerAccountNumber)
                      .ToList();
      } else {
        return entries.OrderBy(a => a.Ledger.Number)
                      .ThenBy(a => a.Currency.Code)
                      .ThenByDescending(a => a.Account.DebtorCreditor)
                      .ThenBy(a => a.Account.Number)
                      .ThenBy(a => a.Sector.Code)
                      .ThenBy(a => a.SubledgerAccountNumber)
                      .ToList();
      }
    }


    #endregion Private methods

  } // class AnaliticoDeCuentasUtility

} // namespace Empiria.FinancialAccounting.BalanceEngine
