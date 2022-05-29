/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Xml Exporters                         *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : IXmlDocumentExporter                  *
*  Type     : CatalogoCuentasSatXmlExporter                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Genera el archivo Xml con el catálogo de cuentas para el SAT.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using System.Xml;

using Empiria.FinancialAccounting.Reporting.Builders;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Xml {

  /// <summary>Genera el archivo Xml con el catálogo de cuentas para el SAT.</summary>
  internal class CatalogoCuentasSatXmlExporter : IXmlDocumentExporter {

    private const string PREFIX = "catalogocuentas";
    private const string LOCAL_NAME= "Catalogo";
    private const string NAMESPACE_URI = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas";

    private readonly ReportDataDto _reportData;


    public CatalogoCuentasSatXmlExporter(ReportDataDto reportData) {
      Assertion.Require(reportData, "reportData");

      _reportData = reportData;
    }

    public string GetFileName() {
      return "catalogo.cuentas.sat";
    }


    public XmlDocument GetXmlDocument() {
      IEnumerable<CatalogoCuentasSatEntry> entries = _reportData.Entries.Select(x => (CatalogoCuentasSatEntry) x);

      return CreateXmlDocument(entries);
    }


    #region Private methods

    private XmlDocument CreateXmlDocument(IEnumerable<CatalogoCuentasSatEntry> entries) {
      var xmlDocument = new XmlDocument();

      XmlElement documentRoot = GetDocumentRootElement(xmlDocument);

      xmlDocument.AppendChild(documentRoot);

      foreach (var entry in entries) {
        XmlElement xmlElement = xmlDocument.CreateElement(PREFIX, "Ctas", NAMESPACE_URI);

        xmlElement.SetAttribute("CodAgrup", entry.CodigoAgrupacion);
        xmlElement.SetAttribute("NumCta", entry.NumeroCuenta);
        xmlElement.SetAttribute("Desc", entry.Descripcion);
        xmlElement.SetAttribute("SubCtaDe", entry.SubcuentaDe);
        xmlElement.SetAttribute("Nivel", entry.Nivel.ToString());
        xmlElement.SetAttribute("Natur", entry.Naturaleza);

        documentRoot.AppendChild(xmlElement);
      }

      return xmlDocument;
    }


    private XmlElement GetDocumentRootElement(XmlDocument doc) {
      XmlElement root = doc.CreateElement(PREFIX, LOCAL_NAME, NAMESPACE_URI);
      
      root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

      root.SetAttribute("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", 
                          NAMESPACE_URI +
                          " http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas/CatalogoCuentas_1_3.xsd");

      root.SetAttribute("Version", "1.3");
      root.SetAttribute("RFC", "BNO670315CD0");
      root.SetAttribute("Mes", _reportData.Command.ToDate.ToString("MM"));
      root.SetAttribute("Anio", _reportData.Command.ToDate.ToString("yyyy"));

      return root;
    }

    #endregion Private methods

  } // class CatalogoCuentasSatXmlExporter

} // namespace Empiria.FinancialAccounting.Reporting.Exporters.Xml
