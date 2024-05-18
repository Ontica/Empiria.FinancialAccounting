/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Interface adapters                      *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Data Transfer Object                    *
*  Type     : ExternalValueInputDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTO used to load financial external values.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.DynamicData;
using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.Datasets;

namespace Empiria.FinancialAccounting.ExternalData.Adapters {

  /// <summary>Input DTO used to load financial external values.</summary>
  public class ExternalValueInputDto : DynamicFields {

    public string VariableUID {
      get; set;
    }

    public string VariableCode {
      get; set;
    }

    public DateTime ApplicationDate {
      get; set;
    }

    public Contact UpdatedBy {
      get; set;
    }

    public DateTime UpdatedDate {
      get; set;
    }

    public Dataset Dataset {
      get;
      internal set;
    }

    public int Position {
      get; set;
    }

    public EntityStatus Status {
      get; set;
    }


    public ExternalVariable GetExternalVariable() {
      var variable = ExternalVariable.TryParseWithCode((ExternalVariablesSet) Dataset.DatasetFamily, VariableCode);

      Assertion.Require(variable,
                        $"No existe la variable externa con clave '{VariableCode}' " +
                        $"dentro del conjunto de variables '{Dataset.DatasetFamily.Name}'.");

      return variable;
    }

    internal JsonObject GetDynamicFieldsAsJson() {
      return this.ToJson();
    }

  }  // class ExternalValueInputDto

} // namespace Empiria.FinancialAccounting.ExternalData.Adapters
