/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Structurer                              *
*  Type     : FinancialConceptsEntriesTree               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Tree data structure with the integration entries of all financial concepts in a group.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Tree data structure with the integration entries of all financial concepts in a group.</summary>
  public class FinancialConceptsEntriesTree {

    private readonly FinancialConceptGroup _group;

    private readonly FixedList<FinancialConceptEntry> _financialConceptsEntries;

    #region Constructors and parsers

    protected internal FinancialConceptsEntriesTree(FinancialConceptGroup group) {
      _group = group;

      _financialConceptsEntries = FinancialConceptsData.GetAllIntegrationEntriesForAGroup(_group);
    }

    #endregion Constructors and parsers

    #region Properties

    public FinancialConceptGroup Group {
      get {
        return _group;
      }
    }


    public FixedList<FinancialConceptEntryAsTreeNode> Roots {
      get {
        return new FixedList<FinancialConceptEntryAsTreeNode>(this.GetRoots()
                                                              .Select(root => new FinancialConceptEntryAsTreeNode(root)));
      }
    }


    internal FixedList<FinancialConceptEntryAsTreeNode> GetNodes() {
      var list = new List<FinancialConceptEntryAsTreeNode>();

      foreach (var root in this.Roots) {
        list.AddRange(GetNode(root));
      }

      return list.ToFixedList();
    }


    private FixedList<FinancialConceptEntryAsTreeNode> GetNode(FinancialConceptEntryAsTreeNode parent) {
      var list = new List<FinancialConceptEntryAsTreeNode>();

      list.Add(parent);

      foreach (var child in parent.ChildrenNodes) {
        list.AddRange(GetNode(child));
      }

      return list.ToFixedList();
    }

    internal FixedList<FinancialConceptEntry> GetRoots() {
      return _financialConceptsEntries.FindAll(x => x.FinancialConcept.IsEmptyInstance)
                                                     .ToFixedList();
    }



    #endregion Properties

  }  // class FinancialConceptsEntriesTree

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
