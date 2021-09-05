/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Data Transfer Object                  *
*  Type     : ExcelFileDto                                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : DTO that returns an Excel file data.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters {

  public class ExcelFileDto {

    internal ExcelFileDto() {
      // no-op
    }

    public string Url {
      get; internal set;
    }

  }  // class ExcelFileDto

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters
