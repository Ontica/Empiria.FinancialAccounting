/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : InputDatasetsUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read and write reconciliation input data sets.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Services;

using Empiria.FinancialAccounting.Datasets.Adapters;

namespace Empiria.FinancialAccounting.Datasets.UseCases {

  /// <summary>Use cases used to read and write input reconciliation data sets.</summary>
  public class InputDatasetsUseCases : UseCase {

    #region Constructors and parsers

    protected InputDatasetsUseCases() {
      // no-op
    }


    static public InputDatasetsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<InputDatasetsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public InputDatasetDto GetDataset(string inputDatasetUID) {
      throw new NotImplementedException();
    }


    public ReconciliationDatasetsDto GetDatasetsList(GetInputDatasetsCommand command) {
      Assertion.AssertObject(command, "command");

      command.EnsureValid();

      return BuildDatasetDto(command.ReconciliationTypeUID, command.Date);
    }


    public ReconciliationDatasetsDto ImportDatasetFromFile(StoreInputDatasetCommand command,
                                                           FileData datasetFileData) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(datasetFileData, "datasetFileData");

      command.EnsureValid();

      FileInfo fileInfo = FileUtilities.SaveFile(datasetFileData);

      var inputDataset = new InputDataset(command, fileInfo);

      inputDataset.Save();

      return BuildDatasetDto(command.ReconciliationTypeUID, command.Date);
    }


    public ReconciliationDatasetsDto RemoveDataset(string inputDataSetUID) {
      Assertion.AssertObject(inputDataSetUID, "inputDataSetUID");

      var inputDataset = InputDataset.Parse(inputDataSetUID);

      inputDataset.Delete();

      inputDataset.Save();

      return BuildDatasetDto(inputDataset.DatasetType.UID, inputDataset.OperationDate);
    }

    #endregion Use cases

    #region Helper methods

    private ReconciliationDatasetsDto BuildDatasetDto(string datasetTypeUID, DateTime operationDate) {
      var datasetType = DatasetType.Parse(datasetTypeUID);

      FixedList<InputDataset> datasets = datasetType.GetInputDatasetsList(operationDate);

      FixedList<InputDatasetType> missing = datasetType.MissingInputDatasetTypes(operationDate);

      return InputDatasetMapper.Map(datasets, missing);
    }

    #endregion Helper methods

  } // class InputDatasetsUseCases

} // Empiria.FinancialAccounting.Datasets.UseCases
