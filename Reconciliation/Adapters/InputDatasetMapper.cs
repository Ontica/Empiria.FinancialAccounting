/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Mapper class                            *
*  Type     : InputDatasetMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map reconciliation input data sets.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Methods used to map reconciliation input data sets.</summary>
  static internal class InputDatasetMapper {

    #region Public mappers

    static internal ReconciliationDatasetsDto Map(FixedList<InputDataset> loaded,
                                                  FixedList<InputDatasetType> missing) {
      var loadedMapped = loaded.Select(x => Map(x));

      var missingMapped = missing.Select(x => Map(x));

      return new ReconciliationDatasetsDto {
        Loaded = new FixedList<InputDatasetDto>(loadedMapped),
        Missing = new FixedList<InputDatasetTypeDto>(missingMapped)
      };
    }

    #endregion Public mappers

    #region Private methods

    static private InputDatasetDto Map(InputDataset dataset) {
      return new InputDatasetDto() {
        UID = dataset.UID,
        DatasetType = dataset.DatasetType.UID,
        DatasetTypeName = dataset.Name,
        ElaborationDate = dataset.ElaborationDate,
        ElaboratedBy = dataset.ElaboratedBy.Alias,
        FileType = dataset.DatasetType.FileType,
        FileSize = dataset.FileSize,
        Url = "http://server/sicofin/files/archivo.xlsx"
      };
    }


    static private InputDatasetTypeDto Map(InputDatasetType type) {
      return new InputDatasetTypeDto() {
        Name = type.Name,
        Type = type.UID,
        FileType = type.FileType,
        Optional = type.Optional,
        Count = type.Count
      };
    }

    #endregion Private methods

  } // class InputDatasetMapper

} // Empiria.FinancialAccounting.Reconciliation.Adapters
