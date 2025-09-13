/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Interface adapters                      *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Type Extension methods                  *
*  Type     : CashAccountTotalsQueryExtensions           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for BaseCashLedgerTotalsQuery interface adapter.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.CashFlow.CashLedger.Adapters;
using Empiria.Data;
using Empiria.Financial.Adapters;
using Empiria.FinancialAccounting.CashLedger.Data;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Extension methods for BaseCashLedgerTotalsQuery interface adapter.</summary>
  static internal class CashAccountTotalsQueryExtensions {

    #region Extension methods

    static internal FixedList<CashAccountTotal> Execute(this CashAccountTotalsQuery query) {

      string filter = GetFilterString(query);

      FixedList<CashAccountTotal> totals = CashAccountTotalsData.GetTotals(filter);

      return totals;
    }


    static public FixedList<CashEntryExtendedDto> ExecuteEntries(this CashAccountTotalsQuery query) {
      string filter = GetFilterString(query);

      FixedList<CashEntryExtended> entries = CashLedgerData.GetExtendedEntries(filter);

      return CashTransactionMapper.MapToDescriptor(entries);
    }

    #endregion Extension methods

    #region Methods

    static private string GetFilterString(CashAccountTotalsQuery query) {
      string ledgerFilter = BuildLedgerFilter(query.AccountingLedgerUID);
      string accountingDateRangeFilter = BuildAccountingDateRangeFilter(query);

      var filter = new Filter(ledgerFilter);

      filter.AppendAnd(accountingDateRangeFilter);

      return filter.ToString();
    }

    #endregion Methods

    #region Helpers

    static private string BuildAccountingDateRangeFilter(CashAccountTotalsQuery query) {
      Assertion.Require(query.FromDate, nameof(query.FromDate));
      Assertion.Require(query.ToDate, nameof(query.ToDate));

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromDate)} <= FECHA_AFECTACION AND " +
             $"FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(query.ToDate.Date.AddDays(1))}";
    }


    static private string BuildLedgerFilter(string accountingLedgerUID) {
      string filter = string.Empty;

      if (accountingLedgerUID.Length != 0) {
        var ledger = Ledger.Parse(accountingLedgerUID);

        filter += $"ID_MAYOR = {ledger.Id}";

        return filter;
      }

      return $"ID_TIPO_CUENTAS_STD = {AccountsChart.IFRS.Id}";
    }

    #endregion Helpers

  }  // class CashAccountTotalsQueryExtensions

} // namespace Empiria.FinancialAccounting.CashLedger.Adapters
