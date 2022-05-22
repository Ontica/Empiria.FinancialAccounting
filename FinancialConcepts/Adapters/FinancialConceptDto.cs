/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Information Holder                      *
*  Type     : FinancialConceptDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Financial concept data transfer object.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Financial concept data transfer object.</summary>
  public class FinancialConceptDto {

    internal FinancialConceptDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public int Position {
      get; internal set;
    }

    public string AccountsChartName {
      get; internal set;
    }

    public string GroupName {
      get; internal set;
    }

  }  // class FinancialConceptDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
