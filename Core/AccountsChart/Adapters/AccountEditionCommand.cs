/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : AccountEditionCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for accounts edition.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Command payload used for accounts edition.</summary>
  public class AccountEditionCommand {

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

    #endregion Public methods

  }  // AccountEditionCommandExtension

}  // namespace Empiria.FinancialAccounting.Adapters
