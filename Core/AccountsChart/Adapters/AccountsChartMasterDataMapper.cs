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

        Currencies = CataloguesMapper.MapCurrencies(masterData.Currencies),
        Sectors = CataloguesMapper.MapSectors(masterData.Sectors),

        Ledgers = LedgerMapper.Map(masterData.Ledgers)
      };
    }

  }  // class AccountsChartMasterDataMapper

}  // namespace Empiria.FinancialAccounting.Adapters
