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


    internal TrialBalance BuildTrialBalance() {
      TrialBalanceCommandData commandData = GetTrialBalanceCommandData();

      FixedList<TrialBalanceEntry> entries = TrialBalanceDataService.GetTrialBalanceEntries(commandData);

      FixedList<TrialBalanceAccumulatedEntry> accumulated = new FixedList<TrialBalanceAccumulatedEntry>();
      entries = RestrictLevels(entries);

      accumulated = Accumulated(entries);

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
      commandData.AccountsChart = AccountsChart.Parse(this.Command.AccountsChartUID);

      return commandData;
    }


    private FixedList<TrialBalanceEntry> RestrictLevels(FixedList<TrialBalanceEntry> entries) {
      if (Command.Level > 0) {
        return entries.FindAll(x => x.Level <= Command.Level);
      } else {
        return entries;
      }
    }


    private FixedList<TrialBalanceAccumulatedEntry> Accumulated(FixedList<TrialBalanceEntry> entries) {

      List<TrialBalanceAccumulatedEntry> accumulated = new List<TrialBalanceAccumulatedEntry>();

      List<int> currencies = new List<int>();

      currencies = entries.Select(x => x.Currency.Id).Distinct().ToList();

      foreach (var currencyId in currencies) {

        var ordering = entries.Where(a => a.Currency.Id == currencyId).OrderByDescending(a => a.Account.Number).ToList();
        
        //int level = 0;

        foreach (var entry in ordering) {

          string currentAccountNumber = entry.Account.Number;
          
          while (currentAccountNumber.Contains("-")) {
            
            string currentParent = currentAccountNumber.Substring(0, currentAccountNumber.LastIndexOf("-"));

            var currentAccumulated = accumulated.Where(a => a.AccountNumber == currentParent 
                                                        && a.SectorCode == entry.Sector.Code
                                                        && a.Currency.Id == entry.Currency.Id).FirstOrDefault();

            if (currentAccumulated != null) {

              currentAccumulated.TotalInitialBalance += entry.InitialBalance;
              currentAccumulated.TotalDebit += entry.Debit;
              currentAccumulated.TotalCredit += entry.Credit;
              currentAccumulated.TotalCurrentBalance += entry.CurrentBalance;

            } else {

              var current = accumulated.Where(a => a.AccountNumber == currentAccountNumber
                                              && a.SectorCode == entry.Sector.Code
                                              && a.Currency.Id == entry.Currency.Id).FirstOrDefault();

              accumulated.Add(new TrialBalanceAccumulatedEntry() {
                Currency = current?.Currency ?? entry.Currency,
                SectorCode = current?.SectorCode ?? entry.Sector.Code,
                AccountNumber = currentParent,
                TotalInitialBalance = current?.TotalInitialBalance ?? entry.InitialBalance,
                TotalCurrentBalance = current?.TotalCurrentBalance ?? entry.CurrentBalance,
                TotalDebit = current?.TotalDebit ?? entry.Debit,
                TotalCredit = current?.TotalCredit ?? entry.Credit
              });

            }
            currentAccountNumber = currentParent;

            if ((EmpiriaString.CountOccurences(currentAccountNumber, '-') + 1) == 1) {

              var parentAccount = accumulated.Where(a => a.AccountNumber == currentAccountNumber
                                                      && a.SectorCode == "00").FirstOrDefault();

              if (parentAccount != null) {
                parentAccount.TotalInitialBalance += entry.InitialBalance;
                parentAccount.TotalDebit += entry.Debit;
                parentAccount.TotalCredit += entry.Credit;
                parentAccount.TotalCurrentBalance += entry.CurrentBalance;
              } else {
                accumulated.Add(new TrialBalanceAccumulatedEntry() {
                  Currency = entry.Currency,
                  SectorCode = "00",
                  AccountNumber = currentAccountNumber,
                  TotalInitialBalance = entry.InitialBalance,
                  TotalCurrentBalance = entry.CurrentBalance,
                  TotalDebit = entry.Debit,
                  TotalCredit = entry.Credit
                });
              }
            }
          }
        }
      }
      FixedList<TrialBalanceAccumulatedEntry> newAccumulated = new FixedList<TrialBalanceAccumulatedEntry>(accumulated);

      return newAccumulated;
    }


    #endregion Private methods

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
