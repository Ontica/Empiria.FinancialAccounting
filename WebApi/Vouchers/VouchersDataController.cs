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

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.Vouchers.WebApi {

  /// <summary>Query web API used to retrive accounting vouchers related data.</summary>
  public class VouchersDataController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/functional-areas")]
    public CollectionModel GetFunctionalAreas() {

      using (var usecases = FunctionalAreasUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> functionalAreas = usecases.FunctionalAreas();

        return new CollectionModel(base.Request, functionalAreas);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/transaction-types")]
    public CollectionModel GetTransactionTypes() {

      using (var usecases = VouchersDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> transactionTypes = usecases.TransactionTypes();

        return new CollectionModel(base.Request, transactionTypes);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/voucher-types")]
    public CollectionModel GetVoucherTypes() {

      using (var usecases = VouchersDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> vouchersTypes = usecases.VoucherTypes();

        return new CollectionModel(base.Request, vouchersTypes);
      }
    }


    #endregion Web Apis

  }  // class VouchersDataController

}  // namespace Empiria.FinancialAccounting.Vouchers.WebApi
