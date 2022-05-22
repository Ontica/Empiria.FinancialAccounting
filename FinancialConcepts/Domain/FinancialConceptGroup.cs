/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Data Object                     *
*  Type     : FinancialConceptGroup                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of financial concepts, which unique purpose is to classify concepts.               *
*             A given financial concept always belongs to a single group.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Holds a set of financial concepts, which unique purpose is to classify concepts.
  /// A given financial concept always belongs to a single group.</summary>
  public class FinancialConceptGroup : GeneralObject {

    #region Fields

    private Lazy<FixedList<GroupingRule>> _groupingRules;

    private Lazy<FixedList<GroupingRuleItem>> _groupingRulesItems;

    #endregion Fields

    #region Constructors and parsers

    protected FinancialConceptGroup() {
      // Required by Empiria Framework.
    }

    static public FinancialConceptGroup Parse(int id) {
      return BaseObject.ParseId<FinancialConceptGroup>(id);
    }


    static public FinancialConceptGroup Parse(string uid) {
      return BaseObject.ParseKey<FinancialConceptGroup>(uid);
    }


    static public FixedList<FinancialConceptGroup> GetList() {
      return BaseObject.GetList<FinancialConceptGroup>(String.Empty, "ObjectName")
                       .ToFixedList();
    }


    static public FixedList<FinancialConceptGroup> GetList(AccountsChart accountsChart) {
      var list = GetList();

      return list.FindAll(x => x.AccountsChart.Equals(accountsChart));
    }


    static public FinancialConceptGroup Empty {
      get {
        return FinancialConceptGroup.ParseEmpty<FinancialConceptGroup>();
      }
    }


    protected override void OnLoad() {
      base.OnLoad();

      if (this.IsEmptyInstance) {
        return;
      }

      _groupingRules = new Lazy<FixedList<GroupingRule>>(() => GroupingRulesData.GetGroupingRules(this));
      _groupingRulesItems = new Lazy<FixedList<GroupingRuleItem>>(() => GroupingRulesData.GetGroupingRulesItems(this));
    }


    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get {
        return base.ExtendedDataField.Get("accountsChartId", AccountsChart.Empty);
      }
    }


    public string Code {
      get {
        return base.ExtendedDataField.Get<string>("code");
      }
    }

    #endregion Properties

    #region Methods

    public void Cleanup() {
      FixedList<GroupingRule> rules = GetGroupingRules();

      foreach (var rule in rules) {
        rule.Cleanup();
        GroupingRulesData.Write(rule);
      }

      FixedList<GroupingRuleItem>  items = GroupingRulesData.GetGroupingRulesItems(this);

      foreach (var item in items) {
        item.Cleanup();
        GroupingRulesData.Write(item);
      }
    }


    public FixedList<GroupingRule> GetGroupingRules() {
      return _groupingRules.Value;
    }


    internal FixedList<GroupingRuleItem> GetGroupingRuleItems(GroupingRule groupingRule) {
      Assertion.AssertObject(groupingRule, "groupingRule");

      return _groupingRulesItems.Value.FindAll(x => x.GroupingRule.Equals(groupingRule));
    }


    internal FixedList<GroupingRuleItem> GetGroupingRulesRoots() {
      return _groupingRulesItems.Value.FindAll(x => x.GroupingRule.IsEmptyInstance);
    }


    internal GroupingRulesTree GetGroupingRulesTree() {
      return new GroupingRulesTree(this);
    }

    #endregion Methods

  } // class FinancialConceptGroup

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
