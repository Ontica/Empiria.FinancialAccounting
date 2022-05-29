/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Aggregate Object                *
*  Type     : FinancialConceptGroup                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of financial concepts, which unique purpose is to classify concepts.               *
*             A given financial concept always belongs to a single group.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.StateEnums;

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


    static public FinancialConceptGroup Empty {
      get {
        return FinancialConceptGroup.ParseEmpty<FinancialConceptGroup>();
      }
    }


    protected override void OnLoad() {
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

    public DateTime StartDate {
      get {
        return base.ExtendedDataField.Get("startDate", AccountsChart.MasterData.StartDate);
      }
    }


    public DateTime EndDate {
      get {
        return base.ExtendedDataField.Get("endDate", AccountsChart.MasterData.EndDate);
      }
    }


    public FixedList<string> CalculationRules {
      get {
        var rules = base.ExtendedDataField.GetFixedList<string>("calculationRules", false);

        if (rules.Count != 0) {
          return rules;
        }

        return new[] { "Default" }.ToFixedList();
      }
    }


    public FixedList<string> DataColumns {
      get {
        var columns = base.ExtendedDataField.GetFixedList<string>("dataColumns", false);

        if (columns.Count != 0) {
          return columns;
        }

        return new[] { "Default" }.ToFixedList();
      }
    }


    public FixedList<ExternalVariablesSet> ExternalVariablesSets {
      get {
        return base.ExtendedDataField.GetFixedList<ExternalVariablesSet>("externalVariablesSets", false);
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
      Assertion.Require(conceptUID, nameof(conceptUID));

      var concept = this.FinancialConcepts.Find(x => x.UID == conceptUID);

      Assertion.Require(concept, $"Este grupo no contiene un concepto con UID '{conceptUID}'.");

      return concept;
    }


    internal FinancialConcept InsertFrom(FinancialConceptEditionCommand command) {
      Assertion.Require(command, nameof(command));

      if (IsFinancialConceptCodeRegistered(command.Code, FinancialConcept.Empty)) {
        Assertion.RequireFail($"Ya existe otro concepto con la clave '{command.Code}'.");
      }

      int position = CalculatePositionFrom(command);

      FinancialConceptFields fields = command.MapToFinancialConceptFields(position);

      FinancialConcept concept = FinancialConcept.Create(fields);

      UpdateList(concept);

      return concept;
    }


    internal void Remove(FinancialConcept concept) {
      Assertion.Require(concept, nameof(concept));

      Assertion.Require(concept.Group.Equals(this),
                        $"El concepto que se desea eliminar no pertenece al grupo de conceptos '{this.Name}'.");

      concept.Delete();

      int oldCount = _financialConcepts.Value.Count;

      UpdateList(concept);

      Assertion.Ensure(_financialConcepts.Value.Count == oldCount - 1);
    }


    internal FinancialConcept UpdateFrom(FinancialConceptEditionCommand command) {
      Assertion.Require(command, nameof(command));

      FinancialConcept concept = GetFinancialConcept(command.FinancialConceptUID);

      if (IsFinancialConceptCodeRegistered(command.Code, concept)) {
        Assertion.RequireFail($"Ya existe otro concepto con la clave '{command.Code}'.");
      }

      int newPosition = CalculatePositionFrom(command, concept.Position);

      FinancialConceptFields fields = command.MapToFinancialConceptFields(newPosition);

      concept.Update(fields);

      UpdateList(concept);

      return concept;
    }


    #endregion Methods

    #region Helpers

    private int CalculatePositionFrom(FinancialConceptEditionCommand command,
                                      int currentPosition = -1) {

      switch (command.PositioningRule) {

        case PositioningRule.AfterOffset:
          var afterOffset = GetFinancialConcept(command.PositioningOffsetConceptUID);

          if (currentPosition != -1 &&
              currentPosition < afterOffset.Position) {
            return afterOffset.Position;
          } else {
            return afterOffset.Position + 1;
          }


        case PositioningRule.AtEnd:

          if (currentPosition != -1) {
            return this.FinancialConcepts.Count;
          } else {
            return this.FinancialConcepts.Count + 1;
          }

        case PositioningRule.AtStart:
          return 1;

        case PositioningRule.BeforeOffset:
          var beforeOffset = GetFinancialConcept(command.PositioningOffsetConceptUID);

          if (currentPosition != -1 &&
              currentPosition < beforeOffset.Position) {
            return beforeOffset.Position - 1;
          } else {
            return beforeOffset.Position;
          }

        case PositioningRule.ByPositionValue:
          Assertion.Require(1 <= command.Position &&
                                command.Position <= this.FinancialConcepts.Count + 1,
            $"Position value is {command.Position}, but must be between 1 and {this.FinancialConcepts.Count + 1}.");

          return command.Position;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled PositioningRule '{command.PositioningRule}'.");
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

      if (concept.Status != EntityStatus.Deleted) {
        _financialConcepts.Value.Insert(concept.Position - 1, concept);
      }

      for (int i = 0; i < this.FinancialConcepts.Count; i++) {
        FinancialConcept item = this.FinancialConcepts[i];

        item.SetPosition(i + 1);
      }
    }


    #endregion Helpers

  } // class FinancialConceptGroup

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
