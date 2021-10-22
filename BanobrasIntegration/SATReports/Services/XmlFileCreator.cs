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

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  public enum OperationalReportType { 
    
    BalanzaSat,

    CatalogoDeCuentaSat
  }


  /// <summary>Creates a Xml file with trial balance information.</summary>
  public class XmlFileCreator {

    private TrialBalanceCommand _command = new TrialBalanceCommand();
    private XmlFile _xmlFile;

    public XmlFileCreator() {

    }


    internal XmlFile CreateXmlFile(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      _command = trialBalance.Command;

      _xmlFile = new XmlFile();

      SetXmlHeader();

      SetXmlContent(trialBalance);

      return _xmlFile;
    }


    #region Private methods

    private void FilOutBalanza(IEnumerable<TrialBalanceEntryDto> entries) {

      XmlDocument doc = _xmlFile.XmlStructure;

      XmlNode root = doc.SelectSingleNode("Balanza");

      foreach (var entry in entries) {
        XmlElement ctas = doc.CreateElement("BCE:Ctas");
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

    private List<XmlFileAttributes> GetBalanceAttributes() {
      List<XmlFileAttributes> attributes = new List<XmlFileAttributes>();

      attributes.Add(new XmlFileAttributes() {
        Name = "xmlns:BCE",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion"
      });

      attributes.Add(new XmlFileAttributes() {
        Name = "xmlns:xsi",
        Property = "http://www.w3.org/2001/XMLSchema-instance"
      });

      attributes.Add(new XmlFileAttributes() {
        Name = "xsi:schemaLocation",
        Property = "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion " +
        "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion/BalanzaComprobacion_1_3.xsd"
      });

      attributes.Add(new XmlFileAttributes() {
        Name = "TipoEnvio",
        Property = "N"
      });

      return attributes;
    }

    private List<XmlFileAttributes> SetHeaderAttributes() {
      List<XmlFileAttributes> attributes = new List<XmlFileAttributes>();

      if (_command.TrialBalanceType == TrialBalanceType.Balanza) {
        attributes = GetBalanceAttributes();
      } else {
      }

      return attributes;
    }

    private void SetXmlContent(TrialBalanceDto trialBalance) {

      switch (_command.TrialBalanceType) {

        case TrialBalanceType.Balanza:
          FilOutBalanza(trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x));
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }

    }

    private void SetXmlHeader() {

      string headerName = SetHeaderName();
      
      XmlDocument xml = new XmlDocument();
      XmlElement header = xml.CreateElement(headerName);
      xml.AppendChild(header);

      List<XmlFileAttributes> attributes = SetHeaderAttributes();

      XmlAttribute version = xml.CreateAttribute("Version");
      version.Value = attributes.First().Version;
      header.Attributes.Append(version);
      
      XmlAttribute rfc = xml.CreateAttribute("RFC");
      rfc.Value = attributes.First().RFC;
      header.Attributes.Append(rfc);

      XmlAttribute mes = xml.CreateAttribute("Mes");
      mes.Value = _command.InitialPeriod.FromDate.ToString("MM");
      header.Attributes.Append(mes);

      XmlAttribute anio = xml.CreateAttribute("Anio");
      anio.Value = _command.InitialPeriod.FromDate.ToString("yyyy");
      header.Attributes.Append(anio);

      foreach (var attr in attributes) {
        XmlAttribute attribute = xml.CreateAttribute(attr.Name);
        attribute.Value = attr.Property;
        header.Attributes.Append(attribute);
      }

      _xmlFile.XmlStructure = xml;
    }

    private string SetHeaderName() {
      
      if (_command.TrialBalanceType == TrialBalanceType.Balanza) {
        return "BCE:Balanza";
      } else {
        return "";
      }
    }


    
    #endregion


  } // class XmlFileCreator

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
