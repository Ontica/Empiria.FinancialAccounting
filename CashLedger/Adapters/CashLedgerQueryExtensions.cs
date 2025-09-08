/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Interface adapters                      *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Type Extension methods                  *
*  Type     : CashLedgerQueryExtensions                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for VouchersQuery interface adapter.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Linq;

using Empiria.Data;
using Empiria.StateEnums;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.Vouchers;

using Empiria.FinancialAccounting.CashLedger.Data;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Extension methods for VouchersQuery interface adapter.</summary>
  static internal class CashLedgerQueryExtensions {

    #region Extension methods

    static internal FixedList<CashEntryExtended> ExecuteAndGetEntries(this CashLedgerQuery query) {
      query.SearchEntries = true;

      string filter = GetFilterString(query);
      string sort = GetSortString(query);
      int pageSize = CalculatePageSize(query);

      FixedList<CashEntryExtended> entries = CashLedgerData.SearchEntries(filter, sort, pageSize);

      return entries;
    }


    static internal FixedList<CashTransaction> ExecuteAndGetTransactions(this CashLedgerQuery query) {
      query.SearchEntries = false;

      string filter = GetFilterString(query);
      string sort = GetSortString(query);
      int pageSize = CalculatePageSize(query);

      FixedList<CashTransaction> transactions = CashLedgerData.SearchTransactions(filter, sort, pageSize);

      return transactions;
    }

    #endregion Extension methods

    #region Methods

    static private int CalculatePageSize(CashLedgerQuery query) {

      int length = BuildAccountingDateRangeFilter(query).Length +
                   BuildRecordingDateRangeFilter(query).Length;

      if (length == 0) {
        return query.PageSize;
      } else {
        return 1000000;
      }
    }

    static private string GetFilterString(CashLedgerQuery query) {
      string accountingDateRangeFilter = BuildAccountingDateRangeFilter(query);
      string recordingDateRangeFilter = BuildRecordingDateRangeFilter(query);
      string ledgerFilter = BuildLedgerFilter(query.AccountingLedgerUID);
      string transactionStatusFilter = BuildTransactionStatusFilter(query.TransactionStatus);
      string cashAccountsStatusFilter = BuildCashAccountsStatusFilter(query.CashAccountStatus);
      string transactionTypeFilter = BuildTransactionTypeFilter(query.TransactionTypeUID);
      string voucherTypeFilter = BuildVoucherTypeFilter(query.VoucherTypeUID);
      string sourceFilter = BuildSourceFilter(query.SourceUID);
      string keywordsFilter = BuildKeywordsFilter(query.Keywords);
      string conceptsFilter = BuildConceptFilter(query.Keywords);
      string entriesFilter = BuildEntriesFilter(query);

      var filter = new Filter(accountingDateRangeFilter);

      filter.AppendAnd(recordingDateRangeFilter);
      filter.AppendAnd(ledgerFilter);

      filter.AppendAnd(transactionStatusFilter);
      filter.AppendAnd(cashAccountsStatusFilter);

      filter.AppendAnd(transactionTypeFilter);
      filter.AppendAnd(voucherTypeFilter);
      filter.AppendAnd(sourceFilter);

      filter.AppendAnd(keywordsFilter);
      filter.AppendAnd(conceptsFilter);
      filter.AppendAnd(entriesFilter);

      return filter.ToString();
    }


    static private string GetSortString(CashLedgerQuery query) {
      if (query.OrderBy.Length != 0) {
        return query.OrderBy;

      } else if (query.SearchEntries) {
        return "ID_MAYOR, NUMERO_TRANSACCION, ID_MOVIMIENTO";

      } else {
        return "ID_MAYOR, NUMERO_TRANSACCION";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildAccountsFilter(string[] accounts) {

      if (accounts.Length == 0) {
        return string.Empty;
      }

      var formatted = accounts.Select(x => $"{EmpiriaString.TrimAll(x)}%");

      return SearchExpression.ParseLike("NUMERO_CUENTA_ESTANDAR", formatted);
    }


    static private string BuildAccountingDateRangeFilter(CashLedgerQuery query) {

      if (ExecutionServer.IsMinOrMaxDate(query.FromAccountingDate) &&
          ExecutionServer.IsMinOrMaxDate(query.ToAccountingDate)) {

        return string.Empty;
      }

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromAccountingDate)} <= FECHA_AFECTACION AND " +
             $"FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(query.ToAccountingDate.Date.AddDays(1))}";
    }


    static private string BuildCashAccountsFilter(string[] cashAccounts) {
      if (cashAccounts.Length == 0) {
        return string.Empty;
      }

      return SearchExpression.ParseInSet("NUM_CONCEPTO_FLUJO", cashAccounts);
    }


    static private string BuildCashAccountsStatusFilter(CashAccountStatus cashAccountStatus) {

      switch (cashAccountStatus) {

        case CashAccountStatus.All:
          return string.Empty;

        case CashAccountStatus.Pending:
          return $"(FLUJO_PENDIENTE > 0)";

        case CashAccountStatus.NoCashFlow:
          return $"(SIN_FLUJO > 0)";

        case CashAccountStatus.CashFlowUnassigned:
          return $"(CON_FLUJO_SIN_ASIGNAR > 0)";

        case CashAccountStatus.CashFlowAssigned:
          return $"(CON_FLUJO > 0)";

        case CashAccountStatus.FalsePositive:
          return $"(FLUJO_INCORRECTO > 0)";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled cash account status {cashAccountStatus}.");
      }
    }


    static private string BuildConceptFilter(string keywords) {

      if (!keywords.StartsWith("@")) {
        return string.Empty;
      }

      keywords = EmpiriaString.TrimAll(keywords, "@", string.Empty);

      return SearchExpression.ParseLike("CONCEPTO_TRANSACCION", keywords.ToUpperInvariant());
    }


    static private string BuildKeywordsFilter(string keywords) {
      keywords = EmpiriaString.TrimAll(keywords, "@", string.Empty);

      return SearchExpression.ParseAndLikeKeywords("TRANSACCION_KEYWORDS", keywords);
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


    static private string BuildRecordingDateRangeFilter(CashLedgerQuery query) {

      if (ExecutionServer.IsMinOrMaxDate(query.FromRecordingDate) &&
          ExecutionServer.IsMinOrMaxDate(query.ToRecordingDate)) {

        return string.Empty;
      }

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromRecordingDate)} <= FECHA_REGISTRO AND " +
             $"FECHA_REGISTRO < {DataCommonMethods.FormatSqlDbDate(query.ToRecordingDate.Date.AddDays(1))}";
    }


    static private string BuildSourceFilter(string sourceUID) {
      if (sourceUID.Length == 0) {
        return string.Empty;
      }

      var source = FunctionalArea.Parse(sourceUID);

      return $"ID_FUENTE = {source.Id}";
    }


    static private string BuildTransactionStatusFilter(TransactionStatus status) {

      switch (status) {

        case TransactionStatus.All:
          return string.Empty;

        case TransactionStatus.Pending:
          return "(STATUS_FLUJO = 'P')";

        case TransactionStatus.Closed:
          return "(STATUS_FLUJO = 'C')";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled cash ledger transaction status '{status}'.");
      }
    }


    static private string BuildSubledgerAccountsFilter(string[] subledgerAccounts) {
      if (subledgerAccounts.Length == 0) {
        return string.Empty;
      }

      var filter = SearchExpression.ParseOrLikeKeywords("CUENTA_AUXILIAR_KEYWORDS",
                                                        string.Join(" , ", subledgerAccounts));
      return $"({filter})";
    }


    static private string BuildEntriesFilter(CashLedgerQuery query) {

      string accountsFilter = BuildAccountsFilter(query.VoucherAccounts);
      string subledgerAccountsFilter = BuildSubledgerAccountsFilter(query.SubledgerAccounts);
      string verificationNumbersFilter = BuildVerificationNumberFilter(query.VerificationNumbers);
      string cashAccountsFilter = BuildCashAccountsFilter(query.CashAccounts);

      var filter = new Filter(accountsFilter);

      filter.AppendAnd(subledgerAccountsFilter);
      filter.AppendAnd(verificationNumbersFilter);
      filter.AppendAnd(cashAccountsFilter);

      return filter.ToString();
    }


    static private string BuildTransactionTypeFilter(string transactionTypeUID) {
      if (transactionTypeUID.Length == 0) {
        return string.Empty;
      }

      var transactionType = TransactionType.Parse(transactionTypeUID);

      return $"ID_TIPO_TRANSACCION = {transactionType.Id}";
    }


    static private string BuildVerificationNumberFilter(string[] verificationNumbers) {
      if (verificationNumbers.Length == 0) {
        return string.Empty;
      }

      return SearchExpression.ParseInSet("NUMERO_VERIFICACION", verificationNumbers);
    }


    static private string BuildVoucherTypeFilter(string voucherTypeUID) {
      if (voucherTypeUID.Length == 0) {
        return string.Empty;
      }

      var vocherType = VoucherType.Parse(voucherTypeUID);

      return $"ID_TIPO_POLIZA = {vocherType.Id}";
    }

    #endregion Helpers

  }  // class CashLedgerQueryExtensions

} // namespace Empiria.FinancialAccounting.CashLedger.Adapters
