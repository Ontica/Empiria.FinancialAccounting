/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Information Holder                      *
*  Type     : InputDatasetType                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds configuration data for a reconciliation input dataset.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.FinancialAccounting.Reconciliation {

  public enum InputDatsetFormat {
    Standard
  }

  /// <summary>Holds configuration data for a reconciliation input dataset.</summary>
  public class InputDatasetType {

    protected internal InputDatasetType() {
      // no-op
    }

    static internal InputDatasetType Parse(JsonObject json) {
      return new InputDatasetType {
        UID = json.Get<string>("uid"),
        Name = json.Get<string>("name"),
        FileType = json.Get<FileType>("fileType", FileType.Csv),
        DataFormat = json.Get<InputDatsetFormat>("format", InputDatsetFormat.Standard),
        Optional = json.Get<bool>("optional", false),
        Count = json.Get<int>("count", 1)
      };
    }

    #region Properties

    public string UID {
      get; private set;
    }


    public string Name {
      get; private set;
    }


    public FileType FileType {
      get; private set;
    }


    public InputDatsetFormat DataFormat {
      get; private set;
    }


    public bool Optional {
      get; private set;
    }


    public int Count {
      get; private set;
    }

    #endregion Properties

  }  // class InputDatasetType

}  // namespace Empiria.FinancialAccounting.Reconciliation
