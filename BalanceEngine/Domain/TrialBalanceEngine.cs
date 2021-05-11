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

  /// <summary>Provides services to retrieve a trial balance.</summary>
  internal class TrialBalanceEngine {

    internal TrialBalanceEngine() {
      //no-op
    }


    internal TrialBalance BuildTrialBalance(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      int balanceGroupId = DetermineBalanceGroup(command.InitialDate);

      TrialBalanceCommandData commandData = new TrialBalanceCommandData();
      commandData.InitialDate = command.InitialDate;
      commandData.FinalDate = command.FinalDate;
      commandData.BalanceGroupId = balanceGroupId;
      commandData.Fields = command.Fields;
      commandData.Condition = command.Condition;
      commandData.Grouping = command.Grouping;
      commandData.Having = command.Having;
      commandData.Ordering = command.Ordering;

      FixedList<TrialBalanceEntry> entries = TrialBalanceDataService.GetTrialBalanceEntries(commandData);

      return new TrialBalance(command, entries);
    }

    private int DetermineBalanceGroup(DateTime initialDate) {
      return 1;
    }
      
  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
