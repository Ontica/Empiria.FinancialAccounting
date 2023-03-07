/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Service provider                        *
*  Type     : FinancialConceptMacro                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Programming macros that contain user defined code for financial concepts calculation.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Expressions;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Programming macros that contain user defined code for financial concepts calculation.</summary>
  public class FinancialConceptMacro : BaseObject, IMacro {

    #region Constructors and parsers

    static public FixedList<FinancialConceptMacro> GetList() {
      return BaseObject.GetList<FinancialConceptMacro>()
                       .ToFixedList();
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ObjectName")]
    public string Name {
      get;
      private set;
    }


    [DataField("ObjectExtData")]
    public string Code {
      get;
      private set;
    }

    #endregion Properties

  }  // class FinancialConceptMacro

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
