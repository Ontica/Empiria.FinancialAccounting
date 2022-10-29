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

namespace Empiria.FinancialAccounting.ExternalData {

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

    #endregion Constructors and parsers

    #region Properties


    [DataField("ID_VARIABLE_EXTERNA")]
    public ExternalVariable ExternalVariable {
      get;
      private set;
    }

    [DataField("VALORES_VARIABLE")]
    private JsonObject ValuesExtData {
      get;
      set;
    } = new JsonObject();


    [DataField("FECHA_APLICACION")]
    public DateTime ValueDate {
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


    [DataField("STATUS_VALOR_EXTERNO", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    }


    public decimal DomesticCurrencyValue {
      get {
        return ValuesExtData.Get<decimal>("MonedaNacional", 0m);
      }
      set {
        ValuesExtData.SetIfValue("MonedaNacional", value);
      }
    }


    public decimal ForeignCurrencyValue {
      get {
        return ValuesExtData.Get<decimal>("MonedaExtranjera", 0m);
      }
      set {
        ValuesExtData.SetIfValue("MonedaExtranjera", value);
      }
    }


    public decimal Total {
      get {
        return this.DomesticCurrencyValue + this.ForeignCurrencyValue;
      }
    }

    #endregion Properties

  } // class ExternalValue

}  // namespace Empiria.FinancialAccounting.ExternalData
