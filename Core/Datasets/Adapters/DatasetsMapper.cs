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
  static public class DatasetsMapper {

    #region Public mappers

    static public DatasetsLoadStatusDto MapToDatasetsLoadStatusDto(FixedList<Dataset> loaded,
                                                                   FixedList<DatasetKind> missing) {
      var loadedMapped = loaded.Select(x => Map(x));

      var missingMapped = missing.Select(x => Map(x));

      return new DatasetsLoadStatusDto {
        LoadedFiles = new FixedList<DatasetDto>(loadedMapped),
        MissingFileTypes = new FixedList<DatasetKindDto>(missingMapped)
      };
    }

    #endregion Public mappers

    #region Private methods

    static private DatasetDto Map(Dataset ds) {
      return new DatasetDto() {
        UID = ds.UID,
        DatasetFamily = ds.DatasetFamily.UID,
        DatasetFamilyName = ds.DatasetFamily.Name,
        ElaborationDate = ds.ElaborationDate,
        ElaboratedBy = ds.ElaboratedBy.Alias,
        FileType = ds.FileType.FileType,
        FileSize = ds.FileSize,
        FileName = ds.FileName,
        Url = ds.FileUrl
      };
    }


    static private DatasetKindDto Map(DatasetKind dsKind) {
      return new DatasetKindDto() {
        Name = dsKind.Name,
        Type = dsKind.UID,
        FileType = dsKind.FileType,
        Optional = dsKind.Optional,
        Count = dsKind.Count
      };
    }

    #endregion Private methods

  } // class DatasetsMapper

} // Empiria.FinancialAccounting.Datasets.Adapters
