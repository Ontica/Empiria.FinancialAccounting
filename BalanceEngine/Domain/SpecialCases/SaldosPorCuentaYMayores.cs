/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorCuentaYMayores                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta y mayores.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por cuenta y mayores.</summary>
  internal class SaldosPorCuentaYMayores {

    private readonly TrialBalanceCommand _command;

    public SaldosPorCuentaYMayores(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetSummaryAndPostingEntries();

      List<TrialBalanceEntry> summaryByAccountAndDelegations = GenerateTotalByAccountAndLedgers(trialBalance);

      trialBalance = CombineAccountsAndLedgers(summaryByAccountAndDelegations);

      trialBalance = helper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Helper methods

    private List<TrialBalanceEntry> GenerateTotalByAccountAndLedgers(
                               List<TrialBalanceEntry> trialBalance) {

      List<TrialBalanceEntry> summaryLedgersList = new List<TrialBalanceEntry>();
      List<TrialBalanceEntry> ledgersGroupList = trialBalance.Where(
                                                  a => a.HasSector && a.Level == 1).ToList();

      foreach (var ledgerGroup in ledgersGroupList) {

        var existLedger = summaryLedgersList.FirstOrDefault(a => a.Ledger.Number == ledgerGroup.Ledger.Number &&
                                                                 a.Currency.Code == ledgerGroup.Currency.Code &&
                                                                 a.Account.Number == ledgerGroup.Account.Number);

        var ledgersById = ledgersGroupList.Where(a => a.Ledger.Number == ledgerGroup.Ledger.Number &&
                                                 a.Currency.Code == ledgerGroup.Currency.Code &&
                                                 a.Account.Number == ledgerGroup.Account.Number).ToList();

        if (existLedger == null) {
          ledgerGroup.GroupName = ledgerGroup.Ledger.Name;
          ledgerGroup.Sector = Sector.Empty;
          ledgerGroup.ItemType = TrialBalanceItemType.BalanceEntry;
          summaryLedgersList.Add(ledgerGroup);
        } else {
          foreach (var ledger in ledgersById) {
            existLedger.Sum(ledger);
          }
        }

      } // foreach

      return summaryLedgersList;
    }


    internal List<TrialBalanceEntry> CombineAccountsAndLedgers(List<TrialBalanceEntry> summaryLedgersList) {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> returnedEntries = new List<TrialBalanceEntry>();
      var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in summaryLedgersList) {

        var existAccount = returnedEntries.FirstOrDefault(a => a.Ledger == Ledger.Empty &&
                                                          a.Currency.Code == entry.Currency.Code &&
                                                          a.Account.Number == entry.Account.Number &&
                                                          a.NotHasSector &&
                                                          a.GroupName == "TOTAL DE LA CUENTA"
                                                         );

        if (existAccount == null) {
          helper.SummaryByAccount(summaryParentEntries, entry, entry.Account, Sector.Empty,
                                  TrialBalanceItemType.BalanceSummary);
        }
      }
      returnedEntries.AddRange(summaryParentEntries.Values.ToList());
      returnedEntries.AddRange(summaryLedgersList);
      returnedEntries = returnedEntries.OrderBy(a => a.Currency.Code)
                                       .ThenBy(a => a.Account.Number)
                                       .ToList();
      return returnedEntries;
    }

    #endregion Helper methods

  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.BalanceEngine
