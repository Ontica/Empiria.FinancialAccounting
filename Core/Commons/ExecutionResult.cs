/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Immutable Type                          *
*  Type     : ExecutionResult                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about the result of an account edition.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  public interface IDto {

  }


  public interface IExecutionCommand {

    string Type {
      get;
    }

    bool DryRun {
      get;
    }

  }


  public class EmptyDto : IDto {

    private static readonly EmptyDto _instance = new EmptyDto();

    private EmptyDto() {
      // no-op
    }

    public static IDto Instance => _instance;

  }  // class EmptyDto


  public class ExecutionResult<T> {

    public ExecutionResult(ExecutionResult result) {
      Assertion.Require(result, nameof(result));

      this.Command  = result.Command;
      this.Commited = result.Commited;
      this.Message  = result.Message;
      this.Actions  = result.Actions;
      this.Issues   = result.Issues;
      this.Warnings = result.Warnings;

      if (result.Commited) {
        this.Outcome = (T) result.Outcome;
      }
    }


    public IExecutionCommand Command {
      get;
    }


    public bool Commited {
      get;
    }


    public T Outcome {
      get;
    }


    public string Message {
      get;
    }


    public FixedList<string> Actions {
      get;
    }


    public FixedList<string> Issues {
      get;
    }


    public FixedList<string> Warnings {
      get;
    }

  }  // class ExecutionResult<T>


  /// <summary>Holds information about the result of an account edition.</summary>
  public class ExecutionResult : IInvariant {

    #region Fields

    private readonly List<string> _actions  = new List<string>();
    private readonly List<string> _issues   = new List<string>();
    private readonly List<string> _warnings = new List<string>();

    #endregion Fields

    #region Constructor

    public ExecutionResult(IExecutionCommand command) {
      Assertion.Require(command, nameof(command));

      this.Command  = command;

      AssertInvariant();
    }


    #endregion Constructor

    #region Properties

    public IExecutionCommand Command {
      get;
    }


    public IDto Outcome {
      get; private set;
    }


    public string Message {
      get; private set;
    }


    public bool Commited {
      get; private set;
    }


    public FixedList<string> Actions {
      get {
        return _actions.ToFixedList();
      }
    }


    public FixedList<string> Issues {
      get {
        return _issues.ToFixedList();
      }
    }


    public FixedList<string> Warnings {
      get {
        return _warnings.ToFixedList();
      }
    }

    #endregion Properties

    #region Methods

    public void AddAction(string action) {
      Assertion.Require(action, nameof(action));

      EnsureNotCommited();

      _actions.Add(action);

      AssertInvariant();
    }


    public void AddActionIf(bool condition, string issue) {
      if (condition) {
        AddAction(issue);
      }
    }

    public void AddIssue(string issue) {
      Assertion.Require(issue, nameof(issue));

      EnsureNotCommited();

      _issues.Add(issue);

      AssertInvariant();
    }


    public void AddIssueIf(bool condition, string issue) {
      if (condition) {
        AddIssue(issue);
      }
    }

    public void AddWarning(string warning) {
      Assertion.Require(warning, nameof(warning));

      EnsureNotCommited();

      _warnings.Add(warning);

      AssertInvariant();
    }


    public void AddWarningIf(bool condition, string issue) {
      if (condition) {
        AddWarning(issue);
      }
    }


    private void EnsureCanBeCommited() {
      EnsureNotCommited();

      Assertion.Require(!this.Command.DryRun,
        $"I can not commit command '{this.Command.Type}' " +
        $"because it was marked as dry-run.");

      Assertion.Require(this.Issues.Count == 0,
        $"There were one or more issues executing command '{this.Command.Type}'. " +
        $"Please invoke the same command as a dry-run.");
    }


    public void DryRunCompleted(string message) {
      Assertion.Require(message, nameof(message));

      this.Message = message;
    }


    public void MarkAsCommited(IDto outcome) {
      MarkAsCommited(outcome, "La operación fue ejecutada con éxito.");
    }


    public void MarkAsCommited(IDto outcome, string message) {
      Assertion.Require(outcome,  nameof(outcome));
      Assertion.Require(message,  nameof(message));

      EnsureCanBeCommited();

      this.Outcome = outcome;
      this.Message  = message;
      this.Commited = true;

      AssertInvariant();
    }

    private void EnsureNotCommited() {
      Assertion.Require(!this.Commited,
        $"Command '{this.Command.Type}' was already commited.");
    }


    private void AssertInvariant() {
      ((IInvariant) this).AssertInvariant();
    }

    void IInvariant.AssertInvariant() {
      Assertion.Ensure(Command, nameof(Command));
    }

    #endregion Methods

  }  // class ExecutionResult

}  // namespace Empiria.FinancialAccounting
