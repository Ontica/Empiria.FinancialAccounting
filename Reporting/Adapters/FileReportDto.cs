/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : FileReportDto                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : DTO that returns information about a server file containing a report.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>DTO that returns information about a server file containing a report.</summary>
  public class FileReportDto {

    public FileReportDto(FileType fileType, string url) {
      Assertion.Require(url, "url");

      this.Type = fileType;
      this.Url = url;
    }


    public string Url {
      get;
    }


    public FileType Type {
      get;
    }


  }  // class FileReportDto

}  // namespace Empiria.FinancialAccounting.Reporting
