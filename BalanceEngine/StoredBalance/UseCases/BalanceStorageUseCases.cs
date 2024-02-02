/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanceStorageUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to store chart of accounts accumulated balances.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to store chart of accounts accumulated balances.</summary>
  public class BalanceStorageUseCases : UseCase {

    #region Constructors and parsers

    protected BalanceStorageUseCases() {
      // no-op
    }

    static public BalanceStorageUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanceStorageUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<StoredBalanceSetDto> BalanceSetsList(string accountsChartUID) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<StoredBalanceSet> list = StoredBalanceSet.GetList(accountsChart);

      return StoredBalanceSetMapper.Map(list);
    }


    public StoredBalanceSetDto CreateBalanceSet(string accountsChartUID,
                                                BalanceStorageCommand command) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));
      Assertion.Require(command, nameof(command));

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      if (StoredBalanceSet.ExistBalanceSet(accountsChart, command.BalancesDate)) {
        Assertion.RequireFail("Ya existe un conjunto de saldos para la fecha proporcionada.");
      }

      var storedBalanceSet = StoredBalanceSet.CreateBalanceSet(accountsChart, command.BalancesDate);

      storedBalanceSet.Save();

      return StoredBalanceSetMapper.Map(storedBalanceSet);
    }


    public StoredBalanceSetDto CalculateBalanceSet(string accountsChartUID, string balanceSetUID) {
      StoredBalanceSet balanceSet = ParseBalanceSet(accountsChartUID, balanceSetUID);

      balanceSet.Calculate();

      return StoredBalanceSetMapper.MapWithBalances(balanceSet);
    }


    public StoredBalanceSetDto GetBalanceSet(string accountsChartUID, string balanceSetUID) {
      StoredBalanceSet balanceSet = ParseBalanceSet(accountsChartUID, balanceSetUID);

      return StoredBalanceSetMapper.MapWithBalances(balanceSet);
    }

    #endregion Use cases

    #region Helper methods

    private StoredBalanceSet ParseBalanceSet(string accountsChartUID, string balanceSetUID) {
      Assertion.Require(accountsChartUID, nameof(accountsChartUID));
      Assertion.Require(balanceSetUID, nameof(balanceSetUID));

      var accountsChart = AccountsChart.Parse(accountsChartUID);
      var balanceSet = StoredBalanceSet.Parse(balanceSetUID);

      Assertion.Require(balanceSet.AccountsChart.Equals(accountsChart),
                       "The requested balance set does not belong to the given accounts chart.");

      return balanceSet;
    }

    #endregion Helper methods

  }  // class BalanceStorageUseCases

}  // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
