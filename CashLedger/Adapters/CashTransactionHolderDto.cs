/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                    Component : Adapters Layer                      *
*  Assembly : FinancialAccounting.CashLedger.dll             Pattern   : Output DTO                          *
*  Type     : CashTransactionHolderDto                       License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Output holder DTO used for a cash transaction.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Output holder DTO used for a cash transaction.</summary>
  public class CashTransactionHolderDto {

    public CashTransactionDescriptor Transaction {
      get; internal set;
    }

    public FixedList<CashTransactionEntryDto> Entries {
      get; internal set;
    }

  }  // class CashTransactionHolderDto



  /// <summary>Output DTO used to retrieve cash ledger transaction entries.</summary>
  public class CashTransactionEntryDto {

    public long Id {
      get; internal set;
    }

    public string AccountNumber {
      get; internal set;
    } = string.Empty;


    public string AccountName {
      get; internal set;
    } = string.Empty;


    public string SectorCode {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountName {
      get; internal set;
    } = string.Empty;


    public string VerificationNumber {
      get; internal set;
    } = string.Empty;


    public string ResponsibilityAreaName {
      get; internal set;
    } = string.Empty;


    public string CurrencyName {
      get; internal set;
    } = string.Empty;


    public decimal ExchangeRate {
      get; internal set;
    }

    public decimal Debit {
      get; internal set;
    }

    public decimal Credit {
      get; internal set;
    }

    public int CashAccountId {
      get; internal set;
    }

  }  // class CashTransactionEntryDto

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
