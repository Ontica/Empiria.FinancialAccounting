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

using Empiria.FinancialAccounting.OfficeIntegration;
using Empiria.FinancialAccounting.OfficeIntegration.Adapters;

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

        var excelExporter = new ExcelExporter();

        ExcelFileDto excelFileDto = excelExporter.Export(trialBalance, command);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    #endregion Web Apis

  } // class TrialBalanceController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
