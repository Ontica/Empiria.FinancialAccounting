/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Mapper class                          *
*  Type     : XmlFileMapper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapper for XmlFile instances.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters {

  /// <summary>Mapper for XmlFile instances.</summary>
  static internal class XmlFileMapper {

    static internal XmlFileDto Map(XmlFile xmlFile) {
      return new XmlFileDto {
        Url = xmlFile.Url
      };
    }

  } // class XmlFileMapper

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters
