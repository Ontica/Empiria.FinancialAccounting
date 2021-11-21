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
using System.Linq;

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

      dto.AccountsChart = account.AccountsChart.MapToNamedEntity();
      dto.AreaRules = account.AreaRules;
      dto.CurrencyRules = account.CurrencyRules;
      dto.SectorRules = LedgerMapper.MapSectorRules(account.SectorRules);
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
      dto.Role = MapToDescriptorRole(account.Role);
      dto.UsesSector = account.Role == AccountRole.Sectorizada;
      dto.UsesSubledger = account.Role == AccountRole.Control ||
                          (account.Role == AccountRole.Sectorizada &&
                           account.GetSectors().All(x => x.SectorRole == AccountRole.Control));
      dto.DebtorCreditor = account.DebtorCreditor;
      dto.Level = account.Level;
      dto.Sector = "00";
      dto.LastLevel = account.Role != AccountRole.Sumaria;
      dto.StartDate = account.StartDate;
      dto.EndDate = account.EndDate != Account.MAX_END_DATE ? account.EndDate : ExecutionServer.DateMaxValue;
      dto.Obsolete = account.EndDate < Account.MAX_END_DATE || account.IsSummaryWithNotChildren;
      dto.Parent = account.HasParent ? account.GetParent().Number : string.Empty;
      dto.SummaryWithNotChildren = account.IsSummaryWithNotChildren;
    }


    static private FixedList<AccountDescriptorDto> MapToAccountDescriptors(FixedList<Account> list) {
      return new FixedList<AccountDescriptorDto>(list.Select((x) => MapToAccountDescriptor(x)));
    }


    static private FixedList<AccountDescriptorDto> MapToAccountDescriptorsWithSectors(FixedList<Account> list) {
      var withSectors = new List<AccountDescriptorDto>(list.Count * 2);

      foreach (var account in list) {
        var descriptor = MapToAccountDescriptor(account);

        if (account.Role != AccountRole.Sectorizada) {
          withSectors.Add(descriptor);
          continue;
        }

        var sectors = account.GetSectors();
        if (sectors.Count == 0) {
          continue;
        }

        foreach (var sectorRule in sectors) {
          descriptor = MapToAccountDescriptor(account);
          descriptor.UsesSector = true;
          descriptor.UsesSubledger = sectorRule.SectorRole == AccountRole.Control;
          descriptor.Sector = sectorRule.Sector.Code;
          descriptor.Role = AccountRole.Detalle;
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


    static private AccountRole MapToDescriptorRole(AccountRole role) {
      if (role == AccountRole.Control || role == AccountRole.Sectorizada) {
        return AccountRole.Detalle;
      }
      return role;
    }

    #endregion Private methods

  }  // class AccountsChartMapper

}  // namespace Empiria.FinancialAccounting.Adapters
