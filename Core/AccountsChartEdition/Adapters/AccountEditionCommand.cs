/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountEditionCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command object used for accounts edition.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters {

  /// <summary>Enumerated constants type used to classify AccountEditionCommand types.</summary>
  public enum AccountEditionCommandType {

    CreateAccount,

    FixAccountName,

    UpdateAccount,

    Undefined

  }  // enum AccountEditionCommandType


  /// <summary>Enumerated constants type used to classify the updated data for an edition command.</summary>
  public enum AccountDataToBeUpdated {

    AccountType,

    Currencies,

    DebtorCreditor,

    MainRole,

    Name,

    Sectors,

    SubledgerRole

  }  // enum AccountDataEdition



  /// <summary>DTO used to update a sector rule.</summary>
  public class SectorInputRuleDto {

    public string Code {
      get; set;
    } = string.Empty;


    public Sector Sector {
      get; internal set;
    }


    public AccountRole Role {
      get; set;
    } = AccountRole.Undefined;


  }  // class SectorInputRuleDto



  /// <summary>Command object used for edit an individual account.</summary>
  public class AccountEditionCommand : Command {

    public AccountEditionCommandType CommandType {
      get; set;
    } = AccountEditionCommandType.Undefined;


    protected override string GetCommandTypeName() {
      return this.CommandType.ToString();
    }

    public string AccountsChartUID {
      get; set;
    } = string.Empty;


    public DateTime ApplicationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string AccountUID {
      get; set;
    } = string.Empty;


    public AccountFieldsDto AccountFields {
      get; set;
    } = new AccountFieldsDto();


    public string[] Currencies {
      get; set;
    } = new string[0];


    public SectorInputRuleDto[] SectorRules {
      get; set;
    } = new SectorInputRuleDto[0];


    public AccountDataToBeUpdated[] DataToBeUpdated {
      get; set;
    } = new AccountDataToBeUpdated[0];


    public string CommandText {
      get; internal set;
    } = string.Empty;


    public string DataSource {
      get; internal set;
    } = string.Empty;


    public FixedList<string> Issues {
      get {
        return base.ExecutionResult.Issues;
      }
    }


    internal EntitiesType Entities {
      get; private set;
    } = new EntitiesType();


    protected override void InitialRequire() {
      Assertion.Require(this.CommandType != AccountEditionCommandType.Undefined, "CommandType");
      Assertion.Require(this.AccountsChartUID, "AccountsChartUID");
      Assertion.Require(this.ApplicationDate != ExecutionServer.DateMinValue, "ApplicationDate");
      Assertion.Require(this.AccountFields, "AccountFields");
      Assertion.Require(this.AccountFields.AccountNumber, "AccountFields.AccountNumber");
      Assertion.Require(this.AccountFields.Name, "AccountFields.Name");
      Assertion.Require(this.AccountFields.AccountTypeUID, "AccountFields.AccountTypeUID");
      Assertion.Require(this.AccountFields.Role != AccountRole.Undefined,
                        "AccountFields.Role");
      Assertion.Require(this.AccountFields.DebtorCreditor != DebtorCreditorType.Undefined,
                        "AccountFields.DebtorCreditor");

      if (this.CommandType == AccountEditionCommandType.CreateAccount) {
        Assertion.Require(this.AccountUID.Length == 0,
                          "command.AccountUID was provided but it's not needed for a CreateAccount command.");

      } else if (this.CommandType == AccountEditionCommandType.FixAccountName) {
        Assertion.Require(this.AccountUID, "AccountUID");


      } else if (this.CommandType == AccountEditionCommandType.UpdateAccount) {
        Assertion.Require(this.AccountUID, "AccountUID");
        Assertion.Require(this.DataToBeUpdated, "DataToBeUpdated");
        Assertion.Require(this.DataToBeUpdated.Length != 0, "DataToBeUpdated must contain one or more values.");
      }
    }



    protected override void SetIssues() {

      if (this.CommandType == AccountEditionCommandType.CreateAccount) {

        AccountsChart accountsChart = this.Entities.AccountsChart;

        Assertion.Require(accountsChart.IsValidAccountNumber(this.AccountFields.AccountNumber),
                          $"Account number '{this.AccountFields.AccountNumber}' has an invalid format.");

      }

      if (this.CommandType != AccountEditionCommandType.CreateAccount) {
        AccountsChart accountsChart = this.Entities.AccountsChart;
        Account accountToEdit = this.Entities.Account;

        Assertion.Require(accountToEdit.AccountsChart.Equals(accountsChart),
                         $"Account to be edited {accountToEdit.Number} does not belong to " +
                         $"the chart of accounts {accountsChart.Name}.");

        Assertion.Require(accountToEdit.EndDate == Account.MAX_END_DATE,
                         "The given accountUID corresponds to an historic account, so it can not be edited.");

        Assertion.Require(accountToEdit.StartDate <= this.ApplicationDate,
                         $"ApplicationDate parameter ({this.ApplicationDate.ToString("dd/MMM/yyyy")}) " +
                         $"must be greater or equal than the given account's " +
                         $"start date {accountToEdit.StartDate.ToString("dd/MMM/yyyy")}.");
      }
    }


    protected override void SetEntities() {
      Entities.AccountsChart = AccountsChart.Parse(AccountsChartUID);

      if (AccountUID.Length != 0) {
        Entities.Account = Account.Parse(AccountUID);
      }

      Entities.AccountType = AccountType.Parse(AccountFields.AccountTypeUID);

      Entities.Currencies = this.Currencies.Select(x => Currency.Parse(x))
                                           .ToFixedList();

      Entities.SectorRules = this.SectorRules.Select(x => {
                                                        x.Sector = Sector.Parse(x.Code);
                                                        return x;
                                                     })
                                             .ToFixedList();
    }



    internal class EntitiesType {

      public AccountsChart AccountsChart {
        get; internal set;
      }

      public Account Account {
        get; internal set;
      } = Account.Empty;


      public AccountType AccountType {
        get; internal set;
      } = AccountType.Empty;


      public FixedList<Currency> Currencies {
        get; internal set;
      } = new FixedList<Currency>();


      public FixedList<SectorInputRuleDto> SectorRules {
        get; internal set;
      } = new FixedList<SectorInputRuleDto>();


    }  // inner class EntitiesType

  }  // class AccountEditionCommand

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters
