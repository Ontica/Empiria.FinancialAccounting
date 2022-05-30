/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Xml Exporters                         *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IXmlDocumentExporter                  *
*  Type     : BalanzaSatXmlExporter                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera el archivo Xml con la balanza de comprobación para el SAT.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Xml;

using Empiria.FinancialAccounting.Reporting.Builders;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Xml {

  /// <summary>Genera el archivo Xml con la balanza de comprobación para el SAT.</summary>
  internal class BalanzaSatXmlExporter : IXmlDocumentExporter {

    private const string PREFIX = "BCE";
    private const string LOCAL_NAME = "Balanza";
    private const string NAMESPACE_URI = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion";

    private readonly ReportDataDto _reportData;


    public BalanzaSatXmlExporter(ReportDataDto reportData) {
      Assertion.Require(reportData, "reportData");

      _reportData = reportData;
    }


    public string GetFileName() {
      return "balanza.sat";
    }

    public XmlDocument GetXmlDocument() {
      IEnumerable<BalanzaSatEntry> entries = _reportData.Entries.Select(x => (BalanzaSatEntry) x);

      return this.CreateXmlDocument(entries);
    }


    #region Private methods

    private XmlDocument CreateXmlDocument(IEnumerable<BalanzaSatEntry> entries) {
      var xmlDocument = new XmlDocument();

      XmlElement documentRoot = GetDocumentRootElement(xmlDocument);

      xmlDocument.AppendChild(documentRoot);

      foreach (var entry in entries) {
        XmlElement xmlElement = xmlDocument.CreateElement(PREFIX, "Ctas", NAMESPACE_URI);

        xmlElement.SetAttribute("NumCta", entry.Cuenta);
        xmlElement.SetAttribute("SaldoIni", entry.SaldoInicial.ToString());
        xmlElement.SetAttribute("Debe", entry.Debe.ToString());
        xmlElement.SetAttribute("Haber", entry.Haber.ToString());
        xmlElement.SetAttribute("SaldoFin", entry.SaldoFinal.ToString());


        documentRoot.AppendChild(xmlElement);
      }

      return xmlDocument;
    }


    private XmlElement GetDocumentRootElement(XmlDocument doc) {
      XmlElement root = doc.CreateElement(PREFIX, LOCAL_NAME, NAMESPACE_URI);

      root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

      root.SetAttribute("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", 
                          NAMESPACE_URI +
                          " http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion/BalanzaComprobacion_1_3.xsd");

      root.SetAttribute("Version", "1.3");
      root.SetAttribute("RFC", "BNO670315CD0");
      root.SetAttribute("Mes", _reportData.Query.ToDate.ToString("MM"));
      root.SetAttribute("Anio", _reportData.Query.ToDate.ToString("yyyy"));
      root.SetAttribute("TipoEnvio", _reportData.Query.SendType.ToString());

      return root;
    }

    #endregion Private methods


  } // class BalanzaSatXmlExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Xml
