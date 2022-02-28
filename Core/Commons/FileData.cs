/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                         Component : Common Types                          *
*  Assembly : Empiria.FinancialAccounting.dll              Pattern   : Input Data Holder                     *
*  Type     : FileData                                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Input data holder for a file stream.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.IO;

namespace Empiria.FinancialAccounting {

  /// <summary>Input data holder for a file stream.</summary>
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

}  // namespace Empiria.FinancialAccounting
