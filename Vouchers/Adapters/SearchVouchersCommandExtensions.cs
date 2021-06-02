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
      command.Keywords = command.Keywords ?? String.Empty;
      command.OrderBy = command.OrderBy ?? "ID_MAYOR, NUMERO_TRANSACCION DESC";
      command.PageSize = command.PageSize <= 0 ? 200 : command.PageSize;
      command.Page = command.Page <= 0 ? 1 : command.Page;
    }


    static internal string MapToFilterString(this SearchVouchersCommand command) {
      string stageStatusFilter = BuildStageStatusFilter(command.Stage, command.Status);
      string keywordsFilter = BuildKeywordsFilter(command.Keywords);

      var filter = new Filter(stageStatusFilter);

      filter.AppendAnd(keywordsFilter);

      return filter.ToString();
    }


    static internal string MapToSortString(this SearchVouchersCommand command) {
      if (!String.IsNullOrWhiteSpace(command.OrderBy)) {
        return command.OrderBy;
      } else {
        return "ID_MAYOR, NUMERO_TRANSACCION DESC";
      }
    }

    #endregion Extension methods

    #region Private methods

    static private string BuildKeywordsFilter(string keywords) {
      return SearchExpression.ParseAndLikeKeywords("KEYWORDS", keywords);
    }


    static private string BuildStageStatusFilter(VoucherStage stage, VoucherStatus status) {
      return string.Empty;
    }


    #endregion Private methods

  }  // class SearchVoucherCommandExtensions

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
