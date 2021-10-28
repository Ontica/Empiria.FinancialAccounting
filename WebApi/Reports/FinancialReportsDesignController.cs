/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                            Component : Web Api                               *
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
    [Route("v2/financial-accounting/financial-reports/design/{financialReportTypeUID}")]
    public SingleObjectModel GetFinancialReportDesign([FromUri] string financialReportTypeUID) {
      using (var usecases = FinancialReportDesignUseCases.UseCaseInteractor()) {
        FinancialReportDesignDto design = usecases.FinancialReportDesign(financialReportTypeUID);

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

    #endregion Web Apis

  }  // class FinancialReportsDesignController

}  // namespace Empiria.FinancialAccounting.WebApi.FinancialReports
