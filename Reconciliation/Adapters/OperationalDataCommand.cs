/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Command payload                         *
*  Type     : OperationalDataCommand                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to describe operational data using datasets.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Command payload used to describe operational data using datasets.</summary>
  public class OperationalDataCommand {

    public OperationalDataCommand() {
      // no-op
    }

    public string ReconciliationTypeUID {
      get; set;
    }


    public string DatasetKind {
      get; set;
    } = String.Empty;


    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public void EnsureValid() {
      Assertion.AssertObject(ReconciliationTypeUID, "ReconciliationTypeUID");
      Assertion.Assert(Date != ExecutionServer.DateMinValue, "Date");
    }


    internal DatasetsCommand MapToCoreDatasetsCommand() {
      var type = ReconciliationType.Parse(ReconciliationTypeUID);

      return new DatasetsCommand {
        DatasetFamilyUID = type.DatasetFamily.UID,
        DatasetKind = this.DatasetKind,
        Date = this.Date
      };
    }

  }  // class OperationalDataCommand

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
