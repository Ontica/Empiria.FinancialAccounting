/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Web api client                          *
*  Type     : CashLedgerTotalsServices                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides financial accounting cash ledger totals services using a web proxy.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting cash ledger totals services using a web proxy.</summary>
  public class CashLedgerTotalsServices : BaseService {

    public async Task<FixedList<T>> GetCashLedgerEntries<T>(BaseCashLedgerTotalsQuery query) where T : CashEntryDescriptor {
      string path = "v2/financial-accounting/cash-ledger/entries";

      return await WebApiClient.PostAsync<FixedList<T>>(query, path);
    }


    public async Task<FixedList<CashLedgerTotalEntryDto>> GetCashLedgerTotals(BaseCashLedgerTotalsQuery query) {

      string path = "v2/financial-accounting/cash-ledger/totals";

      return await WebApiClient.PostAsync<FixedList<CashLedgerTotalEntryDto>>(query, path);
    }

  }  // class CashLedgerTotalsServices

}  // namespace Empiria.FinancialAccounting.ClientServices
