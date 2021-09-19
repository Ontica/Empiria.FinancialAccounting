/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Empiria Data Object                     *
*  Type     : RulesSet                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of financial accounting rules.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Rules.Data;

namespace Empiria.FinancialAccounting.Rules {

  /// <summary>Holds a set of financial accounting rules.</summary>
  public class RulesSet : GeneralObject {

    #region Fields

    private FixedList<GroupingRule> groupingRules;
    private FixedList<GroupingRuleItem> groupingRulesItems;

    #endregion Fields

    #region Constructors and parsers

    protected RulesSet() {
      // Required by Empiria Framework.
    }

    static public RulesSet Parse(int id) {
      return BaseObject.ParseId<RulesSet>(id);
    }


    static public RulesSet Parse(string uid) {
      return BaseObject.ParseKey<RulesSet>(uid);
    }


    static public FixedList<RulesSet> GetList() {
      return BaseObject.GetList<RulesSet>()
                       .ToFixedList();
    }

    static public FixedList<RulesSet> GetList(AccountsChart accountsChart) {
      var list = GetList();

      return list.FindAll(x => x.AccountsChart.Equals(accountsChart));
    }

    static public RulesSet Empty {
      get {
        return RulesSet.ParseEmpty<RulesSet>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public AccountsChart AccountsChart {
      get {
        return base.ExtendedDataField.Get<AccountsChart>("accountsChartId");
      }
    }


    public string Code {
      get {
        return base.ExtendedDataField.Get<string>("code");
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<GroupingRule> GetGroupingRules() {
      if (this.groupingRules == null) {
        this.groupingRules = GroupingRulesData.GetGroupingRules(this);
      }
      return this.groupingRules;
    }


    internal FixedList<GroupingRuleItem> GetGroupingRuleItems(GroupingRule groupingRule) {
      if (this.groupingRulesItems == null) {
        this.groupingRulesItems = GroupingRulesData.GetGroupingRulesItems(this);
      }

      return this.groupingRulesItems.FindAll(x => x.GroupingRule.Equals(groupingRule));
    }


    #endregion Methods

  } // class RulesSet

}  // namespace Empiria.FinancialAccounting.Rules
