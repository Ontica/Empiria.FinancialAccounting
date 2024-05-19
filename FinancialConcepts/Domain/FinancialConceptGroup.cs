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

using Empiria.DynamicData.ExternalData;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;
using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Holds a set of financial concepts, which unique purpose is to classify concepts.
  /// A given financial concept always belongs to a single group.</summary>
  public class FinancialConceptGroup : GeneralObject, IInvariant {

    #region Fields

    private Lazy<List<FinancialConcept>> _financialConcepts;

    private readonly object _locker = new object();

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


    public string Tags {
      get {
        return base.ExtendedDataField.Get<string>("tags", string.Empty);
      }
    }

    #endregion Properties

    #region Methods

    public void Cleanup() {
      FixedList<FinancialConcept> concepts = this.FinancialConcepts;

      for (int i = 0; i < concepts.Count; i++) {
        var concept = concepts[i];

        concept.SetPosition(i + 1);
        concept.Cleanup();
        FinancialConceptsData.Write(concept);

        FixedList<FinancialConceptEntry> items = concept.Integration;

        for (int j = 0; j < items.Count; j++) {
          var item = items[j];

          item.SetPosition(j + 1);
          item.Cleanup();
          FinancialConceptsData.Write(item);
        }

      }  // for i

      AssertInvariant();
    }


    private FinancialConcept GetFinancialConcept(string conceptUID) {
      Assertion.Require(conceptUID, nameof(conceptUID));

      var concept = this.FinancialConcepts.Find(x => x.UID == conceptUID);

      Assertion.Require(concept, $"Este grupo no contiene un concepto con UID '{conceptUID}'.");

      return concept;
    }


    internal FixedList<FinancialConcept> GetFinancialConcepts(DateTime date) {
      return this.FinancialConcepts.FindAll(x => x.StartDate <= date && date <= x.EndDate);
    }


    internal FinancialConcept InsertFrom(EditFinancialConceptCommand command) {
      Assertion.Require(command, nameof(command));

      if (IsFinancialConceptCodeRegistered(command.Code, FinancialConcept.Empty,
                                           command.StartDate, command.EndDate)) {
        Assertion.RequireFail($"Ya existe otro concepto con la clave '{command.Code}' " +
                              $"dentro del rango de fechas proporcionado.");
      }

      int position = command.Positioning.CalculatePosition(this.FinancialConcepts);

      FinancialConceptFields fields = command.MapToFinancialConceptFields(position);

      FinancialConcept concept = FinancialConcept.Create(fields);

      UpdateList(concept);

      AssertInvariant();

      return concept;
    }


    internal void Remove(FinancialConcept removeThis) {
      Assertion.Require(removeThis, nameof(removeThis));

      Assertion.Require(removeThis.Group.Equals(this),
                        $"El concepto que se desea eliminar no pertenece al grupo de conceptos '{this.Name}'.");

      lock (_locker) {
        Assertion.Require(_financialConcepts.Value.Contains(removeThis),
                          $"El concepto que se desea eliminar no está en la lista.");

        int oldCount = _financialConcepts.Value.Count;

        removeThis.Delete();
        UpdateList(removeThis);

        Assertion.Ensure(_financialConcepts.Value.Count == oldCount - 1);
        Assertion.Ensure(!_financialConcepts.Value.Contains(removeThis));
      }  // lock

      AssertInvariant();
    }


    internal FinancialConcept UpdateFrom(EditFinancialConceptCommand command) {
      Assertion.Require(command, nameof(command));

      FinancialConcept concept = GetFinancialConcept(command.FinancialConceptUID);

      if (IsFinancialConceptCodeRegistered(command.Code, concept,
                                           command.StartDate, command.EndDate)) {
        Assertion.RequireFail($"Ya existe otro concepto con la clave '{command.Code}' " +
                              $"dentro del rango de fechas proporcionado.");
      }

      int newPosition = command.Positioning.CalculatePosition(this.FinancialConcepts,
                                                              concept.Position);

      FinancialConceptFields fields = command.MapToFinancialConceptFields(newPosition);

      concept.Update(fields);

      UpdateList(concept);

      AssertInvariant();

      return concept;
    }


    #endregion Methods

    #region Helpers

    private bool IsFinancialConceptCodeRegistered(string code, FinancialConcept excluding,
                                                  DateTime startDate, DateTime endDate) {
      return this.FinancialConcepts.Contains(x => x.Code == code &&
                                                  !x.Equals(excluding) &&
                                                  (x.StartDate <= startDate && startDate <= x.EndDate ||
                                                   x.StartDate <= endDate && endDate <= x.EndDate));
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


    private void AssertInvariant() {
      ((IInvariant) this).AssertInvariant();
    }


    void IInvariant.AssertInvariant() {
      var concepts = _financialConcepts.Value;

      Assertion.Ensure(concepts, nameof(concepts));

      Assertion.Ensure(_locker, nameof(_locker));

      Assertion.Ensure(!concepts.Exists(x => x == null),
                       "Concepts list cannot have null items.");

      Assertion.Ensure(!concepts.Exists(x => x.Status == EntityStatus.Deleted || x.IsEmptyInstance),
                       "Concepts list cannot have deleted or empty instance items.");

      Assertion.Ensure(concepts.TrueForAll(x => x.Group.Equals(this)),
                       $"All concepts must belong to the group ({this.Name}).");

      Assertion.Ensure(concepts.TrueForAll(x => x.Position == concepts.IndexOf(x) + 1),
                       $"For all concepts, Position property must coincide with their indexes in the list.");
    }


    #endregion Helpers

  } // class FinancialConceptGroup

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
