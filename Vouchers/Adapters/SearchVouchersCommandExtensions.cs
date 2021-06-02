/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Type Extension methods                  *
*  Type     : SearchVoucherCommandExtensions             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for SearchVoucherCommand interface adapter.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Extension methods for SearchVoucherCommand interface adapter.</summary>
  static internal class SearchVoucherCommandExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this SearchVouchersCommand command) {
      // no-op
    }


    static internal string MapToFilterString(this SearchVouchersCommand command) {
      string ledgerFilter = BuildLedgerFilter(command);
      string dateRangeFilter = BuildDateRangerFilter(command);
      string transactionTypeFilter = BuildTransactionTypeFilter(command);
      string voucherTypeFilter = BuildVoucherTypeFilter(command);
      string stageStatusFilter = BuildStageStatusFilter(command);
      string keywordsFilter = BuildKeywordsFilter(command.Keywords);

      var filter = new Filter(ledgerFilter);
      filter.AppendAnd(dateRangeFilter);
      filter.AppendAnd(transactionTypeFilter);
      filter.AppendAnd(voucherTypeFilter);
      filter.AppendAnd(stageStatusFilter);
      filter.AppendAnd(keywordsFilter);

      string transactionEntriesFilter = BuildTransactionEntriesFilter(command, filter.ToString());

      filter.AppendAnd(transactionEntriesFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this SearchVouchersCommand command) {
      if (command.OrderBy.Length != 0) {
        return command.OrderBy;
      } else {
        return "ID_MAYOR, NUMERO_TRANSACCION DESC";
      }
    }

    #endregion Extension methods

    #region Private methods

    static private string BuildDateRangerFilter(SearchVouchersCommand command) {
      if (command.DateSearchField == DateSearchField.None) {
        return string.Empty;
      }

      string filter = $"'{CommonMethods.FormatSqlDate(command.FromDate)}' <= @DATE_FIELD@ AND " +
                      $"@DATE_FIELD@ < '{CommonMethods.FormatSqlDate(command.ToDate.Date.AddDays(1))}'";

      if (command.DateSearchField == DateSearchField.AccountingDate) {
        return filter.Replace("@DATE_FIELD@", "FECHA_AFECTACION");

      } else if (command.DateSearchField == DateSearchField.RecordingDate) {
        return filter.Replace("@DATE_FIELD@", "FECHA_REGISTRO");

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    static private string BuildLedgerFilter(SearchVouchersCommand command) {
      string filter = string.Empty;

      if (command.LedgerUID.Length != 0) {
        var ledger = Ledger.Parse(command.LedgerUID);

        filter += $"ID_MAYOR = {ledger.Id}";

        return filter;
      }

      if (command.AccountsChartUID.Length != 0) {
        var accountsChart = AccountsChart.Parse(command.AccountsChartUID);
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += $"ID_TIPO_CUENTAS_STD = {accountsChart.Id}";
      }

      return filter;
    }


    static private string BuildKeywordsFilter(string keywords) {
      return SearchExpression.ParseAndLikeKeywords("TRANSACCION_KEYWORDS", keywords);
    }


    static private string BuildStageStatusFilter(SearchVouchersCommand command) {
      return string.Empty;
    }


    static private string BuildTransactionEntriesFilter(SearchVouchersCommand command, string nestedFilter) {
      if (command.AccountKeywords.Length == 0 && command.SubledgerAccountKeywords.Length == 0) {
        return string.Empty;
      }
      string filter = string.Empty;

      if (command.AccountKeywords.Length != 0) {
        filter = SearchExpression.ParseAndLikeKeywords("CUENTA_ESTANDAR_KEYWORDS",
                                                       command.AccountKeywords);
      }
      if (command.SubledgerAccountKeywords.Length != 0) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += SearchExpression.ParseAndLikeKeywords("CUENTA_AUXILIAR_KEYWORDS",
                                                        command.SubledgerAccountKeywords);
      }

      if (nestedFilter.Length != 0) {
        return $"ID_TRANSACCION IN (SELECT ID_TRANSACCION FROM VW_COF_MOVIMIENTO_SEARCH WHERE ({nestedFilter} AND {filter}))";
      } else {
        return $"ID_TRANSACCION IN (SELECT ID_TRANSACCION FROM VW_COF_MOVIMIENTO_SEARCH WHERE {filter})";
      }

    }

    static private string BuildTransactionTypeFilter(SearchVouchersCommand command) {
      if (command.TransactionTypeUID.Length == 0) {
        return string.Empty;
      }

      var transactionType = TransactionType.Parse(command.TransactionTypeUID);

      return $"ID_TIPO_TRANSACCION = {transactionType.Id}";
    }


    static private string BuildVoucherTypeFilter(SearchVouchersCommand command) {
      if (command.VoucherTypeUID.Length == 0) {
        return string.Empty;
      }

      var vocherType = VoucherType.Parse(command.VoucherTypeUID);

      return $"ID_TIPO_POLIZA = {vocherType.Id}";
    }

    #endregion Private methods

  }  // class SearchVoucherCommandExtensions

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
