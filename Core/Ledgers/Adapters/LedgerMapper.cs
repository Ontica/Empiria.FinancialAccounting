﻿/* Empiria Financial *****************************************************************************************
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
        Number = ledger.Number,
        Name = ledger.Name,
        FullName = ledger.FullName,
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
      var unAssignedMapped = new List<LedgerAccountDto>(unassignedAccounts.Select((x) => MapUnassignedAccount(x, date)));

      list.AddRange(unAssignedMapped);

      list.Sort((x, y) => x.Number.CompareTo(y.Number));

      return new FixedList<LedgerAccountDto>(list);
    }


    static private LedgerAccountDto MapUnassignedAccount(Account account, DateTime date) {
      var historicData = account.GetHistory(date);

      return new LedgerAccountDto {
        Id = 0,
        StandardAccountId = account.StandardAccount.Id,
        Name = account.Name,
        Number = account.Number,
        Description = historicData.Description,
        Role = historicData.Role,
        AccountType = account.AccountType.MapToNamedEntity(),
        DebtorCreditor = account.DebtorCreditor,
        Level = account.Level,
        Currencies = MapToValuedCurrencies(account.AllCurrencyRules),
        Sectors = MapSectorRulesShort(account.AllSectorRules)
      };
    }

    static public LedgerAccountDto MapAccount(LedgerAccount ledgerAccount, DateTime date) {
      var historicData = ledgerAccount.GetHistoric(date);

      return new LedgerAccountDto {
        Id = ledgerAccount.Id,
        StandardAccountId = ledgerAccount.StandardAccount.Id,
        Ledger = ledgerAccount.Ledger.MapToNamedEntity(),
        Name = ledgerAccount.Name,
        ParentFullName = ledgerAccount.ParentFullName,
        Number = ledgerAccount.Number,
        Description = historicData.Description,
        Role = historicData.Role,
        AccountType = ledgerAccount.AccountType.MapToNamedEntity(),
        DebtorCreditor = ledgerAccount.DebtorCreditor,
        Level = ledgerAccount.Level,
        Currencies = MapToValuedCurrencies(ledgerAccount.CurrencyRulesOn(date), ledgerAccount.Ledger.BaseCurrency, date),
        Sectors = MapSectorRulesShort(ledgerAccount.SectorRulesOn(date))
      };
    }


    static internal FixedList<SectorRuleDto> MapSectorRules(FixedList<SectorRule> list) {
      return new FixedList<SectorRuleDto>(list.Select(x => MapSectorRule(x)));
    }


    static public SectorRuleDto MapSectorRule(SectorRule x) {
      return new SectorRuleDto {
        UID = x.Sector.UID,
        Sector = SectorsMapper.MapSector(x.Sector),
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


    static internal FixedList<LedgerRuleDto> MapLedgersRules(FixedList<LedgerRule> list) {
      return new FixedList<LedgerRuleDto>(list.Select(x => MapLedgerRule(x)));
    }


    static internal FixedList<NamedEntityDto> MapToNamedEntityList(FixedList<Ledger> list) {
      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }

    static private FixedList<ValuedCurrencyDto> MapToValuedCurrencies(FixedList<CurrencyRule> list) {
      return new FixedList<ValuedCurrencyDto>(list.Select(x => MapToValuedCurrency(x)));
    }

    static private FixedList<ValuedCurrencyDto> MapToValuedCurrencies(FixedList<CurrencyRule> list,
                                                                      Currency baseCurrency, DateTime date) {

      var valuedCurrencies = new FixedList<ValuedCurrencyDto>(list.Select(x => MapToValuedCurrency(x)));

      if (valuedCurrencies.Count == 0) {
        return valuedCurrencies;
      }

      if (valuedCurrencies.Count == 1 && valuedCurrencies[0].UID == baseCurrency.UID) {
        return valuedCurrencies;
      }

      return ValuateCurrencies(valuedCurrencies, baseCurrency, date);
    }

    static private FixedList<ValuedCurrencyDto> ValuateCurrencies(FixedList<ValuedCurrencyDto> valuedCurrencies,
                                                                  Currency baseCurrency,
                                                                  DateTime date) {
      FixedList<ExchangeRate> exchangeRates = ExchangeRate.GetList(ExchangeRateType.Diario, date);

      foreach (var valuedCurrency in valuedCurrencies) {
        if (valuedCurrency.UID == baseCurrency.UID) {
          continue;
        }

        var exchangeRate = exchangeRates.Find(x => x.ToCurrency.UID == valuedCurrency.UID);

        valuedCurrency.ExchangeRate = (exchangeRate != null ? exchangeRate.Value : 0);
      }

      return valuedCurrencies;
    }


    static private ValuedCurrencyDto MapToValuedCurrency(CurrencyRule currencyRule) {
      return new ValuedCurrencyDto {
         UID = currencyRule.Currency.UID,
         Name = currencyRule.Currency.FullName,
         ExchangeRate = 1m
      };
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
