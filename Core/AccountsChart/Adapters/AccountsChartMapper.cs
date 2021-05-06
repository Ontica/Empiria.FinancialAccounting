/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : AccountsChartMapper                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for accounts charts and their contents.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for accounts charts and their contents.</summary>
  static internal class AccountsChartMapper {

    static internal AccountsChartDto Map(AccountsChart accountsChart) {
      return new AccountsChartDto {
        UID = accountsChart.UID,
        Name = accountsChart.Name,
        Accounts = MapAccounts(accountsChart.Accounts)
      };
    }

    #region Private methods

    static private FixedList<AccountDto> MapAccounts(FixedList<Account> list) {
      return new FixedList<AccountDto>(list.Select((x) => MapAccount(x)));
    }

    static private AccountDto MapAccount(Account account) {
      return new AccountDto {
        UID = account.UID,
        Number = account.Number,
        Name = account.Name,
        Type = account.AccountType,
        Role = account.Role,
        DebtorCreditor = account.DebtorCreditor
      };
    }

    #endregion Private methods

  }  // class AccountsChartMapper

}  // namespace Empiria.FinancialAccounting.Adapters
