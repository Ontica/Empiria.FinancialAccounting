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
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Holds a set of financial concepts, which unique purpose is to classify concepts.
  /// A given financial concept always belongs to a single group.</summary>
  public class FinancialConceptGroup : GeneralObject {

    #region Fields

    private Lazy<List<FinancialConcept>> _financialConcepts;

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
            new Lazy<List<FinancialConcept>>(() => FinancialConceptsData.GetFinancialConcepts(this));

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


    public FixedList<FinancialConcept> FinancialConcepts {
      get {
        return _financialConcepts.Value
                                 .ToFixedList();
      }
    }


    #endregion Properties

    #region Methods

    public void Cleanup() {
      FixedList<FinancialConcept> concepts = this.FinancialConcepts;

      foreach (var concept in concepts) {
        concept.Cleanup();
        FinancialConceptsData.Write(concept);
      }

      FixedList<FinancialConceptEntry> items = FinancialConceptsData.GetAllIntegrationEntriesForAGroup(this)
                                                                    .ToFixedList();

      foreach (var item in items) {
        item.Cleanup();
        FinancialConceptsData.Write(item);
      }
    }


    private FinancialConcept GetFinancialConcept(string conceptUID) {
      Assertion.AssertObject(conceptUID, nameof(conceptUID));

      var concept = this.FinancialConcepts.Find(x => x.UID == conceptUID);

      Assertion.AssertObject(concept,
                            $"Este grupo no contiene un concepto con UID '{conceptUID}'.");

      return concept;
    }


    internal FinancialConcept InsertFrom(FinancialConceptEditionCommand command) {
      Assertion.AssertObject(command, nameof(command));

      if (IsFinancialConceptCodeRegistered(command.Code, FinancialConcept.Empty)) {
        Assertion.AssertFail($"Ya existe otro concepto con la clave '{command.Code}'.");
      }

      int position = CalculatePositionFrom(command);

      FinancialConceptFields fields = command.MapToFinancialConceptFields(position);

      FinancialConcept concept = FinancialConcept.Create(fields);

      UpdateList(concept);

      return concept;
    }


    internal void Remove(FinancialConcept concept) {
      Assertion.AssertObject(concept, nameof(concept));

      Assertion.Assert(concept.Group.Equals(this),
              $"El concepto que se desea eliminar no pertenece al grupo de conceptos '{this.Name}'.");

      throw new NotImplementedException("Remove");
    }


    internal FinancialConcept UpdateFrom(FinancialConceptEditionCommand command) {
      Assertion.AssertObject(command, nameof(command));

      FinancialConcept concept = GetFinancialConcept(command.FinancialConceptUID);

      if (IsFinancialConceptCodeRegistered(command.Code, concept)) {
        Assertion.AssertFail($"Ya existe otro concepto con la clave '{command.Code}'.");
      }

      int newPosition = CalculatePositionFrom(command);

      FinancialConceptFields fields = command.MapToFinancialConceptFields(newPosition);

      concept.Update(fields);

      UpdateList(concept);

      return concept;
    }


    #endregion Methods

    #region Helpers

    private int CalculatePositionFrom(FinancialConceptEditionCommand command) {

      switch (command.PositioningRule) {

        case PositioningRule.AfterOffset:
          var afterOffset = GetFinancialConcept(command.PositioningOffsetConceptUID);

          return afterOffset.Position + 1;

        case PositioningRule.AtEnd:
          return this.FinancialConcepts.Count + 1;

        case PositioningRule.AtStart:
          return 1;

        case PositioningRule.BeforeOffset:
          var beforeOffset = GetFinancialConcept(command.PositioningOffsetConceptUID);

          return beforeOffset.Position;

        case PositioningRule.ByPositionValue:
          Assertion.Assert(1 <= command.Position &&
                                command.Position <= this.FinancialConcepts.Count + 1,
            $"Position value is {command.Position}, but must be between 1 and {this.FinancialConcepts.Count + 1}.");

          return command.Position;

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled PositioningRule '{command.PositioningRule}'.");
      }
    }


    private bool IsFinancialConceptCodeRegistered(string code, FinancialConcept excluding) {
      return this.FinancialConcepts.Contains(x => x.Code == code && !x.Equals(excluding));
    }


    private void UpdateList(FinancialConcept concept) {
      int listIndex = FinancialConcepts.IndexOf(concept);

      if (listIndex != -1) {
        _financialConcepts.Value.RemoveAt(listIndex);
      }

      if (listIndex + 1 == concept.Position) {
        // what to do?
      }

      _financialConcepts.Value.Insert(concept.Position - 1, concept);

      for (int i = 0; i < this.FinancialConcepts.Count; i++) {
        FinancialConcept item = this.FinancialConcepts[i];

        item.SetPosition(i++);
      }
    }


    #endregion Helpers

  } // class FinancialConceptGroup

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
