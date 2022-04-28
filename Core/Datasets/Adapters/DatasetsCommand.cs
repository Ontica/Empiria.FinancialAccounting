/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : DatasetsCommand                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to retrive and describe data sets.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Datasets.Adapters {

  /// <summary>Command payload used to describe datasets.</summary>
  public class DatasetsCommand {

    public DatasetsCommand() {
      // no-op
    }

    public string DatasetFamilyUID {
      get; set;
    }


    public string DatasetKind {
      get; set;
    } = String.Empty;


    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public void EnsureValid() {
      Assertion.AssertObject(DatasetFamilyUID, "DatasetFamilyUID");
      Assertion.Assert(Date != ExecutionServer.DateMinValue, "Date");
    }

  }  // class DatasetsCommand

}  // namespace Empiria.FinancialAccounting.Datasets.Adapters
