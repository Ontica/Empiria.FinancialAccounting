/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Operational Reports                   *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service provider                      *
*  Type     : OperationalReportFile                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for operational report files.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Xml;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary>Provides edition services for operational report files.</summary>
  internal class OperationalReportFile {


    static private string GenerationStoragePath = ConfigurationData.Get<string>("Reports.GenerationStoragePath");


    public OperationalReportFile() {

    }

    #region Properties

    public XmlDocument XmlStructure {
      get; set;
    }


    public string Url {
      get {
        return $"{GenerationStoragePath}/{FileInfo.Name}";
      }
    }


    public FileInfo FileInfo {
      get; private set;
    }

    #endregion Properties

  } // class OperationalReportFile

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
