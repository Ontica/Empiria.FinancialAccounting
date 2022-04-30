/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Use case interactor class               *
*  Type     : ReconciliationExecutionUseCases            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to execute financial accounting reconciliation processes.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.UseCases {

  /// <summary>Use cases used to execute financial accounting reconciliation processes.</summary>
  public class ReconciliationExecutionUseCases : UseCase {

    #region Constructors and parsers

    protected ReconciliationExecutionUseCases() {
      // no-op
    }


    static public ReconciliationExecutionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ReconciliationExecutionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public ReconciliationResultDto Execute(ReconciliationCommand command) {
      Assertion.AssertObject(command, "command");

      command.EnsureValid();

      return ReconciliationMapper.Map(command);
    }

    #endregion Use cases

  } // class ReconciliationExecutionUseCases

} // Empiria.FinancialAccounting.Reconciliation.UseCases
