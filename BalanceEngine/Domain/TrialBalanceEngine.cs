/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : TrialBalanceEngine                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to retrieve a trial balance.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum TrialBalanceType {

    Traditional,

    Valued

  }


  public enum BalancesType {

    AllAccounts,

    WithCurrentBalance,

    WithCurrenBalanceOrMovements,

    WithMovements

  }


  /// <summary>Provides services to retrieve a trial balance.</summary>
  internal class TrialBalanceEngine {


    internal TrialBalanceEngine(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      this.Command = command;
    }


    public TrialBalanceCommand Command {
      get;
    }

    public AccountsChart AccountsChart {
      get {
        return AccountsChart.Parse(this.Command.AccountsChartUID);
      }
    }

    internal TrialBalance BuildTrialBalance() {
      TrialBalanceCommandData commandData = GetTrialBalanceCommandData();

      FixedList<TrialBalanceEntry> entries = TrialBalanceDataService.GetTrialBalanceEntries(commandData);

      entries = RestrictLevels(entries);

      entries = Accumulated(entries);

      return new TrialBalance(Command, entries);
    }


    #region Private methods

    private StoredBalanceSet DetermineStoredBalanceSet() {
      return StoredBalanceSet.GetBestBalanceSet(AccountsChart.Parse(this.Command.AccountsChartUID),
                                          this.Command.FromDate);
    }

    private TrialBalanceCommandData GetTrialBalanceCommandData() {
      var clausesHelper = new TrialBalanceClausesHelper(this.Command);

      var commandData = new TrialBalanceCommandData();

      commandData.StoredInitialBalanceSet = DetermineStoredBalanceSet();
      commandData.FromDate = Command.FromDate;
      commandData.ToDate = Command.ToDate;
      commandData.InitialFields = clausesHelper.GetInitialFields();
      commandData.Fields = clausesHelper.GetOutputFields();
      commandData.Filters = clausesHelper.GetFilterString();
      commandData.AccountFilters = clausesHelper.GetAccountFilterString();
      commandData.InitialGrouping = clausesHelper.GetInitialGroupingClause();
      commandData.Grouping = clausesHelper.GetGroupingClause();
      commandData.Having = clausesHelper.GetHavingClause();
      commandData.Ordering = clausesHelper.GetOrderClause();
      commandData.AccountsChart = this.AccountsChart;

      return commandData;
    }


    private FixedList<TrialBalanceEntry> RestrictLevels(FixedList<TrialBalanceEntry> entries) {
      if (Command.Level > 0) {
        return entries.FindAll(x => x.Level <= Command.Level);
      } else {
        return entries;
      }
    }


    private FixedList<TrialBalanceEntry> Accumulated(FixedList<TrialBalanceEntry> entries) {

      List<TrialBalanceEntry> Entries = new List<TrialBalanceEntry>(entries);
      List<TrialBalanceEntry> accumulated = new List<TrialBalanceEntry>();

      List<int> currencies = new List<int>();

      currencies = entries.Select(x => x.Currency.Id).Distinct().ToList();

      foreach (var currencyId in currencies) {

        var ordering = entries.Where(a => a.Currency.Id == currencyId).OrderByDescending(a => a.Account.Number).ToList();

        foreach (var entry in ordering) {
          if (entry.Account.Level == 1) {
            continue;
          }

          string currentAccountNumber = entry.Account.Number;

          while (true) {

            string currentParent = currentAccountNumber.Substring(0, currentAccountNumber.LastIndexOf("-"));

            var currentAccumulated = accumulated.Where(a => a.Account.Number == currentParent
                                                        && a.Sector.Code == entry.Sector.Code
                                                        && a.Currency.Id == entry.Currency.Id).FirstOrDefault();

            if (currentAccumulated != null) {

              currentAccumulated.InitialBalance += entry.InitialBalance;
              currentAccumulated.Debit += entry.Debit;
              currentAccumulated.Credit += entry.Credit;
              currentAccumulated.CurrentBalance += entry.CurrentBalance;

            } else {

              var current = accumulated.Where(a => a.Account.Number == currentAccountNumber
                                              && a.Sector.Code == entry.Sector.Code
                                              && a.Currency.Id == entry.Currency.Id).FirstOrDefault();

              var ledger = Ledger.Parse(current?.Ledger.Id ?? entry.Ledger.Id);
              var sector = Sector.Parse(current?.Sector.Code ?? entry.Sector.Code);
              var account = this.AccountsChart.GetAccount(currentParent);

              accumulated.Add(new TrialBalanceEntry() {
                Ledger = ledger,
                Currency = current?.Currency ?? entry.Currency,
                Sector = sector,
                Account = account,
                InitialBalance = current?.InitialBalance ?? entry.InitialBalance,
                Debit = current?.Debit ?? entry.Debit,
                Credit = current?.Credit ?? entry.Credit,
                CurrentBalance = current?.CurrentBalance ?? entry.CurrentBalance,
                ItemType = "BalanceSummaries"
              });

            }
            
            if (!currentParent.Contains("-")) {
              break;
            } else {
              currentAccountNumber = currentParent;
            }
              //if ((EmpiriaString.CountOccurences(currentAccountNumber, '-') + 1) == 1) {

              //  var parentAccount = accumulated.Where(a => a.Account.Number == currentAccountNumber
              //                                          && a.Sector.Code == "00"
              //                                          && a.Currency.Id == currencyId).FirstOrDefault();

              //  var ledger = Ledger.Parse(parentAccount?.Ledger.Id ?? -1);
              //  var sector = Sector.Parse("00");
              //  var account = this.AccountsChart.GetAccount(currentAccountNumber);

              //  if (parentAccount != null) {
              //    parentAccount.InitialBalance += entry.InitialBalance;
              //    parentAccount.Debit += entry.Debit;
              //    parentAccount.Credit += entry.Credit;
              //    parentAccount.CurrentBalance += entry.CurrentBalance;
              //  } else {
              //    accumulated.Add(new TrialBalanceEntry() {
              //      Ledger = ledger,
              //      Currency = entry.Currency,
              //      Sector = sector,
              //      Account = account,
              //      InitialBalance = entry.InitialBalance,
              //      Debit = entry.Debit,
              //      Credit = entry.Credit,
              //      CurrentBalance = entry.CurrentBalance,
              //      ItemType = "BalanceSummary"
              //    });
              //  }

              //}


            
          }
        }
      }

      foreach (var item in accumulated) {
        Entries.Add(item);
      }

      Entries = Entries.OrderBy(a => a.Currency.Id).ToList();

      FixedList<TrialBalanceEntry> newEntries = new FixedList<TrialBalanceEntry>(Entries);

      return newEntries;
    }


    #endregion Private methods

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
