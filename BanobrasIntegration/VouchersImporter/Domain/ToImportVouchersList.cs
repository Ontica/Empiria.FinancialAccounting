/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : StandardVouchersStructure                     License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Standard structure for vouchers importation from distinct sources.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Standard structure for vouchers importation from distinct sources.</summary>
  internal class ToImportVouchersList {

    internal void AddVoucher(ToImportVoucher standardVoucher) {
      throw new NotImplementedException();
    }

  }  // class StandardVouchersStructure

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
