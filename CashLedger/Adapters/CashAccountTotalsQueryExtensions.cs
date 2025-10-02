/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Interface adapters                      *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Type Extension methods                  *
*  Type     : CashAccountTotalsQueryExtensions           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for BaseCashLedgerTotalsQuery interface adapter.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

using Empiria.Financial.Adapters;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.CashLedger.Data;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Extension methods for BaseCashLedgerTotalsQuery interface adapter.</summary>
  static internal class CashAccountTotalsQueryExtensions {

    #region Extension methods

    static internal FixedList<CashAccountTotal> Execute(this RecordsSearchQuery query) {

      string filter = GetFilterString(query);

      if (query.QueryType == RecordSearchQueryType.None) {
      }

      return CashAccountTotalsData.GetTotalsByCashAccount(filter);
    }


    static public FixedList<CashEntryExtendedDto> ExecuteEntries(this RecordsSearchQuery query) {
      string filter = GetFilterString(query);

      FixedList<CashEntryExtended> entries = CashLedgerData.GetExtendedEntries(filter);

      return CashTransactionMapper.MapToDescriptor(entries);
    }

    #endregion Extension methods

    #region Methods

    static private string GetFilterString(RecordsSearchQuery query) {
      string ledgerFilter = BuildLedgerFilter(query.Ledgers);
      string accountingDateRangeFilter = BuildAccountingDateRangeFilter(query);

      var filter = new Filter(ledgerFilter);

      filter.AppendAnd(accountingDateRangeFilter);

      return filter.ToString();
    }

    #endregion Methods

    #region Helpers

    static private string BuildAccountingDateRangeFilter(RecordsSearchQuery query) {
      Assertion.Require(query.FromDate, nameof(query.FromDate));
      Assertion.Require(query.ToDate, nameof(query.ToDate));

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromDate)} <= FECHA_AFECTACION AND " +
             $"FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(query.ToDate.Date.AddDays(1))}";
    }


    static private string BuildLedgerFilter(string[] ledgers) {
      string filter = string.Empty;

      if (ledgers.Length != 0) {
        var ledger = Ledger.Parse(ledgers[0]);

        return $"ID_MAYOR = {ledger.Id}";
      }

      return $"ID_TIPO_CUENTAS_STD = {AccountsChart.IFRS.Id}";
    }

    #endregion Helpers

  }  // class CashAccountTotalsQueryExtensions

} // namespace Empiria.FinancialAccounting.CashLedger.Adapters
