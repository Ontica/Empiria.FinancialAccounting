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
        Accounts = MapToAccountDescriptors(accountsChart.Accounts)
      };
    }

    static internal AccountDto MapAccount(Account account) {
      var dto = new AccountDto();

      FillAccountDescriptorDto(dto, account);

      dto.Currencies = account.Currencies;
      dto.Sectors = account.Sectors;

      return dto;
    }

    #region Private methods


    static private void FillAccountDescriptorDto(AccountDescriptorDto dto, Account account) {
      dto.UID = account.UID;
      dto.Number = account.Number;
      dto.Name = account.Name;
      dto.Type = account.AccountType;
      dto.Role = account.Role;
      dto.DebtorCreditor = account.DebtorCreditor;
      dto.Level = account.Level;
    }


    static private FixedList<AccountDescriptorDto> MapToAccountDescriptors(FixedList<Account> list) {
      return new FixedList<AccountDescriptorDto>(list.Select((x) => MapToAccountDescriptor(x)));
    }


    static private AccountDescriptorDto MapToAccountDescriptor(Account account) {
      var dto = new AccountDescriptorDto();

      FillAccountDescriptorDto(dto, account);

      return dto;
    }

    #endregion Private methods

  }  // class AccountsChartMapper

}  // namespace Empiria.FinancialAccounting.Adapters
