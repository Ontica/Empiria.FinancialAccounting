/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : VouchersDataController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounting vouchers related data.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.Vouchers.WebApi {

  /// <summary>Query web API used to retrive accounting vouchers related data.</summary>
  public class SearchVouchersController : WebApiController {

    #region Web Apis


    [HttpPost]
    [Route("v2/financial-accounting/vouchers")]
    public CollectionModel SearchVouchers(SearchVouchersCommand command) {
      base.RequireBody(command);

      using (var usecases = VouchersUseCases.UseCaseInteractor()) {
        FixedList<VoucherDescriptorDto> vouchers = usecases.SearchVouchers(command);

        return new CollectionModel(base.Request, vouchers);
      }
    }

    #endregion Web Apis

  }  // class SearchVouchersController

}  // namespace Empiria.FinancialAccounting.Vouchers.WebApi
