/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data Transfer Object                    *
*  Type     : ReconciliationResultDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with a reconciliation result.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Output DTO with a reconciliation result.</summary>
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

    public FixedList<ExportToDto> ExportTo {
      get; internal set;
    } = new FixedList<ExportToDto>();


  }  // class ReconciliationTypeDto

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
