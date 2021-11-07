/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : File Exportation Services            *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : XmlExporter                                   License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to generate financial accounting reports.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Xml;

using Empiria.FinancialAccounting.Reporting.Exporters.Xml;

namespace Empiria.FinancialAccounting.Reporting.Exporters {

  internal interface IXmlDocumentExporter {

    XmlDocument GetXmlDocument();

    string GetFileName();

  }  // interface IXmlDocumentExporter



  internal class XmlExporter : IReportExporter {

    public FileReportDto Export(ReportDataDto reportData) {
      Assertion.AssertObject(reportData, "reportData");

      IXmlDocumentExporter xmlConverter = GetXmlConverter(reportData);

      XmlDocument xmlDocument = xmlConverter.GetXmlDocument();

      var xmlFile = new XmlFile(xmlDocument);

      xmlFile.Save(xmlConverter.GetFileName());

      return xmlFile.ToFileReportDto();
    }


    private IXmlDocumentExporter GetXmlConverter(ReportDataDto reportData) {
      switch (reportData.Command.ReportType) {
        case "BalanzaSAT":
          return new BalanzaSatXmlExporter(reportData);

        case "CatalogoSAT":
          return new CatalogoCuentasSatXmlExporter(reportData);

        default:
          throw Assertion.AssertNoReachThisCode($"Unhandled reportType '{reportData.Command.ReportType}'.");
      }
    }

  }  // class XmlExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters
