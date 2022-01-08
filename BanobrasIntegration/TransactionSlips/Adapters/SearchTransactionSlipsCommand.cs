/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Command payload                      *
*  Type     : SearchTransactionSlipsCommand                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Command payload used to search transaction slips (volantes).                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Command payload used to search transaction slips (volantes).</summary>
  public class SearchTransactionSlipsCommand {

    public string AccountsChartUID {
      get;
      set;
    } = string.Empty;


    public string SystemUID {
      get;
      set;
    } = string.Empty;


    public string Keywords {
      get;
      set;
    } = string.Empty;


    public DateTime FromDate {
      get;
      set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToDate {
      get;
      set;
    } = ExecutionServer.DateMaxValue;


    public DateSearchField DateSearchField {
      get;
      set;
    } = DateSearchField.None;


    public TransactionSlipStatus Status {
      get;
      set;
    } = TransactionSlipStatus.Pending;


  }  // class SearchTransactionSlipsCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
