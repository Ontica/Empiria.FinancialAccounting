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

using Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {

  /// <summary>Provides search services over transaction slips (volantes).</summary>
  internal class TransactionSlipSearcher {

    private readonly SearchTransactionSlipsCommand _command;

    #region Constructors and parsers

    private TransactionSlipSearcher(SearchTransactionSlipsCommand command) {
      _command = command;
    }


    static internal FixedList<TransactionSlip> Search(SearchTransactionSlipsCommand command) {
      var searcher = new TransactionSlipSearcher(command);

      return searcher.ExecuteSearch();
    }


    #endregion Constructors and parsers

    #region Private methods

    private FixedList<TransactionSlip> ExecuteSearch() {
      string filter = BuildFilterString();
      string sort = BuildSortString();

      if (_command.Status == TransactionSlipStatus.Pending) {
        return TransactionSlipData.GetPendingTransactionSlips(filter, sort);
      } else {
        return TransactionSlipData.GetProcessedTransactionSlips(filter, sort);
      }
    }

    private string BuildSortString() {
      return string.Empty;
    }

    private string BuildFilterString() {
      return string.Empty;
    }

    #endregion Private methods

  }  // class TransactionSlipSearcher

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips
