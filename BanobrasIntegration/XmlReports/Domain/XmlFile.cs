/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service provider                      *
*  Type     : OperationalReportFile                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for operational report files.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Xml;

namespace Empiria.FinancialAccounting.BanobrasIntegration.XmlReports {

  /// <summary>Provides edition services for operational report files.</summary>
  internal class XmlFile {


    static private string GenerationStoragePath = ConfigurationData.Get<string>("Reports.GenerationStoragePath");
    static private string BaseUrl = ConfigurationData.Get<string>("Reports.BaseUrl");

    public XmlFile() {

    }


    #region Properties

    public XmlDocument XmlStructure {
      get; set;
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

    internal void Save(XmlDocument xmlStructure, string reportType) {
      if (xmlStructure != null) {
        var copyFileName = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss-") + reportType + ".xml";
        var path = Path.Combine(GenerationStoragePath, copyFileName);
        xmlStructure.Save(path);
        this.FileInfo = new FileInfo(path);
      }
    }

    #endregion
  } // class OperationalReportFile

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
