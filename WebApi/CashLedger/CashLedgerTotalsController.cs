/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                  Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Web Api Controller              *
*  Type     : CashLedgerTotalsController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive cash ledger totals.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Web.Http;

using Empiria.WebApi;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.CashLedger.UseCases;

namespace Empiria.FinancialAccounting.CashLedger.WebApi {

  /// <summary>Query web API used to retrive cash ledger totals.</summary>
  public class CashLedgerTotalsController : WebApiController {

    #region Query web apis

    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/entries")]
    public CollectionModel GetCashLedgerEntries([FromBody] BaseCashLedgerTotalsQuery query) {

      using (var usecases = CashLedgerTotalsUseCases.UseCaseInteractor()) {
        FixedList<CashEntryDescriptor> entries = usecases.GetCashLedgerEntries(query);

        return new CollectionModel(base.Request, entries);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/cash-ledger/totals")]
    public CollectionModel GetCashLedgerTotals([FromBody] BaseCashLedgerTotalsQuery query) {

      using (var usecases = CashLedgerTotalsUseCases.UseCaseInteractor()) {
        FixedList<CashLedgerTotalEntryDto> totals = usecases.GetCashLedgerTotals(query);

        return new CollectionModel(base.Request, totals);
      }
    }

    #endregion Query web apis

  }  // class CashLedgerTotalsController

}  // namespace Empiria.FinancialAccounting.CashLedger.WebApi
