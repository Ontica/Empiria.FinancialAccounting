/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Aggregate Object                *
*  Type     : FinancialConcept                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data about a financial concept, which has an arithmetic integration of other          *
*             financial concepts, financial accounting accounts or external financial values.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.FinancialConcepts.Data;
using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Contains data about a financial concept, which has an arithmetic integration of other
  /// financial concepts, financial accounting accounts or external financial values.</summary>
  public class FinancialConcept : BaseObject {

    #region Fields

    private Lazy<List<FinancialConceptEntry>> _integration;

    #endregion Fields

    #region Constructors and parsers

    protected FinancialConcept() {
      // Required by Empiria Framework.
    }


    static public FinancialConcept Parse(int id) {
      return BaseObject.ParseId<FinancialConcept>(id);
    }


    static public FinancialConcept Parse(string uid) {
      return BaseObject.ParseKey<FinancialConcept>(uid);
    }


    static internal FinancialConcept Create(FinancialConceptFields fields) {
      Assertion.Require(fields, nameof(fields));

      return new FinancialConcept {
        Group = fields.Group,
        Code = fields.Code,
        Name = fields.Name,
        Position = fields.Position,
        StartDate = fields.StartDate,
        EndDate = fields.EndDate,
        UpdatedBy = ExecutionServer.CurrentIdentity.User.AsContact()
      };
    }

    internal ExecutionResult InsertEntryFrom2(EditFinancialConceptEntryCommand command) {
      throw new NotImplementedException();
    }

    static public FinancialConcept Empty {
      get {
        return FinancialConcept.ParseEmpty<FinancialConcept>();
      }
    }

    internal ExecutionResult RemoveEntry(EditFinancialConceptEntryCommand command) {
      throw new NotImplementedException();
    }

    internal ExecutionResult UpdateEntryFrom2(EditFinancialConceptEntryCommand command) {
      throw new NotImplementedException();
    }

    protected override void OnLoad() {
      if (this.IsEmptyInstance) {
        return;
      }

      _integration = new Lazy<List<FinancialConceptEntry>>(
                        () => FinancialConceptsData.GetFinancialConceptEntries(this)
                     );
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_GRUPO")]
    public FinancialConceptGroup Group {
      get; private set;
    }


    [DataField("CLAVE_CONCEPTO")]
    public string Code {
      get; private set;
    }


    [DataField("NOMBRE_CONCEPTO")]
    public string Name {
      get; private set;
    }


    [DataField("POSICION")]
    public int Position {
      get; private set;
    }


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get; private set;
    }


    [DataField("FECHA_FIN")]
    public DateTime EndDate {
      get; private set;
    }


    [DataField("STATUS_CONCEPTO", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }


    [DataField("ID_EDITADO_POR")]
    public Contact UpdatedBy {
      get; private set;
    }


    public FixedList<FinancialConceptEntry> Integration {
      get {
        return _integration.Value
                           .ToFixedList();
      }
    }


    public int Level {
      get {
        return 1;
      }
    }

    #endregion Properties

    #region Methods

    internal void Cleanup() {
      this.Code = EmpiriaString.Clean(this.Code);
      this.Name = EmpiriaString.Clean(this.Name);
    }


    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }


    internal FinancialConceptEntry GetEntry(string financialConceptEntryUID) {
      Assertion.Require(financialConceptEntryUID, nameof(financialConceptEntryUID));

      var entry = _integration.Value.Find(x => x.UID == financialConceptEntryUID);

      Assertion.Require(entry,
        $"El concepto no contiene la regla de integración '{financialConceptEntryUID}'.");

      return entry;
    }


    internal FinancialConceptEntry InsertEntryFrom(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      int position = CalculatePositionFrom(command);

      FinancialConceptEntryFields fields = command.MapToFields(position);

      FinancialConceptEntry entry = FinancialConceptEntry.Create(fields);

      UpdateList(entry);

      return entry;
    }


    internal void RemoveEntry(FinancialConceptEntry entry) {
      Assertion.Require(entry, nameof(entry));

      Assertion.Require(entry.FinancialConcept.Equals(this),
          $"La regla de integración que se desea eliminar no pertenece al concepto '{this.Name}'.");

      entry.Delete();

      UpdateList(entry);
    }


    protected override void OnSave() {
      FinancialConceptsData.Write(this);
    }


    internal void SetPosition(int position) {
      Assertion.Require(position > 0, "Position must be greater than zero.");

      this.Position = position;
    }


    internal void Update(FinancialConceptFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.Code = fields.Code;
      this.Name = fields.Name;
      this.Position = fields.Position;
      this.StartDate = fields.StartDate;
      this.EndDate = fields.EndDate;
      this.UpdatedBy = ExecutionServer.CurrentIdentity.User.AsContact();
    }


    internal FinancialConceptEntry UpdateEntryFrom(EditFinancialConceptEntryCommand command) {
      Assertion.Require(command, nameof(command));

      FinancialConceptEntry entry = GetEntry(command.FinancialConceptEntryUID);

      int newPosition = CalculatePositionFrom(command, entry.Position);

      FinancialConceptEntryFields fields = command.MapToFields(newPosition);

      entry.Update(fields);

      UpdateList(entry);

      return entry;
    }

    #endregion Methods

    #region Helpers

    private int CalculatePositionFrom(EditFinancialConceptEntryCommand command,
                                      int currentPosition = -1) {

      switch (command.PositioningRule) {

        case PositioningRule.AfterOffset:
          var afterOffset = GetEntry(command.PositioningOffsetEntryUID);

          if (currentPosition != -1 &&
              currentPosition < afterOffset.Position) {
            return afterOffset.Position;
          } else {
            return afterOffset.Position + 1;
          }


        case PositioningRule.AtEnd:

          if (currentPosition != -1) {
            return _integration.Value.Count;
          } else {
            return _integration.Value.Count + 1;
          }

        case PositioningRule.AtStart:
          return 1;

        case PositioningRule.BeforeOffset:
          var beforeOffset = GetEntry(command.PositioningOffsetEntryUID);

          if (currentPosition != -1 &&
              currentPosition < beforeOffset.Position) {
            return beforeOffset.Position - 1;
          } else {
            return beforeOffset.Position;
          }

        case PositioningRule.ByPositionValue:
          Assertion.Require(1 <= command.Position &&
                                command.Position <= _integration.Value.Count + 1,
            $"Position value is {command.Position}, but must be between 1 and {_integration.Value.Count + 1}.");

          return command.Position;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled PositioningRule '{command.PositioningRule}'.");
      }
    }


    private void UpdateList(FinancialConceptEntry entry) {
      int listIndex = Integration.IndexOf(entry);

      if (listIndex != -1) {
        _integration.Value.RemoveAt(listIndex);
      }

      if (entry.Status != EntityStatus.Deleted) {
        _integration.Value.Insert(entry.Position - 1, entry);
      }

      for (int i = 0; i < _integration.Value.Count; i++) {
        FinancialConceptEntry item = _integration.Value[i];

        item.SetPosition(i + 1);
      }
    }


    #endregion Helpers

  } // class FinancialConcept



  /// <summary>Fields structure used to update financial concepts.</summary>
  internal class FinancialConceptFields {

    internal FinancialConceptGroup Group {
      get; set;
    }

    internal string Code {
      get; set;
    }

    internal string Name {
      get; set;
    }

    internal int Position {
      get; set;
    }

    internal DateTime StartDate {
      get; set;
    }

    internal DateTime EndDate {
      get; set;
    }

  }  // class FinancialConceptFields

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
