/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Type Extension methods                  *
*  Type     : VouchersQueryExtensions                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Extension methods for VouchersQuery interface adapter.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Extension methods for VouchersQuery interface adapter.</summary>
  static internal class VouchersQueryExtensions {

    #region Extension methods

    static internal void EnsureIsValid(this VouchersQuery query) {
      // no-op
    }


    static internal string MapToFilterString(this VouchersQuery query) {
      string ledgerFilter = BuildLedgerFilter(query);
      string editedByFilter = BuildEditedByFilter(query);
      string accountingDateRangeFilter = BuildAccountingDateRangeFilter(query);
      string recordingDateRangeFilter = BuildRecordingDateRangeFilter(query);
      string transactionTypeFilter = BuildTransactionTypeFilter(query);
      string voucherTypeFilter = BuildVoucherTypeFilter(query);
      string stageStatusFilter = BuildStageStatusFilter(query);
      string statusFilter = BuildStatusFilter(query.Status);
      string keywordsFilter = BuildKeywordsFilter(query.Keywords);
      string conceptsFilter = BuildConceptFilter(query.Concept);
      string voucherIDFilter = BuildVoucherIDFilter(query.VoucherID);
      string numberFilter = BuildNumberFilter(query.Number);

      var filter = new Filter(ledgerFilter);
      filter.AppendAnd(editedByFilter);
      filter.AppendAnd(accountingDateRangeFilter);
      filter.AppendAnd(recordingDateRangeFilter);
      filter.AppendAnd(transactionTypeFilter);
      filter.AppendAnd(voucherTypeFilter);
      filter.AppendAnd(stageStatusFilter);
      filter.AppendAnd(statusFilter);
      filter.AppendAnd(conceptsFilter);
      filter.AppendAnd(voucherIDFilter);
      filter.AppendAnd(numberFilter);
      filter.AppendAnd(keywordsFilter);

      string transactionEntriesFilter = BuildTransactionEntriesFilter(query);

      filter.AppendAnd(transactionEntriesFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this VouchersQuery query) {
      if (query.OrderBy.Length != 0) {
        return query.OrderBy;
      } else {
        return "ID_MAYOR, NUMERO_TRANSACCION DESC, FECHA_REGISTRO DESC, FECHA_AFECTACION DESC, CONCEPTO_TRANSACCION";
      }
    }

    #endregion Extension methods

    #region Helpers

    static private string BuildAccountingDateRangeFilter(VouchersQuery query) {
      if (query.FromAccountingDate == ExecutionServer.DateMinValue && query.ToAccountingDate == ExecutionServer.DateMaxValue) {
        return string.Empty;
      }

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromAccountingDate)} <= FECHA_AFECTACION AND " +
             $"FECHA_AFECTACION < {DataCommonMethods.FormatSqlDbDate(query.ToAccountingDate.Date.AddDays(1))}";
    }


    static private string BuildConceptFilter(string keywords) {
      return SearchExpression.ParseLike("CONCEPTO_TRANSACCION", keywords.ToUpperInvariant());
    }


    static private string BuildEditedByFilter(VouchersQuery query) {
      if (query.EditorType.Length == 0 || query.EditorUID.Length == 0) {
        return string.Empty;
      }

      switch (query.EditorType) {
        case "ElaboratedBy":
          return $"ID_ELABORADA_POR = {query.EditorUID}";
        case "AuthorizedBy":
          return $"ID_AUTORIZADA_POR = {query.EditorUID}";
        case "PostedBy":
          return $"ID_ENVIADA_DIARIO_POR = {query.EditorUID}";
        default:
          return $"ID_ELABORADA_POR = {query.EditorUID} OR " +
                 $"ID_AUTORIZADA_POR = {query.EditorUID} OR " +
                 $"ID_ENVIADA_DIARIO_POR = {query.EditorUID}";
      }
    }


    static private string BuildVoucherIDFilter(string voucherID) {
      if (voucherID == string.Empty) {
        return string.Empty;
      }

      string[] array = voucherID.Split(',');
      string temp = string.Empty;

      foreach (string id in array) {
        var idAsString = EmpiriaString.TrimAll(id);

        if (!EmpiriaString.IsInteger(idAsString)) {
          continue;
        }

        if (temp.Length != 0) {
          temp += " OR ";
        }
        temp += $"ID_TRANSACCION = {EmpiriaString.TrimAll(idAsString)}";
      }

      return temp;
    }


    static private string BuildKeywordsFilter(string keywords) {
      return SearchExpression.ParseAndLikeKeywords("TRANSACCION_KEYWORDS", keywords);
    }


    static private string BuildLedgerFilter(VouchersQuery query) {
      string filter = string.Empty;

      if (query.LedgerUID.Length != 0) {
        var ledger = Ledger.Parse(query.LedgerUID);

        filter += $"ID_MAYOR = {ledger.Id}";

        return filter;
      }

      if (query.AccountsChartUID.Length != 0) {
        var accountsChart = AccountsChart.Parse(query.AccountsChartUID);
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += $"ID_TIPO_CUENTAS_STD = {accountsChart.Id}";
      }

      return filter;
    }


    static private string BuildMyInboxFilter() {
      string baseFilter = $"ESTA_ABIERTA <> 0 AND " +
                          $"(ID_ELABORADA_POR = {ExecutionServer.CurrentUserId} OR " +
                          $"ID_AUTORIZADA_POR = {ExecutionServer.CurrentUserId} " +
                           "{{WORKGROUP.CONDITION}})";

      string workgroupCondition = BuildWorkgroupConditionFilter();

      if (workgroupCondition.Length == 0) {
        return baseFilter.Replace("{{WORKGROUP.CONDITION}}", string.Empty);
      } else {
        return baseFilter.Replace("{{WORKGROUP.CONDITION}}", $" OR ({workgroupCondition})");
      }
    }



    static private string BuildNumberFilter(string number) {
      return SearchExpression.ParseLike("NUMERO_TRANSACCION", number.ToUpperInvariant());
    }


    static private string BuildRecordingDateRangeFilter(VouchersQuery query) {
      if (query.FromRecordingDate == ExecutionServer.DateMinValue && query.ToRecordingDate == ExecutionServer.DateMaxValue) {
        return string.Empty;
      }

      return $"{DataCommonMethods.FormatSqlDbDate(query.FromRecordingDate)} <= FECHA_REGISTRO AND " +
             $"FECHA_REGISTRO < {DataCommonMethods.FormatSqlDbDate(query.ToRecordingDate.Date.AddDays(1))}";
    }


    static private string BuildStageStatusFilter(VouchersQuery query) {
      switch(query.Stage) {
        case VoucherStage.All:
          return string.Empty;

        case VoucherStage.Completed:
          return "ESTA_ABIERTA = 0";

        case VoucherStage.Pending:
          return "ESTA_ABIERTA <> 0";

        case VoucherStage.ControlDesk:
          return $"ESTA_ABIERTA <> 0";

        case VoucherStage.MyInbox:
          return BuildMyInboxFilter();

        default:
          throw Assertion.EnsureNoReachThisCode();
      }
    }


    static private string BuildStatusFilter(VoucherStatus status) {
      if (status == VoucherStatus.All) {
        return string.Empty;
      }
      if (status == VoucherStatus.Posted) {
        return "ESTA_ABIERTA <> 0";
      }
      if (status == VoucherStatus.Pending) {
        return "ESTA_ABIERTA = 0 AND ID_AUTORIZADA_POR = 0";
      }
      if (status == VoucherStatus.Revision) {
        return "ESTA_ABIERTA = 0 AND ID_AUTORIZADA_POR > 0";
      }

      throw Assertion.EnsureNoReachThisCode($"Unrecognized status {status}.");
    }


    static private string BuildTransactionEntriesFilter(VouchersQuery query) {
      if (query.AccountKeywords.Length == 0 && query.SubledgerAccountKeywords.Length == 0) {
        return string.Empty;
      }
      string filter = string.Empty;

      if (query.AccountKeywords.Length != 0) {
        filter = SearchExpression.ParseAndLikeKeywords("CUENTA_ESTANDAR_KEYWORDS",
                                                       query.AccountKeywords);
      }

      if (query.SubledgerAccountKeywords.Length != 0) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += SearchExpression.ParseAndLikeKeywords("CUENTA_AUXILIAR_KEYWORDS",
                                                        query.SubledgerAccountKeywords);
      }

      string verficationNumberFilter = BuildVerificationNumberFilter(query.VerificationNumber);

      if (verficationNumberFilter != string.Empty) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += $"({verficationNumberFilter})";
      }

      return $"ID_TRANSACCION IN (SELECT ID_TRANSACCION FROM VW_COF_MOVIMIENTO_SEARCH WHERE {filter})";
    }


    static private string BuildTransactionTypeFilter(VouchersQuery query) {
      if (query.TransactionTypeUID.Length == 0) {
        return string.Empty;
      }

      var transactionType = TransactionType.Parse(query.TransactionTypeUID);

      return $"ID_TIPO_TRANSACCION = {transactionType.Id}";
    }


    static private string BuildVerificationNumberFilter(string verificationNumber) {
      if (verificationNumber == string.Empty) {
        return string.Empty;
      }

      string[] array = verificationNumber.Split(',');
      string temp = string.Empty;

      foreach (string verifNumber in array) {
        if (temp.Length != 0) {
          temp += " OR ";
        }
        temp += $"NUMERO_VERIFICACION = '{EmpiriaString.TrimAll(verifNumber)}'";
      }

      return temp;
    }


    static private string BuildVoucherTypeFilter(VouchersQuery query) {
      if (query.VoucherTypeUID.Length == 0) {
        return string.Empty;
      }

      var vocherType = VoucherType.Parse(query.VoucherTypeUID);

      return $"ID_TIPO_POLIZA = {vocherType.Id}";
    }


    static private string BuildWorkgroupConditionFilter() {
      var currentUser = Participant.Parse(ExecutionServer.CurrentUserId);

      FixedList<VoucherWorkgroup> userWorkgroups = VoucherWorkgroup.GetListFor(currentUser);

      var filter = new Filter();

      foreach (VoucherWorkgroup workgroup in userWorkgroups) {
        filter.AppendOr(workgroup.VouchersCondition);
      }

      return filter.ToString();
    }

    #endregion Helpers

  }  // class VouchersQueryExtensions

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
