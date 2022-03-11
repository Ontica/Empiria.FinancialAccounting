/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : InputDatasetTypeDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map reconciliation input data sets.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Datasets.Adapters {

  public class InputDatasetTypeDto {

    public string Name {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public FileType FileType {
      get; internal set;
    }

    public bool Optional {
      get; internal set;
    }

    public int Count {
      get; internal set;
    }

  }  // class InputDatasetTypeDto

}  // namespace Empiria.FinancialAccounting.Datasets.Adapters
