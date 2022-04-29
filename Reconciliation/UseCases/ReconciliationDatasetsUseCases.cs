/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : ReconciliationDatasetsUseCases             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read and write reconciliation data sets.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Datasets.UseCases;
using Empiria.FinancialAccounting.Datasets.Adapters;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.UseCases {

  /// <summary>Use cases used to read and write reconciliation data sets.</summary>
  public class ReconciliationDatasetsUseCases : UseCase {

    #region Constructors and parsers

    protected ReconciliationDatasetsUseCases() {
      // no-op
    }


    static public ReconciliationDatasetsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ReconciliationDatasetsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public DatasetDto GetDataset(string datasetUID) {
      Assertion.AssertObject(datasetUID, "datasetUID");

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {
        return usecase.GetDataset(datasetUID);
      }
    }


    public DatasetsLoadStatusDto GetDatasetsLoadStatus(ReconciliationDatasetsCommand command) {
      Assertion.AssertObject(command, "command");

      command.EnsureValid();

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {
        var coreDatasetCommand = command.MapToCoreDatasetsCommand();

        return usecase.GetDatasetsLoadStatus(coreDatasetCommand);
      }
    }


    public DatasetsLoadStatusDto ImportDatasetFromFile(ReconciliationDatasetsCommand command,
                                                       FileData fileData) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(fileData, "fileData");

      command.EnsureValid();

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {
        var coreDatasetCommand = command.MapToCoreDatasetsCommand();

        return usecase.ImportDatasetFromFile(coreDatasetCommand, fileData);
      }
    }


    public DatasetsLoadStatusDto RemoveDataset(string datasetUID) {
      Assertion.AssertObject(datasetUID, "datasetUID");

      using (var usecase = DatasetsUseCases.UseCaseInteractor()) {
        return usecase.RemoveDataset(datasetUID);
      }
    }

    #endregion Use cases

  } // class ReconciliationDatasetsUseCases

} // Empiria.FinancialAccounting.Reconciliation.UseCases
