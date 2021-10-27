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
using Empiria.FinancialAccounting.BanobrasIntegration.SATReports;

namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters {

  /// <summary>Mapper for ExcelFile instances.</summary>
  static internal class ExcelFileMapper {

    static internal FileReportDto Map(ExcelFile excelFile) {
      return new FileReportDto {
        Url = excelFile.Url
      };
    }

    static internal FileReportDto MapXml(OperationalReportFile xmlFile) {
      return new FileReportDto {
        Url = xmlFile.Url
      };
    }

  }  // class ExcelFileMapper

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
