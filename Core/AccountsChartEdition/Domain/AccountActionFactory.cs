/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Factory service                         *
*  Type     : AccountActionFactory                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Factory service for AccountAction instances.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Factory service for AccountAction instances.</summary>
  internal class AccountActionFactory {

    private readonly AccountsChart _accountsChart;
    private readonly AccountEditionCommand _command;

    public AccountActionFactory(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      _command = command;
      _accountsChart = _command.GetAccountsChart();
    }

    internal AccountAction BuildForAddCurrency(Account account, Currency currency) {
      Assertion.Require(account, nameof(account));
      Assertion.Require(currency, nameof(currency));

      DataOperation operation = GetAddCurrencyOperation(account, currency);

      return new AccountAction(operation, $"Agregar la moneda {currency.FullName} a la cuenta.");
    }


    internal AccountAction BuildForAddSector(Account account, SectorInputRuleDto sectorRule) {
      Assertion.Require(account, nameof(account));
      Assertion.Require(sectorRule, nameof(sectorRule));

      DataOperation operation = GetAddSectorOperation(account, sectorRule);

      return new AccountAction(operation, $"Agregar el sector {sectorRule.Sector.FullName} a la cuenta.");
    }


    internal AccountAction BuildForCreateAccount(AccountFieldsDto fields) {
      Assertion.Require(fields, nameof(fields));

      throw new NotImplementedException();
    }

    #region Data operation builders

    private DataOperation GetAddCurrencyOperation(Account account, Currency currency) {
      return DataOperation.Parse("apd_cof_currency", account.StandardAccountId,
                                  currency.Id,
                                 _command.ApplicationDate, ExecutionServer.DateMaxValue);
    }


    private DataOperation GetAddSectorOperation(Account account, SectorInputRuleDto sectorRule) {
      return DataOperation.Parse("apd_cof_sector", account.StandardAccountId,
                                 sectorRule.Sector.Id, (char) sectorRule.Role,
                                 _command.ApplicationDate, ExecutionServer.DateMaxValue);
    }

    #endregion Data operation builders

  }  // class AccountActionFactory

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
