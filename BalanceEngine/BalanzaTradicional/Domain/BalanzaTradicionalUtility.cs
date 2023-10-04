/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IExcelExporter                        *
*  Type     : BalanzaTradicionalUtility                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Utility methods to manage accounts information for balanza tradicional.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Utility methods to manage accounts information for balanza tradicional.</summary>
  internal class BalanzaTradicionalUtility {

    private readonly TrialBalanceQuery _query;

    internal BalanzaTradicionalUtility(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods


    internal List<TrialBalanceEntry> CombineParentsAndAccountEntries(
                                      List<TrialBalanceEntry> parentAccountEntries,
                                      List<TrialBalanceEntry> accountEntries) {

      var returnedEntries = new List<TrialBalanceEntry>(accountEntries);
      var entriesValidator = new TrialBalanceEntriesValidator();

      if (entriesValidator.ValidateToCountEntries(parentAccountEntries)) {
        returnedEntries.AddRange(parentAccountEntries);
      }

      var trialBalanceHelper = new TrialBalanceHelper(_query);

      trialBalanceHelper.SetSubledgerAccountInfoByEntry(returnedEntries);

      List<TrialBalanceEntry> orderingAccountEntries =
        trialBalanceHelper.OrderingParentsAndAccountEntries(returnedEntries);

      return orderingAccountEntries;
    }


    internal List<TrialBalanceEntry> CombineTotalsByCurrencyAndAccountEntries(
                                      List<TrialBalanceEntry> accountEntries,
                                      List<TrialBalanceEntry> totalsByCurrency) {

      if (totalsByCurrency.Count == 0) {
        return accountEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();
      var entriesValidator = new TrialBalanceEntriesValidator();

      foreach (var currencyEntry in totalsByCurrency) {

        var entriesByCurrency = accountEntries.Where(a => a.Ledger.Id == currencyEntry.Ledger.Id &&
                                                     a.Currency.Code == currencyEntry.Currency.Code)
                                              .ToList();
        if (entriesValidator.ValidateToCountEntries(entriesByCurrency)) {

          entriesByCurrency.Add(currencyEntry);
          returnedEntries.AddRange(entriesByCurrency);
        }
      }

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineTotalConsolidatedByLedgerAndAccountEntries(
                                      List<TrialBalanceEntry> balanceEntries,
                                      List<TrialBalanceEntry> totalConsolidatedByLedger) {

      if (totalConsolidatedByLedger.Count == 0) {
        return balanceEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();
      var entriesValidator = new TrialBalanceEntriesValidator();

      foreach (var totalByLedger in totalConsolidatedByLedger) {
        var entries = balanceEntries.Where(a => a.Ledger.Id == totalByLedger.Ledger.Id)
                                     .ToList();

        if (entriesValidator.ValidateToCountEntries(entries)) {
          entries.Add(totalByLedger);
          returnedEntries.AddRange(entries);

        }
      }
      return returnedEntries;
    }


    internal List<TrialBalanceEntry> CombineTotalDebtorCreditorsByCurrencyAndAccountEntries(
                                     List<TrialBalanceEntry> accountEntries,
                                     List<TrialBalanceEntry> totalDebtorCreditors) {

      if (totalDebtorCreditors.Count == 0) {
        return accountEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();
      var entriesValidator = new TrialBalanceEntriesValidator();

      foreach (var debtorCreditorEntry in totalDebtorCreditors) {

        var entries = accountEntries.Where(a => a.Ledger.Id == debtorCreditorEntry.Ledger.Id &&
                                           a.Currency.Code == debtorCreditorEntry.Currency.Code &&
                                           a.DebtorCreditor == debtorCreditorEntry.DebtorCreditor).ToList();

        if (entriesValidator.ValidateToCountEntries(entries)) {
          entries.Add(debtorCreditorEntry);
          returnedEntries.AddRange(entries);
        }
      }

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    internal List<TrialBalanceEntry> CombineTotalGroupEntriesAndAccountEntries(
                                      List<TrialBalanceEntry> balanzaEntries,
                                      FixedList<TrialBalanceEntry> totalGroupEntries) {

      if (totalGroupEntries.Count == 0) {
        return balanzaEntries;
      }

      var returnedEntries = new List<TrialBalanceEntry>();
      var validator = new TrialBalanceEntriesValidator();

      foreach (var totalGroupEntry in totalGroupEntries) {

        var accountEntries = balanzaEntries.Where(
                                  a => a.Account.GroupNumber == totalGroupEntry.GroupNumber &&
                                  a.Ledger.Id == totalGroupEntry.Ledger.Id &&
                                  a.Currency.Id == totalGroupEntry.Currency.Id &&
                                  a.Account.DebtorCreditor == totalGroupEntry.DebtorCreditor).ToList();

        if (validator.ValidateToCountEntries(accountEntries)) {
          accountEntries.Add(totalGroupEntry);
          returnedEntries.AddRange(accountEntries);
        }
      }

      return returnedEntries.OrderBy(a => a.Ledger.Number)
                            .ThenBy(a => a.Currency.Code)
                            .ToList();
    }


    #endregion Public methods


  } // class BalanzaTradicionalUtility

} // namespace Empiria.FinancialAccounting.BalanceEngine
