/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : XmlExporter                                  License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export accounting and balances information to xml.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports.Adapters;
using Empiria.FinancialAccounting.FinancialReports.Adapters;
using Empiria.FinancialAccounting.Rules.Adapters;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary>Main service to export accounting and balances information to xml.</summary>
  public class XmlExporter {

    public XmlFileDto Exporter(TrialBalanceDto trialBalance) {
      Assertion.AssertObject(trialBalance, "trialBalance");

      var creator = new XmlFileCreator();

      XmlFile xmlFile = creator.CreateXmlFile(trialBalance);

      return XmlFileMapper.Map(xmlFile);
    }
  } // class XmlExporter

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
