/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : RealTransaction                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to load RealTransaction.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Used to load RealTransaction.</summary>
  public class RealTransaction {

    #region Constructors and Parsers

    internal RealTransaction() {
      // Required by Empiria Framework
    }

    internal RealTransaction(string UID, AccountingOperation accountingOperation, VoucherEntryDescriptorDto transaction) {
      this.UID = UID;
      this.AccountingOperationType = accountingOperation.AccountingOperationType;
      this.Transaction = transaction;
      this.AccountingOperation = accountingOperation;
    }

    #endregion Constructors and Parsers

    #region Properties

    public string UID {
      get; set;
    }


    public AccountingOperationType AccountingOperationType {
      get; set;
    }


    public AccountingOperation AccountingOperation {
      get; set;
    }


    public VoucherEntryDescriptorDto Transaction {
      get; set;
    }


    public int IdMonedaReal {
      get; set;
    } = -1;


    public decimal MontoReal {
      get; set;
    }

    #endregion Properties



  } // class RealTransaction

} // namespace Empiria.FinancialAccounting.Reclassification
