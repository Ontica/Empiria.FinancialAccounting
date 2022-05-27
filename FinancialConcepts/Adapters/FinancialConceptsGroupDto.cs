/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptsGroupDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Financial concepts group data transfer object.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Financial concepts group data transfer object.</summary>
  public class FinancialConceptsGroupDto {

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public NamedEntityDto AccountsChart {
      get; internal set;
    }


    public DateTime StartDate {
      get; internal set;
    }


    public DateTime EndDate {
      get; internal set;
    }


    public FixedList<string> CalculationRules {
      get; internal set;
    }


    public FixedList<string> DataColumns {
      get; internal set;
    }


    public FixedList<NamedEntityDto> ExternalVariablesSets {
      get; internal set;
    }

  }  // class FinancialConceptsGroupDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
