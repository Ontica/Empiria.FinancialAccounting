/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : OperationalReportXmlFileCreator              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Xml file with operational reports information.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports;

namespace Empiria.FinancialAccounting.BanobrasIntegration.XmlReports {

  /// <summary>Creates a Xml file with operational reports information.</summary>
  internal class OperationalReportXmlFileCreator {

    private OperationalReportCommand _command = new OperationalReportCommand();

    private XmlFile _xmlFile;

    private string[] headerName;

    public OperationalReportXmlFileCreator() {
      // no-op
    }


    internal XmlFile CreateXmlFile(OperationalReportDto operationalReport) {
      Assertion.AssertObject(operationalReport, "operationalReport");

      _command = operationalReport.Command;

      _xmlFile = new XmlFile();

      headerName = GetXmlHeaderName();

      SetXmlContent(operationalReport);

      _xmlFile.Save(_xmlFile.XmlStructure, GetReportNameByType(_command.ReportType));

      return _xmlFile;
    }


    #region Private methods


    private void FillOutCatalogoDeCuentas(IEnumerable<OperationalReportEntryDto> entries) {

      XmlDocument doc = new XmlDocument();
      XmlElement root = XmlRootGenerator(doc);

      foreach (var entry in entries) {
        XmlElement ctas = doc.CreateElement(headerName[0], "Ctas", headerName[2]);
        root.AppendChild(ctas);
        
        XmlAttribute codAgrup = doc.CreateAttribute("CodAgrup");
        codAgrup.Value = entry.GroupingCode;
        ctas.Attributes.Append(codAgrup);

        XmlAttribute numCta = doc.CreateAttribute("NumCta");
        numCta.Value = entry.AccountNumber;
        ctas.Attributes.Append(numCta);

        XmlAttribute desc = doc.CreateAttribute("Desc");
        desc.Value = entry.AccountName;
        ctas.Attributes.Append(desc);

        XmlAttribute subCtaDe = doc.CreateAttribute("SubCtaDe");
        subCtaDe.Value = entry.AccountParent;
        ctas.Attributes.Append(subCtaDe);

        XmlAttribute nivel = doc.CreateAttribute("Nivel");
        nivel.Value = entry.AccountLevel.ToString();
        ctas.Attributes.Append(nivel);

        XmlAttribute natur = doc.CreateAttribute("Natur");
        natur.Value = entry.Naturaleza;
        ctas.Attributes.Append(natur);

      }
      _xmlFile.XmlStructure = doc;
    }


    private void FillOutBalanza(IEnumerable<OperationalReportEntryDto> entries) {

      XmlDocument doc = new XmlDocument();
      XmlElement root = XmlRootGenerator(doc);

      foreach (var entry in entries) {
        XmlElement ctas = doc.CreateElement(headerName[0], "Ctas", headerName[2]);
        root.AppendChild(ctas);

        XmlAttribute numCta = doc.CreateAttribute("NumCta");
        numCta.Value = entry.AccountNumber;
        ctas.Attributes.Append(numCta);

        XmlAttribute saldoIni = doc.CreateAttribute("SaldoIni");
        saldoIni.Value = entry.InitialBalance.ToString();
        ctas.Attributes.Append(saldoIni);

        XmlAttribute debe = doc.CreateAttribute("Debe");
        debe.Value = entry.Debit.ToString();
        ctas.Attributes.Append(debe);

        XmlAttribute haber = doc.CreateAttribute("Haber");
        haber.Value = entry.Credit.ToString();
        ctas.Attributes.Append(haber);

        XmlAttribute saldoFin = doc.CreateAttribute("SaldoFin");
        saldoFin.Value = entry.InitialBalance.ToString();
        ctas.Attributes.Append(saldoFin);

      }
      _xmlFile.XmlStructure = doc;
    }


    private List<XmlFileAttributes> GetAccountsChartAttributes() {
      List<XmlFileAttributes> attributes = new List<XmlFileAttributes>();

      attributes.Add(new XmlFileAttributes() {
        Name = "xsi:schemaLocation",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas " +
                   "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas/CatalogoCuentas_1_3.xsd"
      });

      return attributes;
    }


    private List<XmlFileAttributes> GetBalanceAttributes() {
      List<XmlFileAttributes> attributes = new List<XmlFileAttributes>();

      attributes.Add(new XmlFileAttributes() {
        Name = "TipoEnvio",
        Property = "N"
      });

      attributes.Add(new XmlFileAttributes() {
        Name = "xsi:schemaLocation",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion " +
        "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion/BalanzaComprobacion_1_3.xsd"
      });

      return attributes;
    }


    private string GetReportNameByType(OperationalReportType reportType) {
      switch (reportType) {
        case OperationalReportType.BalanzaSAT:
          return "balanza.sat";

        case OperationalReportType.CatalogoSAT:

          return "catalogo.cuentas.sat";

        default:

          throw Assertion.AssertNoReachThisCode();
      }
      throw new NotImplementedException();
    }


    private string[] GetXmlHeaderName() {

      if (_command.ReportType == OperationalReportType.BalanzaSAT) {
        return new string[] { "BCE", "Balanza", "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion" };
      } else if (_command.ReportType == OperationalReportType.CatalogoSAT) {
        return new string[] { "catalogocuentas", "Catalogo", "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas" };
      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    private List<XmlFileAttributes> SetHeaderAttributes() {
      List<XmlFileAttributes> attributes = new List<XmlFileAttributes>();

      if (_command.ReportType == OperationalReportType.BalanzaSAT) {
        attributes = GetBalanceAttributes();

      } else if (_command.ReportType == OperationalReportType.CatalogoSAT) {
        attributes = GetAccountsChartAttributes();

      }

      return attributes;
    }


    private void SetXmlContent(OperationalReportDto operationalReport) {
      switch (_command.ReportType) {

        case OperationalReportType.BalanzaSAT:
          FillOutBalanza(operationalReport.Entries.Select(x => (OperationalReportEntryDto) x));
          return;

        case OperationalReportType.CatalogoSAT:
          FillOutCatalogoDeCuentas(operationalReport.Entries.Select(x => (OperationalReportEntryDto) x));
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }


    private XmlElement XmlRootGenerator(XmlDocument doc) {

      XmlElement root = doc.CreateElement(headerName[0], headerName[1], headerName[2]);
      doc.AppendChild(root);

      List<XmlFileAttributes> attributes = SetHeaderAttributes();

      foreach (var attr in attributes) {
        XmlAttribute attribute = doc.CreateAttribute(attr.Name);
        attribute.Value = attr.Property;
        root.Attributes.Append(attribute);
      }

      XmlAttribute xsi = doc.CreateAttribute("xmlns:xsi");
      xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
      root.Attributes.Append(xsi);

      XmlAttribute version = doc.CreateAttribute("Version");
      version.Value = attributes.First().Version;
      root.Attributes.Append(version);

      XmlAttribute rfc = doc.CreateAttribute("RFC");
      rfc.Value = attributes.First().RFC;
      root.Attributes.Append(rfc);

      XmlAttribute mes = doc.CreateAttribute("Mes");
      mes.Value = _command.Date.ToString("MM");
      root.Attributes.Append(mes);

      XmlAttribute anio = doc.CreateAttribute("Anio");
      anio.Value = _command.Date.ToString("yyyy");
      root.Attributes.Append(anio);

      return root;
    }

    #endregion


  } // class OperationalReportXmlFileCreator

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
