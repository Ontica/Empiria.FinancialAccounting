/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Types    : ExportToMapper                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapper class for ExportTo entities.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Mapper class for ExportTo entities.</summary>
  static public class ExportToMapper {

    static public FixedList<ExportToDto> Map(FixedList<ExportTo> exportTo) {
      return exportTo.Select(x => Map(x))
                     .ToFixedList();
    }


    static public ExportToDto Map(ExportTo exportTo) {
      return new ExportToDto {
        UID = exportTo.UID,
        Name = exportTo.Name,
        FileType = exportTo.FileType,
        Dataset = exportTo.Dataset,
        StartDate = exportTo.StartDate,
        EndDate = exportTo.EndDate
      };
    }


    static public FixedList<ExportToDto> MapWithCalculatedUID(FixedList<ExportTo> exportTo) {
      return exportTo.Select(x => MapWithCalculatedUID(x))
                     .ToFixedList();
    }


    static private ExportToDto MapWithCalculatedUID(ExportTo exportTo) {
      return new ExportToDto {
        UID = exportTo.CalculatedUID,
        Name = exportTo.Name,
        FileType = exportTo.FileType,
        Dataset = exportTo.Dataset
      };
    }

  }  // class ExportToMapper

}  // namespace Empiria.FinancialAccounting
