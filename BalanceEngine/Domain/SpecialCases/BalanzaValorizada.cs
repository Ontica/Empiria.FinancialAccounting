/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaValorizada                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza valorizada en dolares.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza valorizada en dolares.</summary>
  internal class BalanzaValorizada {

    private readonly TrialBalanceCommand _command;

    public BalanzaValorizada(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      List<TrialBalanceEntry> summaryEntries = helper.GenerateSummaryEntries(trialBalance.ToFixedList());

      EmpiriaHashTable<TrialBalanceEntry> getLedgerAccounts = GetLedgerAccountsList(summaryEntries);

      //EmpiriaHashTable<TrialBalanceEntry> getAccountsByCurrency = GetAccountsByCurrency(summaryEntries, getLedgerAccounts);

      List<ValuedTrialBalanceEntry> mergeTrialBalancetoToValuedBalances = 
                                    MergeTrialBalanceIntoValuedBalances(getLedgerAccounts);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                mergeTrialBalancetoToValuedBalances.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

    private EmpiriaHashTable<TrialBalanceEntry> GetAccountsByCurrency(
                                                List<TrialBalanceEntry> summaryEntries, 
                                                EmpiriaHashTable<TrialBalanceEntry> ledgerAccounts) {
      var returnedBalances = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var ledger in ledgerAccounts.ToFixedList()) {

        string hash = $"{ledger.Account.Number}";


      }

      return returnedBalances;
    }

    private EmpiriaHashTable<TrialBalanceEntry> GetLedgerAccountsList(List<TrialBalanceEntry> trialBalance) {

      var helper = new TrialBalanceHelper(_command);

      var ledgersList = trialBalance.Where(a => a.Level == 1 && a.Sector.Code == "00" && 
                                                a.Currency.Code == "02").ToList();

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in ledgersList) {

        helper.SummaryByEntry(hashReturnedEntries, entry, entry.Account, 
                              Sector.Empty, TrialBalanceItemType.BalanceSummary);

      }

      return hashReturnedEntries;
    }

    private List<ValuedTrialBalanceEntry> MergeTrialBalanceIntoValuedBalances(
                                          EmpiriaHashTable<TrialBalanceEntry> getLedgerAccounts) {

      List<ValuedTrialBalanceEntry> returnedValuedBalance = new List<ValuedTrialBalanceEntry>();
      foreach (var entry in getLedgerAccounts.ToFixedList()) {
        returnedValuedBalance.Add(entry.MapToValuedBalanceEntry());
      }

      return returnedValuedBalance;
    }

  } // class BalanzaValorizada

} // namespace Empiria.FinancialAccounting.BalanceEngine
