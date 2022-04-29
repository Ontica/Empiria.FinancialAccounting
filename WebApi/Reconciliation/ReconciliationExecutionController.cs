/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                      Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : ReconciliationExecutionController            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to execute financial accounting reconciliation processes.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reconciliation.UseCases;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.WebApi.Reconciliation {

  /// <summary>Web API used to execute financial accounting reconciliation processes.</summary>
  public class ReconciliationExecutionController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/reconciliation")]
    public SingleObjectModel ExecuteReconciliation([FromBody] ReconciliationCommand command) {

      base.RequireBody(command);

      using (var usecases = ReconciliationExecutionUseCases.UseCaseInteractor()) {
        ReconciliationResultDto result = usecases.Execute(command);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Web Apis

  }  // class ReconciliationExecutionController

}  // namespace Empiria.FinancialAccounting.WebApi.Reconciliation
