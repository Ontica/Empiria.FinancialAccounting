/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Service Provider                        *
*  Type     : VoucherValidator                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Validates a voucher before be sent to the ledger.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Validates a voucher before be sent to the ledger.</summary>
  internal class VoucherValidator {

    private Voucher _voucher;

    public VoucherValidator(Voucher voucher) {
      _voucher = voucher;
    }

    internal bool IsValid() {
      return false;
    }

  }  // class VoucherValidator

}  /// namespace Empiria.FinancialAccounting.Vouchers
