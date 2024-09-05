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
using System.Linq;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods from Account instances to FlatAccounts.</summary>
  static public class FlatAccountMapper {

    static internal FixedList<FlatAccountDto> Map(FixedList<Account> accounts,
                                                  DateTime fromDate, DateTime toDate) {
      var flattenedAccounts = new List<FlatAccountDto>(accounts.Count * 4);

      foreach (Account account in accounts) {
        flattenedAccounts.AddRange(Map(account, fromDate, toDate));
      }

      flattenedAccounts.TrimExcess();

      return flattenedAccounts.ToFixedList();
    }


    static public FixedList<FlatAccountDto> Map(Account account,
                                                DateTime fromDate, DateTime toDate) {

      FixedList<Currency> currencies = GetAccountCurrencies(account, fromDate, toDate);
      FixedList<FlatSector> flatSectors = GetAccountSectors(account, fromDate, toDate);

      if (flatSectors.Count != 0) {
        return MapSectorizedAccount(account, currencies, flatSectors);
      } else {
        return MapNoSectorsAccount(account, currencies);
      }
    }


    #region Helpers

    static private FixedList<Currency> GetAccountCurrencies(Account account,
                                                            DateTime fromDate, DateTime toDate) {
      return account.GetCascadeCurrencies(fromDate, toDate)
                    .Select(x => x.Currency).Distinct()
                    .ToFixedList()
                    .Sort((x, y) => x.Code.CompareTo(y.Code));
    }


    static private FixedList<FlatSector> GetAccountSectors(Account account,
                                                           DateTime fromDate, DateTime toDate) {
      return account.GetCascadeSectors(fromDate, toDate)
                    .Select(x => new FlatSector { Sector = x.Sector, SectorRole = x.SectorRole })
                    .Distinct()
                    .ToFixedList()
                    .Sort((x, y) => x.Sector.Code.CompareTo(y.Sector.Code));
    }


    static private FixedList<FlatAccountDto> MapNoSectorsAccount(Account account,
                                                                 FixedList<Currency> currencies) {
      var flattenedAccounts = new List<FlatAccountDto>(4);

      foreach (Currency currency in currencies) {
        FlatAccountDto flatAccount = MapFlatAccount(account, currency);

        flattenedAccounts.Add(flatAccount);
      }

      return flattenedAccounts.ToFixedList();
    }


    static private FixedList<FlatAccountDto> MapSectorizedAccount(Account account,
                                                                  FixedList<Currency> currencies,
                                                                  FixedList<FlatSector> flatSectors) {
      var flattenedAccounts = new List<FlatAccountDto>(4);

      foreach (Currency currency in currencies) {

        foreach (FlatSector flatSector in flatSectors) {

          FlatAccountDto flatAccount = MapFlatAccount(account, currency);

          flatAccount.Sector = flatSector.Sector;
          if (account.Role != AccountRole.Sumaria) {
            flatAccount.Role = flatSector.SectorRole;
          }

          flattenedAccounts.Add(flatAccount);
        }
      }

      return flattenedAccounts.ToFixedList();
    }


    static private FlatAccountDto MapFlatAccount(Account account, Currency currency) {
      return new FlatAccountDto {
        UID = account.UID,
        StandardAccountId = account.StandardAccountId,
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


    /// <summary>Holds a sector with its role.</summary>
    private class FlatSector {

      internal Sector Sector {
        get; set;
      }

      internal AccountRole SectorRole {
        get; set;
      }

      public override bool Equals(object obj) => this.Equals(obj as FlatSector);

      public bool Equals(FlatSector obj) {
        if (obj == null) {
          return false;
        }
        if (Object.ReferenceEquals(this, obj)) {
          return true;
        }
        if (this.GetType() != obj.GetType()) {
          return false;
        }

        return this.Sector.Equals(obj.Sector) && this.SectorRole == obj.SectorRole;
      }

      public override int GetHashCode() {
        return (this.Sector.Id, this.SectorRole).GetHashCode();
      }

    }   // private class FlatSector

  }  // class FlatAccountMapper

}  // namespace Empiria.FinancialAccounting.Adapters
