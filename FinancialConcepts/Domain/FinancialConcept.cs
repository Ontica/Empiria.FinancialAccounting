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

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Contains data about a financial concept, which has an arithmetic integration of other
  /// financial concepts, financial accounting accounts or external financial values.</summary>
  public class FinancialConcept : BaseObject {

    #region Fields

    private FixedList<FinancialConceptIntegrationEntry> _integration;

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

    public FixedList<FinancialConceptIntegrationEntry> Integration {
      get {
        if (_integration == null) {
          _integration = Group.FinancialConceptIntegrationEntries(this);
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

    #endregion Methods

  } // class FinancialConcept

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
