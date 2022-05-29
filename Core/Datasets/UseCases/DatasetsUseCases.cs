/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : DatasetsUseCases                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read and write data sets.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Services;

using Empiria.FinancialAccounting.Datasets.Adapters;
using Empiria.FinancialAccounting.Datasets.Data;

namespace Empiria.FinancialAccounting.Datasets.UseCases {

  /// <summary>Use cases used to read and write data sets.</summary>
  public class DatasetsUseCases : UseCase {

    #region Constructors and parsers

    protected DatasetsUseCases() {
      // no-op
    }


    static public DatasetsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<DatasetsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public Dataset CreateDataset(DatasetsCommand command, FileData fileData) {
      Assertion.Require(command, "command");
      Assertion.Require(fileData, "fileData");

      command.EnsureValid();

      FileInfo fileInfo = FileUtilities.SaveFile(fileData);

      var dataset = new Dataset(command, fileData, fileInfo);

      dataset.Save();

      return dataset;
    }


    public DatasetDto GetDataset(string datasetUID) {
      Assertion.Require(datasetUID, "datasetUID");

      var dataset = Dataset.Parse(datasetUID);

      return DatasetsMapper.Map(dataset);
    }


    public DatasetsLoadStatusDto GetDatasetsLoadStatus(DatasetsCommand command) {
      Assertion.Require(command, "command");

      command.EnsureValid();

      return BuildDatasetsLoadStatusDto(command.DatasetFamilyUID, command.Date);
    }


    public DatasetsLoadStatusDto RemoveDataset(string datasetUID) {
      Assertion.Require(datasetUID, "datasetUID");

      var dataset = Dataset.Parse(datasetUID);

      dataset.Delete();

      dataset.Save();

      return BuildDatasetsLoadStatusDto(dataset.DatasetFamily.UID, dataset.OperationDate);
    }


    public void RemoveOldDatasets(string datasetFamilyUID, TimeSpan timeSpan) {
      Assertion.Require(datasetFamilyUID, nameof(datasetFamilyUID));
      Assertion.Require(timeSpan, nameof(timeSpan));

      var family = DatasetFamily.Parse(datasetFamilyUID);

      FixedList<Dataset> datasetsToRemove =
                          DatasetData.GetDatasetsBeforeDate(family, DateTime.Now.Subtract(timeSpan));

      foreach (var dataset in datasetsToRemove) {
        dataset.Delete();
        dataset.Save();
      }
    }

    #endregion Use cases

    #region Helper methods

    private DatasetsLoadStatusDto BuildDatasetsLoadStatusDto(string datasetFamilyUID,
                                                             DateTime operationDate) {

      var datasetFamily = DatasetFamily.Parse(datasetFamilyUID);

      FixedList<Dataset> datasets = datasetFamily.GetDatasetsList(operationDate);

      FixedList<DatasetKind> missing = datasetFamily.MissingDatasetKinds(operationDate);

      return DatasetsMapper.MapToDatasetsLoadStatusDto(datasets, missing);
    }

    #endregion Helper methods

  } // class DatasetsUseCases

} // Empiria.FinancialAccounting.Datasets.UseCases
