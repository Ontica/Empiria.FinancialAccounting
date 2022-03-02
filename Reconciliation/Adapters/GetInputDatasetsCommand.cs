/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Command payload                         *
*  Type     : GetInputDatasetsCommand                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to retrive reconciliation input data sets.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Command payload used to retrive reconciliation input data sets.</summary>
  public class GetInputDatasetsCommand {

    public GetInputDatasetsCommand() {
      // no-op
    }

    public string ReconciliationTypeUID {
      get; set;
    }

    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    internal void EnsureValid() {
      Assertion.AssertObject(ReconciliationTypeUID, "ReconciliationTypeUID");
      Assertion.Assert(Date != ExecutionServer.DateMinValue, "Date");
    }

  }  // class GetInputDatasetsCommand

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
