/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Service provider                        *
*  Type     : ExternalValuesWriter                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Persists financial external values belonging to a dataset.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.ExternalData.Adapters;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Persists financial external values belonging to a dataset.</summary>
  internal class ExternalValuesWriter {

    private readonly Dataset _dataset;
    private readonly FixedList<ExternalValueInputDto> _externalValues;

    public ExternalValuesWriter(Dataset dataset, FixedList<ExternalValueInputDto> externalValues) {
      Assertion.Require(dataset, nameof(dataset));
      Assertion.Require(externalValues, nameof(externalValues));

      _dataset = dataset;
      _externalValues = externalValues;
    }

    internal void Write() {
      foreach (var data in _externalValues) {
        Prepare(data);

        var externalValue = new ExternalValue(data);

        externalValue.Save();
      }
    }

    private void Prepare(ExternalValueInputDto data) {
      data.ApplicationDate = _dataset.OperationDate;
      data.UpdatedDate = _dataset.UpdatedTime;
      data.UpdatedBy = _dataset.UploadedBy;
      data.Dataset = _dataset;
      data.Status = _dataset.Status;
    }

  }  // class ExternalValuesWriter

}  // namespace Empiria.FinancialAccounting.ExternalData
