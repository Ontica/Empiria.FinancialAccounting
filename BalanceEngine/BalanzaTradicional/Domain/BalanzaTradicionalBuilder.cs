﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaTradicional                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanzas tradicionales.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanzas tradicionales.</summary>
  internal class BalanzaTradicionalBuilder {

    private readonly TrialBalanceQuery _query;

    internal BalanzaTradicionalBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var helper = new BalanzaTradicionalHelper(_query);
      
      FixedList<TrialBalanceEntry> accountEntries = helper.GetPostingEntries();

      if (accountEntries.Count == 0) {
        return new TrialBalance(_query, new FixedList<ITrialBalanceEntry>());
      }

      FixedList<TrialBalanceEntry> parentAccounts = helper.GetCalculatedParentAccounts(
                                                    accountEntries);

      List<TrialBalanceEntry> parentsAndAccountEntries = GetAccountsAndParentsWithSectorization(
                                                         accountEntries, parentAccounts);

      List<TrialBalanceEntry> balanza = GetTotals(
                                        parentsAndAccountEntries, accountEntries);

      var balanceHelper = new TrialBalanceHelper(_query);
      balanceHelper.RestrictLevels(balanza);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              balanza.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    private List<TrialBalanceEntry> GetAccountsAndParentsWithSectorization(
                                    FixedList<TrialBalanceEntry> accountEntries,
                                    FixedList<TrialBalanceEntry> parentAccounts) {

      var balanceHelper = new TrialBalanceHelper(_query);

      balanceHelper.SetSummaryToParentEntries(accountEntries);

      List<TrialBalanceEntry> accountEntriesMapped = balanceHelper.GetEntriesMappedForSectorization(
                                                     accountEntries.ToList());

      List<TrialBalanceEntry> accountAndSectorization =
        balanceHelper.GetSummaryAccountsAndSectorization(accountEntriesMapped);

      List<TrialBalanceEntry> parentAccountsAndSectorization =
        balanceHelper.GetSummaryAccountsAndSectorization(parentAccounts.ToList());

      var utility = new BalanzaTradicionalUtility(_query);

      List<TrialBalanceEntry> parentsAndAccountEntries = utility.CombineParentsAndAccountEntries(
                                                         parentAccountsAndSectorization,
                                                         accountAndSectorization);

      return parentsAndAccountEntries;

    }


    private List<TrialBalanceEntry> GetTotals(
                                    List<TrialBalanceEntry> parentsAndAccountEntries,
                                    FixedList<TrialBalanceEntry> accountEntries) {

      List<TrialBalanceEntry> balanza;

      if (!_query.IsOperationalReport) {
        balanza = GenerateTotalsForBalanza(
                  parentsAndAccountEntries, accountEntries);

      } else {
        balanza = GetOperationalReports(parentsAndAccountEntries);
      }

      return balanza;
    }


    #region Private methods


    private List<TrialBalanceEntry> GenerateTotalsForBalanza(
                                    List<TrialBalanceEntry> parentsAndAccountEntries,
                                    FixedList<TrialBalanceEntry> accountEntries) {

      if (parentsAndAccountEntries.Count == 0 || accountEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }

      var helper = new BalanzaTradicionalHelper(_query);


      FixedList<TrialBalanceEntry> groupTotalsEntries = helper.GenerateTotalGroupEntries(
                                                         accountEntries);

      var utility = new BalanzaTradicionalUtility(_query);

      List<TrialBalanceEntry> totalGroupAndAccountEntries =
                              utility.CombineTotalGroupEntriesAndAccountEntries(
                              parentsAndAccountEntries, groupTotalsEntries);

      List<TrialBalanceEntry> totalDebtorCreditorEntries =
                              helper.GenerateTotalDebtorCreditorsByCurrency(accountEntries.ToList());

      List<TrialBalanceEntry> totalDebtorCreditorsAndAccountEntries =
                              utility.CombineTotalDebtorCreditorsByCurrencyAndAccountEntries(
                              totalGroupAndAccountEntries, totalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalsByCurrency = helper.GenerateTotalByCurrency(
                                                totalDebtorCreditorEntries);

      List<TrialBalanceEntry> totalByCurrencyAndAccountEntries =
                              utility.CombineTotalsByCurrencyAndAccountEntries(
                              totalDebtorCreditorsAndAccountEntries, totalsByCurrency);

      if (_query.ShowCascadeBalances) {
        List<TrialBalanceEntry> totalsConsolidatedByLedger =
                              helper.GenerateTotalsConsolidatedByLedger(totalsByCurrency);

        totalByCurrencyAndAccountEntries = utility.CombineTotalConsolidatedByLedgerAndAccountEntries(
                                           totalByCurrencyAndAccountEntries, totalsConsolidatedByLedger);
      }

      List<TrialBalanceEntry> returnedBalance = new List<TrialBalanceEntry>(totalByCurrencyAndAccountEntries);

      TrialBalanceEntry totalConsolidated = helper.TryGenerateTotalConsolidated(totalsByCurrency);

      if (totalConsolidated != null) {
        returnedBalance.Add(totalConsolidated);
      }

      return returnedBalance;
    }


    private List<TrialBalanceEntry> GetOperationalReports(List<TrialBalanceEntry> trialBalance) {

      var totalByAccountEntries = new EmpiriaHashTable<TrialBalanceEntry>(trialBalance.Count);

      if (_query.ConsolidateBalancesToTargetCurrency) {

        foreach (var entry in trialBalance) {
          SummaryByAccount(totalByAccountEntries, entry);
        }

        return totalByAccountEntries.ToFixedList().ToList();

      } else {

        return trialBalance;

      }
    }


    internal void SummaryByAccount(EmpiriaHashTable<TrialBalanceEntry> totalByEntries,
                                                       TrialBalanceEntry balanceEntry) {

      TrialBalanceEntry entry = balanceEntry.CreatePartialCopy();

      if (entry.ItemType == TrialBalanceItemType.Summary && entry.Level == 1 && entry.HasSector) {
        entry.InitialBalance = 0;
        entry.Debit = 0;
        entry.Credit = 0;
        entry.CurrentBalance = 0;
      }
      entry.LastChangeDate = balanceEntry.LastChangeDate;

      string hash = $"{entry.Account.Number}";
      
      var balanceHelper = new TrialBalanceHelper(_query);
      balanceHelper.GenerateOrIncreaseEntries(totalByEntries, entry, entry.Account,
                                entry.Sector, TrialBalanceItemType.Entry, hash);
    }


    #endregion

  }  // class BalanzaTradicional

}  // namespace Empiria.FinancialAccounting.BalanceEngine
