/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                   Component : Adapters Layer                       *
*  Assembly : FinancialAccounting.CashLedger.dll            Pattern   : Output DTO                           *
*  Type     : CashTransactionHolder, Descriptor, Entry      License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to retrieve cash ledger transactions.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Financial.Integration.CashLedger;

namespace Empiria.FinancialAccounting.CashLedger.Adapters {

  /// <summary>Output holder DTO used for a cash transaction.</summary>
  public class CashTransactionHolderDto {

    public CashTransactionDescriptor Transaction {
      get; set;
    }

    public FixedList<CashTransactionEntryDto> Entries {
      get; set;
    }

  }  // class CashTransactionHolderDto



  /// <summary>Output DTO used to retrieve cash ledger transactions for use in lists.</summary>
  public class CashTransactionDescriptor : SharedCashTransactionDescriptor {

  }  // class CashTransactionDescriptor



  /// <summary>Output DTO used to retrieve cash ledger transaction entries.</summary>
  public class CashTransactionEntryDto : SharedCashTransactionEntryDto {

  }  // class CashTransactionEntryDto

}  // namespace Empiria.FinancialAccounting.CashLedger.Adapters
