/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Input Fields DTO                        *
*  Type     : VoucherCreationFields                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves to create or update vouchers data.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Data structure that serves to create or update vouchers data.</summary>
  public sealed class VoucherCreationFields {

    public string LedgerNo {
      get; set;
    } = string.Empty;


    public string SourceSystemCode {
      get; set;
    } = string.Empty;


    public string OperationTypeCode {
      get; set;
    } = string.Empty;


    public string VoucherTypeCode {
      get; set;
    } = string.Empty;


    public DateTime AccountingDate {
      get; set;
    }


    public string Concept {
      get; set;
    } = string.Empty;


    public FixedList<VoucherMovementFields> Movements {
      get; set;
    } = new FixedList<VoucherMovementFields>();

  }  // class VoucherCreationFields



  /// <summary>Data structure that serves to create or update voucher movement data.</summary>
  public sealed class VoucherMovementFields {

    public string LedgerAccountNo {
      get; set;
    } = string.Empty;


    public string SubledgerAccountNo {
      get; set;
    } = string.Empty;


    public string ResponsibilityAreaCode {
      get; set;
    } = string.Empty;


    public string BudgetAccountCode {
      get; set;
    } = string.Empty;


    public string BudgetControlNo {
      get; set;
    } = string.Empty;


    public decimal DebitAmount {
      get; set;
    }


    public decimal CreditAmount {
      get; set;
    }


    public string CurrencyISOCode {
      get; set;
    } = Currency.MXN.ISOCode;


    public decimal ExchangeRate {
      get; set;
    } = 1;


    public string Concept {
      get; set;
    } = string.Empty;

  }  // class VoucherMovementFields

} // namespace Empiria.FinancialAccounting.Vouchers.Adapters
