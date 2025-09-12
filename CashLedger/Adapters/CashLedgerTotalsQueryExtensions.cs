/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Interface adapters                      *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Type Extension methods                  *
*  Type     : CashLedgerTotalsQueryExtensions            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for BaseCashLedgerTotalsQuery interface adapter.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.CashLedger.Data;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Extension methods for BaseCashLedgerTotalsQuery interface adapter.</summary>
  static internal class CashLedgerTotalsQueryExtensions {

    #region Extension methods

    static internal FixedList<CashLedgerTotal> Execute(this BaseCashLedgerTotalsQuery query) {

      string filter = GetFilterString(query);

      FixedList<CashLedgerTotal> totals = CashLedgerTotalsData.GetTotals(filter);

      return totals;
    }


    static public FixedList<CashEntryDescriptor> ExecuteEntries(this BaseCashLedgerTotalsQuery query) {
      string filter = GetFilterString(query);

      FixedList<CashEntryExtended> entries = CashLedgerTotalsData.GetEntries(filter);

      return CashTransactionMapper.MapToDescriptor(entries);
    }

    #endregion Extension methods

    #region Methods

    static private string GetFilterString(BaseCashLedgerTotalsQuery query) {
      string ledgerFilter = BuildLedgerFilter(query.AccountingLedgerUID);
      string accountingDateRangeFilter = BuildAccountingDateRangeFilter(query);

      var filter = new Filter(ledgerFilter);

      filter.AppendAnd(accountingDateRangeFilter);

      return filter.ToString();
    }

    #endregion Methods

    #region Helpers

    static private string BuildAccountingDateRangeFilter(BaseCashLedgerTotalsQuery query) {
      Assertion.Require(query.FromAccountingDate, nameof(query.FromAccountingDate));
      Assertion.Require(query.ToAccountingDate, nameof(query.ToAccountingDate));

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromAccountingDate)} <= FECHA_AFECTACION AND " +
             $"FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(query.ToAccountingDate.Date.AddDays(1))}";
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

  }  // class CashLedgerTotalsQueryExtensions

} // namespace Empiria.FinancialAccounting.CashLedger.Adapters
