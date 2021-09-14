/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                            Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : FinancialReportsController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to generate financial reports.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.UseCases;

using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports;
using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters;

namespace Empiria.FinancialAccounting.WebApi.FinancialReports {

  /// <summary>Query web API used to generate financial reports.</summary>
  public class FinancialReportsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/financial-reports")]
    public SingleObjectModel GetFinancialReport([FromBody] FinancialReportCommand command) {
      base.RequireBody(command);

      using (var usecases = FinancialReportsUseCases.UseCaseInteractor()) {
        FinancialReportDto financialReport = usecases.GenerateFinancialReport(command);

        return new SingleObjectModel(base.Request, financialReport);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-reports/excel")]
    public SingleObjectModel GetExcelFinancialReport([FromBody] FinancialReportCommand command) {
      base.RequireBody(command);

      using (var usecases = FinancialReportsUseCases.UseCaseInteractor()) {
        FinancialReportDto financialReport = usecases.GenerateFinancialReport(command);

        var excelExporter = new ExcelExporter();

        ExcelFileDto excelFileDto = excelExporter.Export(financialReport);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    #endregion Web Apis

  }  // class FinancialReportsController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialReports
