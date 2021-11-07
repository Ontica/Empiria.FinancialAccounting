/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Excel Exporters                       *
*  Assembly : FinancialAccounting.Reporting.dll            Pattern   : Information Holder                    *
*  Type     : ExcelTemplateConfig                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds configuration data about a Microsoft Excel template file.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel {

  /// <summary>Holds configuration data about a Microsoft Excel template file.</summary>
  internal class ExcelTemplateConfig : GeneralObject {

    #region Constructors and parsers

    protected ExcelTemplateConfig() {
      // Required by Empiria Framework
    }


    static internal ExcelTemplateConfig Parse(int id) {
      return BaseObject.ParseId<ExcelTemplateConfig>(id);
    }


    static internal ExcelTemplateConfig Parse(string uid) {
      return BaseObject.ParseKey<ExcelTemplateConfig>(uid);
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


    static private string TemplatesStoragePath {
      get {
        return ConfigurationData.Get<string>("Reports.TemplatesStoragePath");
      }
    }

    #endregion Constructors and parsers

    #region Properties

    private string OriginalFileName {
      get {
        return base.ExtendedDataField.Get<string>("fileName");
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


    public string Title {
      get {
        return base.ExtendedDataField.Get<string>("title", string.Empty);
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

}  // namespace Empiria.FinancialAccounting.Reporting.Exporters.Excel
