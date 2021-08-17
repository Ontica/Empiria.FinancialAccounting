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

using Empiria.FinancialAccounting.Vouchers.UseCases;

namespace Empiria.FinancialAccounting.Vouchers.WebApi {

  /// <summary>Query web API used to retrive accounting vouchers related data.</summary>
  public class VoucherDataController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/event-types")]
    public CollectionModel GetEventTypes() {

      using (var usecases = VoucherDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> eventTypes = usecases.EventTypes();

        return new CollectionModel(base.Request, eventTypes);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/functional-areas")]
    public CollectionModel GetFunctionalAreas() {

      using (var usecases = VoucherDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> functionalAreas = usecases.FunctionalAreas();

        return new CollectionModel(base.Request, functionalAreas);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/opened-accounting-dates/{ledgerUID:guid}")]
    public CollectionModel GetOpenedAccountingDates([FromUri] string ledgerUID) {

      using (var usecases = VoucherDataUseCases.UseCaseInteractor()) {
        FixedList<DateTime> openedAccountingDates = usecases.OpenedAccountingDates(ledgerUID);

        return new CollectionModel(base.Request, openedAccountingDates);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/transaction-types")]
    public CollectionModel GetTransactionTypes() {

      using (var usecases = VoucherDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> transactionTypes = usecases.TransactionTypes();

        return new CollectionModel(base.Request, transactionTypes);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/voucher-types")]
    public CollectionModel GetVoucherTypes() {

      using (var usecases = VoucherDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> vouchersTypes = usecases.VoucherTypes();

        return new CollectionModel(base.Request, vouchersTypes);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/search-accounts")]
    public CollectionModel SearchAccountsForVoucher([FromUri] int voucherId,
                                                    [FromUri] string keywords) {

      using (var usecases = VoucherDataUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> accounts = usecases.SearchAccountsForVoucher(voucherId, keywords);

        return new CollectionModel(base.Request, accounts);
      }
    }

    #endregion Web Apis

  }  // class VouchersDataController

}  // namespace Empiria.FinancialAccounting.Vouchers.WebApi
