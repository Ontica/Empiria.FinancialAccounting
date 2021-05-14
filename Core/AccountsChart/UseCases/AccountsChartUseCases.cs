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
using System.Linq;

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


    public FixedList<NamedEntityDto> GetAccountsChartsList() {
      FixedList<AccountsChart> accountsChartsList = AccountsChart.GetList();

      return accountsChartsList.MapToNamedEntityList();
    }


    public FixedList<AccountsChartMasterDataDto> GetAccountsChartsMasterData() {
      FixedList<AccountsChart> accountsChartsList = AccountsChart.GetList();

      var masterDataList = new FixedList<AccountsChartMasterData>(accountsChartsList.Select(x => x.MasterData));

      return AccountsChartMasterDataMapper.Map(masterDataList);
    }


    public AccountsChartDto SearchAccounts(string accountsChartUID,
                                           AccountsSearchCommand command) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");
      Assertion.AssertObject(command, "command");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      string filter = command.MapToFilterString();

      FixedList<Account> accounts = accountsChart.Search(filter);

      accounts = command.Restrict(accounts);

      return AccountsChartMapper.Map(accountsChart, accounts);
    }

    #endregion Use cases

  }  // class AccountsChartUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
