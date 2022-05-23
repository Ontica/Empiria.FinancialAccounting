/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Structurer                              *
*  Type     : FinancialConceptEntryAsTreeNode            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : A tree node data structure for a financial concept integration entry.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>A tree node data structure for a financial concept integration entry.</summary>
  public class FinancialConceptEntryAsTreeNode {

    #region Constructors and parsers

    private FinancialConceptEntryAsTreeNode() {
      this.IntegrationEntry = FinancialConceptIntegrationEntry.Empty;
      this.ParentNode = this;
    }


    internal FinancialConceptEntryAsTreeNode(FinancialConceptIntegrationEntry integrationEntry) {
      this.IntegrationEntry = integrationEntry;
      this.ParentNode = Empty;
    }


    internal FinancialConceptEntryAsTreeNode(FinancialConceptIntegrationEntry integrationEntry,
                                             FinancialConceptEntryAsTreeNode parentNode) {
      this.IntegrationEntry = integrationEntry;
      this.ParentNode = parentNode;
    }


    static public FinancialConceptEntryAsTreeNode Empty {
      get {
        return new FinancialConceptEntryAsTreeNode();
      }
    }


    #endregion Constructors and parsers

    #region Properties

    public FinancialConceptIntegrationEntry IntegrationEntry {
      get;
    }


    public FinancialConceptEntryAsTreeNode ParentNode {
      get;
    }


    public FixedList<FinancialConceptEntryAsTreeNode> ChildrenNodes {
      get {
        if (this.IntegrationEntry.Type != IntegrationEntryType.FinancialConceptReference) {
          return new FixedList<FinancialConceptEntryAsTreeNode>();
        }

        var referencedConcept =  this.IntegrationEntry.ReferencedFinancialConcept;

        var list = this.IntegrationEntry.Group.FinancialConceptIntegrationEntries(referencedConcept);

        return list.Select(x => new FinancialConceptEntryAsTreeNode(x, this))
                   .ToFixedList();
      }
    }


    public int Level {
      get {
        if (this.IsRoot) {
          return 1;
        } else {
          return this.ParentNode.Level + 1;
        }
      }
    }


    public bool IsRoot {
      get {
        return this.ParentNode.IntegrationEntry.IsEmptyInstance;
      }
    }


    public bool IsLeaf {
      get {
        return (this.ChildrenNodes.Count == 0);
      }
    }

    #endregion Properties

  }  // class FinancialConceptEntryAsTreeNode

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
