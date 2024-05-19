/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Input DTO                               *
*  Type     : OperationalDataDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Inpuit DTO used to describe operational data using datasets.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DynamicData.Datasets.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Input DTO used to describe operational data using datasets.</summary>
  public class OperationalDataDto {

    public OperationalDataDto() {
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


  }  // class OperationalDataDto


  /// <summary>OperationalDataDto type extension methods.</summary>
  static internal class OperationalDataDtoExtensions {

    static public void EnsureIsValid(this OperationalDataDto dto) {
      Assertion.Require(dto.ReconciliationTypeUID, nameof(dto.ReconciliationTypeUID));
      Assertion.Require(dto.Date != ExecutionServer.DateMinValue, nameof(dto.Date));
    }


    static internal ReconciliationType GetReconciliationType(this OperationalDataDto dto) {
      return ReconciliationType.Parse(dto.ReconciliationTypeUID);
    }


    static internal DatasetInputDto MapToCoreDatasetInputDto(this OperationalDataDto dto) {
      var type = ReconciliationType.Parse(dto.ReconciliationTypeUID);

      return new DatasetInputDto {
        DatasetFamilyUID = type.UID,
        DatasetKind      = dto.DatasetKind,
        Date             = dto.Date
      };
    }

  }  // class OperationalDataDtoExtensions

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
