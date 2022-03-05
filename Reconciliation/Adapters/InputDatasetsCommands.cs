/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Command payload                         *
*  Type     : Command for InputDatasets                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payloads used to retrive and store reconciliation input data sets.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Command payload used to retrive reconciliation input datasets.</summary>
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



  /// <summary>Command used to store reconciliation input datasets.</summary>
  public class StoreInputDatasetCommand {

    public StoreInputDatasetCommand() {
      // no-op
    }

    public string ReconciliationTypeUID {
      get; set;
    }

    public string DatasetType {
      get; set;
    }

    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    internal void EnsureValid() {
      Assertion.AssertObject(ReconciliationTypeUID, "ReconciliationTypeUID");
      Assertion.AssertObject(DatasetType, "DatasetType");
      Assertion.Assert(Date != ExecutionServer.DateMinValue, "Date");
    }

  }  // class StoreInputDatasetCommand


}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
