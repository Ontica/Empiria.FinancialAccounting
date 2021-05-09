/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : LedgerController                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to manage accounting ledger books.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to manage accounting ledger books..</summary>
  public class LedgerController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/ledgers/{ledgerUID:guid}")]
    public SingleObjectModel GetLedger([FromUri] string ledgerUID) {

      using (var usecases = LedgerUseCases.UseCaseInteractor()) {
        LedgerDto ledger = usecases.GetLedger(ledgerUID);

        return new SingleObjectModel(base.Request, ledger);
      }
    }

    #endregion Web Apis

  }  // class LedgerController

}  // namespace Empiria.FinancialAccounting.WebApi
