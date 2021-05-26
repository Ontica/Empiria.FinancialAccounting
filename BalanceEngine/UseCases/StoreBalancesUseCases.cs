/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : StoreBalancesUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to store account or account aggrupation balances.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to store account or account aggrupation balances.</summary>
  public class StoreBalancesUseCases : UseCase {

    #region Constructors and parsers

    protected StoreBalancesUseCases() {
      // no-op
    }

    static public StoreBalancesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<StoreBalancesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public void StoreBalances(StoreBalancesCommand command) {
      Assertion.AssertObject(command, "command");

      var accountsChart = AccountsChart.Parse(command.AccountsChartUID);

      var storedBalanceSet = new StoredBalanceSet(accountsChart, command.BalancesDate);

      storedBalanceSet.Save();
    }

    #endregion Use cases

  }  // class StoreBalancesUseCases

}  // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
