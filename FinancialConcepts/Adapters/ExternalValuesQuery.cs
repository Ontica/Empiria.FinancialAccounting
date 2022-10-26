/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Query payload                           *
*  Type     : ExternalValuesQuery                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to retrieve external values.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Datasets.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Query payload used to retrieve external values.</summary>
  public class ExternalValuesQuery {

    public ExternalValuesQuery() {
      // no-op
    }

    public string ExternalVariablesSetUID {
      get; set;
    }

    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string ExportTo {
      get; set;
    } = string.Empty;


    internal ExternalVariablesSet GetExternalVariablesSet() {
      return ExternalVariablesSet.Parse(this.ExternalVariablesSetUID);
    }


    public void EnsureValid() {
      Assertion.Require(ExternalVariablesSetUID, "ExternalVariablesSetUID");
      Assertion.Require(Date != ExecutionServer.DateMinValue, "Date");
    }

  }  // class ExternalValuesQuery



  /// <summary>Input DTO used to describe external variables data using datasets.</summary>
  public class ExternalValuesDatasetDto {

    public ExternalValuesDatasetDto() {
      // no-op
    }

    public string ExternalVariablesSetUID {
      get; set;
    }


    public string DatasetKind {
      get; set;
    } = String.Empty;



    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;

  }  // class ExternalValuesDatasetDto



  /// <summary>ExternalValuesDatasetDto type extension methods.</summary>
  static internal class ExternalValuesDatasetDtoExtensions {

    static public void EnsureIsValid(this ExternalValuesDatasetDto dto) {
      Assertion.Require(dto.ExternalVariablesSetUID, nameof(dto.ExternalVariablesSetUID));
      Assertion.Require(dto.Date != ExecutionServer.DateMinValue, nameof(dto.Date));
    }


    static internal ExternalVariablesSet GetExternalVariablesSet(this ExternalValuesDatasetDto dto) {
      return ExternalVariablesSet.Parse(dto.ExternalVariablesSetUID);
    }


    static internal DatasetInputDto MapToCoreDatasetInputDto(this ExternalValuesDatasetDto dto) {
      var set = ExternalVariablesSet.Parse(dto.ExternalVariablesSetUID);

      return new DatasetInputDto {
        DatasetFamilyUID = set.UID,
        DatasetKind = dto.DatasetKind,
        Date = dto.Date
      };
    }

  }  // class ExternalValuesDatasetDtoExtensions

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
