/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : ReclassifiedVoucherEntry                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information for a reclassified accounting voucher entry.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Used to load ReclassifiedVoucherEntry.</summary>
  public class ReclassifiedVoucherEntry {

    #region Constructors and Parsers

    internal ReclassifiedVoucherEntry(string UID, VoucherEntry entry, AccountingRule rule) {
      this.UID = UID;
      this.VoucherEntry = entry;
      this.OperationType = rule.AccountingOperationType;
      this.AccountingRule = rule;
    }

    #endregion Constructors and Parsers

    #region Properties

    public string UID {
      get;
    }

    internal VoucherEntry VoucherEntry {
      get;
    }

    public AccountingOperationType OperationType {
      get;
    }

    public AccountingRule AccountingRule {
      get;
    }

    public Currency NewCurrency {
      get; private set;
    } = Currency.Empty;


    public decimal NewAmount {
      get; private set;
    }

    #endregion Properties

    #region Methods

    internal void ReclassifyCurrency(Currency newCurrency, decimal newAmount) {
      this.NewCurrency = newCurrency;
      this.NewAmount = newAmount;
    }

    #endregion Methods

  } // class ReclassifiedVoucherEntry

} // namespace Empiria.FinancialAccounting.Reclassification
