/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Mapper class                            *
*  Type     : ReconciliationDatasetsDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map reconciliation input data sets.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  public class ReconciliationDatasetsDto {

    internal ReconciliationDatasetsDto() {
      // no-op
    }

    public FixedList<InputDatasetDto> Loaded {
      get; internal set;
    }

    public FixedList<InputDatasetTypeDto> Missing {
      get; internal set;
    }

  }  // class ReconciliationDatasetsDto

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
