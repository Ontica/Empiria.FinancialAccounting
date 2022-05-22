/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Structurer                              *
*  Type     : GroupingRulesTreeItem                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : A tree item data structure for financial accounting grouping rules.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>A tree item data structure for financial accounting grouping rules.</summary>
  public class GroupingRulesTreeItem {

    #region Constructors and parsers

    private GroupingRulesTreeItem() {
      this.IntegrationEntry = FinancialConceptIntegrationEntry.Empty;
      this.Parent = this;
    }


    internal GroupingRulesTreeItem(FinancialConceptIntegrationEntry integrationEntry) {
      this.IntegrationEntry = integrationEntry;
      this.Parent = Empty;
    }


    internal GroupingRulesTreeItem(FinancialConceptIntegrationEntry integrationEntry,
                                   GroupingRulesTreeItem parent) {
      this.IntegrationEntry = integrationEntry;
      this.Parent = parent;
    }


    static public GroupingRulesTreeItem Empty {
      get {
        return new GroupingRulesTreeItem();
      }
    }


    #endregion Constructors and parsers

    #region Properties

    public FinancialConceptIntegrationEntry IntegrationEntry {
      get;
    }


    public GroupingRulesTreeItem Parent {
      get;
    }


    public FixedList<GroupingRulesTreeItem> Children {
      get {
        if (this.IntegrationEntry.Type == IntegrationEntryType.FinancialConceptReference) {
          var list = this.IntegrationEntry.Group.FinancialConceptIntegrationEntries(this.IntegrationEntry.ReferencedFinancialConcept);

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
        return this.Parent.IntegrationEntry.IsEmptyInstance;
      }
    }


    public bool IsLeaf {
      get {
        return (this.Children.Count == 0);
      }
    }

    #endregion Properties

  }  // class GroupingRulesTreeItem

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
