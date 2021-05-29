/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalancesStoreUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to store account or account aggrupation balances.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to store account or account aggrupation balances.</summary>
  public class BalancesStoreUseCases : UseCase {

    #region Constructors and parsers

    protected BalancesStoreUseCases() {
      // no-op
    }

    static public BalancesStoreUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalancesStoreUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public StoredBalancesSetDto CreateOrGetStoredBalanceSet(string accountsChartUID,
                                                            StoreBalancesCommand command) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");
      Assertion.AssertObject(command, "command");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      var storedBalanceSet = StoredBalanceSet.CreateOrGetBalancesSet(accountsChart, command.BalancesDate);

      storedBalanceSet.Save();

      return StoredBalancesMapper.Map(storedBalanceSet);
    }


    public FixedList<StoredBalancesSetDto> StoredBalancesSets(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<StoredBalanceSet> list = StoredBalanceSet.GetList(accountsChart);

      return StoredBalancesMapper.Map(list);
    }

    #endregion Use cases

  }  // class BalancesStoreUseCases

}  // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
