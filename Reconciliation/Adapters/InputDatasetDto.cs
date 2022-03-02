/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data Transfer Object                    *
*  Type     : InputDatasetDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a reconciliation input data set.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Output DTO used to return a reconciliation data set.</summary>
  public class InputDatasetDto {

    internal InputDatasetDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

  }  // class InputDatasetDto

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
