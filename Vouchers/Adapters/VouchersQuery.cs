/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Query payload                           *
*  Type     : VouchersQuery                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Quer payload used to search accounting vouchers.                                               *
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


  /// <summary>Enumerates the possible status of a voucher with respect of the office workflow.</summary>
  public enum VoucherStatus {

    Pending = 'P',

    Revision = 'R',

    Posted = 'C',

    All = '@',

  }  // enum VoucherStatus

  public class VouchersQuery {

    public string[] VouchersID {
      get;
      set;
    } = new string[0];


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


    public string[] VouchersNumbers {
      get;
      set;
    } = new string[0];


    public string LedgersGroupUID {
      get;
      set;
    } = string.Empty;


    public string LedgerUID {
      get;
      set;
    } = string.Empty;


    public DateTime FromAccountingDate {
      get;
      set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToAccountingDate {
      get;
      set;
    } = ExecutionServer.DateMaxValue;



    public DateTime FromRecordingDate {
      get;
      set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToRecordingDate {
      get;
      set;
    } = ExecutionServer.DateMaxValue;


    public string[] Accounts {
      get;
      set;
    } = new string[0];


    public string[] SubledgerAccounts {
      get;
      set;
    } = new string[0];


    public string[] VerificationNumbers {
      get; set;
    } = new string[0];


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
    } = 5000;


    public int Page {
      get;
      set;
    } = 1;


  }  // class VouchersQuery

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
