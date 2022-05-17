/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : RulesUseCases                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive financial accounting rules.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

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


    public void CleanupRules(string rulesSetUID) {
      Assertion.AssertObject(rulesSetUID, "rulesSetUID");

      var rulesSet = RulesSet.Parse(rulesSetUID);

      rulesSet.Cleanup();
    }


    public FixedList<GroupingRuleDto> GroupingRules(string rulesSetUID) {
      Assertion.AssertObject(rulesSetUID, "rulesSetUID");

      var rulesSet = RulesSet.Parse(rulesSetUID);

      FixedList<GroupingRule> rules = rulesSet.GetGroupingRules();

      return GroupingRulesMapper.Map(rules);
    }


    public FixedList<GroupingRuleItemDto> GroupingRuleItems(string groupingRuleUID) {
      Assertion.AssertObject(groupingRuleUID, "groupingRuleUID");

      var groupingRule = GroupingRule.Parse(groupingRuleUID);

      return GroupingRulesMapper.Map(groupingRule.Items);
    }


    public FixedList<NamedEntityDto> GroupingRulesSetsFor(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<RulesSet> rulesSets = RulesSet.GetList(accountsChart);

      return rulesSets.MapToNamedEntityList();
    }


    public FixedList<GroupingRulesTreeItemDto> GroupingRulesFlatTree(string rulesSetUID) {
      Assertion.AssertObject(rulesSetUID, "rulesSetUID");

      var rulesSet = RulesSet.Parse(rulesSetUID);

      GroupingRulesTree rulesTree = rulesSet.GetGroupingRulesTree();

      return GroupingRulesTreeMapper.MapFlat(rulesTree.GetItemsList());
    }


    #endregion Use cases

  }  // class GroupingRulesUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
