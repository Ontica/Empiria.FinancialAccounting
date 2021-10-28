/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service                               *
*  Type     : ExcelExporter                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Main service to export accounting information to Microsoft Excel.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports;

namespace Empiria.FinancialAccounting.BanobrasIntegration.XmlReports {

  /// <summary>Main service to export accounting information to Microsoft Excel.</summary>
  public class XmlExporter {

    public FileReportDto ExportToXml(OperationalReportDto reportDTO) {

      var xmlFileCreator = new OperationalReportXmlFileCreator();

      XmlFile xmlFile = xmlFileCreator.CreateXmlFile(reportDTO);

      return xmlFile.ToFileReportDto();
    }

  }  // class XmlExporter

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports
