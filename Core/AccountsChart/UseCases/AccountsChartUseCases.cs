/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : AccountsChartUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounts chart management.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for accounts chart management.</summary>
  public class AccountsChartUseCases : UseCase {

    #region Constructors and parsers

    protected AccountsChartUseCases() {
      // no-op
    }

    static public AccountsChartUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountsChartUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public AccountsChartDto GetAccounts(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      return AccountsChartMapper.Map(accountsChart);
    }


    public AccountsChartDto SearchAccounts(string accountsChartUID,
                                           AccountsSearchCommand searchCommand) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");
      Assertion.AssertObject(searchCommand, "searchCommand");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      return AccountsChartMapper.Map(accountsChart);
    }

    #endregion Use cases

  }  // class AccountsChartUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
