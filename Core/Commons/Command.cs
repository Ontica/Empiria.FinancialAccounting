/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Base type                               *
*  Type     : Command                                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command base payload.                                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Command base payload.</summary>
  public abstract class Command : IExecutionCommand {

    private readonly ExecutionResult _executionResult;

    protected Command() {
      _executionResult = new ExecutionResult(this);
    }

    #region Properties

    string IExecutionCommand.Type {
      get {
        return GetCommandTypeName();
      }
    }

    public bool DryRun {
      get; set;
    }


    protected ExecutionResult ExecutionResult {
      get {
        return _executionResult;
      }
    }

    public ExecutionResult<T> MapToExecutionResult<T>() {
      return new ExecutionResult<T>(this.ExecutionResult);
    }

    public bool IsValid {
      get {
        return (_executionResult.Issues.Count == 0);
      }
    }

    #endregion Properties

    #region Methods

    protected virtual void Initialize() {
      // no-op
    }

    protected virtual void InitialRequire() {
      // no-op
    }


    protected abstract string GetCommandTypeName();


    protected virtual void FinalRequire() {
      // no-op
    }


    protected virtual void SetActions() {
      // no-op
    }


    protected virtual void SetEntities() {
      // no-op
    }


    protected virtual void SetIssues() {
      // no-op
    }


    public void Done(IDto outcome, string message) {
      Assertion.Require(outcome, nameof(outcome));
      Assertion.Require(message, nameof(message));

      _executionResult.MarkAsCommited(outcome, message);
    }


    public void Arrange() {
      Initialize();
      InitialRequire();
      SetEntities();
      SetIssues();
      SetActions();
      FinalRequire();
    }

    #endregion Methods

  }  // class Command

}  // namespace Empiria.FinancialAccounting
