/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Mapper                                  *
*  Type     : CashLedgerTotalsMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides mapping services for cash ledger totals.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Provides mapping services for cash ledger totals.</summary>
  static internal class CashLedgerTotalsMapper {

    static internal FixedList<CashLedgerTotalEntryDto> Map(FixedList<CashLedgerTotal> totals) {
      return totals.Select(x => Map(x))
                   .ToFixedList();
    }

    #region Helpers

    static private CashLedgerTotalEntryDto Map(CashLedgerTotal total) {
      return new CashLedgerTotalEntryDto {
        CashAccountId = total.CashAccountId,
        CashAccountNo = total.CashAccountNo,
        CurrencyId = total.Currency.Id,
        Debit = total.Debit,
        Credit = total.Credit,
      };
    }

    #endregion Helpers

  }  // class CashLedgerTotalsMapper

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
