/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Use case interactor class               *
*  Type     : OperationalDataUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read and write operational data for reconciliation processes.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;
using Empiria.Storage;

using Empiria.DynamicData.Datasets;
using Empiria.DynamicData.Datasets.UseCases;
using Empiria.DynamicData.Datasets.Adapters;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.UseCases {

  /// <summary>Use cases used to read and write operational data for reconciliation processes.</summary>
  public class OperationalDataUseCases : UseCase {

    #region Constructors and parsers

    protected OperationalDataUseCases() {
      // no-op
    }


    static public OperationalDataUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<OperationalDataUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public DatasetsLoadStatusDto CreateDataset(OperationalDataDto dto,
                                               InputFile fileData) {
      Assertion.Require(dto,      nameof(dto));
      Assertion.Require(fileData, nameof(fileData));

      dto.EnsureIsValid();

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {
        var mappedDto = dto.MapToCoreDatasetInputDto();

        Dataset dataset = usecase.CreateDataset(mappedDto, fileData);

        var reader = new OperationalEntriesReader(dataset);

        if (!reader.AllEntriesAreValid()) {

          usecase.RemoveDataset(dataset.UID);

          Assertion.RequireFail(
            "El archivo tiene un formato que no reconozco o la información que contiene es incorrecta."
          );
        }
      }

      return GetDatasetsLoadStatus(dto);
    }


    public DatasetOutputDto GetDataset(string datasetUID) {
      Assertion.Require(datasetUID, nameof(datasetUID));

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {
        return usecase.GetDataset(datasetUID);
      }
    }


    public DatasetsLoadStatusDto GetDatasetsLoadStatus(OperationalDataDto dto) {
      Assertion.Require(dto, nameof(dto));

      dto.EnsureIsValid();

      RemoveOldDatasetsFor(dto.GetReconciliationType());

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {

        DatasetInputDto mappedDto = dto.MapToCoreDatasetInputDto();

        return usecase.GetDatasetsLoadStatus(mappedDto);
      }
    }


    public DatasetsLoadStatusDto RemoveDataset(string datasetUID) {
      Assertion.Require(datasetUID, nameof(datasetUID));

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {

        return usecase.RemoveDataset(datasetUID);
      }
    }


    internal void RemoveOldDatasetsFor(ReconciliationType reconciliationType) {
      const int REMOVE_EVERY_TWO_HOURS = 2;

      Assertion.Require(reconciliationType, nameof(reconciliationType));

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {

        usecase.RemoveOldDatasets(reconciliationType.UID,
                                  TimeSpan.FromHours(REMOVE_EVERY_TWO_HOURS));
      }
    }

    #endregion Use cases

  } // class OperationalDatasetsUseCases

} // Empiria.FinancialAccounting.Reconciliation.UseCases
