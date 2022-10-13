/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : ValorizacionHelper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build reporte de valorización.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build reporte de valorización.</summary>
  internal class ValorizacionHelper {

    private readonly TrialBalanceQuery _query;

    internal ValorizacionHelper(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods


    internal FixedList<TrialBalanceEntry> GetAccountsByCurrency(FixedList<TrialBalanceEntry> accountEntries) {
      var trialBalanceHelper = new TrialBalanceHelper(_query);
      var balanzaColumnasHelper = new BalanzaColumnasMonedaHelper(_query);

      trialBalanceHelper.RoundDecimals(accountEntries);

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> parentAccountsEntries = trialBalanceHelper.GetCalculatedParentAccounts(
                                                                          accountEntries.ToFixedList());

      List<TrialBalanceEntry> debtorAccounts = balanzaColumnasHelper.GetSumFromCreditorToDebtorAccounts(
                                                        parentAccountsEntries);

      balanzaColumnasHelper.CombineAccountEntriesAndDebtorAccounts(accountEntries.ToList(), debtorAccounts);

      FixedList<TrialBalanceEntry> accountEntriesByCurrency =
                                          balanzaColumnasHelper.GetAccountEntriesByCurrency(debtorAccounts);

      return accountEntriesByCurrency;
    }


    internal List<ValorizacionEntry> MergeAccountsIntoAccountsByCurrency(
                                    FixedList<TrialBalanceEntry> accountEntries) {
      var returnedEntries = new List<ValorizacionEntry>();

      foreach (var entry in accountEntries.Where(a => a.Currency.Equals(Currency.USD))) {

        returnedEntries.Add(MapToValorizedReport(entry));

      }

      MergeForeignBalancesByAccount(returnedEntries, accountEntries);

      return returnedEntries.OrderBy(a => a.Account.Number).ToList();
    }


    #endregion Public methods


    #region Private methods


    private ValorizacionEntry MapToValorizedReport(TrialBalanceEntry entry) {
      ValorizacionEntry valorizacion = new ValorizacionEntry();

      valorizacion.ItemType = entry.ItemType;
      valorizacion.Account = entry.Account;
      valorizacion.Currency = entry.Currency;
      valorizacion.Sector = entry.Sector;

      valorizacion.CurrentBalance = entry.CurrentBalance;
      valorizacion.InitialBalance = entry.InitialBalance;
      valorizacion.ExchangeRate = entry.ExchangeRate;

      ValidateCurrencyByEntry(valorizacion, entry);

      valorizacion.HasParentPostingEntry = entry.HasParentPostingEntry;
      valorizacion.IsParentPostingEntry = entry.IsParentPostingEntry;

      return valorizacion;
    }


    private void MergeForeignBalancesByAccount(List<ValorizacionEntry> returnedEntries, FixedList<TrialBalanceEntry> accountEntries) {

      foreach (var entry in accountEntries) {

        var returnedEntry = returnedEntries.Find(a => a.Account.Number == entry.Account.Number);

        if (returnedEntry == null) {
          returnedEntries.Add(MapToValorizedReport(entry));
        } else {

          ValidateCurrencyByEntry(returnedEntry, entry);

        }

      } // foreach

    }


    private void ValidateCurrencyByEntry(ValorizacionEntry returnedEntry, TrialBalanceEntry entry) {
      if (entry.Currency.Equals(Currency.USD)) {
        returnedEntry.DollarBalance = entry.CurrentBalance;
      }
      if (entry.Currency.Equals(Currency.YEN)) {
        returnedEntry.YenBalance = entry.CurrentBalance;
      }
      if (entry.Currency.Equals(Currency.EUR)) {
        returnedEntry.EuroBalance = entry.CurrentBalance;
      }
      if (entry.Currency.Equals(Currency.UDI)) {
        returnedEntry.UdisBalance = entry.CurrentBalance;
      }
    }


    #endregion Private methods

  } // class ValorizacionHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
