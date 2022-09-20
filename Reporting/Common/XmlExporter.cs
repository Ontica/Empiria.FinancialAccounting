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
      Assertion.Require(reportData, "reportData");

      IXmlDocumentExporter xmlConverter = GetXmlConverter(reportData);

      XmlDocument xmlDocument = xmlConverter.GetXmlDocument();

      var xmlFile = new XmlFile(xmlDocument);

      xmlFile.Save(xmlConverter.GetFileName());

      return xmlFile.ToFileReportDto();
    }


    private IXmlDocumentExporter GetXmlConverter(ReportDataDto reportData) {
      switch (reportData.Query.ReportType) {
        case ReportTypes.BalanzaSAT:
          return new BalanzaSatXmlExporter(reportData);

        case ReportTypes.CatalogoCuentasSAT:
          return new CatalogoCuentasSatXmlExporter(reportData);

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled reportType '{reportData.Query.ReportType}'.");
      }
    }

  }  // class XmlExporter

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters
