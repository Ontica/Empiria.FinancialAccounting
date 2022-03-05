/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Use case interactor class               *
*  Type     : InputDatasetsUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read and write reconciliation input data sets.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Services;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.UseCases {

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

      return BuildReconciliationDatasetsDto(command.ReconciliationTypeUID, command.Date);
    }


    public ReconciliationDatasetsDto ImportDatasetFromFile(StoreInputDatasetCommand command,
                                                           FileData datasetFileData) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(datasetFileData, "datasetFileData");

      command.EnsureValid();

      FileInfo fileInfo = FileUtilities.SaveFile(datasetFileData);

      var inputDataset = new InputDataset(command, fileInfo);

      inputDataset.Save();

      return BuildReconciliationDatasetsDto(command.ReconciliationTypeUID, command.Date);
    }


    public ReconciliationDatasetsDto RemoveDataset(string inputDataSetUID) {
      Assertion.AssertObject(inputDataSetUID, "inputDataSetUID");

      var inputDataset = InputDataset.Parse(inputDataSetUID);

      inputDataset.Delete();

      return BuildReconciliationDatasetsDto(inputDataset.ReconciliationType.UID,
                                            inputDataset.ReconciliationDate);
    }

    #endregion Use cases

    #region Helper methods

    private ReconciliationDatasetsDto BuildReconciliationDatasetsDto(string reconciliationTypeUID,
                                                                     DateTime reconciliationDate) {
      var reconciliationType = ReconciliationType.Parse(reconciliationTypeUID);

      FixedList<InputDataset> datasets = reconciliationType.GetInputDatasetsList(reconciliationDate);

      FixedList<InputDatasetType> missing = reconciliationType.MissingInputDatasetTypes(reconciliationDate);

      return InputDatasetMapper.Map(datasets, missing);
    }

    #endregion Helper methods

  } // class InputDatasetsUseCases

} // Empiria.FinancialAccounting.Reconciliation.UseCases
