/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : XmlTemplateConfig                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds configuration data about xml file.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

namespace Empiria.FinancialAccounting.BanobrasIntegration.XmlReports {

  /// <summary>Holds configuration data about xml file.</summary>
  internal class XmlTemplateConfig : GeneralObject {

    #region Constructors and parsers

    protected XmlTemplateConfig() {
      // Required by Empiria Framework
    }

    static public string BaseUrl {
      get {
        return ConfigurationData.Get<string>("Reports.BaseUrl");
      }
    }


    static private string GenerationStoragePath {
      get {
        return ConfigurationData.Get<string>("Reports.GenerationStoragePath");
      }
    }

    #endregion

    #region Properties

    private string OriginalFileName {
      get {
        return base.ExtendedDataField.Get<string>("fileName");
      }
    }


    public string Title {
      get {
        return base.ExtendedDataField.Get<string>("title", string.Empty);
      }
    }

    #endregion

    #region Methods

    public string NewFilePath() {
      var copyFileName = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss-") + this.OriginalFileName;

      return Path.Combine(GenerationStoragePath, copyFileName);
    }

    #endregion

  }
}
