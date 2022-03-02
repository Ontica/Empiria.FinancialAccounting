/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data Transfer Object                    *
*  Type     : ReconciliationTypeDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return reconciliation types data.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Output DTO used to return reconciliation types data.</summary>
  public class ReconciliationTypeDto {

    internal ReconciliationTypeDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


  }  // class ReconciliationTypeDto

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
