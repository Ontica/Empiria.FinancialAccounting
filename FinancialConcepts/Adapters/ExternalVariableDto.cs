/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : ExternalVariableDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO for ExternalVariable instances.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>DTO for ExternalVariablesSet instances.</summary>
  public class ExternalVariablesSetDto {

    internal ExternalVariablesSetDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }

    public FixedList<ExportToDto> ExportTo {
      get; internal set;
    }

  }  // class ExternalVariablesSetDto



  /// <summary>DTO for ExternalVariable instances.</summary>
  public class ExternalVariableDto {

    internal ExternalVariableDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string Notes {
      get; internal set;
    }

    public int? Position {
      get; internal set;
    }

    public string SetUID {
      get; internal set;
    }

  }  // class ExternalVariableDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
