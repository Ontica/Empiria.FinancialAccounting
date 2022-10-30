/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Interface adapters                      *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Data Transfer Object                    *
*  Type     : ExternalValuesDto                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for external values query.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.ExternalData.Adapters {

  /// <summary>Output DTO for external values query.</summary>
  public class ExternalValuesDto {

    internal ExternalValuesDto() {
      // no-op
    }

    public ExternalValuesQuery Query {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    public FixedList<ExternalValuesEntryDto> Entries {
      get; internal set;
    }

  }  // class ExternalValuesDto



  /// <summary>Output DTO that holds an external value with dynamic fields.</summary>
  public class ExternalValuesEntryDto {

    internal ExternalValuesEntryDto() {
      // no-op
    }

    public string VariableCode {
      get; internal set;
    }

    public string VariableName {
      get; internal set;
    }

  }  // class ExternalValuesEntryDto

}  // namespace Empiria.FinancialAccounting.ExternalData.Adapters
