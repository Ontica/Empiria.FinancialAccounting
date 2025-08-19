/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Interface adapters                      *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Type Extension methods                  *
*  Type     : CashLedgerQueryExtensions                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for VouchersQuery interface adapter.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Data;
using Empiria.StateEnums;

using Empiria.Financial.Integration.CashLedger;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Extension methods for VouchersQuery interface adapter.</summary>
  static internal class CashLedgerQueryExtensions {

    #region Extension methods

    static internal int CalculatePageSize(this CashLedgerQuery query) {
      string datesFilter = BuildAccountingDateRangeFilter(query) + BuildRecordingDateRangeFilter(query);

      if (datesFilter.Length == 0) {
        return query.PageSize;
      } else {
        return 1000000;
      }
    }


    static internal void EnsureIsValid(this CashLedgerQuery query) {
      // no-op
    }


    static internal string MapToFilterString(this CashLedgerQuery query) {
      string ledgerFilter = BuildLedgerFilter(query.AccountingLedgerUID);
      string accountingDateRangeFilter = BuildAccountingDateRangeFilter(query);
      string recordingDateRangeFilter = BuildRecordingDateRangeFilter(query);
      string transactionTypeFilter = BuildTransactionTypeFilter(query.TransactionTypeUID);
      string voucherTypeFilter = BuildVoucherTypeFilter(query.VoucherTypeUID);
      string sourceFilter = BuildSourceFilter(query.SourceUID);
      string transactionStatusFilter = BuildTransactionStatusFilter(query.TransactionStatus);
      string keywordsFilter = BuildKeywordsFilter(query.Keywords);
      string conceptsFilter = BuildConceptFilter(query.Keywords);

      var filter = new Filter(ledgerFilter);

      filter.AppendAnd(accountingDateRangeFilter);
      filter.AppendAnd(recordingDateRangeFilter);

      filter.AppendAnd(transactionTypeFilter);
      filter.AppendAnd(voucherTypeFilter);
      filter.AppendAnd(sourceFilter);
      filter.AppendAnd(transactionStatusFilter);
      filter.AppendAnd(conceptsFilter);
      filter.AppendAnd(keywordsFilter);

      if (query.SearchEntries) {
        string entriesFilter = BuildEntriesFilter(query);

        filter.AppendAnd(entriesFilter);
      } else {
        string transactionEntriesFilter = BuildTransactionEntriesFilter(query);

        filter.AppendAnd(transactionEntriesFilter);
      }

      return filter.ToString();
    }


    static internal string MapToSortString(this CashLedgerQuery query) {
      if (query.OrderBy.Length != 0) {
        return query.OrderBy;
      } else {
        return "ID_MAYOR, FECHA_AFECTACION, NUMERO_TRANSACCION";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildAccountsFilter(string[] accounts) {

      if (accounts.Length == 0) {
        return string.Empty;
      }

      var filter = new Filter();

      foreach (string account in accounts) {

        if (EmpiriaString.TrimAll(account).Length == 0) {
          continue;
        }

        var temp = $"NUMERO_CUENTA_ESTANDAR LIKE '{EmpiriaString.TrimAll(account)}%'";

        filter.AppendOr(temp);
      }

      return $"({filter.ToString()})";
    }


    static private string BuildAccountingDateRangeFilter(CashLedgerQuery query) {
      if (query.FromAccountingDate == ExecutionServer.DateMinValue && query.ToAccountingDate == ExecutionServer.DateMaxValue) {
        return string.Empty;
      }

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromAccountingDate)} <= FECHA_AFECTACION AND " +
             $"FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(query.ToAccountingDate.Date.AddDays(1))}";
    }


    static private string BuildCashAccountStatusFilter(SharedCashAccountStatus cashAccountStatus) {
      switch (cashAccountStatus) {
        case SharedCashAccountStatus.All:
          return string.Empty;

        case SharedCashAccountStatus.CashAccountPending:
          return $"(ID_MOVIMIENTO_REFERENCIA = 0)";

        case SharedCashAccountStatus.CashAccountWaiting:
          return $"(ID_MOVIMIENTO_REFERENCIA = -2)";

        case SharedCashAccountStatus.NoCashAccount:
          return $"(ID_MOVIMIENTO_REFERENCIA = -1)";

        case SharedCashAccountStatus.WithCashAccount:
          return $"(ID_MOVIMIENTO_REFERENCIA > 0)";

        default:
          throw Assertion.EnsureNoReachThisCode();
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
      if (query.FromRecordingDate == ExecutionServer.DateMinValue && query.ToRecordingDate == ExecutionServer.DateMaxValue) {
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
      var startDateFilter = $"FECHA_AFECTACION >= {DataCommonMethods.FormatSqlDbDate(new DateTime(2025, 5, 1))}";

      switch (status) {
        case TransactionStatus.All:
          return $"(ESTA_ABIERTA = 0 AND {startDateFilter})";

        case TransactionStatus.Closed:
          return $"(ESTA_ABIERTA = 0 AND {startDateFilter})";

        case TransactionStatus.Pending:
          return $"(ESTA_ABIERTA = 0 AND {startDateFilter})";

        default:
          throw Assertion.EnsureNoReachThisCode();
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
      string cashAccountStatusFilter = BuildCashAccountStatusFilter(query.CashAccountStatus);
      string accountsFilter = BuildAccountsFilter(query.VoucherAccounts);
      string subledgerAccountsFilter = BuildSubledgerAccountsFilter(query.SubledgerAccounts);
      string verificationNumbersFilter = BuildVerificationNumberFilter(query.VerificationNumbers);

      var filter = new Filter(cashAccountStatusFilter);

      filter.AppendAnd(accountsFilter);
      filter.AppendAnd(subledgerAccountsFilter);
      filter.AppendAnd(verificationNumbersFilter);

      return filter.ToString();
    }


    static private string BuildTransactionEntriesFilter(CashLedgerQuery query) {

      string filter = BuildEntriesFilter(query);

      if (filter.Length == 0) {
        return string.Empty;
      }

      return $"ID_TRANSACCION IN (SELECT ID_TRANSACCION FROM VW_COF_MOVIMIENTO WHERE {filter})";
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

      string temp = string.Empty;

      foreach (string item in verificationNumbers) {
        if (EmpiriaString.TrimAll(item).Length == 0) {
          continue;
        }
        if (temp.Length != 0) {
          temp += " OR ";
        }
        temp += $"NUMERO_VERIFICACION = '{EmpiriaString.TrimAll(item)}'";
      }

      return $"({temp})";
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
