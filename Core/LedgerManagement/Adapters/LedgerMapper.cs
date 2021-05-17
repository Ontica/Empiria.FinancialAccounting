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

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for accounting ledger books.</summary>
  static internal class LedgerMapper {

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
        SubsidiaryAccountsPrefix = ledger.SubsidiaryAccountsPrefix,
        BaseCurrency = ledger.BaseCurrency.MapToNamedEntity()
      };
    }


    static internal LedgerAccountDto MapAccount(LedgerAccount ledgerAccount) {
      return new LedgerAccountDto {
        Id = ledgerAccount.Id,
        Ledger = ledgerAccount.Ledger.MapToNamedEntity(),
        Name = ledgerAccount.Name,
        Number = ledgerAccount.Number,
        Description = ledgerAccount.Description,
        Role = ledgerAccount.Role,
        AccountType = ledgerAccount.AccountType,
        DebtorCreditor = ledgerAccount.DebtorCreditor,
        Level = ledgerAccount.Level,
        CurrencyRules = ledgerAccount.CurrencyRules,
        SectorRules = ledgerAccount.SectorRules
      };
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
