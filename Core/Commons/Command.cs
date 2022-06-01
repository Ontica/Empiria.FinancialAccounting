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
  public class Command : IExecutionCommand {

    private readonly ExecutionResult _executionResult;

    public Command() {
      _executionResult = new ExecutionResult(this);
    }

    #region Properties

    public string Type {
      get; set;
    } = string.Empty;


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

    protected virtual void Clean() {
      // no-op
    }


    protected virtual void Require() {
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


    protected virtual void SetWarnings() {
      // no-op
    }


    public void Done(IDto outcome, string message) {
      Assertion.Require(outcome, nameof(outcome));
      Assertion.Require(message, nameof(message));

      _executionResult.MarkAsCommited(outcome, message);
    }


    public void Arrange() {
      Clean();
      Require();
      SetIssues();
      SetWarnings();
      SetEntities();
      SetActions();
    }

    #endregion Methods

  }  // class Command

}  // namespace Empiria.FinancialAccounting
