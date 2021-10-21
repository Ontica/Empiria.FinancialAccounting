/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : SATReportsController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive SAT Reports.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports;
using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;

namespace Empiria.FinancialAccounting.WebApi.SATReports {

  /// <summary>Query web API used to retrive SAT Reports.</summary>
  public class SATReportsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/sat-reports/trial-balance")]
    public SingleObjectModel GetTrialBalance([FromBody] TrialBalanceCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(command);

        return new SingleObjectModel(this.Request, trialBalance);
      }
    }



    [HttpPost]
    [Route("v2/financial-accounting/sat-reports/trial-balance/xml")]
    public SingleObjectModel GetExcelTrialBalance([FromBody] TrialBalanceCommand command) {
      base.RequireBody(command);

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(command);

        var xmlExporter = new XmlExporter();

        XmlFileDto xmlFileDto = xmlExporter.Exporter(trialBalance);

        return new SingleObjectModel(this.Request, xmlFileDto);
      }
    }


    #endregion Web Apis

  } // class SATReportsController

} // namespace Empiria.FinancialAccounting.WebApi.SATReports
