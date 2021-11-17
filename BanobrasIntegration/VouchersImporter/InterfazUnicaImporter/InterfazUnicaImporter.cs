/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Service provider                     *
*  Type     : InterfazUnicaImporter                         License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services to import vouchers using a Banobras' InterfazUnica compatible structure.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Provides services to import vouchers using a Banobras'
  /// InterfazUnica compatible data structure.</summary>
  internal class InterfazUnicaImporter {

    #region Constructors and parsers

    private readonly InterfazUnicaImporterCommand _command;


    internal InterfazUnicaImporter(InterfazUnicaImporterCommand command) {
      Assertion.AssertObject(command, "command");

      _command = command;
    }


    #endregion Constructors and parsers

    #region Methods

    internal ImportVouchersResult DryRunImport() {
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.DryRunImport();
    }


    internal ImportVouchersResult Import() {
      VoucherImporter voucherImporter = GetVoucherImporter();

      return voucherImporter.Import();
    }


    private VoucherImporter GetVoucherImporter() {
      throw new NotImplementedException("GetVoucherImporter");
    }

    #endregion Methods

  }  // class InterfazUnicaImporter

}  //namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
