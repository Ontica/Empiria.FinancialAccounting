/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Data Object                     *
*  Type     : ExternalVariable                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Defines an external variable like a financial indicator or business income.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Holds data about an external variable or value like a
  /// financial indicator or business income.</summary>
  public class ExternalValue : BaseObject {

    #region Constructors and parsers

    protected ExternalValue() {
      // Required by Empiria Framework.
    }

    static public ExternalValue Parse(int id) {
      return BaseObject.ParseId<ExternalValue>(id);
    }


    static public ExternalValue Empty {
      get {
        return ExternalValue.ParseEmpty<ExternalValue>();
      }
    }


    static public ExternalValue GetValue(string externalVariableCode,
                                         DateTime date) {
      return ExternalValuesData.GetValue(externalVariableCode, date);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("CLAVE_VARIABLE")]
    public string ExternalVariableCode {
      get;
      private set;
    }


    [DataField("FECHA_APLICACION")]
    public DateTime ValueDate {
      get;
      private set;
    }


    [DataField("NOTAS")]
    public string Notes {
      get;
      private set;
    }


    [DataField("MONEDA_NACIONAL")]
    public decimal DomesticCurrencyValue {
      get;
      private set;
    }


    [DataField("MONEDA_EXTRANJERA")]
    public decimal ForeignCurrencyValue {
      get;
      private set;
    }


    [DataField("ID_EDITADO_POR")]
    public Contact UpdatedBy {
      get;
      private set;
    }


    [DataField("FECHA_EDICION")]
    public DateTime UpdatedDate {
      get;
      private set;
    }


    [DataField("STATUS_VALOR_VARIABLE", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    }


    public decimal Total {
      get {
        return this.DomesticCurrencyValue + this.ForeignCurrencyValue;
      }
    }

    #endregion Properties

  } // class ExternalValue

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
