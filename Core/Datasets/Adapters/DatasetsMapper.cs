/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : DatasetsMapper                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map data sets.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Datasets.Adapters {

  /// <summary>Methods used to map data sets.</summary>
  static internal class DatasetsMapper {

    #region Public mappers

    static internal DatasetOutputDto Map(Dataset ds) {
      return new DatasetOutputDto() {
        UID = ds.UID,
        DatasetFamily = ds.DatasetFamily.Name,
        DatasetKind = ds.DatasetKind.Name,
        ElaborationDate = ds.UpdatedTime,
        ElaboratedBy = ds.UploadedBy.Alias,
        FileType = ds.DatasetKind.FileType,
        FileSize = ds.MediaLength,
        FileName = ds.OriginalFileName,
        //  Url = ds.FileUrl
      };
    }


    static internal DatasetsLoadStatusDto MapToDatasetsLoadStatusDto(FixedList<Dataset> loadedDatasets,
                                                                   FixedList<DatasetKind> missing) {
      var loadedMapped = loadedDatasets.Select(x => Map(x));

      var missingMapped = missing.Select(x => Map(x));

      return new DatasetsLoadStatusDto {
        LoadedDatasets = new FixedList<DatasetOutputDto>(loadedMapped),
        MissingDatasetKinds = new FixedList<DatasetKindDto>(missingMapped)
      };
    }

    #endregion Public mappers

    #region Private methods

    static private DatasetKindDto Map(DatasetKind dsKind) {
      return new DatasetKindDto() {
        Name = dsKind.Name,
        Type = dsKind.UID,
        FileType = dsKind.FileType,
        Optional = dsKind.Optional,
        Count = dsKind.Count,
        TemplateUrl = dsKind.TemplateUrl
      };
    }

    #endregion Private methods

  } // class DatasetsMapper

} // Empiria.FinancialAccounting.Datasets.Adapters
