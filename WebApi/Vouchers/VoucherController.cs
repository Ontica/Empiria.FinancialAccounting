/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : VoucherController                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounting vouchers.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.WebApi {

  /// <summary>Query web API used to retrive accounting vouchers.</summary>
  public class VoucherController : WebApiController {

    #region Web Apis


    [HttpPost]
    [Route("v2/financial-accounting/vouchers")]
    public CollectionModel SearchVouchers([FromBody] SearchVouchersCommand command) {
      base.RequireBody(command);

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        FixedList<VoucherDescriptorDto> vouchers = usecases.SearchVouchers(command);

        return new CollectionModel(base.Request, vouchers);
      }
    }

    #endregion Web Apis

  }  // class VoucherController

}  // namespace Empiria.FinancialAccounting.Vouchers.WebApi
