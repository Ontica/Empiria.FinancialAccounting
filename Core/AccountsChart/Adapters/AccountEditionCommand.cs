﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountEditionCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command object used for accounts edition.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command object used for accounts edition.</summary>
  public class AccountEditionCommand {

    public AccountEditionCommandType Type {
      get; set;
    } = AccountEditionCommandType.Undefined;


    public bool DryRun {
      get; set;
    }


    public DateTime ApplicationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string AccountsChartUID {
      get; set;
    } = string.Empty;


    public string AccountUID {
      get; set;
    } = string.Empty;


    public AccountFieldsDto AccountFields {
      get; set;
    } = new AccountFieldsDto();


    public string[] Currencies {
      get; set;
    } = new string[0];


    public string[] Sectors {
      get; set;
    } = new string[0];


  }  // class AccountEditionCommand



  /// <summary>Extension methods for AccountEditionCommand interface adapter.</summary>
  static internal class AccountEditionCommandExtension {

    #region Public methods

    static internal AccountsChart GetAccountsChart(this AccountEditionCommand command) {
      Assertion.AssertObject(command.AccountsChartUID, "command.AccountsChartUID");

      return AccountsChart.Parse(command.AccountsChartUID);
    }


    static internal Account GetAccountToEdit(this AccountEditionCommand command) {
      Assertion.AssertObject(command.AccountUID, "command.AccountUID");

      return Account.Parse(command.AccountUID);
    }


    static internal FixedList<Currency> GetCurrencies(this AccountEditionCommand command) {
      return command.Currencies.Select(x => Currency.Parse(x))
                               .ToFixedList();
    }


    static internal FixedList<Sector> GetSectors(this AccountEditionCommand command) {
      return command.Sectors.Select(x => Sector.Parse(x))
                            .ToFixedList();
    }

    static internal void EnsureValid(this AccountEditionCommand command) {
      Assertion.AssertObject(command.AccountsChartUID, "command.AccountsChartUID");
      Assertion.Assert(command.Type != AccountEditionCommandType.Undefined, "command.Type");
      Assertion.Assert(command.ApplicationDate != ExecutionServer.DateMinValue, "command.ApplicationDate");

      if (command.Type != AccountEditionCommandType.CreateAccount) {
        command.EnsureAccountToEditIsValid();
      }

      switch (command.Type) {
        case AccountEditionCommandType.AddCurrencies:
        case AccountEditionCommandType.RemoveCurrencies:
          command.EnsureIsValidForCurrenciesEdition();
          break;

        case AccountEditionCommandType.AddSectors:
        case AccountEditionCommandType.RemoveSectors:
          command.EnsureIsValidForSectorsEdition();
          break;

        case AccountEditionCommandType.CreateAccount:
        case AccountEditionCommandType.UpdateAccount:
          Assertion.AssertObject(command.AccountFields, "command.AccountFields");

          break;
      }
    }

    static private void EnsureAccountToEditIsValid(this AccountEditionCommand command) {
      AccountsChart accountsChart = command.GetAccountsChart();
      Account accountToEdit = command.GetAccountToEdit();

      Assertion.AssertObject(command.AccountUID, "command.AccountUID");

      Assertion.Assert(accountToEdit.AccountsChart.Equals(accountsChart),
                       $"Account to be edited {accountToEdit.Number} does not belong to accounts chart {accountsChart.Name}.");

      Assertion.Assert(accountToEdit.EndDate == ExecutionServer.DateMaxValue,
                       "The given accountUID corresponds to an historic account, so it can not be edited.");

      Assertion.Assert(accountToEdit.StartDate < command.ApplicationDate,
                       $"The given accountUID has a start date {accountToEdit.StartDate} which is , so it can not be edited.");

    }


    static private void EnsureIsValidForCurrenciesEdition(this AccountEditionCommand command) {
      Assertion.AssertObject(command.Currencies, "command.Currencies");
      Assertion.Assert(command.Currencies.Length > 0, "Currencies list must have one or more values.");

      foreach (var currencyUID in command.Currencies) {
        try {
          _ = Currency.Parse(currencyUID);
        } catch {
          Assertion.AssertFail($"A currency with UID '{currencyUID}' does not exists.");
        }
      }
    }


    static private void EnsureIsValidForSectorsEdition(this AccountEditionCommand command) {
      Assertion.AssertObject(command.Sectors, "command.Sectors");
      Assertion.Assert(command.Sectors.Length > 0, "Sectors list must have one or more values.");

      foreach (var sectorUID in command.Sectors) {
        try {
          _ = Sector.Parse(sectorUID);
        } catch {
          Assertion.AssertFail($"A sector with UID '{sectorUID}' does not exists.");
        }
      }
    }

    #endregion Public methods

  }  // AccountEditionCommandExtension

}  // namespace Empiria.FinancialAccounting.Adapters
