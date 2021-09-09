/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Use case interactor class               *
*  Type     : RulesUseCases                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive financial accounting rules.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Rules.Adapters;

namespace Empiria.FinancialAccounting.Rules.UseCases {

  /// <summary>Use cases used to retrive financial accounting rules.</summary>
  public class GroupingRulesUseCases : UseCase {

    #region Constructors and parsers

    protected GroupingRulesUseCases() {
      // no-op
    }

    static public GroupingRulesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<GroupingRulesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<GroupingRuleDto> GroupingRules(string rulesSetUID) {
      Assertion.AssertObject(rulesSetUID, "rulesSetUID");

      var rulesSet = RulesSet.Parse(rulesSetUID);

      FixedList<GroupingRule> rules = rulesSet.GetGroupingRules();

      return GroupingRulesMapper.Map(rules);
    }

    public FixedList<NamedEntityDto> GroupingRulesSetsFor(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<RulesSet> rulesSets = RulesSet.GetList(accountsChart);

      return rulesSets.MapToNamedEntityList();
    }

    #endregion Use cases

  }  // class GroupingRulesUseCases

}  // namespace Empiria.FinancialAccounting.Rules.UseCases
