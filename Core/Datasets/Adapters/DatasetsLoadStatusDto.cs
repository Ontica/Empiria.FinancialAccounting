/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : DatasetsLoadStatusDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that describes the loading status of datasets in a given date.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Datasets.Adapters {

  /// <summary>Output DTO that describes the loading status of datasets in a given date.</summary>
  public class DatasetsLoadStatusDto {

    internal DatasetsLoadStatusDto() {
      // no-op
    }

    public FixedList<DatasetOutputDto> LoadedDatasets {
      get; internal set;
    }

    public FixedList<DatasetKindDto> MissingDatasetKinds {
      get; internal set;
    }

  }  // class DatasetsLoadStatusDto

}  // namespace Empiria.FinancialAccounting.Datasets.Adapters
