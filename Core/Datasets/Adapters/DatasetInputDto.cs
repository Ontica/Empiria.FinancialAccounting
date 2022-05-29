/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Input DTO                               *
*  Type     : DatasetInputDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Input DTO used to retrive and describe data sets.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Datasets.Adapters {

  /// <summary>Input DTO used to retrive and describe data sets.</summary>
  public class DatasetInputDto {

    public DatasetInputDto() {
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


    public void EnsureIsValid() {
      Assertion.Require(DatasetFamilyUID, "DatasetFamilyUID");
      Assertion.Require(Date != ExecutionServer.DateMinValue, "Date");
    }

  }  // class DatasetInputDto

}  // namespace Empiria.FinancialAccounting.Datasets.Adapters
