/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Concrete Builder                        *
*  Type     : NivelacionCuentasCompraventaVoucherBuilder License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds a voucher that mantains the right valued balance for a list of accounts pairs,          *
*             typically accounts handled in different currencies at different times.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.SpecialCases {

  /// <summary>Builds a voucher that mantains the right valued balance for a list of accounts pairs,
  /// typically accounts handled in different currencies at different times.</summary>
  internal class NivelacionCuentasCompraventaVoucherBuilder : VoucherBuilder {

    internal NivelacionCuentasCompraventaVoucherBuilder() {
      // no-op
    }


    internal override Voucher BuildVoucher() {
      throw new NotImplementedException("NivelacionCuentasCompraventaVoucherBuilder");
    }


  }  // class NivelacionCuentasCompraventaVoucherBuilder

}  // namespace Empiria.FinancialAccounting.Vouchers.SpecialCases
