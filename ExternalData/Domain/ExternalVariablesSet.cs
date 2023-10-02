/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Empiria Aggregate Object                *
*  Type     : ExternalVariablesSet                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of external variables, which the purpose of classify them.                         *
*             A given external variable always belongs to a single set.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Storage;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.ExternalData.Adapters;
using Empiria.FinancialAccounting.ExternalData.Data;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds a set of external variables, which the purpose of classify them.
  /// A given external variable always belongs to a single set.</summary>
  public class ExternalVariablesSet : DatasetFamily {

    #region Fields

    private Lazy<List<ExternalVariable>> _externalVariables;

    #endregion Fields

    #region Constructors and parsers

    protected ExternalVariablesSet() {
      // Required by Empiria Framework.
    }


    static public new ExternalVariablesSet Parse(int id) {
      return BaseObject.ParseId<ExternalVariablesSet>(id);
    }


    static public new ExternalVariablesSet Parse(string uid) {
      return BaseObject.ParseKey<ExternalVariablesSet>(uid);
    }


    static public FixedList<ExternalVariablesSet> GetList() {
      return BaseObject.GetList<ExternalVariablesSet>(String.Empty, "ObjectName")
                       .ToFixedList();
    }


    static public ExternalVariablesSet Empty {
      get {
        return BaseObject.ParseEmpty<ExternalVariablesSet>();
      }
    }


    protected override void OnLoad() {
      if (this.IsEmptyInstance) {
        return;
      }

      _externalVariables =
            new Lazy<List<ExternalVariable>>(() => ExternalVariablesData.GetExternalVariables(this));

    }

    #endregion Constructors and parsers

    #region Properties

    public FixedList<DataTableColumn> DataColumns {
      get {
        return base.ExtendedDataField.GetFixedList<DataTableColumn>("dataColumns");
      }
      private set {
        base.ExtendedDataField.Set("dataColumns", value);
      }
    }


    public FixedList<ExportTo> ExportTo {
      get {
        return base.ExtendedDataField.GetFixedList<ExportTo>("exportTo");
      }
      private set {
        base.ExtendedDataField.Set("exportTo", value);
      }
    }


    public FixedList<ExternalVariable> ExternalVariables {
      get {
        return _externalVariables.Value.ToFixedList();
      }
    }

    #endregion Properties

    #region Methods

    internal ExternalVariable AddVariable(ExternalVariableFields fields) {
      Cleanup(fields);
      EnsureCanAdd(fields);

      var variable = new ExternalVariable(this, fields);

      _externalVariables.Value.Add(variable);

      return variable;
    }


    internal ExternalVariable GetVariable(string variableUID) {
      Assertion.Require(variableUID, nameof(variableUID));

      ExternalVariable variable = _externalVariables.Value.Find(x => x.UID == variableUID);

      Assertion.Require(variableUID,
                $"A variable with uid {variableUID} not found or does not belong to this set.");

      return variable;
    }


    public FixedList<ExternalVariable> GetVariables(DateTime date) {
      return this.ExternalVariables.FindAll(x => x.StartDate <= date && date <= x.EndDate);
    }


    internal void DeleteVariable(ExternalVariable variable) {
      Assertion.Require(variable, nameof(variable));
      Assertion.Require(variable.Set.Equals(this), $"Variable set mismatch.");

      EnsureCanDelete(variable);

      variable.Delete();

      _externalVariables.Value.Remove(variable);
    }


    public ExternalVariable TryGetVariable(string code) {
      return _externalVariables.Value.Find(x => x.Code == code);
    }


    internal void UpdateVariable(ExternalVariable variable,
                                 ExternalVariableFields fields) {
      Assertion.Require(variable, nameof(variable));
      Assertion.Require(variable.Set.Equals(this), $"Variable set mismatch.");

      Cleanup(fields);
      EnsureCanUpdate(variable, fields);

      variable.Update(fields);
    }

    #endregion Methods

    #region Helpers

    private void Cleanup(ExternalVariableFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.Code = EmpiriaString.TrimAll(fields.Code);
      fields.Name = EmpiriaString.TrimAll(fields.Name);
    }


    private void EnsureCanAdd(ExternalVariableFields fields) {
      RequireFieldsAreValid(fields);

      Assertion.Require(TryGetVariable(fields.Code) == null,
                    "Ya existe otra variable con esa clave o código.");
    }


    private void EnsureCanDelete(ExternalVariable variable) {
      FixedList<ExternalValue> values = ExternalValuesData.GetValues(variable);

      Assertion.Require(values.Count == 0,
                    "La variable ya está en uso por lo que no puede ser eliminada.");
    }


    private void EnsureCanUpdate(ExternalVariable variable,
                                 ExternalVariableFields fields) {
      RequireFieldsAreValid(fields);

      if (variable.Code != fields.Code) {

        Assertion.Require(TryGetVariable(fields.Code) == null,
                      "Ya existe otra variable con ese código.");
      }
    }


    private void RequireFieldsAreValid(ExternalVariableFields fields) {
      Assertion.Require(fields.Code,
                     "No se proporcionó la clave o código de la variable.");
      Assertion.Require(fields.Name,
                    "No se proporcionó el nombre de la variable.");
      Assertion.Require(fields.StartDate,
                    "No se proporcionó la fecha de inicio.");
      Assertion.Require(fields.EndDate,
                    "No se proporcionó la fecha de finalización.");
      Assertion.Require(fields.StartDate <= fields.EndDate,
                    "La fecha de finalización debe ser mayor o igual que la fecha de inicio.");
    }

    #endregion Helpers

  } // class ExternalVariablesSet

}  // namespace Empiria.FinancialAccounting.ExternalData
