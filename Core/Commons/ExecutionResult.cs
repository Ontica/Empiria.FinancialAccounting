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


  public interface IExecutionCommand : IDto {

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


  /// <summary>Holds information about the result of an account edition.</summary>
  public class ExecutionResult : IInvariant {

    #region Fields

    private readonly List<string> _actions  = new List<string>();
    private readonly List<string> _issues   = new List<string>();
    private readonly List<string> _warnings = new List<string>();

    private readonly object _entity = null;


    #endregion Fields

    #region Constructor

    internal ExecutionResult(IExecutionCommand command) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(command.DryRun, "Constructor only allows dry-run commands.");

      this.Command  = command;
      this.DryRun   = true;

      Assertion.CheckInvariant(this);
    }


    internal ExecutionResult(IExecutionCommand command, object entity) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(entity,  nameof(entity));

      Assertion.Require(!command.DryRun, "This constructor only allows none dry-run commands.");

      this.Command  = command;
      _entity = entity;

      Assertion.CheckInvariant(this);
    }

    #endregion Constructor

    #region Properties

    public IExecutionCommand Command {
      get;
    }


    public IDto Instance {
      get; private set;
    }


    public string Message {
      get; private set;
    }


    public bool Commited {
      get;
      private set;
    }


    public bool DryRun {
      get;
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

      Assertion.CheckInvariant(this);
    }


    public void AddIssue(string issue) {
      Assertion.Require(issue, nameof(issue));

      EnsureNotCommited();

      _issues.Add(issue);

      Assertion.CheckInvariant(this);
    }


    public void AddWarning(string warning) {
      Assertion.Require(warning, nameof(warning));

      EnsureNotCommited();

      _warnings.Add(warning);

      Assertion.CheckInvariant(this);
    }


    public void EnsureCanBeCommited() {
      EnsureNotCommited();

      Assertion.Require(!this.DryRun,
        $"I can not commit command '{this.Command.Type}' " +
        $"because it was marked as dry-run.");

      Assertion.Require(_entity,
        $"I can not commit command '{this.Command.Type}' " +
        $"because this commitable entity was not assigned.");

      Assertion.Require(this.Issues.Count == 0,
        $"There were one or more issues executing command '{this.Command.Type}'. " +
        $"Please invoke the same command as a dry-run.");
    }


    public T GetEntity<T>() {
      Assertion.Require(_entity, "Entity object was not provided.");

      return (T) _entity;
    }


    public void MarkAsCommited(IDto instance) {
      MarkAsCommited(instance, "La operación fue ejecutada con éxito.");
    }


    public void MarkAsCommited(IDto instance, string message) {
      Assertion.Require(instance,  nameof(instance));
      Assertion.Require(message,   nameof(message));

      EnsureCanBeCommited();

      this.Instance = instance;
      this.Message  = message;
      this.Commited = true;

      Assertion.CheckInvariant(this);
    }


    private void EnsureNotCommited() {
      Assertion.Require(!this.Commited,
        $"Command '{this.Command.Type}' was already commited.");
    }


    bool IInvariant.Invariant() {
      return true;
    }

    #endregion Methods

  }  // class ExecutionResult

}  // namespace Empiria.FinancialAccounting
