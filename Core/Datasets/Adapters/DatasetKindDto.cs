/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : DatasetKindDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that describes a dataset rule.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Datasets.Adapters {

  public class DatasetKindDto {

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

  }  // class DatasetKindDto

}  // namespace Empiria.FinancialAccounting.Datasets.Adapters
