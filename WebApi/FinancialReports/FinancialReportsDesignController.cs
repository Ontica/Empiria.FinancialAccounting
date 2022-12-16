/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : FinancialReportsDesignController             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to return financial reports design configuration.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.UseCases;

namespace Empiria.FinancialAccounting.WebApi.FinancialReports {

  /// <summary>Query web API used to return financial reports design configuration.</summary>
  public class FinancialReportsDesignController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}")]
    public SingleObjectModel GetFinancialReportDesign([FromUri] string reportTypeUID) {
      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FinancialReportDesignDto design = usecases.FinancialReportDesign(reportTypeUID);

        return new SingleObjectModel(base.Request, design);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/financial-reports/design/types/{accountsChartUID:guid}")]
    public CollectionModel GetFinancialReportTypesForDesign([FromUri] string accountsChartUID) {
      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> list = usecases.FinancialReportTypesForDesign(accountsChartUID);

        return new CollectionModel(base.Request, list);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}/cells")]
    public SingleObjectModel InsertCell([FromUri] string reportTypeUID,
                                        [FromBody] EditFinancialReportCommand command) {

      base.RequireBody(command);

      command.Payload.ReportTypeUID = reportTypeUID;

      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FinancialReportCellDto cell = usecases.InsertCell(command);

        return new SingleObjectModel(base.Request, cell);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}/rows")]
    public SingleObjectModel InsertRow([FromUri] string reportTypeUID,
                                       [FromBody] EditFinancialReportCommand command) {

      base.RequireBody(command);

      command.Payload.ReportTypeUID = reportTypeUID;

      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FinancialReportRowDto row = usecases.InsertRow(command);

        return new SingleObjectModel(base.Request, row);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}/cells/{cellUID:guid}")]
    public NoDataModel RemoveCell([FromUri] string reportTypeUID,
                                  [FromUri] string cellUID) {

      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        usecases.RemoveCell(reportTypeUID, cellUID);

        return new NoDataModel(base.Request);
      }
    }


    [HttpDelete]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}/rows/{rowUID:guid}")]
    public NoDataModel RemoveRow([FromUri] string reportTypeUID,
                                 [FromUri] string rowUID) {

      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        usecases.RemoveRow(reportTypeUID, rowUID);

        return new NoDataModel(base.Request);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}/cells/{cellUID:guid}")]
    public SingleObjectModel UpdateCell([FromUri] string reportTypeUID,
                                        [FromUri] string cellUID,
                                        [FromBody] EditFinancialReportCommand command) {

      base.RequireBody(command);

      command.Payload.ReportTypeUID = reportTypeUID;
      command.Payload.ReportItemUID = cellUID;

      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FinancialReportCellDto cell = usecases.UpdateCell(cellUID, command);

        return new SingleObjectModel(base.Request, cell);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v2/financial-accounting/financial-reports/design/{reportTypeUID}/rows/{rowUID:guid}")]
    public SingleObjectModel UpdateRow([FromUri] string reportTypeUID,
                                       [FromUri] string rowUID,
                                       [FromBody] EditFinancialReportCommand command) {

      base.RequireBody(command);

      command.Payload.ReportTypeUID = reportTypeUID;
      command.Payload.ReportItemUID = rowUID;

      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FinancialReportRowDto row = usecases.UpdateRow(rowUID, command);

        return new SingleObjectModel(base.Request, row);
      }
    }

    #endregion Web Apis

  }  // class FinancialReportsDesignController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialReports
