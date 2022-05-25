/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Data Object                     *
*  Type     : FinancialConcept                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data about a financial concept, which has an arithmetic integration of other          *
*             financial concepts, financial accounting accounts or external financial values.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Contains data about a financial concept, which has an arithmetic integration of other
  /// financial concepts, financial accounting accounts or external financial values.</summary>
  public class FinancialConcept : BaseObject {

    #region Fields

    private FixedList<FinancialConceptEntry> _integration;

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
      Assertion.AssertObject(fields, nameof(fields));

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


    static public FinancialConcept Empty {
      get {
        return FinancialConcept.ParseEmpty<FinancialConcept>();
      }
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
        if (_integration == null) {
          _integration = FinancialConceptsData.GetFinancialConceptEntries(this);
        }
        return _integration;
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


    internal void Remove() {
      this.Status = EntityStatus.Deleted;
    }


    protected override void OnSave() {
      FinancialConceptsData.Write(this);
    }


    internal void SetPosition(int position) {
      Assertion.Assert(position > 0, "Position must be greater than zero.");

      this.Position = position;
    }


    internal void Update(FinancialConceptFields fields) {
      Assertion.AssertObject(fields, nameof(fields));

      this.Code = fields.Code;
      this.Name = fields.Name;
      this.Position = fields.Position;
      this.StartDate = fields.StartDate;
      this.EndDate = fields.EndDate;
      this.UpdatedBy = ExecutionServer.CurrentIdentity.User.AsContact();
    }

    #endregion Methods

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
