/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Information Holder                      *
*  Type     : VoucherDescriptorDto                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes an accounting voucher.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Describes an accounting voucher.</summary>
  public class VoucherDescriptorDto {

    internal VoucherDescriptorDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }

    public string LedgerName {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public string TransactionTypeName {
      get; internal set;
    }

    public string VoucherTypeName {
      get; internal set;
    }

    public string SourceName {
      get; internal set;
    }

    public DateTime AccountingDate {
      get; internal set;
    }

    public DateTime RecordingDate {
      get; internal set;
    }

    public string ElaboratedBy {
      get; internal set;
    }

    public string AuthorizedBy {
      get; internal set;
    }

    public string Status {
      get; internal set;
    }

  }  // class VoucherDescriptorDto

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
