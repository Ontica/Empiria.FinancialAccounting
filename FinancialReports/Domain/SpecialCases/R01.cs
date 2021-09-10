/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : R01                                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generador del reporte regulatorio R01.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.Rules;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Generador del reporte regulatorio R01.</summary>
  internal class R01 {

    private readonly FinancialReportCommand _command;

    internal R01(FinancialReportCommand command) {
      _command = command;
    }

    internal FinancialReport Generate() {
      FixedList<GroupingRule> groupingRules = GetGroupingRules();

      var entries = GetEntries(groupingRules);

      return new FinancialReport(_command, entries);
    }

    private FixedList<FinancialReportEntry> GetEntries(FixedList<GroupingRule> groupingRules) {
      var entries = new FixedList<FinancialReportEntry>(groupingRules.Select(x => new FinancialReportEntry { GroupingRule = x }));

      return entries;
    }

    private FixedList<GroupingRule> GetGroupingRules() {
      RulesSet rulesSet;

      var accountsChart = AccountsChart.Parse(_command.AccountsChartUID);

      if (accountsChart.Id == 1) {
        rulesSet = RulesSet.Parse(901);
      } else if (accountsChart.Id == 152) {
        rulesSet = RulesSet.Parse(902);
      } else {
        rulesSet = RulesSet.Empty;
      }

      return rulesSet.GetGroupingRules();
    }

  }  // class R01

}  // namespace Empiria.FinancialAccounting.FinancialReports
