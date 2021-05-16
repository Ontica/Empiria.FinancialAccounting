/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : CataloguesMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for sectors and currencies.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for sectors and currencies.</summary>
  static internal class CataloguesMapper {


    static internal FixedList<CurrencyDto> MapCurrencies(FixedList<Currency> list) {
      return new FixedList<CurrencyDto>(list.Select((x) => MapCurrency(x)));
    }


    static internal CurrencyDto MapCurrency(Currency currency) {
      return new CurrencyDto {
        UID = currency.UID,
        Name = currency.Name,
        FullName = currency.FullName,
        Symbol = currency.Symbol,
        Code = currency.Code,
        Abbrev = currency.Abbrev
      };
    }


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

  }  // class CataloguesMapper

}  // namespace Empiria.FinancialAccounting.Adapters
