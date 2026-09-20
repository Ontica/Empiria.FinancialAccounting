/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Transactions            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : FinancialTransactionsController              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to integrate financial transactions with financial accounting processes.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.Financial.Transactions.Adapters;

using Empiria.FinancialAccounting.Transactions.UseCases;

namespace Empiria.FinancialAccounting.Transactions.WebApi {

  /// <summary>Web API used to integrate financial transactions with financial accounting processes.</summary>
  public class FinancialTransactionsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/transactions")]
    public SingleObjectModel PostTransaction([FromBody] FinancialTransactionFields fields) {

      using (var usecases = FinancialTransactionUseCases.UseCaseInteractor()) {

        int result = usecases.PostTransaction(fields);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Web Apis

  }  // class FinancialTransactionsController

}  // namespace Empiria.FinancialAccounting.Transactions.WebApi
