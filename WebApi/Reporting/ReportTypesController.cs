/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : ReportTypesController                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to return reports types configuration data.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.Reporting {

  /// <summary>Query web API used to return reports types configuration data.</summary>
  public class ReportTypesController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/reporting/report-types")]
    public CollectionModel GetReportTypes() {

      using (var service = ReportingService.ServiceInteractor()) {
        FixedList<ReportTypeDto> reportTypes = service.GetReportTypes();

        return new CollectionModel(this.Request, reportTypes);
      }
    }

    #endregion Web Apis

  } // class ReportTypesController

} // namespace Empiria.FinancialAccounting.WebApi.Reporting
