/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Command Controller                    *
*  Type     : VoucherGenerationController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to generate vouchers for financial transactions.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Transactions;

using Empiria.FinancialAccounting.AccountingRules.Processor.Services;

namespace Empiria.FinancialAccounting.WebApi.AccountingRules {

  /// <summary>Web API used to generate vouchers for financial transactions.</summary>
  public class VoucherGenerationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/transactions/{transactionId:int}/generate-voucher")]
    public SingleObjectModel GenerateVoucher([FromUri] int transactionId) {

      using (var service = FinancialTransactionVoucherGenerationServices.ServicesInteractor()) {

        var transaction = FinancialTransaction.Parse(transactionId);

        var result = service.GenerateVoucher(transaction);

        return new SingleObjectModel(base.Request, result);
      }
    }

    #endregion Web Apis

  }  // class VoucherGenerationController

}  // namespace Empiria.FinancialAccounting.WebApi.AccountingRules
