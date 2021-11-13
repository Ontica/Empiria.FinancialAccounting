/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : VoucherSpecialCaseController                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to process voucher special cases.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.WebApi.Vouchers {

  /// <summary>Query web API used to retrive accounting vouchers.</summary>
  public class VoucherSpecialCaseController : WebApiController {

    #region Web Apis


    [HttpPost]
    [Route("v2/financial-accounting/vouchers/special-case/create-voucher")]
    public SingleObjectModel CreateVoucher([FromBody] VoucherSpecialCaseFields fields) {
      base.RequireBody(fields);

      using (var usecases = VoucherSpecialCasesUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.CreateSpecialCaseVoucher(fields);

        return new SingleObjectModel(base.Request, voucher);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/special-case-types")]
    public CollectionModel GetSpecialCases() {
      using (var usecases = VoucherSpecialCasesUseCases.UseCaseInteractor()) {
        FixedList<VoucherSpecialCaseTypeDto> specialCases = usecases.GetSpecialCaseTypes();

        return new CollectionModel(base.Request, specialCases);
      }
    }


    #endregion Web Apis

  }  // class VoucherSpecialCaseController

}  // namespace Empiria.FinancialAccounting.WebApi.Vouchers
