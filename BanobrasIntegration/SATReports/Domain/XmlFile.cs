/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml Reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service provider                      *
*  Type     : ExcelFile                                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for Xml files.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Xml;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary></summary>
  internal class XmlFile {

    public XmlFile() {
      
    }

    #region Properties

    public XmlDocument XmlStructure {
      get; set;
    }

    public string XmlDesign {
      get; set;
    }

    public string Url {
      get {
        return $"{OperationalReportTemplateConfig.BaseUrl}/{FileInfo.Name}";
      }
    }


    public FileInfo FileInfo {
      get; private set;
    }

    #endregion Properties

  } // class XmlFile

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
