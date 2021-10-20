/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : XmlFileCreator                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Creates a Xml file with trial balance information.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

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
    private void SetXmlHeader() {

      string headerName = SetHeaderName();
      
      XmlDocument xml = new XmlDocument();
      XmlElement header = xml.CreateElement(headerName);
      xml.AppendChild(header);

      string[] attributes = SetHeaderAttributes();

      foreach (var attr in attributes) {
        XmlAttribute attribute = xml.CreateAttribute(attr);
        attribute.Value = "";
        header.Attributes.Append(attribute);
      }

    }

    private string SetHeaderName() {
      
      if (true) {
        return "BCE:Balanza";
      } else {
        
      }
    }

    private string[] SetHeaderAttributes() {
      
      string[] attributes = { };

      if (true) {

        attributes.Append("xmlns:BCE");
        attributes.Append("xmlns:xsi");
        attributes.Append("xsi:schemaLocation");
        attributes.Append("Version");
        attributes.Append("RFC");
        attributes.Append("Mes");
        attributes.Append("Anio");
        attributes.Append("TipoEnvio");

      } else {

      }

      return attributes;
    }

    private void SetXmlContent(TrialBalanceDto trialBalance) {

      switch (trialBalance.Command.TrialBalanceType) {

        case TrialBalanceType.Balanza:
          FilOutBalanza(trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x));
          return;

        default:
          throw Assertion.AssertNoReachThisCode();
      }

    }


    private void FilOutBalanza(IEnumerable<TrialBalanceEntryDto> entries) {
      throw new NotImplementedException();
    }
    #endregion


  } // class XmlFileCreator

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
