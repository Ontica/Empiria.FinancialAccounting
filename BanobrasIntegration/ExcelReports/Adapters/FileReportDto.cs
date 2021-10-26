/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : File Reports                          *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Data Transfer Object                  *
*  Type     : FileReportDto                                 License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : DTO that returns an Excel file data.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters {

  public class FileReportDto {

    internal FileReportDto() {
      // no-op
    }

    public string Url {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

  }  // class FileReportDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters
