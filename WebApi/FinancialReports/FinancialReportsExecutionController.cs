/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : FinancialReportsExecutionController          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to generate financial reports.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.UseCases;

using Empiria.FinancialAccounting.Reporting.FinancialReports.Exporters;

namespace Empiria.FinancialAccounting.WebApi.FinancialReports {

  /// <summary>Query web API used to generate financial reports.</summary>
  public class FinancialReportsExecutionController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-reports/types/{accountsChartUID:guid}")]
    public CollectionModel GetFinancialReportTypes([FromUri] string accountsChartUID) {
      using (var usecases = FinancialReportsUseCases.UseCaseInteractor()) {
        FixedList<FinancialReportTypeDto> list = usecases.FinancialReportTypes(accountsChartUID);

        return new CollectionModel(base.Request, list);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-reports/data")]
    [Route("v2/financial-accounting/financial-reports/generate")]
    public SingleObjectModel GetFinancialReport([FromBody] FinancialReportQuery buildQuery) {
      base.RequireBody(buildQuery);

      SetOperation($"Se generó el reporte regulatorio " +
                   $"{buildQuery.GetFinancialReportType().Name}.");

      using (var usecases = FinancialReportsUseCases.UseCaseInteractor()) {
        FinancialReportDto financialReport = usecases.GenerateFinancialReport(buildQuery);

        return new SingleObjectModel(base.Request, financialReport);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-reports/data/breakdown/{reportRowUID:guid}")]
    [Route("v2/financial-accounting/financial-reports/generate/breakdown/{reportRowUID:guid}")]
    public SingleObjectModel GetFinancialReportBreakdown([FromUri] string reportRowUID,
                                                         [FromBody] FinancialReportQuery buildQuery) {
      base.RequireBody(buildQuery);

      SetOperation($"Se generó el detalle del reporte regulatorio " +
                   $"{buildQuery.GetFinancialReportType().Name}.");

      using (var usecases = FinancialReportsUseCases.UseCaseInteractor()) {
        FinancialReportDto financialReport = usecases.GetFinancialReportBreakdown(reportRowUID, buildQuery);

        return new SingleObjectModel(base.Request, financialReport);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-reports/export")]
    public SingleObjectModel ExportFinancialReport([FromBody] FinancialReportQuery buildQuery) {

      base.RequireBody(buildQuery);

      SetOperation($"Se exportó a un archivo Excel/CSV el reporte " +
                   $"{buildQuery.GetFinancialReportType().Name}.");

      using (var usecases = FinancialReportsUseCases.UseCaseInteractor()) {

        FinancialReportDto financialReport = usecases.GenerateFinancialReport(buildQuery);

        var exporter = FinancialReportExportService.ServiceInteractor();

        FileDto fileDto = exporter.Export(financialReport);

        return new SingleObjectModel(this.Request, fileDto);
      }
    }

    #endregion Web Apis

  }  // class FinancialReportsExecutionController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialReports
