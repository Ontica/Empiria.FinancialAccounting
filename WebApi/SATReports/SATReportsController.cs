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

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports;
using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.UseCases;
using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters;

namespace Empiria.FinancialAccounting.WebApi.SATReports {

  /// <summary>Query web API used to retrive SAT Reports.</summary>
  public class SATReportsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/operational-reports")]
    public SingleObjectModel GetOperationalReport([FromBody] OperationalReportCommand command) {
      base.RequireBody(command);

      using (var usecases = OperationalReportsUseCases.UseCaseInteractor()) {
        OperationalReportDto operationalReport = usecases.GetOperationalReport(command);

        return new SingleObjectModel(this.Request, operationalReport);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/operational-reports/xml")]
    public SingleObjectModel GetExcelTrialBalance([FromBody] OperationalReportCommand command) {
      base.RequireBody(command);
      
      using (var usecases = OperationalReportsUseCases.UseCaseInteractor()) {
        OperationalReportDto operationalReport = usecases.GetOperationalReport(command);

        var operationalExporter = new OperationalReportExporter();

        FileReportDto xmlFileDto = operationalExporter.Export(operationalReport, command);

        return new SingleObjectModel(this.Request, xmlFileDto);
      }

    }

    #endregion Web Apis

  } // class SATReportsController

} // namespace Empiria.FinancialAccounting.WebApi.SATReports
