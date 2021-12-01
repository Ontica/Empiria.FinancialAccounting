/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanceHelper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balances.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balances.</summary>
  internal class BalanceHelper {

    private readonly BalanceCommand _command;

    internal BalanceHelper(BalanceCommand command) {
      _command = command;
    }

    internal FixedList<BalanceEntry> GetBalanceEntries() {

      TrialBalanceCommandData commandData = GetBalanceCommandMapped();

      return GetTrialBalanceMappedToBalance(commandData);

    }

    
    #region Private methods


    private TrialBalanceCommandData GetBalanceCommandMapped() {

      TrialBalanceCommand trialBalanceCommand = BalanceCommand.MapToTrialBalanceCommand(_command);

      TrialBalanceCommandData commandData = trialBalanceCommand
                                            .MapToTrialBalanceCommandData(trialBalanceCommand.InitialPeriod);

      return commandData;
    }


    private FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceCommandData commandData) {

      return TrialBalanceDataService.GetTrialBalanceEntries(commandData);

    }


    private FixedList<BalanceEntry> GetTrialBalanceMappedToBalance(TrialBalanceCommandData commandData) {

      FixedList<TrialBalanceEntry> entries = GetTrialBalanceEntries(commandData);

      return MapToBalance(entries);
    }


    private FixedList<BalanceEntry> MapToBalance(FixedList<TrialBalanceEntry> entries) {
      var mappedEntries = new List<BalanceEntry>();

      foreach (var entry in entries) {
        var balanceEntry = new BalanceEntry();
        balanceEntry.ItemType = entry.ItemType;
        balanceEntry.Ledger = entry.Ledger;
        balanceEntry.Currency = entry.Currency;
        balanceEntry.Account = entry.Account;
        balanceEntry.Sector = entry.Sector;
        balanceEntry.SubledgerAccountId = entry.SubledgerAccountId;
        balanceEntry.CurrentBalance = Math.Round(entry.CurrentBalance, 2);
        balanceEntry.LastChangeDate = entry.LastChangeDate;
        balanceEntry.DebtorCreditor = entry.DebtorCreditor;
        mappedEntries.Add(balanceEntry);
      }
      return mappedEntries.ToFixedList();
    }


    internal void SummaryBySubledgerAccount(EmpiriaHashTable<BalanceEntry> entries, 
                                    BalanceEntry entry, TrialBalanceItemType balanceType) {

      string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                    $"{entry.SubledgerAccountIdParent}||{Sector.Empty.Code}";

      GenerateOrIncreaseBalances(entries, entry, StandardAccount.Empty, Sector.Empty, balanceType, hash);
    }


    private void GenerateOrIncreaseBalances(EmpiriaHashTable<BalanceEntry> entries, 
                                            BalanceEntry entry, StandardAccount account, 
                                            Sector sector, TrialBalanceItemType balanceType, string hash) {

      BalanceEntry returnedEntry;

      entries.TryGetValue(hash, out returnedEntry);

      if (returnedEntry == null) {

        returnedEntry = new BalanceEntry {
          Ledger = entry.Ledger,
          Currency = entry.Currency,
          Sector = sector,
          Account = account,
          ItemType = balanceType,
          //GroupNumber = entry.GroupNumber,
          GroupName = entry.GroupName,
          DebtorCreditor = entry.DebtorCreditor,
          SubledgerAccountIdParent = entry.SubledgerAccountIdParent,
          LastChangeDate = entry.LastChangeDate
        };
        returnedEntry.CurrentBalance += entry.CurrentBalance;

        entries.Insert(hash, returnedEntry);

      } else {
        returnedEntry.CurrentBalance += entry.CurrentBalance;
      }
    }


    #endregion

  } // class BalanceHelper 

} // Empiria.FinancialAccounting.BalanceEngine
