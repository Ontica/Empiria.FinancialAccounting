/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : DbVouchersStructurer                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds a voucher's structure coming from database tables.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Holds a voucher's structure coming from database tables.</summary>
  internal class DbVouchersStructurer {

    private readonly FixedList<Encabezado> _encabezados;
    private readonly FixedList<Movimiento> _movimientos;

    internal DbVouchersStructurer(FixedList<Encabezado> encabezados, FixedList<Movimiento> movimientos) {
      Assertion.AssertObject(encabezados, "encabezados");
      Assertion.AssertObject(movimientos, "movimientos");

      _encabezados = encabezados;
      _movimientos = movimientos;
    }

    internal StandardVouchersStructure GetStandardStructure() {
      return new StandardVouchersStructure();
    }

  }  // class DbVouchersStructurer

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
