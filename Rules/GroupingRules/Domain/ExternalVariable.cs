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

using Empiria.Json;

namespace Empiria.FinancialAccounting.Rules {

  /// <summary>Holds data about an external variable or value like a
  /// financial indicator or business income.</summary>
  public class ExternalVariable : BaseObject {

    #region Constructors and parsers

    protected ExternalVariable() {
      // Required by Empiria Framework.
    }

    static public ExternalVariable Parse(int id) {
      return BaseObject.ParseId<ExternalVariable>(id);
    }

    static public ExternalVariable Parse(string uid) {
      return BaseObject.ParseKey<ExternalVariable>(uid);
    }

    static internal ExternalVariable TryParseWithCode(string externalVariableCode) {
      return BaseObject.TryParse<ExternalVariable>($"ETIQUETA_VARIABLE = '{externalVariableCode}'");
    }

    static public ExternalVariable Empty {
      get {
        return ExternalVariable.ParseEmpty<ExternalVariable>();
      }
    }

    #endregion Constructors and parsers

    #region Properties


    [DataField("ETIQUETA_VARIABLE")]
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


    [DataField("CONFIG_VARIABLE")]
    internal JsonObject ExtData {
      get;
      private set;
    }


    public bool HasDomesticCurrencyValue {
      get {
        return ExtData.Get("hasDomesticCurrencyValue", true);
      }
    }


    public bool HasForeignCurrencyValue {
      get {
        return ExtData.Get("hasForeignCurrencyValue", true);
      }
    }

    #endregion Properties

  } // class ExternalVariable

}  // namespace Empiria.FinancialAccounting.Rules
