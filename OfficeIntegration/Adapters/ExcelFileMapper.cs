/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Mapper class                          *
*  Type     : ExcelFileMapper                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Mapper for ExcelFile instances.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.OfficeIntegration.Adapters {

  /// <summary>Mapper for ExcelFile instances.</summary>
  static internal class ExcelFileMapper {

    static internal ExcelFileDto Map(ExcelFile excelFile) {
      return new ExcelFileDto {
        Url = excelFile.Url
      };
    }

  }  // class ExcelFileMapper

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
