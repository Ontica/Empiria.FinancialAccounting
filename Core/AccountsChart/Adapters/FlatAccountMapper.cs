/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : FlatAccountMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods from Account instances to FlatAccounts.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods from Account instances to FlatAccounts.</summary>
  static public class FlatAccountMapper {

    static internal FixedList<FlatAccountDto> Map(FixedList<Account> accounts,
                                                  DateTime fromDate,
                                                  DateTime toDate) {
      var flattenedAccounts = new List<FlatAccountDto>(accounts.Count * 4);

      flattenedAccounts.TrimExcess();

      return flattenedAccounts.ToFixedList();
    }


    static public FixedList<FlatAccountDto> Map(Account account, DateTime fromDate, DateTime toDate) {
      FixedList<SectorRule> accountSectors = account.GetCascadeSectors(fromDate, toDate);

      if (accountSectors.Count != 0) {
        return MapSectorizedAccount(account, accountSectors, fromDate, toDate);
      } else {
        return MapNoSectorsAccount(account, fromDate, toDate);
      }
    }

    #region Helpers

    static private FixedList<FlatAccountDto> MapNoSectorsAccount(Account account,
                                                                 DateTime fromDate, DateTime toDate) {
      var flattenedAccounts = new List<FlatAccountDto>(4);

      foreach (CurrencyRule currencyRule in account.GetCascadeCurrencies(fromDate, toDate)) {
        FlatAccountDto flatAccount = MapFlatAccount(account, currencyRule.Currency);

        flattenedAccounts.Add(flatAccount);
      }

      return flattenedAccounts.ToFixedList();
    }


    static private FixedList<FlatAccountDto> MapSectorizedAccount(Account account,
                                                                  FixedList<SectorRule> sectorRules,
                                                                  DateTime fromDate, DateTime toDate) {
      var flattenedAccounts = new List<FlatAccountDto>(4);

      foreach (SectorRule sectorRule in sectorRules) {
        foreach (CurrencyRule currencyRule in account.GetCascadeCurrencies(fromDate, toDate)) {
          FlatAccountDto flatAccount = MapFlatAccount(account, currencyRule.Currency);

          flatAccount.Sector = sectorRule.Sector;
          flatAccount.Role = sectorRule.SectorRole;

          flattenedAccounts.Add(flatAccount);
        }
      }

      return flattenedAccounts.ToFixedList();
    }


    static private FlatAccountDto MapFlatAccount(Account account, Currency currency) {
      return new FlatAccountDto {
        UID = account.UID,
        Number = account.Number,
        Name = account.Name,
        Description = account.Description,
        Type = account.AccountType.MapToNamedEntity(),
        DebtorCreditor = account.DebtorCreditor,
        Role = account.Role,
        Level = account.Level,
        LastLevel = account.Role != AccountRole.Sumaria,
        Currency = currency,
        Sector = Sector.Empty,
        SummaryWithNotChildren = account.IsSummaryWithNotChildren
      };
    }

    #endregion Helpers

  }  // class FlatAccountMapper

}  // namespace Empiria.FinancialAccounting.Adapters
