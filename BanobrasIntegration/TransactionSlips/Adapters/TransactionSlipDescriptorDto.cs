/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Interface adapters                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : TransactionSlipDescriptorDto                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data transfer objects for transaction slips.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters {

  /// <summary>Data transfer objects for transaction slips.</summary>
  public class TransactionSlipDescriptorDto {

    internal TransactionSlipDescriptorDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string SlipNumber {
      get; internal set;
    }


    public string Concept {
      get; internal set;
    }


    public DateTime AccountingDate {
      get; internal set;
    }


    public DateTime RecordingDate {
      get; internal set;
    }


    public string FunctionalArea {
      get; internal set;
    }


    public string VerificationNumber {
      get; internal set;
    }


    public string ElaboratedBy {
      get; internal set;
    }


    public decimal ControlTotal {
      get; internal set;
    }


    public long AccountingVoucherId {
      get; internal set;
    }


    public TransactionSlipStatus Status {
      get; internal set;
    }


    public string StatusName {
      get; internal set;
    }


    public int AccountsChartId {
      get;
      set;
    }

    public int SystemId {
      get;
      set;
    }

    public DateTime ProcessingDate {
      get;
      set;
    }

  }  // class TransactionSlipDescriptorDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips.Adapters
