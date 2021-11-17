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

namespace Empiria.FinancialAccounting.Rules {

  /// <summary>Holds data about an external variable or value like a
  /// financial indicator or business income.</summary>
  public class ExternalVariable : GeneralObject {

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

    static public ExternalVariable Empty {
      get {
        return GroupingRule.ParseEmpty<ExternalVariable>();
      }
    }

    #endregion Constructors and parsers

    #region Properties


    public string ConceptCode {
      get {
        return ExtendedDataField.Get("conceptCode", string.Empty);
      }
    }


    public bool HasDomesticCurrencyValue {
      get {
        return ExtendedDataField.Get("hasDomesticCurrencyValue", true);
      }
    }


    public bool HasForeignCurrencyValue {
      get {
        return ExtendedDataField.Get("hasForeignCurrencyValue", true);
      }
    }

    #endregion Properties

  } // class ExternalVariable

}  // namespace Empiria.FinancialAccounting.Rules
