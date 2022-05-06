/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues                                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : SectorsMapper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for economic sectors.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for economic sectors.</summary>
  static internal class SectorsMapper {

    static internal FixedList<SectorDto> MapSectors(FixedList<Sector> list) {
      return new FixedList<SectorDto>(list.Select((x) => MapSector(x)));
    }

    static internal SectorDto MapSector(Sector sector) {
      return new SectorDto {
        UID = sector.UID,
        Name = sector.Name,
        FullName = sector.FullName,
        Code = sector.Code
      };
    }

  }  // class SectorsMapper

}  // namespace Empiria.FinancialAccounting.Adapters
