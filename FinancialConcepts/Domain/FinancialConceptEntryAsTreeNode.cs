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
      this.IntegrationEntry = FinancialConceptEntry.Empty;
      this.ParentNode = this;
    }


    internal FinancialConceptEntryAsTreeNode(FinancialConceptEntry integrationEntry) {
      this.IntegrationEntry = integrationEntry;
      this.ParentNode = Empty;
    }


    internal FinancialConceptEntryAsTreeNode(FinancialConceptEntry integrationEntry,
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

    public FinancialConceptEntry IntegrationEntry {
      get;
    }


    public FinancialConceptEntryAsTreeNode ParentNode {
      get;
    }


    public FixedList<FinancialConceptEntryAsTreeNode> ChildrenNodes {
      get {
        if (this.IntegrationEntry.Type != FinancialConceptEntryType.FinancialConceptReference) {
          return new FixedList<FinancialConceptEntryAsTreeNode>();
        }

        FinancialConcept referencedConcept = this.IntegrationEntry.ReferencedFinancialConcept;

        return referencedConcept.Integration.Select(x => new FinancialConceptEntryAsTreeNode(x, this))
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
