/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : XmlFileCreator                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Xml file with operational reports information.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  public enum OperationalReportType {

    BalanzaSat,

    CatalogoDeCuentaSat
  }

  public enum FileType {

    Excel,

    PDF,

    Xml,

  }


  /// <summary>Creates a Xml file with trial balance information.</summary>
  internal class XmlFileCreator {

    private OperationalReportCommand _command = new OperationalReportCommand();

    private OperationalReportFile _xmlFile;


    public XmlFileCreator() {
      // no-op
    }


    internal OperationalReportFile CreateOperationalReportFile(OperationalReportDto operationalReport,
                                                               OperationalReportCommand command) {
      Assertion.AssertObject(operationalReport, "operationalReport");

      _command = command;

      _xmlFile = new OperationalReportFile();

      SetXmlHeader();

      SetXmlContent(operationalReport);

      return _xmlFile;
    }


    #region Private methods

    private string GetXmlHeaderName() {

      if (_command.ReportType == OperationalReportType.BalanzaSat) {
        return "BCE:Balanza";
      } else if (_command.ReportType == OperationalReportType.CatalogoDeCuentaSat) {
        return "catalogocuentas:Catalogo";
      } else {
        return "";
      }
    }


    private void SetXmlHeader() {
      string headerName = GetXmlHeaderName();

      XmlDocument xml = new XmlDocument();
      XmlElement header = xml.CreateElement(headerName);
      xml.AppendChild(header);

      List<OperationalReportFileAttributes> attributes = SetHeaderAttributes();

      XmlAttribute anio = xml.CreateAttribute("Anio");
      anio.Value = _command.Date.ToString("yyyy");
      header.Attributes.Append(anio);

      XmlAttribute mes = xml.CreateAttribute("Mes");
      mes.Value = _command.Date.ToString("MM");
      header.Attributes.Append(mes);

      XmlAttribute version = xml.CreateAttribute("Version");
      version.Value = attributes.First().Version;
      header.Attributes.Append(version);

      XmlAttribute rfc = xml.CreateAttribute("RFC");
      rfc.Value = attributes.First().RFC;
      header.Attributes.Append(rfc);

      XmlAttribute xsi = xml.CreateAttribute("xmlns:xsi");
      xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
      header.Attributes.Append(xsi);


      foreach (var attr in attributes) {
        XmlAttribute attribute = xml.CreateAttribute(attr.Name);
        attribute.Value = attr.Property;
        header.Attributes.Append(attribute);
      }

      _xmlFile.XmlStructure = xml;
    }


    private List<OperationalReportFileAttributes> SetHeaderAttributes() {
      List<OperationalReportFileAttributes> attributes = new List<OperationalReportFileAttributes>();

      if (_command.ReportType == OperationalReportType.BalanzaSat) {
        attributes = GetBalanceAttributes();
      } else if(_command.ReportType == OperationalReportType.CatalogoDeCuentaSat) {
        attributes = GetAccountsChartAttributes();
      }

      return attributes;
    }


    private void SetXmlContent(OperationalReportDto operationalReport) {

      switch (_command.ReportType) {

        case OperationalReportType.BalanzaSat:
          FillOutBalanza(operationalReport.Entries.Select(x => (OperationalReportEntryDto) x));
          return;

        case OperationalReportType.CatalogoDeCuentaSat:
          FillOutCatalogoDeCuentas(operationalReport.Entries.Select(x => (OperationalReportEntryDto) x));
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }

    }


    private List<OperationalReportFileAttributes> GetAccountsChartAttributes() {
      List<OperationalReportFileAttributes> attributes = new List<OperationalReportFileAttributes>();

      attributes.Add(new OperationalReportFileAttributes() {
        Name = "xsi:schemaLocation",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas " +
                   "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas/CatalogoCuentas_1_3.xsd"
      });

      attributes.Add(new OperationalReportFileAttributes() {
        Name = "xmlns:catalogocuentas",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas"
      });

      return attributes;
    }


    private List<OperationalReportFileAttributes> GetBalanceAttributes() {
      List<OperationalReportFileAttributes> attributes = new List<OperationalReportFileAttributes>();

      attributes.Add(new OperationalReportFileAttributes() {
        Name = "TipoEnvio",
        Property = "N"
      });

      attributes.Add(new OperationalReportFileAttributes() {
        Name = "xmlns:BCE",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion"
      });

      attributes.Add(new OperationalReportFileAttributes() {
        Name = "xsi:schemaLocation",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion " +
        "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion/BalanzaComprobacion_1_3.xsd"
      });

      return attributes;
    }


    private void FillOutCatalogoDeCuentas(IEnumerable<OperationalReportEntryDto> entries) {

      XmlDocument doc = _xmlFile.XmlStructure;

      XmlNode root = doc.SelectSingleNode("Catalogo");

      foreach (var entry in entries) {
        XmlElement ctas = doc.CreateElement("catalogocuentas:Ctas");
        root.AppendChild(ctas);

        XmlAttribute natur = doc.CreateAttribute("Natur");
        natur.Value = entry.Naturaleza;
        ctas.Attributes.Append(natur);

        XmlAttribute nivel = doc.CreateAttribute("Nivel");
        nivel.Value = entry.AccountLevel.ToString();
        ctas.Attributes.Append(nivel);

        XmlAttribute subCtaDe = doc.CreateAttribute("SubCtaDe");
        subCtaDe.Value = entry.AccountNumber.ToString();
        ctas.Attributes.Append(subCtaDe);

        XmlAttribute desc = doc.CreateAttribute("Desc");
        desc.Value = entry.AccountName.ToString();
        ctas.Attributes.Append(desc);

        XmlAttribute numCta = doc.CreateAttribute("NumCta");
        numCta.Value = entry.AccountNumber;
        ctas.Attributes.Append(numCta);

        XmlAttribute codAgrup = doc.CreateAttribute("CodAgrup");
        codAgrup.Value = entry.GroupingCode;
        ctas.Attributes.Append(codAgrup);

      }
      _xmlFile.XmlStructure = doc;
    }

    private void FillOutBalanza(IEnumerable<OperationalReportEntryDto> entries) {

      XmlDocument doc = _xmlFile.XmlStructure;

      XmlNode root = doc.SelectSingleNode("Balanza");

      foreach (var entry in entries) {
        XmlElement ctas = doc.CreateElement("BCE:Ctas");
        root.AppendChild(ctas);

        XmlAttribute saldoFin = doc.CreateAttribute("SaldoFin");
        saldoFin.Value = entry.InitialBalance.ToString();
        ctas.Attributes.Append(saldoFin);

        XmlAttribute haber = doc.CreateAttribute("Haber");
        haber.Value = entry.Credit.ToString();
        ctas.Attributes.Append(haber);

        XmlAttribute debe = doc.CreateAttribute("Debe");
        debe.Value = entry.Debit.ToString();
        ctas.Attributes.Append(debe);

        XmlAttribute saldoIni = doc.CreateAttribute("SaldoIni");
        saldoIni.Value = entry.InitialBalance.ToString();
        ctas.Attributes.Append(saldoIni);

        XmlAttribute numCta = doc.CreateAttribute("NumCta");
        numCta.Value = entry.AccountNumber;
        ctas.Attributes.Append(numCta);

      }
      _xmlFile.XmlStructure = doc;
    }


    #endregion


  } // class XmlFileCreator

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
