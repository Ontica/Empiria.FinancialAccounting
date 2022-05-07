/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : ReconciliationResultBuilder                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a list with reconciliation results.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Builds a list with reconciliation results.</summary>
  internal class ReconciliationResultBuilder {

    private readonly FixedList<OperationalEntryDto>  _operationalEntries;
    private readonly FixedList<TrialBalanceEntryDto> _balances;
    private readonly List<ReconciliationResultEntry> _resultsList;

    internal ReconciliationResultBuilder(FixedList<OperationalEntryDto> operationalEntries,
                                         FixedList<TrialBalanceEntryDto> balances,
                                         int listCapacity = 1024) {

      Assertion.AssertObject(operationalEntries, nameof(operationalEntries));
      Assertion.AssertObject(balances, nameof(balances));

      _operationalEntries = operationalEntries;
      _balances = balances;
      _resultsList = new List<ReconciliationResultEntry>(listCapacity);
    }


    internal void InsertEntriesFor(AccountsListItem account) {
      Assertion.AssertObject(account, nameof(account));

      FixedList<OperationalEntryDto> operationalEntries = GetOperationalEntriesFor(account);

      InsertOrUpdateOperationalEntries(operationalEntries);

      FixedList<TrialBalanceEntryDto> accountingEntries = GetAccountingEntriesFor(account);

      InsertOrUpdateAccountingEntries(accountingEntries);
    }


    internal FixedList<ReconciliationResultEntry> ToFixedList() {
      var sorted = _resultsList.OrderBy(x => x.CurrencyCode)
                                .ThenBy(x => x.AccountNumber)
                                .ThenBy(x => x.SectorCode);

      return new FixedList<ReconciliationResultEntry>(sorted);
    }


    #region Helpers


    private FixedList<TrialBalanceEntryDto> GetAccountingEntriesFor(AccountsListItem account) {
      return _balances.FindAll(x => x.AccountNumber == account.AccountNumber &&
                                    x.SectorCode == "00");
    }


    private FixedList<OperationalEntryDto> GetOperationalEntriesFor(AccountsListItem account) {
      return _operationalEntries.FindAll(x => x.AccountNumber == account.AccountNumber);
    }


    private void InsertOrUpdateAccountingEntries(FixedList<TrialBalanceEntryDto> accountingEntries) {
      foreach (var accountingEntry in accountingEntries) {

        if (this.ResultListContains(accountingEntry)) {
          UpdateResultEntry(accountingEntry);

        } else {
          InsertResultEntry(accountingEntry);

        }
      }  // foreach
    }


    private void InsertOrUpdateOperationalEntries(FixedList<OperationalEntryDto> operationalEntries) {
      foreach (var operationalEntry in operationalEntries) {

        if (this.ResultListContains(operationalEntry)) {
          UpdateResultEntry(operationalEntry);

        } else {
          InsertResultEntry(operationalEntry);

        }
      }  // foreach
    }


    private string BuildReconciliationEntryKey(OperationalEntryDto entry) {
      return entry.AccountNumber + "_" + entry.CurrencyCode;
    }


    private string BuildReconciliationEntryKey(TrialBalanceEntryDto entry) {
      return entry.AccountNumber + "_" + entry.CurrencyCode;
    }


    private void InsertResultEntry(OperationalEntryDto operationalEntry) {
      string entryKey = BuildReconciliationEntryKey(operationalEntry);

      var newEntry = new ReconciliationResultEntry {
        UniqueKey = entryKey,
        AccountNumber = operationalEntry.AccountNumber,
        CurrencyCode = operationalEntry.CurrencyCode,
        SectorCode = operationalEntry.SectorCode,
        OperationalTotal = operationalEntry.Debits - operationalEntry.Credits
      };

      _resultsList.Add(newEntry);
    }


    private void InsertResultEntry(TrialBalanceEntryDto accountingEntry) {
      string entryKey = BuildReconciliationEntryKey(accountingEntry);

      var newEntry = new ReconciliationResultEntry {
        UniqueKey = entryKey,
        AccountNumber = accountingEntry.AccountNumber,
        CurrencyCode = accountingEntry.CurrencyCode,
        SectorCode = accountingEntry.SectorCode,
        AccountingTotal = accountingEntry.Debit - accountingEntry.Credit
      };

      _resultsList.Add(newEntry);
    }


    private bool ResultListContains(OperationalEntryDto operationalEntry) {
      string key = BuildReconciliationEntryKey(operationalEntry);

      return _resultsList.Exists(x => x.UniqueKey == key);
    }


    private bool ResultListContains(TrialBalanceEntryDto accountingEntry) {
      string key = BuildReconciliationEntryKey(accountingEntry);

      return _resultsList.Exists(x => x.UniqueKey == key);
    }


    private void UpdateResultEntry(OperationalEntryDto operationalEntry) {
      string entryKey = BuildReconciliationEntryKey(operationalEntry);

      int index = _resultsList.FindIndex(x => x.UniqueKey == entryKey);

      _resultsList[index].OperationalTotal += operationalEntry.Debits - operationalEntry.Credits;
    }


    private void UpdateResultEntry(TrialBalanceEntryDto accountingEntry) {
      string entryKey = BuildReconciliationEntryKey(accountingEntry);

      int index = _resultsList.FindIndex(x => x.UniqueKey == entryKey);

      _resultsList[index].AccountingTotal += accountingEntry.Debit - accountingEntry.Credit;
    }

    #endregion Helpers

  }  // class ReconciliationResultBuilder

}  // namespace Empiria.FinancialAccounting.Reconciliation
