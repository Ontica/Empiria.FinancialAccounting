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


    internal TrialBalance BuildTrialBalance(TrialBalanceCommand command, string[] fieldsGrouping, string filter, string having) {
      Assertion.AssertObject(command, "command");

      int balanceGroupId = DetermineBalanceGroup(command.StartDate);

      TrialBalanceCommandData commandData = new TrialBalanceCommandData();
      commandData.StartDate = command.StartDate;
      commandData.EndDate = command.EndDate;
      commandData.BalanceGroupId = balanceGroupId;
      //commandData.Grouping = command.Grouping;
      //commandData.Having = command.Having;
      commandData.Ordering = command.Ordering;

      FixedList<TrialBalanceEntry> entries = TrialBalanceDataService.GetTrialBalanceEntries(commandData, fieldsGrouping, filter, having);

      entries = RestrictLevels(command.Level, entries);

      return new TrialBalance(command, entries);
    }

    private int DetermineBalanceGroup(DateTime startDate) {
      return 1;
    }

    static private FixedList<TrialBalanceEntry> RestrictLevels(int level, FixedList<TrialBalanceEntry> entries) {
      if (level > 0) {
        return entries.FindAll(x => x.Level <= level);
      } else {
        return entries;
      }
    }

  } // class TrialBalanceEngine

} // namespace Empiria.FinancialAccounting.BalanceEngine
