/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Empiria Data Object                     *
*  Type     : ExternalVariable                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Defines an external variable like a financial indicator or business income.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

using Empiria.FinancialAccounting.Rules.Data;

namespace Empiria.FinancialAccounting.Rules {

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


    [DataField("FECHA")]
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


    [DataField("ID_ACTUALIZO")]
    public Contact UpdatedBy {
      get;
      private set;
    }


    [DataField("FECHA_EDICION")]
    public DateTime UpdatedDate {
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

}  // namespace Empiria.FinancialAccounting.Rules
