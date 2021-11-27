/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Interface                               *
*  Type     : IVoucherEntry                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Interface that homologates voucher entry types for validation purpose.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Interface that homologates voucher entry types for validation purpose.</summary>
  public interface IVoucherEntry {

    VoucherEntryType VoucherEntryType {
      get;
    }

    Currency Currency {
      get;
    }

    Sector Sector {
      get;
    }

    decimal Amount {
      get;
    }

    decimal BaseCurrencyAmount {
      get;
    }

    bool HasSector {
      get;
    }

    bool HasSubledgerAccount {
      get;
    }

    bool HasEventType {
      get;
    }

    Account GetAccount(DateTime accountingDate);

  }  // interface IVoucherEntry

}  // namespace Empiria.FinancialAccounting.Vouchers
