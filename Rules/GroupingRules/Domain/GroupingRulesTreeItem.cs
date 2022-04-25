/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Structurer                              *
*  Type     : GroupingRulesTreeItem                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : A tree item data structure for financial accounting grouping rules.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Rules {

  /// <summary>A tree item data structure for financial accounting grouping rules.</summary>
  public class GroupingRulesTreeItem {

    #region Constructors and parsers

    private GroupingRulesTreeItem() {
      this.GroupingRuleItem = GroupingRuleItem.Empty;
      this.Parent = this;
    }


    internal GroupingRulesTreeItem(GroupingRuleItem groupingRuleItem) {
      this.GroupingRuleItem = groupingRuleItem;
      this.Parent = Empty;
    }


    internal GroupingRulesTreeItem(GroupingRuleItem groupingRuleItem, GroupingRulesTreeItem parent) {
      this.GroupingRuleItem = groupingRuleItem;
      this.Parent = parent;
    }


    static public GroupingRulesTreeItem Empty {
      get {
        return new GroupingRulesTreeItem();
      }
    }


    #endregion Constructors and parsers

    #region Properties

    public GroupingRuleItem GroupingRuleItem {
      get;
    }


    public GroupingRulesTreeItem Parent {
      get;
    }


    public FixedList<GroupingRulesTreeItem> Children {
      get {
        if (this.GroupingRuleItem.Type == GroupingRuleItemType.Agrupation) {
          var list = this.GroupingRuleItem.RulesSet.GetGroupingRuleItems(this.GroupingRuleItem.Reference);

          return new FixedList<GroupingRulesTreeItem>(list.Select(x => new GroupingRulesTreeItem(x, this)));
        }

        return new FixedList<GroupingRulesTreeItem>();
      }
    }


    public int Level {
      get {
        if (this.IsRoot) {
          return 1;
        } else {
          return this.Parent.Level + 1;
        }
      }
    }


    public bool IsRoot {
      get {
        return this.Parent.GroupingRuleItem.IsEmptyInstance;
      }
    }


    public bool IsLeaf {
      get {
        return (this.Children.Count == 0);
      }
    }

    #endregion Properties

  }  // class GroupingRulesTreeItem

}  // namespace Empiria.FinancialAccounting.Rules
