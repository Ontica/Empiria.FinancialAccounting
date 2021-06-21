/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Office Integration                           Component : Excel Exporter                        *
*  Assembly : FinancialAccounting.OficeIntegration.dll     Pattern   : Information Holder                    *
*  Type     : ExcelTemplateConfig                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds configuration data about a Microsoft Excel template file.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

namespace Empiria.FinancialAccounting.OfficeIntegration {

  /// <summary>Holds configuration data about a Microsoft Excel template file.</summary>
  internal class ExcelTemplateConfig : GeneralObject {

    #region Constructors and parsers

    private ExcelTemplateConfig() {
      // Required by Empiria Framework
    }

    static internal ExcelTemplateConfig Parse(string uid) {
      return BaseObject.ParseKey<ExcelTemplateConfig>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    static public string BaseUrl {
      get {
        return ConfigurationData.Get<string>("ExcelTemplateConfig.BaseUrl");
      }
    }

    private string OriginalFileName {
      get {
        return base.ExtendedDataField.Get<string>("fileName");
      }
    }


    static private string GenerationStoragePath {
      get {
        return ConfigurationData.Get<string>("ExcelTemplateConfig.GenerationStoragePath");
      }
    }


    private string TemplateFileName {
      get {
        return base.ExtendedDataField.Get<string>("templateFile");
      }
    }


    public string TemplateFullPath {
      get {
        return Path.Combine(TemplatesStoragePath, this.TemplateFileName);
      }
    }


    static private string TemplatesStoragePath {
      get {
        return ConfigurationData.Get<string>("ExcelTemplateConfig.TemplatesStoragePath");
      }
    }


    #endregion Properties

    #region Methods

    public string NewFilePath() {
      var copyFileName = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss-") + this.OriginalFileName;

      return Path.Combine(GenerationStoragePath, copyFileName);
    }

    #endregion Methods

  }  // class ExcelTemplateConfig

}  // namespace Empiria.FinancialAccounting.OfficeIntegration
