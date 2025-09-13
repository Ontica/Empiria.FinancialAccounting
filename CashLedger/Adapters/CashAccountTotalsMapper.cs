/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Mapper                                  *
*  Type     : CashAccountTotalsMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides mapping services for cash ledger accounts totals.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.CashFlow.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Provides mapping services for cash ledger accounts totals.</summary>
  static internal class CashAccountTotalsMapper {

    static internal FixedList<CashAccountTotalDto> Map(FixedList<CashAccountTotal> totals) {
      return totals.Select(x => Map(x))
                   .ToFixedList();
    }

    #region Helpers

    static private CashAccountTotalDto Map(CashAccountTotal total) {
      return new CashAccountTotalDto {
        AccountNo = total.LedgerAccount.Number,
        AccountName = total.LedgerAccount.Name,
        CashAccountId = total.CashAccountId,
        CashAccountNo = total.CashAccountNo,
        CurrencyCode = total.Currency.Code,
        Debit = total.Debit,
        Credit = total.Credit
      };
    }

    #endregion Helpers

  }  // class CashAccountTotalsMapper

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
