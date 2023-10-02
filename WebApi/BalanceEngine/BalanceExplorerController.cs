/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                               Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : BalanceExplorerController                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to interfact with the balance explorer.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.FinancialAccounting.Reporting.Balances;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.WebApi.BalanceEngine {

  /// <summary>Query web API used to interfact with the balance explorer.</summary>
  public class BalanceExplorerController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v2/financial-accounting/balance-explorer/balances")]
    public SingleObjectModel GetBalancesForBalanceExplorer([FromBody] BalanceExplorerQuery query) {
      base.RequireBody(query);

      using (var usecases = BalanceExplorerUseCases.UseCaseInteractor()) {

        BalanceExplorerDto balancesForBalanceExplorer = usecases.GetBalances(query);

        return new SingleObjectModel(this.Request, balancesForBalanceExplorer);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-explorer/balances/excel")]
    public SingleObjectModel ExportBalanceExplorerBalancesToExcel(
                                         [FromBody] BalanceExplorerQuery query) {
      base.RequireBody(query);

      using (var usecases = BalanceExplorerUseCases.UseCaseInteractor()) {

        BalanceExplorerDto balanceExplorerBalances = usecases.GetBalances(query);

        var excelExporter = new BalancesExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(balanceExplorerBalances);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }

    #endregion Web Apis

  } // class BalanceExplorerController

} // namespace Empiria.FinancialAccounting.WebApi.BalanceEngine
