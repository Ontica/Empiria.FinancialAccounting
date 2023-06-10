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
using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Contains data about a financial concept, which has an arithmetic integration of other
  /// financial concepts, financial accounting accounts or external financial values.</summary>
  public class FinancialConcept : BaseObject, IPositionable {

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


    static public FinancialConcept ParseWithVariableID(string variableID) {
      Assertion.Require(variableID, nameof(variableID));

      var concept = BaseObject.TryParse<FinancialConcept>($"ID_Variable = '{variableID}'");

      Assertion.Require(concept, $"A financial concept with variable ID = '{variableID}' was not found.");

      return concept;
    }


    static internal FinancialConcept Create(FinancialConceptFields fields) {
      Assertion.Require(fields, nameof(fields));

      return new FinancialConcept {
        Group = fields.Group,
        Code = fields.Code,
        Name = fields.Name,
        VariableID = fields.VariableID,
        CalculationScript = fields.CalculationScript,
        Position = fields.Position,
        StartDate = fields.StartDate,
        EndDate = fields.EndDate,
        UpdatedBy = ExecutionServer.CurrentContact
      };
    }


    static public FinancialConcept Empty {
      get {
        return FinancialConcept.ParseEmpty<FinancialConcept>();
      }
    }


    protected override void OnLoad() {
      if (this.IsEmptyInstance) {
        return;
      }

      _integration =
            new Lazy<List<FinancialConceptEntry>>(() => FinancialConceptsData.GetFinancialConceptEntries(this));
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


    [DataField("ID_VARIABLE")]
    public string VariableID {
      get; private set;
    }


    [DataField("SCRIPT_CALCULO")]
    public string CalculationScript {
      get; private set;
    }


    public bool HasScript {
      get {
        return CalculationScript.Length != 0;
      }
    }


    public bool SkipInnerCalculation {
      get {
        return ExtendedDataField.Get<bool>("scriptConfig/skipInnerCalculation", false);
      }
    }


    [DataField("CONCEPTO_EXT_DATA")]
    internal protected JsonObject ExtendedDataField {
      get; set;
    } = JsonObject.Empty;


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(Code, Name, VariableID, Group.Code, Group.Name);
      }
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
        return _integration.Value.ToFixedList();
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
      this.VariableID = EmpiriaString.Clean(this.VariableID);
    }


    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }


    internal FinancialConceptEntry GetEntry(string financialConceptEntryUID) {
      Assertion.Require(financialConceptEntryUID, nameof(financialConceptEntryUID));

      var entry = _integration.Value.Find(x => x.UID == financialConceptEntryUID);

      Assertion.Require(entry,
        $"El concepto {Name} no contiene la regla de integración '{financialConceptEntryUID}'.");

      return entry;
    }


    internal FinancialConceptEntry InsertEntry(FinancialConceptEntryFields fields,
                                               Positioning positioning) {
      Assertion.Require(fields,      nameof(fields));
      Assertion.Require(positioning, nameof(positioning));

      AssertThereAreNotCycles(fields);

      FinancialConceptEntry entry = FinancialConceptEntry.Create(fields);

      int position = positioning.CalculatePosition(_integration.Value);

      UpdateList(entry, position);

      return entry;
    }


    internal void RemoveEntry(FinancialConceptEntry entry) {
      Assertion.Require(entry, nameof(entry));

      Assertion.Require(entry.FinancialConcept.Equals(this),
          $"La regla de integración que se desea eliminar no pertenece al concepto '{Name}'.");

      entry.Delete();

      UpdateList(entry, entry.Position);
    }


    protected override void OnSave() {
      FinancialConceptsData.Write(this);
    }


    internal void SetPosition(int position) {
      Assertion.Require(position > 0, "Position must be greater than zero.");

      Position = position;
    }


    internal void Update(FinancialConceptFields fields) {
      Assertion.Require(fields, nameof(fields));

      Code = fields.Code;
      Name = fields.Name;
      VariableID = fields.VariableID;
      CalculationScript = fields.CalculationScript;
      Position = fields.Position;
      StartDate = fields.StartDate;
      EndDate = fields.EndDate;
      UpdatedBy = ExecutionServer.CurrentContact;
    }


    internal FinancialConceptEntry UpdateEntry(FinancialConceptEntry entry,
                                               FinancialConceptEntryFields fields,
                                               Positioning positioning) {
      Assertion.Require(fields, nameof(fields));
      Assertion.Require(positioning, nameof(positioning));

      AssertThereAreNotCycles(fields);

      entry.Update(fields);

      int newPosition = positioning.CalculatePosition(_integration.Value,
                                                       entry.Position);

      UpdateList(entry, newPosition);

      return entry;
    }


    #endregion Methods

    #region Helpers

    private void AssertThereAreNotCycles(FinancialConceptEntryFields fields) {
      if (!(fields is FinancialConceptReferenceEntryTypeFields referenceEntryFields)) {
        return;
      }

      var referencedConcept = referenceEntryFields.ReferencedFinancialConcept;

      if (referencedConcept.Equals(this)) {
        Assertion.RequireFail($"No se puede agregar el concepto '{this.Code}'" +
                              $"como parte de su propia integración.");
      }

      FinancialConceptEntry cycledEntry = TryFindCycledIntegrationConceptEntry(referencedConcept);

      if (cycledEntry != null) {
        Assertion.RequireFail(
              $"No se puede agregar el concepto '{referencedConcept.Code}' a la integración " +
              $"de este concepto ('{this.Code}'), ya que este a su vez está integrado en el " +
              $"concepto '{cycledEntry.FinancialConcept.Code}' el cual desciende de '{this.Code}'. " +
              $"La operación no se puede realizar porque se generaría un ciclo infinito.");
      }
    }


    private FinancialConceptEntry TryFindCycledIntegrationConceptEntry(FinancialConcept referencedConcept) {

      var integration = referencedConcept.Integration.FindAll(x => x.Type == FinancialConceptEntryType.FinancialConceptReference);

      foreach (var innerEntry in integration) {

        if (innerEntry.ReferencedFinancialConcept.Equals(this)) {
          return innerEntry;
        }

        var cycledEntry = TryFindCycledIntegrationConceptEntry(innerEntry.ReferencedFinancialConcept);

        if (cycledEntry != null) {
          return cycledEntry;
        }
      }

      return null;
    }


    private void UpdateList(FinancialConceptEntry entry, int position) {
      int listIndex = Integration.IndexOf(entry);

      if (listIndex != -1) {
        _integration.Value.RemoveAt(listIndex);
      }

      entry.SetPosition(position);

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

    public string CalculationScript {
      get; set;
    }

    public string VariableID {
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
