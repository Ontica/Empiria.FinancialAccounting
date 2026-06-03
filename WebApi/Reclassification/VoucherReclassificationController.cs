/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                    Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : VoucherReclassificationController            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to reclassify vouchers.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reclassification.Services;

namespace Empiria.FinancialAccounting.WebApi.Reclassification {

  /// <summary>Web API used to reclassify vouchers.</summary>
  public class VoucherReclassificationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/reclassification")]
    public NoDataModel ReclassifyVouchers([FromUri] DateTime fromDate, DateTime toDate) {

      using (var usecases = VoucherReclassificationServices.ServiceInteractor()) {
        usecases.ReclassifyVouchers(fromDate, toDate);

        return new NoDataModel(base.Request);
      }
    }

    #endregion Web Apis

  }  // class VoucherReclassificationController

}  // namespace Empiria.FinancialAccounting.WebApi.Reclassification
