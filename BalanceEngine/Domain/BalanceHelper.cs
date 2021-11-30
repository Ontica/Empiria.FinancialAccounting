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


    internal FixedList<BalanceEntry> GetOrderingBalance(FixedList<BalanceEntry> balance) {
      var orderingBalance = balance.OrderBy(a => a.Ledger.Number)
                                   .ThenBy(a => a.Currency.Code)
                                   .ThenBy(a => a.Account.Number)
                                   .ThenBy(a => a.Sector.Code).ToList();

      return orderingBalance.ToFixedList();
    }


    private FixedList<TrialBalanceEntry> GetTrialBalanceEntries(TrialBalanceCommandData commandData) {

      return TrialBalanceDataService.GetTrialBalanceEntries(commandData);

    }


    private FixedList<BalanceEntry> GetTrialBalanceMappedToBalance(TrialBalanceCommandData commandData) {

      FixedList<TrialBalanceEntry> entries = GetTrialBalanceEntries(commandData);

      return Map(entries);
    }


    private FixedList<BalanceEntry> Map(FixedList<TrialBalanceEntry> entries) {

      var mappedEntries = entries.Select((x) => MapToBalance((TrialBalanceEntry) x));
      return (FixedList<BalanceEntry>) mappedEntries;
    }


    static private BalanceEntry MapToBalance(TrialBalanceEntry entry) {

      var balanceEntry = new BalanceEntry();

      balanceEntry.Ledger = entry.Ledger;
      balanceEntry.Currency = entry.Currency;
      balanceEntry.Account = entry.Account;
      balanceEntry.Sector = entry.Sector;
      balanceEntry.SubledgerAccountId = entry.SubledgerAccountId;
      balanceEntry.CurrentBalance = Math.Round(entry.CurrentBalance, 2);
      balanceEntry.LastChangeDate = entry.LastChangeDate;
      balanceEntry.DebtorCreditor = entry.DebtorCreditor;
      
      return balanceEntry;
    }


    #endregion

  } // class BalanceHelper 

} // Empiria.FinancialAccounting.BalanceEngine
