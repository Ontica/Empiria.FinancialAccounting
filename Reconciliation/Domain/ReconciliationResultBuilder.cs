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

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Builds a list with reconciliation results.</summary>
  internal class ReconciliationResultBuilder {

    private readonly FixedList<OperationalEntryDto>  _operationalEntries;
    private readonly FixedList<BalanzaTradicionalEntryDto> _balances;
    private readonly List<ReconciliationResultEntry> _resultsList;

    internal ReconciliationResultBuilder(FixedList<OperationalEntryDto> operationalEntries,
                                         FixedList<BalanzaTradicionalEntryDto> balances,
                                         int listCapacity = 1024) {

      Assertion.Require(operationalEntries, nameof(operationalEntries));
      Assertion.Require(balances, nameof(balances));

      _operationalEntries = operationalEntries;
      _balances = balances;
      _resultsList = new List<ReconciliationResultEntry>(listCapacity);
    }


    internal void InsertEntriesFor(ConciliacionDerivadosListItem account) {
      Assertion.Require(account, nameof(account));

      FixedList<OperationalEntryDto> operationalEntries = GetOperationalEntriesFor(account);

      InsertOrUpdateOperationalEntries(operationalEntries);

      FixedList<BalanzaTradicionalEntryDto> accountingEntries = GetAccountingEntriesFor(account);

      InsertOrUpdateAccountingEntries(accountingEntries);
    }


    internal FixedList<ReconciliationResultEntry> ToFixedList() {
      var sorted = _resultsList.OrderBy(x => x.CurrencyCode)
                                .ThenBy(x => x.AccountNumber)
                                .ThenBy(x => x.SectorCode);

      return sorted.ToFixedList();
    }


    #region Helpers


    private decimal CalculateTotal(OperationalEntryDto operationalEntry) {
      var account = AccountsChart.IFRS.TryGetAccount(operationalEntry.AccountNumber);

      if (account == null || account.DebtorCreditor == DebtorCreditorType.Deudora) {
        return operationalEntry.Debits - operationalEntry.Credits;
      } else {
        return operationalEntry.Credits - operationalEntry.Debits;
      }
    }


    private decimal CalculateTotal(BalanzaTradicionalEntryDto accountingEntry) {
      if (accountingEntry.DebtorCreditor == DebtorCreditorType.Deudora) {
        return accountingEntry.Debit - accountingEntry.Credit;
      } else {
        return accountingEntry.Credit - accountingEntry.Debit;
      }
    }

    private FixedList<BalanzaTradicionalEntryDto> GetAccountingEntriesFor(ConciliacionDerivadosListItem account) {
      return _balances.FindAll(x => x.AccountNumber == account.Account.Number &&
                                    x.SectorCode == "00");
    }


    private FixedList<OperationalEntryDto> GetOperationalEntriesFor(ConciliacionDerivadosListItem account) {
      return _operationalEntries.FindAll(x => x.AccountNumber == account.Account.Number);
    }


    private void InsertOrUpdateAccountingEntries(FixedList<BalanzaTradicionalEntryDto> accountingEntries) {
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


    private string BuildReconciliationEntryKey(BalanzaTradicionalEntryDto entry) {
      return entry.AccountNumber + "_" + entry.CurrencyCode;
    }


    private void InsertResultEntry(OperationalEntryDto operationalEntry) {
      string entryKey = BuildReconciliationEntryKey(operationalEntry);

      var newEntry = new ReconciliationResultEntry {
        UniqueKey = entryKey,
        AccountNumber = operationalEntry.AccountNumber,
        CurrencyCode = operationalEntry.CurrencyCode,
        SectorCode = operationalEntry.SectorCode,
        OperationalTotal = CalculateTotal(operationalEntry)
      };

      _resultsList.Add(newEntry);
    }


    private void InsertResultEntry(BalanzaTradicionalEntryDto accountingEntry) {
      string entryKey = BuildReconciliationEntryKey(accountingEntry);

      var newEntry = new ReconciliationResultEntry {
        UniqueKey = entryKey,
        AccountNumber = accountingEntry.AccountNumber,
        CurrencyCode = accountingEntry.CurrencyCode,
        SectorCode = accountingEntry.SectorCode,
        AccountingTotal = CalculateTotal(accountingEntry)
      };

      _resultsList.Add(newEntry);
    }


    private bool ResultListContains(OperationalEntryDto operationalEntry) {
      string key = BuildReconciliationEntryKey(operationalEntry);

      return _resultsList.Exists(x => x.UniqueKey == key);
    }


    private bool ResultListContains(BalanzaTradicionalEntryDto accountingEntry) {
      string key = BuildReconciliationEntryKey(accountingEntry);

      return _resultsList.Exists(x => x.UniqueKey == key);
    }


    private void UpdateResultEntry(OperationalEntryDto operationalEntry) {
      string entryKey = BuildReconciliationEntryKey(operationalEntry);

      int index = _resultsList.FindIndex(x => x.UniqueKey == entryKey);

      _resultsList[index].OperationalTotal += CalculateTotal(operationalEntry);
    }


    private void UpdateResultEntry(BalanzaTradicionalEntryDto accountingEntry) {
      string entryKey = BuildReconciliationEntryKey(accountingEntry);

      int index = _resultsList.FindIndex(x => x.UniqueKey == entryKey);

      _resultsList[index].AccountingTotal += CalculateTotal(accountingEntry);
    }

    #endregion Helpers

  }  // class ReconciliationResultBuilder

}  // namespace Empiria.FinancialAccounting.Reconciliation
