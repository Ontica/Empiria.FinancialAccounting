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
using System.Collections.Generic;

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
  public class ExternalValuesEntryDto : DynamicFields {

    internal ExternalValuesEntryDto() {
      // no-op
    }

    public string VariableCode {
      get; internal set;
    }

    public string VariableName {
      get; internal set;
    }

    public override IEnumerable<string> GetDynamicMemberNames() {
      List<string> members = new List<string>();

      members.Add("VariableCode");
      members.Add("VariableName");

      members.AddRange(base.GetDynamicMemberNames());

      return members;
    }

  }  // class ExternalValuesEntryDto

}  // namespace Empiria.FinancialAccounting.ExternalData.Adapters
