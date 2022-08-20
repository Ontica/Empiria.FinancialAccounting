/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : AccountStatementController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to generate accounting account's statements.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.UseCases;

namespace Empiria.FinancialAccounting.WebApi.Reporting {

  /// <summary>Query web API used to generate accounting account's statements.</summary>
  public class AccountStatementController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/account-statement")]
    public SingleObjectModel BuildAccountStatement([FromBody] AccountStatementQuery buildQuery) {

      base.RequireBody(buildQuery);

      using (var usecases = AccountStatementUseCases.UseCaseInteractor()) {

        AccountStatementDto accountStatement = usecases.BuildAccountStatement(buildQuery);

        return new SingleObjectModel(this.Request, accountStatement);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/account-statement/excel")]
    public SingleObjectModel ExportAccountStatementToExcel([FromBody] AccountStatementQuery buildQuery) {

      base.RequireBody(buildQuery);

      using (var usecases = AccountStatementUseCases.UseCaseInteractor()) {

        AccountStatementDto accountStatement = usecases.BuildAccountStatement(buildQuery);

        var excelExporter = new AccountStatementExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(accountStatement);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }

    #endregion Web Apis

  } // class AccountStatementController

} // namespace Empiria.FinancialAccounting.WebApi.Reporting
