/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Operational Reports                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : OperationalReportTemplateConfig              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds configuration data about an operational report template file.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary>Holds configuration data about an operational report template file.</summary>
  internal class OperationalReportTemplateConfig : GeneralObject {
    #region Constructors and parsers

    protected OperationalReportTemplateConfig() {
      // Required by Empiria Framework
    }


    static internal OperationalReportTemplateConfig Parse(int id) {
      return BaseObject.ParseId<OperationalReportTemplateConfig>(id);
    }


    static internal OperationalReportTemplateConfig Parse(string uid) {
      return BaseObject.ParseKey<OperationalReportTemplateConfig>(uid);
    }


    static public string BaseUrl {
      get {
        return ConfigurationData.Get<string>("OperationalReportTemplateConfig.BaseUrl");
      }
    }


    static private string GenerationStoragePath {
      get {
        return ConfigurationData.Get<string>("OperationalReportTemplateConfig.GenerationStoragePath");
      }
    }


    static private string TemplatesStoragePath {
      get {
        return ConfigurationData.Get<string>("OperationalReportTemplateConfig.TemplatesStoragePath");
      }
    }

    #endregion
  } // class OperationalReportTemplateConfig

} // Empiria.FinancialAccounting.BanobrasIntegration.SATReports
