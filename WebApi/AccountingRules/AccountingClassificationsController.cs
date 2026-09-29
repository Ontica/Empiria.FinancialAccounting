/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : AccountingClassificationsController          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounting classifications.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Transactions.Adapters;

using Empiria.FinancialAccounting.AccountingRules.UseCases;

namespace Empiria.FinancialAccounting.WebApi.AccountingRules {

  /// <summary>Query web API used to retrive accounts charts.</summary>
  public class AccountingClassificationsController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/rules/accounting-classifications")]
    public CollectionModel GetAccountingClassifications() {

      using (var usecases = AccountingClassificationsUseCases.UseCaseInteractor()) {
        FixedList<AccountingClassificationDto> classifications = usecases.GetAccountingClassifications();

        return new CollectionModel(base.Request, classifications);
      }
    }

    #endregion Web Apis

  }  // class AccountingClassificationsController

}  // namespace Empiria.FinancialAccounting.WebApi.AccountingRules
