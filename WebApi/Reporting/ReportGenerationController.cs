/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : ReportGenerationController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to generate financial accounting reports: financial, operational, fiscal.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.Reporting {

  /// <summary>Query web API used to generate financial accounting reports:
  /// financial, operational, fiscal.</summary>
  public class ReportGenerationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/reporting/data")]
    public SingleObjectModel BuildReport([FromBody] ReportBuilderQuery buildQuery) {
      base.RequireBody(buildQuery);

      base.SetOperation($"Se generó el reporte '{buildQuery.ReportType}'.");

      using (var service = ReportingService.ServiceInteractor()) {
        ReportDataDto reportData = service.GenerateReport(buildQuery);

        return new SingleObjectModel(this.Request, reportData);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reporting/export")]
    public SingleObjectModel ExportReportData([FromBody] ReportBuilderQuery buildQuery) {
      base.RequireBody(buildQuery);

      SetOperation($"Se exportó a un archivo el reporte '{buildQuery.ReportType}'.");

      using (var service = ReportingService.ServiceInteractor()) {

        ReportDataDto reportData = service.GenerateReport(buildQuery);

        FileDto fileDto = service.ExportReport(buildQuery, reportData);

        return new SingleObjectModel(this.Request, fileDto);
      }
    }

    #endregion Web Apis

  } // class ReportGenerationController

} // namespace Empiria.FinancialAccounting.WebApi.Reporting
