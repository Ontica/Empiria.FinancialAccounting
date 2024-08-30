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

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to retrive accounts charts.</summary>
  public class AccountEditionController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/ifrs")]
    public SingleObjectModel GetIFRSAccounts() {

      base.SetOperation($"Se leyeron todas las cuentas del catálogo de cuentas.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.GetAccounts(AccountsChart.IFRS.UID);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}")]
    public SingleObjectModel GetAccounts([FromUri] string accountsChartUID) {

      base.SetOperation("Se leyeron todas las cuentas del catálogo de cuentas.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.GetAccounts(accountsChartUID);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/flattened-accounts")]
    public CollectionModel GetFlattenedAccounts([FromUri] string accountsChartUID,
                                                [FromBody] AccountsChartQueryDto query) {

      base.RequireBody(query);

      base.SetOperation("Se leyeron las cuentas aplanadas del catálogo de cuentas.");

      query.AccountsChart = AccountsChart.Parse(accountsChartUID);

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        FixedList<FlatAccountDto> flatAccounts = usecases.GetFlattenedAccounts(query);

        return new CollectionModel(base.Request, flatAccounts);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts")]
    public CollectionModel GetAccountsChartsList() {

      base.SetOperation("Se leyó la lista de catálogos de cuentas.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> accountsChartList = usecases.GetAccountsChartsList();

        return new CollectionModel(base.Request, accountsChartList);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts-master-data")]
    public CollectionModel GetAccountsChartsMasterDataList() {

      base.SetOperation("Se leyeron los datos maestros del catálogo de cuentas.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        FixedList<AccountsChartMasterDataDto> masterDataList = usecases.GetAccountsChartsMasterData();

        return new CollectionModel(base.Request, masterDataList);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/excel")]
    public SingleObjectModel ExportAccountsToExcelFile([FromUri]  string accountsChartUID,
                                                       [FromBody] AccountsQuery query) {
      base.RequireBody(query);

      base.SetOperation("Se exportaron una o más cuentas del catálogo de cuentas a un archivo Excel.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(accountsChartUID, query);

        var excelExporter = new ExcelExporterService();

        FileDto excelFileDto = excelExporter.Export(accountsChart);

        return new SingleObjectModel(base.Request, excelFileDto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/ifrs")]
    public SingleObjectModel SearchIFRSAccounts([FromBody] AccountsQuery query) {
      base.RequireBody(query);

      base.SetOperation("Se efectuó una búsqueda en el catálogo de cuentas.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(AccountsChart.IFRS.UID, query);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}")]
    public SingleObjectModel SearchAccounts([FromUri] string accountsChartUID,
                                            [FromBody] AccountsQuery query) {
      base.RequireBody(query);

      base.SetOperation("Se efectuó una búsqueda en el catálogo de cuentas.");

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsChartDto accountsChart = usecases.SearchAccounts(accountsChartUID, query);

        return new SingleObjectModel(base.Request, accountsChart);
      }
    }

    #endregion Web Apis

  }  // class AccountsChartController

}  // namespace Empiria.FinancialAccounting.WebApi
