/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Structurer                              *
*  Type     : GroupingRulesTree                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Tree data structure for financial accounting grouping rules.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Tree data structure for financial accounting grouping rules.</summary>
  public class GroupingRulesTree {

    private readonly FinancialConceptGroup _group;

    #region Constructors and parsers

    protected internal GroupingRulesTree(FinancialConceptGroup group) {
      _group = group;
    }

    #endregion Constructors and parsers

    #region Properties

    public FinancialConceptGroup Group {
      get {
        return _group;
      }
    }


    public FixedList<GroupingRulesTreeItem> Roots {
      get {
        return new FixedList<GroupingRulesTreeItem>(_group.RootIntegrationEntries()
                                                          .Select(y => new GroupingRulesTreeItem(y)));
      }
    }


    internal FixedList<GroupingRulesTreeItem> GetItemsList() {
      var list = new List<GroupingRulesTreeItem>();

      foreach (var root in this.Roots) {
        list.AddRange(GetItemsList(root));
      }
      return list.ToFixedList();
    }


    private FixedList<GroupingRulesTreeItem> GetItemsList(GroupingRulesTreeItem item) {
      var list = new List<GroupingRulesTreeItem>();

      list.Add(item);
      foreach (var child in item.Children) {
        list.AddRange(GetItemsList(child));
      }
      return list.ToFixedList();
    }


    #endregion Properties

  }  // class GroupingRulesTree

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
