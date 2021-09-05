/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Mapper class                          *
*  Type     : ExcelFileMapper                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Mapper for ExcelFile instances.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters {

  /// <summary>Mapper for ExcelFile instances.</summary>
  static internal class ExcelFileMapper {

    static internal ExcelFileDto Map(ExcelFile excelFile) {
      return new ExcelFileDto {
        Url = excelFile.Url
      };
    }

  }  // class ExcelFileMapper

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
