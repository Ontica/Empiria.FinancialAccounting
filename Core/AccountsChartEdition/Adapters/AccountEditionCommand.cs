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

    DeleteAccount,

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

  }  // enum AccountDataToBeUpdated



  /// <summary>DTO used to update a sector rule.</summary>
  public class SectorInputRuleDto {

    public string Code {
      get; set;
    } = string.Empty;


    public AccountRole Role {
      get; set;
    } = AccountRole.Undefined;


    internal Sector Sector {
      get; set;
    }

  }  // class SectorInputRuleDto



  /// <summary>Command object used for edit an individual account.</summary>
  public class AccountEditionCommand : Command {

    public AccountEditionCommandType CommandType {
      get; set;
    } = AccountEditionCommandType.Undefined;


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


    internal bool SkipParentAccountValidation {
      get; set;
    } = false;


    public FixedList<string> Issues {
      get {
        return base.ExecutionResult.Issues;
      }
    }


    internal EntitiesType Entities {
      get; private set;
    }


    protected override string GetCommandTypeName() {
      return this.CommandType.ToString();
    }


    protected override void InitialRequire() {
      var validator = new AccountEditionCommandValidator(this);

      validator.InitialRequire();

      Entities = new EntitiesType(this);
    }


    protected override void SetIssues() {
      var validator = new AccountEditionCommandValidator(this);

      FixedList<string> issues = validator.GetIssues();

      if (issues.Count != 0) {
        base.ExecutionResult.AddIssues(issues);
      }
    }


    protected override void SetActions() {
      base.ExecutionResult.AddAction("Se modificará el catálogo de cuentas");
    }


    protected override void SetEntities() {
      Entities.AccountsChart = AccountsChart.Parse(AccountsChartUID);

      if (AccountUID.Length != 0) {
        Entities.Account = Account.Parse(AccountUID);
      }

      Entities.AccountType = AccountType.Parse(AccountFields.AccountTypeUID);
    }


    internal class EntitiesType {

      private readonly AccountEditionCommand _command;

      internal EntitiesType(AccountEditionCommand command) {
        _command = command;
      }

      public AccountsChart AccountsChart {
        get; internal set;
      }

      public Account Account {
        get; internal set;
      } = Account.Empty;


      public AccountType AccountType {
        get; internal set;
      } = AccountType.Empty;


      public FixedList<Currency> GetCurrencies() {
        return _command.Currencies.Select(x => Currency.Parse(x))
                                  .ToFixedList();
      }


      public FixedList<SectorInputRuleDto> GetSectorRules() {
        return _command.SectorRules.Select(x => {
                                              x.Sector = Sector.Parse(x.Code);
                                              return x;
                                           })
                                   .ToFixedList();
      }


    }  // inner class EntitiesType

  }  // class AccountEditionCommand

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters
