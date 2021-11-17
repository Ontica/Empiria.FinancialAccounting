/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Input Data Holder                     *
*  Type     : VoucherImportationCommand                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command payload used to import vouchers using rules and coming from external systems.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>Command payload used to import vouchers using rules and coming from external systems.</summary>
  public class VoucherImportationCommand {

    public string SystemUID {
      get; set;
    } = string.Empty;


    public string ImportationRuleUID {
      get; set;
    } = string.Empty;


    public ToImportVoucherDto[] Vouchers {
      get; set;
    } = new ToImportVoucherDto[0];

  }  // class VoucherImportationCommand


  public class ToImportVoucherDto {

    public string ImportationSet {
      get; set;
    } = string.Empty;


    public string UniqueID {
      get; set;
    } = string.Empty;


    public string LedgerCode {
      get; set;
    } = string.Empty;


    public string Concept {
      get; set;
    } = string.Empty;


    public DateTime AccountingDate {
      get; internal set;
    }


    public DateTime RecordingDate {
      get; internal set;
    }


    public string VoucherTypeUID {
      get; set;
    } = string.Empty;


    public string TransactionTypeUID {
      get; set;
    } = string.Empty;


    public string FunctionalAreaCode {
      get; set;
    } = string.Empty;


    public string ElaboratedByCode {
      get; set;
    } = string.Empty;


    public string OperationNumber {
      get; set;
    } = string.Empty;


    public ToImportVoucherEntryDto[] Entries {
      get; set;
    } = new ToImportVoucherEntryDto[0];


  }  // class ToImportVoucherDto


  public class ToImportVoucherEntryDto {

    public string AccountNumber {
      get; set;
    } = string.Empty;


    public string SectorCode {
      get; set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;


    public string ResponsibilityAreaCode {
      get; set;
    } = string.Empty;


    public string BudgetConcept {
      get; set;
    } = string.Empty;


    public string EventTypeCode {
      get; set;
    } = string.Empty;


    public string VerificationNumber {
      get; set;
    } = string.Empty;


    public VoucherEntryType VoucherEntryType {
      get; set;
    }


    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string Concept {
      get; set;
    } = string.Empty;


    public string CurrencyCode {
      get; set;
    } = string.Empty;


    public decimal Amount {
      get; set;
    }


    public decimal ExchangeRate {
      get; set;
    }


    public bool Protected {
      get; set;
    }


    public string OperationNumber {
      get; set;
    } = string.Empty;


  }  // class ToImportVoucherEntryDto

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters
