/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Domain types                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : TransactionSlipSearcher                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides search services over transaction slips (volantes).                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {

  /// <summary>Provides search services over transaction slips (volantes).</summary>
  internal class TransactionSlipSearcher {

    private readonly TransactionSlipsQuery _query;

    #region Constructors and parsers

    private TransactionSlipSearcher(TransactionSlipsQuery query) {
      _query = query;
    }


    static internal FixedList<TransactionSlip> Search(TransactionSlipsQuery query) {
      Assertion.Require("query", nameof(query));

      var searcher = new TransactionSlipSearcher(query);

      return searcher.ExecuteSearch();
    }


    #endregion Constructors and parsers

    #region Private methods

    private FixedList<TransactionSlip> ExecuteSearch() {
      string filter = FilterString();

      if (_query.Status == TransactionSlipStatus.Pending) {
        return TransactionSlipData.GetPendingTransactionSlips(filter);
      } else {
        return TransactionSlipData.GetProcessedTransactionSlips(filter);
      }
    }


    private string FilterString() {
      string accountsChartFilter = AccountsChartFilter();
      string transactionalSystemFilter = TransactionalSystemFilter();
      string datePeriodFilter = DatePeriodFilter();
      string statusFilter = StatusFilter();

      var filter = new Filter(accountsChartFilter);

      filter.AppendAnd(transactionalSystemFilter);
      filter.AppendAnd(datePeriodFilter);
      filter.AppendAnd(statusFilter);

      return filter.ToString();
    }


    private string AccountsChartFilter() {
      var accountsChart = AccountsChart.Parse(_query.AccountsChartUID);

      return $"ENC_TIPO_CONT = {accountsChart.Id}";
    }


    private string DatePeriodFilter() {
      string fieldName;

      switch (_query.DateSearchField) {
        case DateSearchField.AccountingDate:
          fieldName = "ENC_FECHA_VOL";
          break;
        case DateSearchField.RecordingDate:
          fieldName = "ENC_FECHA_CAP";
          break;

        case DateSearchField.None:
          return string.Empty;

        default:
          throw Assertion.EnsureNoReachThisCode();
      }

      return $"{CommonMethods.FormatSqlDbDate(_query.FromDate)} <= {fieldName} " +
             $"AND {fieldName} <= {CommonMethods.FormatSqlDbDate(_query.ToDate)}";
    }


    private string StatusFilter() {
      switch (_query.Status) {
        case TransactionSlipStatus.Pending:
          return String.Empty;

        case TransactionSlipStatus.Processed:
          return String.Empty;

        case TransactionSlipStatus.ProcessedOK:
          return "STATUS = 'C'";

        case TransactionSlipStatus.ProcessedWithIssues:
          return "STATUS = 'E'";

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    private string TransactionalSystemFilter() {
      var system = TransactionalSystem.Parse(_query.SystemUID);

      return $"ENC_SISTEMA = {system.SourceSystemId}";
    }

    #endregion Private methods

  }  // class TransactionSlipSearcher

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips
