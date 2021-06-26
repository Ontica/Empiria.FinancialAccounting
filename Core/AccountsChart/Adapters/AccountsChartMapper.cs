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
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for accounts charts and their contents.</summary>
  static public class AccountsChartMapper {

    static internal AccountsChartDto Map(AccountsChart accountsChart) {
      return new AccountsChartDto {
        UID = accountsChart.UID,
        Name = accountsChart.Name,
        Accounts = MapToAccountDescriptors(accountsChart.Accounts)
      };
    }


    static internal AccountsChartDto Map(AccountsChart accountsChart,
                                         FixedList<Account> accounts) {
      return new AccountsChartDto {
        UID = accountsChart.UID,
        Name = accountsChart.Name,
        WithSectors = false,
        Accounts = MapToAccountDescriptors(accounts)
      };
    }


    static internal AccountDto MapAccount(Account account) {
      var dto = new AccountDto();

      FillAccountDescriptorDto(dto, account);

      dto.Description = account.Description;
      dto.StartDate = account.StartDate;
      dto.EndDate = account.EndDate;
      dto.AccountsChart = account.AccountsChart.MapToNamedEntity();
      dto.AreaRules = account.AreaRules;
      dto.CurrencyRules = account.CurrencyRules;
      dto.SectorRules = account.SectorRules;
      dto.LedgerRules = LedgerMapper.MapLedgersRules(account.LedgerRules);
      dto.History = MapAccountHistory(account.GetHistory());

      return dto;
    }


    static public AccountDescriptorDto MapToAccountDescriptor(Account account) {
      var dto = new AccountDescriptorDto();

      FillAccountDescriptorDto(dto, account);

      return dto;
    }

    static internal AccountsChartDto MapWithSectors(AccountsChart accountsChart,
                                                    FixedList<Account> accounts) {
      return new AccountsChartDto {
        UID = accountsChart.UID,
        Name = accountsChart.Name,
        WithSectors = true,
        Accounts = MapToAccountDescriptorsWithSectors(accounts)
      };
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
      dto.Sector = "00";
      dto.StartDate = account.StartDate;
      dto.LastLevel = account.Role != AccountRole.Sumaria;
      dto.Obsolete = account.EndDate < Account.MAX_END_DATE;
    }


    static private FixedList<AccountDescriptorDto> MapToAccountDescriptors(FixedList<Account> list) {
      return new FixedList<AccountDescriptorDto>(list.Select((x) => MapToAccountDescriptor(x)));
    }


    static private FixedList<AccountDescriptorDto> MapToAccountDescriptorsWithSectors(FixedList<Account> list) {
      List<AccountDescriptorDto> withSectors = new List<AccountDescriptorDto>(list.Count * 2);

      foreach (var account in list) {
        var descriptor = MapToAccountDescriptor(account);
        if (account.Role == AccountRole.Sectorizada) {
          descriptor.LastLevel = false;
        }
        withSectors.Add(descriptor);

        if (account.SectorRules.Count == 0) {
          continue;
        }
        foreach (var sectorRule in account.SectorRules) {
          descriptor = MapToAccountDescriptor(account);
          descriptor.Sector = sectorRule.Sector.Code;
          descriptor.Role = sectorRule.SectorRole;
          descriptor.LastLevel = true;
          withSectors.Add(descriptor);
        }
      }
      return withSectors.ToFixedList();
    }


    static private FixedList<AccountHistoryDto> MapAccountHistory(FixedList<Account> list) {
      return new FixedList<AccountHistoryDto>(list.Select((x) => MapToAccountHistory(x)));
    }


    static private AccountHistoryDto MapToAccountHistory(Account account) {
      return new AccountHistoryDto {
        UID = account.UID,
        Name = account.Name,
        Number = account.Number,
        Role = account.Role,
        Type = account.AccountType,
        DebtorCreditor = account.DebtorCreditor,
        Description = account.Description,
        StartDate = account.StartDate,
        EndDate = account.EndDate
      };
    }

    #endregion Private methods

  }  // class AccountsChartMapper

}  // namespace Empiria.FinancialAccounting.Adapters
