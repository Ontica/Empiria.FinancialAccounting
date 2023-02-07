/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Empiria Data Object                     *
*  Type     : ExternalVariable                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Defines an external variable like a financial indicator or business income.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.ExternalData.Adapters;
using Empiria.FinancialAccounting.ExternalData.Data;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds data about an external variable or value like a
  /// financial indicator or business income.</summary>
  public class ExternalVariable : BaseObject {

    #region Constructors and parsers

    protected ExternalVariable() {
      // Required by Empiria Framework.
    }


    internal ExternalVariable(ExternalVariablesSet set,
                              ExternalVariableFields fields) {
      Assertion.Require(set, nameof(set));
      Assertion.Require(fields, nameof(fields));

      this.Set = set;

      Load(fields);
    }


    static public ExternalVariable Parse(int id) {
      return BaseObject.ParseId<ExternalVariable>(id);
    }

    static public ExternalVariable Parse(string uid) {
      return BaseObject.ParseKey<ExternalVariable>(uid);
    }

    public static bool ExistsCode(string code) {
      return TryParseWithCode(code) != null;
    }

    static public ExternalVariable TryParseWithCode(string code) {
      return BaseObject.TryParse<ExternalVariable>($"CLAVE_VARIABLE = '{code}'");
    }

    static public ExternalVariable TryParseWithCode(ExternalVariablesSet set, string code) {
      return BaseObject.TryParse<ExternalVariable>($"ID_CONJUNTO_BASE = {set.Id} AND CLAVE_VARIABLE = '{code}'");
    }


    static public ExternalVariable Empty {
      get {
        return ExternalVariable.ParseEmpty<ExternalVariable>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CONJUNTO_BASE")]
    public ExternalVariablesSet Set {
      get;
      private set;
    }


    [DataField("CLAVE_VARIABLE")]
    public string Code {
      get;
      private set;
    }


    [DataField("NOMBRE_VARIABLE")]
    public string Name {
      get;
      private set;
    }


    [DataField("NOTAS")]
    public string Notes {
      get;
      private set;
    }


    [DataField("CONFIGURACION_VARIABLE")]
    internal JsonObject ExtData {
      get;
      private set;
    }


    [DataField("POSICION")]
    public int Position {
      get;
      private set;
    }


    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get;
      private set;
    }


    [DataField("FECHA_FIN")]
    public DateTime EndDate {
      get;
      private set;
    }


    [DataField("STATUS_VARIABLE_EXTERNA", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    }


    [DataField("ID_EDITADA_POR")]
    public Contact UpdatedBy {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }


    internal void Update(ExternalVariableFields fields) {
      Assertion.Require(fields, nameof(fields));

      Load(fields);
    }


    private void Load(ExternalVariableFields fields) {
      this.Code       = base.PatchField(fields.Code,      this.Code);
      this.Name       = base.PatchField(fields.Name,      this.Name);
      this.Notes      = base.PatchField(fields.Notes,     this.Notes);
      this.StartDate  = base.PatchField(fields.StartDate, this.StartDate);
      this.EndDate    = base.PatchField(fields.EndDate,   this.EndDate);
      this.UpdatedBy  = ExecutionServer.CurrentContact;
    }


    protected override void OnSave() {
      ExternalVariablesData.Write(this);
    }

    #endregion Methods

  } // class ExternalVariable

}  // namespace Empiria.FinancialAccounting.ExternalData
