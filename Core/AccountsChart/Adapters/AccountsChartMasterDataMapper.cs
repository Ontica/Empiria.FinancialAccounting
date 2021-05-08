/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : AccountsChartMasterDataMapper              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for the master data of an accounts chart.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for the master data of an accounts chart.</summary>
  static internal class AccountsChartMasterDataMapper {


    static internal FixedList<AccountsChartMasterDataDto> Map(FixedList<AccountsChartMasterData> list) {
      return new FixedList<AccountsChartMasterDataDto>(list.Select((x) => Map(x)));
    }


    static internal AccountsChartMasterDataDto Map(AccountsChartMasterData masterData) {
      return new AccountsChartMasterDataDto {
        UID = masterData.AccountsChart.UID,
        Name = masterData.AccountsChart.Name,

        AccountsPattern = masterData.AccountsPattern,
        AccountNumberSeparator = masterData.AccountNumberSeparator,
        MaxAccountLevel = masterData.MaxAccountLevel,

        StartDate = masterData.StartDate,
        EndDate = masterData.EndDate,

        AccountRoles = masterData.AccountRoles,
        AccountTypes = masterData.AccountTypes.MapToNamedEntityList(),

        Currencies = MapCurrencies(masterData.Currencies),
        Sectors = MapSectors(masterData.Sectors)
      };
    }

    #region Private methods

    static private FixedList<CurrencyDto> MapCurrencies(FixedList<Currency> list) {
      return new FixedList<CurrencyDto>(list.Select((x) => MapCurrency(x)));
    }


    static private CurrencyDto MapCurrency(Currency currency) {
      return new CurrencyDto {
        UID = currency.UID,
        Name = currency.Name,
        Symbol = currency.Symbol,
        Code = currency.Code,
        Abbrev = currency.Abbrev
      };
    }


    static private FixedList<SectorDto> MapSectors(FixedList<Sector> list) {
      return new FixedList<SectorDto>(list.Select((x) => MapSector(x)));
    }


    static private SectorDto MapSector(Sector sector) {
      return new SectorDto {
        UID = sector.UID,
        Name = sector.Name,
        Code = sector.Code
      };
    }

    #endregion Private methods

  }  // class AccountsChartMasterDataMapper

}  // namespace Empiria.FinancialAccounting.Adapters
