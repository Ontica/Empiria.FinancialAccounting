/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Input Data Holder                       *
*  Type     : VoucherFields                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves as an adapter to create or update vouchers data.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Data structure that serves as an adapter to create or update vouchers data.</summary>
  public class VoucherFields {

    public string Concept {
      get; set;
    } = string.Empty;


    public DateTime AccountingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public DateTime RecordingDate {
      get; set;
    } = ExecutionServer.DateMaxValue;


    public string ElaboratedByUID {
      get; set;
    } = string.Empty;


    public string LedgerUID {
      get; set;
    } = string.Empty;


    public string TransactionTypeUID {
      get; set;
    } = TransactionType.Manual.UID;


    public string VoucherTypeUID {
      get; set;
    } = string.Empty;


    public int FunctionalAreaId {
      get; set;
    } = -1;


    internal void EnsureIsValid() {

    }

  }  // class VoucherFields

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
