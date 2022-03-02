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

      var reconciliationType = ReconciliationType.Parse(command.ReconciliationTypeUID);

      FixedList<InputDataset> datasets = reconciliationType.GetInputDatasetsList(command.Date);

      FixedList<InputDatasetType> missing = reconciliationType.MissingInputDatasetTypes(command.Date);

      return InputDatasetMapper.Map(datasets, missing);
    }


    public ReconciliationDatasetsDto ImportDatasetFromExcelFile(GetInputDatasetsCommand command,
                                                                FileData excelFile) {
      return GetDatasetsList(command);
    }


    public InputDatasetDto RemoveDataset(string inputDataSetUID) {
      throw new NotImplementedException();
    }

    #endregion Use cases

  } // class InputDatasetsUseCases

} // Empiria.FinancialAccounting.Reconciliation.UseCases
