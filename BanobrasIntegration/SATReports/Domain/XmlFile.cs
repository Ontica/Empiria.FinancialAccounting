/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Excel Reports                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Service provider                      *
*  Type     : ExcelFile                                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides edition services for Xml files.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports {

  /// <summary></summary>
  internal class XmlFile {

    public XmlFile() {
    }

    #region Properties

    public string Url {
      get {
        return $"/{FileInfo.Name}";
      }
    }


    public FileInfo FileInfo {
      get; private set;
    }

    #endregion Properties

  } // class XmlFile

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports
