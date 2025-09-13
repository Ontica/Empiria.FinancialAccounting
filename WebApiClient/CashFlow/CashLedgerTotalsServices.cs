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

using Empiria.Financial.Adapters;

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting cash ledger totals services using a web proxy.</summary>
  public class CashLedgerTotalsServices : BaseService {

    public Task<FixedList<CashAccountTotalDto>> GetCashAccountTotals(CashAccountTotalsQuery query) {

      string path = "v2/financial-accounting/cash-ledger/totals";

      return WebApiClient.PostAsync<FixedList<CashAccountTotalDto>>(query, path);
    }


    public Task<FixedList<T>> GetCashLedgerEntries<T>(CashAccountTotalsQuery query)
                                                                      where T : BaseCashEntryDto {
      string path = "v2/financial-accounting/cash-ledger/entries";

      return WebApiClient.PostAsync<FixedList<T>>(query, path);
    }

  }  // class CashLedgerTotalsServices

}  // namespace Empiria.FinancialAccounting.ClientServices
