/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : BalanceStorageController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to store account chart of accouints accumulated balances.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.BalanceEngine {

  /// <summary>Web API used to store account chart of accouints accumulated balances.</summary>
  public class BalanceStorageController : WebApiController {

    #region Query web apis

    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store/{balanceSetUID:guid}")]
    public SingleObjectModel GetStoredBalanceSet([FromUri] string accountsChartUID,
                                                 [FromUri] string balanceSetUID) {
      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        StoredBalanceSetDto balanceSet = usecases.GetBalanceSet(accountsChartUID, balanceSetUID);

        return new SingleObjectModel(this.Request, balanceSet);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store")]
    public CollectionModel GetStoredBalanceSetsList([FromUri] string accountsChartUID) {
      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        FixedList<StoredBalanceSetDto> list = usecases.BalanceSetsList(accountsChartUID);

        return new CollectionModel(this.Request, list);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store/{balanceSetUID:guid}/excel")]
    public SingleObjectModel ExportStoredBalancesToExcel([FromUri] string accountsChartUID,
                                                         [FromUri] string balanceSetUID) {

      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        StoredBalanceSetDto balanceSet = usecases.GetBalanceSet(accountsChartUID, balanceSetUID);

        var excelExporter = new ExcelExporterService();

        FileDto excelFileDto = excelExporter.Export(balanceSet);

        return new SingleObjectModel(base.Request, excelFileDto);
      }
    }

    #endregion Query web apis

    #region Command web apis

    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store/{balanceSetUID:guid}/calculate")]
    public SingleObjectModel CalculateStoredBalanceSet([FromUri] string accountsChartUID,
                                                       [FromUri] string balanceSetUID) {
      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        StoredBalanceSetDto balanceSet = usecases.CalculateBalanceSet(accountsChartUID, balanceSetUID);

        return new SingleObjectModel(this.Request, balanceSet);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store")]
    public SingleObjectModel CreateBalanceSet([FromUri] string accountsChartUID,
                                              [FromBody] BalanceStorageCommand command) {
      base.RequireBody(command);

      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        StoredBalanceSetDto balanceSet = usecases.CreateBalanceSet(accountsChartUID, command);

        return new SingleObjectModel(this.Request, balanceSet);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/accounts-charts/{accountsChartUID:guid}/balance-store/{balanceSetUID:guid}")]
    public NoDataModel DeleteStoredBalanceSet([FromUri] string accountsChartUID,
                                              [FromUri] string balanceSetUID) {
      using (var usecases = BalanceStorageUseCases.UseCaseInteractor()) {
        usecases.DeleteBalanceSet(accountsChartUID, balanceSetUID);

        return new NoDataModel(this.Request);
      }
    }

    #endregion Command web apis

  } // class BalanceStorageController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
