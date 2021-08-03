/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : StoredBalanceSetMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map stored balance sets.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map stored balance sets.</summary>
  static internal class StoredBalanceSetMapper {

    #region Public mappers

    static internal FixedList<StoredBalanceSetDto> Map(FixedList<StoredBalanceSet> list) {
      return new FixedList<StoredBalanceSetDto>(list.Select(x => Map(x)));
    }

    static internal StoredBalanceSetDto Map(StoredBalanceSet balanceSet) {
      return new StoredBalanceSetDto {
        UID = balanceSet.UID,
        Name = balanceSet.Name,
        AccountsChart = balanceSet.AccountsChart.MapToNamedEntity(),
        BalancesDate = balanceSet.BalancesDate,
        Calculated = balanceSet.Calculated,
        CalculationTime = balanceSet.CalculationTime,
        Protected = balanceSet.Protected
      };
    }

    static internal StoredBalanceSetDto MapWithBalances(StoredBalanceSet balanceSet) {
      var dto = Map(balanceSet);

      dto.Balances = MapBalances(balanceSet.Balances);

      return dto;
    }

    #endregion Public mappers

    static private FixedList<StoredBalanceDto> MapBalances(FixedList<StoredBalance> balances) {
      return new FixedList<StoredBalanceDto>(balances.Select(x => MapBalance(x)));
    }


    static private StoredBalanceDto MapBalance(StoredBalance balance) {
      return new StoredBalanceDto {
        StandardAccountId = balance.StandardAccountId,
        Ledger = balance.Ledger.MapToNamedEntity(),
        Currency = balance.Currency.MapToNamedEntity(),
        SectorCode = balance.Sector.Code,
        SubsidiaryAccountId = balance.SubsidiaryAccountId,
        AccountName = balance.AccountName,
        AccountNumber = balance.AccountNumber,
        SubsidiaryAccountNumber = balance.SubsidiaryAccountNumber,
        SubsidiaryAccountName = balance.SubsidiaryAccountName,
        Balance = balance.Balance
        //LastChangeDate = balance.LastChangeDate
      };
    }

  } // class StoredBalanceSetMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
