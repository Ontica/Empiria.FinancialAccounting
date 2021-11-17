/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Information Holder                      *
*  Type     : VoucherTotal                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the total of debit and credit entries for a voucher, per currency.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Contains the total of debit and credit entries for a voucher, per currency.</summary>
  public class VoucherTotal {

    internal VoucherTotal(Currency currency, decimal debitTotal, decimal creditTotal) {
      this.Currency = currency;
      this.DebitTotal = debitTotal;
      this.CreditTotal = creditTotal;
    }


    public Currency Currency {
      get;
    }


    public decimal DebitTotal {
      get;
    }


    public decimal CreditTotal {
      get;
    }


    public bool IsBalanced {
      get {
        return (this.Difference == 0);
      }
    }


    public decimal Difference {
      get {
        return (this.DebitTotal - this.CreditTotal);
      }
    }


  }  // class VoucherTotal

}  // namespace Empiria.FinancialAccounting.Vouchers
