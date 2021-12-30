/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : TrialBalanceController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive trial balances.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.BalanceEngine {

  /// <summary>Query web API used to retrive trial balances.</summary>
  public class TrialBalanceController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/trial-balance")]
    public SingleObjectModel GetTrialBalance([FromBody] TrialBalanceCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(command);

        return new SingleObjectModel(this.Request, trialBalance);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/trial-balance/excel")]
    public SingleObjectModel GetExcelTrialBalance([FromBody] TrialBalanceCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(command);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(trialBalance);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance")]
    public SingleObjectModel GetBalances([FromBody] BalanceCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanceDto balance = usecases.BuildBalanceSearch(command);

        return new SingleObjectModel(this.Request, balance);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance/excel")]
    public SingleObjectModel GetExcelBalances([FromBody] BalanceCommand command) {
      base.RequireBody(command);

      //bool? inProcess = null;
      //Assertion.AssertObject(inProcess, $"Funcionalidad en proceso de desarrollo.");

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        BalanceDto balance = usecases.BuildBalanceSearch(command);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(balance);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    #endregion Web Apis

  } // class TrialBalanceController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
