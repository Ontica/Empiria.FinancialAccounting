/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Command payload                         *
*  Type     : SearchVouchersCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to search accounting vouchers.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Enumerates the different workflow stages for an accounting voucher.</summary>
  public enum VoucherStage {

    MyInbox,

    Pending,

    ControlDesk,

    Completed,

    All

  }  // enum VoucherStage


  /// <summary>Enumerates the possible statuses of a vouhcer with respect of the office workflow.</summary>
  public enum VoucherStatus {

    Undefined,

    Control = 'K',

    Elaboration = 'E',

    Revision = 'V',

    OnHold = 'H',

    Posted = 'C',

    All = '@',

  }  // enum VoucherStatus


  public enum DateSearchField {

    None,

    AccountingDate,

    RecordingDate,

  }


  public class SearchVouchersCommand {

    public VoucherStage Stage {
      get;
      set;
    } = VoucherStage.All;


    public VoucherStatus Status {
      get;
      set;
    } = VoucherStatus.All;


    public string AccountsChartUID {
      get;
      set;
    } = string.Empty;


    public string Keywords {
      get;
      set;
    } = string.Empty;


    public string Concept {
      get;
      set;
    } = string.Empty;


    public string Number {
      get;
      set;
    } = string.Empty;



    public string LedgersGroupUID {
      get;
      set;
    } = string.Empty;


    public string LedgerUID {
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


    public string AccountKeywords {
      get;
      set;
    } = String.Empty;


    public string SubledgerAccountKeywords {
      get;
      set;
    } = String.Empty;


    public string TransactionTypeUID {
      get;
      set;
    } = String.Empty;


    public string VoucherTypeUID {
      get;
      set;
    } = String.Empty;


    public string EditorType {
      get;
      set;
    } = String.Empty;


    public string EditorUID {
      get;
      set;
    } = String.Empty;


    public string OrderBy {
      get;
      set;
    } = string.Empty;


    public int PageSize {
      get;
      set;
    } = 500;


    public int Page {
      get;
      set;
    } = 1;


  }  // class SearchVouchersCommand

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
