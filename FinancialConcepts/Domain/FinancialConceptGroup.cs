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

    private Lazy<FixedList<FinancialConcept>> _financialConcepts;

    private Lazy<FixedList<FinancialConceptIntegrationEntry>> _financialConceptsIntegrationEntries;

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

      _financialConcepts =
            new Lazy<FixedList<FinancialConcept>>(() => FinancialConceptsData.GetFinancialConcepts(this));

      _financialConceptsIntegrationEntries =
            new Lazy<FixedList<FinancialConceptIntegrationEntry>>(() => FinancialConceptsData.GetAllIntegrationEntriesForAGroup(this));
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
      FixedList<FinancialConcept> concepts = FinancialConcepts();

      foreach (var concept in concepts) {
        concept.Cleanup();
        FinancialConceptsData.Write(concept);
      }

      FixedList<FinancialConceptIntegrationEntry> items = FinancialConceptsData.GetAllIntegrationEntriesForAGroup(this);

      foreach (var item in items) {
        item.Cleanup();
        FinancialConceptsData.Write(item);
      }
    }


    public FixedList<FinancialConcept> FinancialConcepts() {
      return _financialConcepts.Value;
    }


    internal FixedList<FinancialConceptIntegrationEntry> FinancialConceptIntegrationEntries(FinancialConcept financialConcept) {
      Assertion.AssertObject(financialConcept, nameof(financialConcept));

      return _financialConceptsIntegrationEntries.Value.FindAll(x => x.FinancialConcept.Equals(financialConcept));
    }


    internal FixedList<FinancialConceptIntegrationEntry> RootIntegrationEntries() {
      return _financialConceptsIntegrationEntries.Value.FindAll(x => x.FinancialConcept.IsEmptyInstance);
    }


    internal GroupingRulesTree GetGroupingRulesTree() {
      return new GroupingRulesTree(this);
    }

    #endregion Methods

  } // class FinancialConceptGroup

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
