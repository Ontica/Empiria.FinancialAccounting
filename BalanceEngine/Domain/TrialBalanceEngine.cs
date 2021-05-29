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

      entries = RestrictLevels(entries);

      return new TrialBalance(Command, entries);
    }


    #region Private methods

    private StoredBalanceSet DetermineStoredBalanceSet() {
      return StoredBalanceSet.GetBestSet(StoredBalanceSetType.AccountBalances,
                                         AccountsChart.Parse(this.Command.AccountsChartUID),
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

    #endregion Private methods

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
