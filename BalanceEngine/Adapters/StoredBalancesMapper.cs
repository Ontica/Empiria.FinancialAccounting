/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : StoredBalancesMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map stored account balances and account aggrupation balances.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map stored account balances and account aggrupation balances.</summary>
  static internal class StoredBalancesMapper {

    #region Public mappers

    static internal FixedList<StoredBalancesSetDto> Map(FixedList<StoredBalanceSet> list) {
      return new FixedList<StoredBalancesSetDto>(list.Select(x => Map(x)));
    }

    static internal StoredBalancesSetDto Map(StoredBalanceSet balanceSet) {
      return new StoredBalancesSetDto {
        UID = balanceSet.UID,
        Name = balanceSet.Name,
        AccountsChart = balanceSet.AccountsChart.MapToNamedEntity(),
        BalancesDate = balanceSet.BalancesDate,
        Calculated = balanceSet.Calculated,
        CalculationTime = balanceSet.CalculationTime
      };
    }

    #endregion Public mappers

  } // class StoredBalancesMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
