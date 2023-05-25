/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Structurer                              *
*  Type     : FinancialConceptsTree                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Tree data structure with the integration entries of all financial concepts in a group.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Tree data structure with the integration entries of all financial concepts in a group.</summary>
  public class FinancialConceptsTree {

    #region Constructors and parsers

    protected internal FinancialConceptsTree(FinancialConceptGroup group) {

      Nodes = group.FinancialConcepts.Select(x => new FinancialConceptNode(x))
                                      .ToFixedList();

    }

    #endregion Constructors and parsers

    #region Properties

    internal FixedList<FinancialConceptNode> Nodes {
      get;
    }

    #endregion Properties

  }  // class FinancialConceptsTree



  /// <summary>Represents a financial concept as a tree node.</summary>
  public class FinancialConceptNode {

    internal FinancialConceptNode(FinancialConcept financialConcept) {
      FinancialConcept = financialConcept;
    }

    public FinancialConcept FinancialConcept {
      get;
    }


    public FixedList<FinancialConceptEntry> Children {
      get {
        return FinancialConcept.Integration
                               .FindAll(x => x.Type != FinancialConceptEntryType.FinancialConceptReference)
                               .ToFixedList();
      }
    }

  }  // class FinancialConceptNode


}  // namespace Empiria.FinancialAccounting.FinancialConcepts
