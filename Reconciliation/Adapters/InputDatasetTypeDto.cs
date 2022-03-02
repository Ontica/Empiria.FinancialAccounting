/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Mapper class                            *
*  Type     : InputDatasetMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map reconciliation input data sets.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  public class InputDatasetTypeDto {

    public string Name {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public FileType FileType {
      get; internal set;
    }

    public bool Optional {
      get; internal set;
    }

    public int Count {
      get; internal set;
    }

  }

}
