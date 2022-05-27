/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Aggregate Object                *
*  Type     : ExternalVariablesSet                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of external variables, with its single purpose is to classify them.                *
*             A given external variables always belongs to a single set.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;


using Empiria.FinancialAccounting.FinancialConcepts.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Holds a set of external variables, with its unique single purpose is to classify them.
  /// A given external variables always belongs to a single set.</summary>
  public class ExternalVariablesSet : GeneralObject {

    #region Fields

    private Lazy<FixedList<ExternalVariable>> _externalVariables;

    #endregion Fields

    #region Constructors and parsers

    protected ExternalVariablesSet() {
      // Required by Empiria Framework.
    }

    static public ExternalVariablesSet Parse(int id) {
      return BaseObject.ParseId<ExternalVariablesSet>(id);
    }


    static public ExternalVariablesSet Parse(string uid) {
      return BaseObject.ParseKey<ExternalVariablesSet>(uid);
    }


    static public FixedList<ExternalVariablesSet> GetList() {
      return BaseObject.GetList<ExternalVariablesSet>(String.Empty, "ObjectName")
                       .ToFixedList();
    }


    static public FinancialConceptGroup Empty {
      get {
        return FinancialConceptGroup.ParseEmpty<FinancialConceptGroup>();
      }
    }


    protected override void OnLoad() {
      if (this.IsEmptyInstance) {
        return;
      }

      _externalVariables =
            new Lazy<FixedList<ExternalVariable>>(() => ExternalValuesData.GetExternalVariables(this));

    }

    #endregion Constructors and parsers

    #region Properties


    public FixedList<ExternalVariable> ExternalVariables {
      get {
        return _externalVariables.Value;
      }
    }

    #endregion Properties

  } // class ExternalVariablesSet

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
