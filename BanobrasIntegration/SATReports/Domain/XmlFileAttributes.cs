/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Xml reports                           *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Empiria Plain Object                  *
*  Type     : XmlFileAttributes                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents an attributes entry for an xml file.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  /// <summary>Represents an attributes entry for an xml file.</summary>
  public class XmlFileAttributes {

    public string RFC {
      get; set;
    } = "BNO670315CD0";

    public string Version {
      get; set;
    } = "1.3";

    public string Month {
      get; set;
    }

    public string Year {
      get; set;
    }

    public string Name {
      get; set;
    }

    public string Property {
      get; set;
    }

  } // class XmlFileAttributes

} // namespace Empiria.FinancialAccounting.BanobrasIntegration
