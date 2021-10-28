/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : SATReportsController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive operational reports.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BanobrasIntegration;

using Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports;


namespace Empiria.FinancialAccounting.WebApi.SATReports {

  /// <summary>Query web API used to retrive SAT Reports.</summary>
  public class OperationalReportsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/operational-reports")]
    public SingleObjectModel GetOperationalReport([FromBody] OperationalReportCommand command) {
      base.RequireBody(command);

      using (var usecases = OperationalReportsUseCases.UseCaseInteractor()) {
        OperationalReportDto reportData = usecases.GetOperationalReport(command);

        return new SingleObjectModel(this.Request, reportData);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/operational-reports/export")]
    public SingleObjectModel ExportOperationReport([FromBody] OperationalReportCommand command) {
      base.RequireBody(command);

      using (var usecases = OperationalReportsUseCases.UseCaseInteractor()) {
        FileReportDto fileReportDto = usecases.ExportOperationalReport(command);

        return new SingleObjectModel(this.Request, fileReportDto);
      }

    }

    #endregion Web Apis

  } // class SATReportsController

} // namespace Empiria.FinancialAccounting.WebApi.SATReports
