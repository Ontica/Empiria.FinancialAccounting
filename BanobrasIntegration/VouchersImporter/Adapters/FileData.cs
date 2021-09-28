/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Input Data Holder                     *
*  Type     : FileData                                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Input data holder for an file stream containing vouchers data.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.IO;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>Input data holder for an file stream containing vouchers data.</summary>
  public class FileData {

    public Stream InputStream {
      get; set;
    }

    public string MediaType {
      get; set;
    }

    public int MediaLength {
      get; set;
    }

    public string OriginalFileName {
      get; set;
    }

  }  // class FileData

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters
