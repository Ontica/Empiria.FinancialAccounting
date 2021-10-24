/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Data Transfer Object                  *
*  Type     : XmlFileDto                                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : DTO that returns an Xml file data.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Xml;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters {

  /// <summary>DTO that returns an Xml file data.</summary>
  public class XmlFileDto {

    internal XmlFileDto() {
      // no-op
    }

    public string XmlStructure {
      get; internal set;
    }


  } // class XmlFileDto

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters
