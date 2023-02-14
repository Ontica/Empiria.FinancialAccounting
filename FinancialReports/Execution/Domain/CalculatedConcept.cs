/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information holder                      *
*  Type     : CalculatedConcept                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Value type with the calculated values of a financial concept.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports {

  /// <summary>Value type with the calculated values of a financial concept.</summary>
  internal class CalculatedConcept {

    internal CalculatedConcept(FinancialConcept concept, IFinancialConceptValues values) {
      Assertion.Require(concept, nameof(concept));
      Assertion.Require(values, nameof(values));

      this.Concept = concept;
      this.Values = values;
    }


    internal FinancialConcept Concept {
      get;
    }

    internal IFinancialConceptValues Values {
      get;
    }

  }  // class CalculatedConcept

}  // namespace Empiria.FinancialAccounting.FinancialReports
