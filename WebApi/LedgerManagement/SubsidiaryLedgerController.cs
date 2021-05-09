/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : SubsidiaryLedgerController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to get information about subsidiary ledger books and subsidiary accounts.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.WebApi {

  /// <summary>Query web API used to get information about subsidiary ledger
  /// books and subsidiary accounts.</summary>
  public class SubsidiaryLedgerController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/subsidiary-ledgers/{subsidiaryLedgerUID:guid}")]
    public SingleObjectModel GetSubsidiaryLedgers([FromUri] string subsidiaryLedgerUID) {

      using (var usecases = SubsidiaryLedgerUseCases.UseCaseInteractor()) {
        SubsidiaryLedgerDto subsidiaryLedger = usecases.GetSubsidiaryLedger(subsidiaryLedgerUID);

        return new SingleObjectModel(base.Request, subsidiaryLedger);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/subsidiary-ledgers/{subsidiaryLedgerUID:guid}" +
           "/accounts/{subsidiaryAccountId:int}")]
    public SingleObjectModel GetLedgerAccount([FromUri] string subsidiaryLedgerUID,
                                              [FromUri] int subsidiaryAccountId) {

      using (var usecases = SubsidiaryLedgerUseCases.UseCaseInteractor()) {
        SubsidiaryAccountDto subsidiaryAccount = usecases.GetSubsidiaryAccount(subsidiaryLedgerUID,
                                                                               subsidiaryAccountId);

        return new SingleObjectModel(base.Request, subsidiaryAccount);
      }
    }

    #endregion Web Apis

  }  // class SubsidiaryLedgerController

}  // namespace Empiria.FinancialAccounting.WebApi
