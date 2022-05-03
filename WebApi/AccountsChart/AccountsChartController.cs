/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : AccountsChartController                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounts charts.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to retrive accounts charts.</summary>
  public class AccountEditionController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/accounts/{accountUID:guid}")]
    public SingleObjectModel GetAccount([FromUri] string accountsChartUID,
                                        [FromUri] string accountUID) {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountDto account = usecases.GetAccount(accountsChartUID, accountUID);

        return new SingleObjectModel(base.Request, account);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}")]
    public SingleObjectModel GetAccounts([FromUri] string accountsChartUID) {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.GetAccounts(accountsChartUID);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts")]
    public CollectionModel GetAccountsChartsList() {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> accountsChartList = usecases.GetAccountsChartsList();

        return new CollectionModel(base.Request, accountsChartList);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts-master-data")]
    public CollectionModel GetAccountsChartsMasterDataList() {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        FixedList<AccountsChartMasterDataDto> masterDataList = usecases.GetAccountsChartsMasterData();

        return new CollectionModel(base.Request, masterDataList);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/excel")]
    public SingleObjectModel GetAccountsInExcelFile([FromUri] string accountsChartUID,
                                                    [FromBody] AccountsSearchCommand searchCommand) {
      base.RequireBody(searchCommand);

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(accountsChartUID, searchCommand);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(accountsChart, searchCommand);

        return new SingleObjectModel(base.Request, excelFileDto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}")]
    public SingleObjectModel SearchAccounts([FromUri] string accountsChartUID,
                                            [FromBody] AccountsSearchCommand searchCommand) {
      base.RequireBody(searchCommand);

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(accountsChartUID, searchCommand);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }

    #endregion Web Apis

  }  // class AccountsChartController

}  // namespace Empiria.FinancialAccounting.WebApi
