/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : LedgerMapper                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for accounting ledger books.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for accounting ledger books.</summary>
  static public class LedgerMapper {

    static internal FixedList<LedgerDto> Map(FixedList<Ledger> list) {
      return new FixedList<LedgerDto>(list.Select((x) => Map(x)));
    }


    static internal LedgerDto Map(Ledger ledger) {
      return new LedgerDto {
        UID = ledger.UID,
        Name = ledger.Name,
        FullName = ledger.FullName,
        Number = ledger.Number,
        Subnumber = ledger.Subnumber,
        AccountsChart = ledger.AccountsChart.MapToNamedEntity(),
        SubledgerAccountsPrefix = ledger.SubledgerAccountsPrefix,
        SubledgerAccountsTypes = ledger.SubledgerTypes().MapToNamedEntityList(),
        BaseCurrency = ledger.BaseCurrency.MapToNamedEntity()
      };
    }


    static public FixedList<LedgerAccountDto> MapAccountsForVoucherEdition(FixedList<LedgerAccount> assignedAccounts,
                                                                           FixedList<Account> unassignedAccounts,
                                                                           DateTime date) {
      var list = new List<LedgerAccountDto>(assignedAccounts.Select((x) => MapAccount(x, date)));
      var unAssignedMapped = new List<LedgerAccountDto>(unassignedAccounts.Select((x) => MapUnassignedAccount(x)));

      list.AddRange(unAssignedMapped);

      list.Sort((x, y) => x.Number.CompareTo(y.Number));

      return new FixedList<LedgerAccountDto>(list);
    }


    static private LedgerAccountDto MapUnassignedAccount(Account account) {
      return new LedgerAccountDto {
        Id = 0,
        StandardAccountId = account.StandardAccountId,
        Name = account.Name,
        Number = account.Number,
        Description = account.Description,
        Role = account.Role,
        AccountType = account.AccountType,
        DebtorCreditor = account.DebtorCreditor,
        Level = account.Level,
        Currencies = MapToNamedEntityList(account.CurrencyRules),
        Sectors = MapSectorRulesShort(account.SectorRules)
      };
    }


    static public LedgerAccountDto MapAccount(LedgerAccount ledgerAccount, DateTime date) {
      return new LedgerAccountDto {
        Id = ledgerAccount.Id,
        StandardAccountId = ledgerAccount.StandardAccountId,
        Ledger = ledgerAccount.Ledger.MapToNamedEntity(),
        Name = ledgerAccount.Name,
        Number = ledgerAccount.Number,
        Description = ledgerAccount.Description,
        Role = ledgerAccount.Role,
        AccountType = ledgerAccount.AccountType,
        DebtorCreditor = ledgerAccount.DebtorCreditor,
        Level = ledgerAccount.Level,
        Currencies = MapToNamedEntityList(ledgerAccount.CurrencyRulesOn(date)),
        Sectors = MapSectorRulesShort(ledgerAccount.SectorRulesOn(date))
      };
    }


    static internal FixedList<SectorRuleDto> MapSectorRules(FixedList<SectorRule> list) {
      return new FixedList<SectorRuleDto>(list.Select(x => MapSectorRule(x)));
    }


    static public SectorRuleDto MapSectorRule(SectorRule x) {
      return new SectorRuleDto {
        UID = x.Sector.UID,
        Sector = CataloguesMapper.MapSector(x.Sector),
        SectorRole = x.SectorRole,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
      };
    }


    static internal FixedList<SectorRuleShortDto> MapSectorRulesShort(FixedList<SectorRule> list) {
      return new FixedList<SectorRuleShortDto>(list.Select(x => MapSectorRulesShort(x)));
    }


    static public SectorRuleShortDto MapSectorRulesShort(SectorRule x) {
      return new SectorRuleShortDto {
        Id = x.Sector.Id,
        Code = x.Sector.Code,
        Name = x.Sector.Name,
        Role = x.SectorRole
      };
    }


    static private FixedList<NamedEntityDto> MapToNamedEntityList(FixedList<CurrencyRule> list) {
      return new FixedList<NamedEntityDto>(list.Select(x => x.Currency.MapToNamedEntity()));
    }


    static internal FixedList<LedgerRuleDto> MapLedgersRules(FixedList<LedgerRule> list) {
      return new FixedList<LedgerRuleDto>(list.Select(x => MapLedgerRule(x)));
    }


    static internal FixedList<NamedEntityDto> MapToNamedEntityList(FixedList<Ledger> list) {
      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }


    static private LedgerRuleDto MapLedgerRule(LedgerRule ledgerRule) {
      return new LedgerRuleDto {
        UID = ledgerRule.UID,
        Ledger = ledgerRule.Ledger.MapToNamedEntity(),
        StartDate = ledgerRule.StartDate,
        EndDate = ledgerRule.EndDate
      };
    }

  }  // class LedgerMapper

}  // namespace Empiria.FinancialAccounting.Adapters
