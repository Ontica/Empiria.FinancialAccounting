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
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public enum TrialBalanceType {

    Traditional,

    SubsidiaryAccounts

  }

  public enum BalancesType {

    AllAccounts,

    WithCurrentBalance,

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

      TrialBalanceClausesHelper clausesHelper = new TrialBalanceClausesHelper(this.Command);

      TrialBalanceCommandData commandData = new TrialBalanceCommandData();

      commandData.FromDate = Command.FromDate;
      commandData.ToDate = Command.ToDate;
      commandData.InitialBalanceGroupId = DetermineBalanceGroup();
      commandData.Fields = clausesHelper.GetOutputFields();
      commandData.Filters = clausesHelper.GetFilterString();
      commandData.Grouping = clausesHelper.GetGroupingClause();
      commandData.Having = clausesHelper.GetHavingClause();
      commandData.Ordering = clausesHelper.GetOrderClause();


      FixedList<TrialBalanceEntry> entries = TrialBalanceDataService.GetTrialBalanceEntries(commandData);

      entries = RestrictLevels(entries);

      return new TrialBalance(Command, entries);
    }


    private int DetermineBalanceGroup() {
      // Command.FromDate
      return 1;
    }

    private FixedList<TrialBalanceEntry> RestrictLevels(FixedList<TrialBalanceEntry> entries) {
      if (Command.Level > 0) {
        return entries.FindAll(x => x.Level <= Command.Level);
      } else {
        return entries;
      }
    }


  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
