/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Xml Exporters                         *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : Standard Class                        *
*  Type     : XmlFile                                      License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for operational report files.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Xml;

using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Xml {

  /// <summary>Provides edition services for operational report files.</summary>
  internal class XmlFile {


    static private string GenerationStoragePath = ConfigurationData.Get<string>("Reports.GenerationStoragePath");

    static private string BaseUrl = ConfigurationData.Get<string>("Reports.BaseUrl");


    public XmlFile(XmlDocument xmlDocument) {
      Assertion.Require(xmlDocument, "xmlDocument");

      this.XmlDocument = xmlDocument;
    }


    #region Properties

    public XmlDocument XmlDocument {
      get;
    }


    public string Url {
      get {
        return $"{BaseUrl}/{FileInfo.Name}";
      }
    }


    public FileInfo FileInfo {
      get; private set;
    }


    internal FileReportDto ToFileReportDto() {
      return new FileReportDto(FileType.Xml, this.Url);
    }


    #endregion Properties

    #region Methods

    internal void Save(string fileName) {
      var copyFileName = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss-") + fileName + ".xml";
      var path = Path.Combine(GenerationStoragePath, copyFileName);

      XmlDocument.Save(path);

      this.FileInfo = new FileInfo(path);
    }

    #endregion Methods

  } // class XmlFile

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
