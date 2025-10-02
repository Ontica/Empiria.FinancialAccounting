/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                  Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Web Api Controller              *
*  Type     : CashAccountTotalsController                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive cash ledger accounts totals.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.Financial.Adapters;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.CashLedger.UseCases;

namespace Empiria.FinancialAccounting.CashLedger.WebApi {

  /// <summary>Query web API used to retrive cash ledger accounts totals.</summary>
  public class CashAccountTotalsController : WebApiController {

    #region Query web apis

    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/entries")]
    public CollectionModel GetCashLedgerEntries([FromBody] RecordsSearchQuery query) {

      using (var usecases = CashAccountTotalsUseCases.UseCaseInteractor()) {
        FixedList<CashEntryExtendedDto> entries = usecases.GetCashLedgerEntries(query);

        return new CollectionModel(base.Request, entries);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/totals")]
    public CollectionModel GetCashLedgerTotals([FromBody] RecordsSearchQuery query) {

      using (var usecases = CashAccountTotalsUseCases.UseCaseInteractor()) {
        FixedList<CashAccountTotalDto> totals = usecases.GetCashLedgerTotals(query);

        return new CollectionModel(base.Request, totals);
      }
    }

    #endregion Query web apis

  }  // class CashAccountTotalsController

}  // namespace Empiria.FinancialAccounting.CashLedger.WebApi
