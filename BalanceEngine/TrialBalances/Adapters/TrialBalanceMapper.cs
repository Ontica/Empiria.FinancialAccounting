/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : TrialBalanceMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map trial balances.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  static internal class TrialBalanceMapper {

    #region Public mappers

    static internal TrialBalanceDto Map(TrialBalance trialBalance) {
      return new TrialBalanceDto {
        Query = trialBalance.Query,
        Columns = trialBalance.DataColumns(),
        Entries = Map(trialBalance.Query, trialBalance.Entries)
      };
    }

    static internal AnaliticoDeCuentasEntry MapToAnalyticBalanceEntry(
                                                AnaliticoDeCuentasEntry balanceEntry) {
      var entry = new AnaliticoDeCuentasEntry();
      entry.Account = balanceEntry.Account;
      entry.AccountId = balanceEntry.AccountId;
      entry.SubledgerAccountId = balanceEntry.SubledgerAccountId;
      entry.Ledger = balanceEntry.Ledger;
      entry.Currency = balanceEntry.Currency;
      entry.ItemType = balanceEntry.ItemType;
      entry.Sector = balanceEntry.Sector;
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;
      entry.DomesticBalance = balanceEntry.DomesticBalance;
      entry.ForeignBalance = balanceEntry.ForeignBalance;
      entry.TotalBalance = balanceEntry.TotalBalance;
      entry.ExchangeRate = balanceEntry.ExchangeRate;
      //entry.LastChangeDate = balanceEntry.LastChangeDate;

      return entry;
    }

    static internal ValuedTrialBalanceEntry MapValuedTrialBalanceEntry(ValuedTrialBalanceEntry valuedEntry) {
      var entry = new ValuedTrialBalanceEntry();
      entry.Account = valuedEntry.Account;
      entry.Currency = valuedEntry.Currency;
      entry.Sector = Sector.Empty;
      entry.GroupName = valuedEntry.GroupName;
      entry.ItemType = valuedEntry.ItemType;

      return entry;
    }
    #endregion Public mappers

    #region Helpers

    static private FixedList<ITrialBalanceEntryDto> Map(TrialBalanceQuery query,
                                                        FixedList<ITrialBalanceEntry> list) {
      switch (query.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          var analitico = list.Select((x) => AnaliticoDeCuentasMapper.MapEntry((AnaliticoDeCuentasEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(analitico);

        case TrialBalanceType.Balanza:
          var balanza = list.Select((x) => BalanzaTradicionalMapper.MapEntry((TrialBalanceEntry) x, query));

          return new FixedList<ITrialBalanceEntryDto>(balanza);

        case TrialBalanceType.BalanzaConContabilidadesEnCascada:
        case TrialBalanceType.GeneracionDeSaldos:

          var mappedItems = list.Select((x) => MapToTrialBalance((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(mappedItems);


        case TrialBalanceType.SaldosPorAuxiliar:

          var subledgerItems = list.Select((x) => MapToBalanceBySubledgerAccount((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(subledgerItems);

        case TrialBalanceType.SaldosPorCuenta:

          var saldosPorCuenta = list.Select((x) => SaldosPorCuentaMapper.MapEntry((TrialBalanceEntry) x, query));
          return new FixedList<ITrialBalanceEntryDto>(saldosPorCuenta);

        case TrialBalanceType.BalanzaEnColumnasPorMoneda:
          var currencyMappedItems = list.Select((x) =>
                MapToTrialBalanceByCurrency((TrialBalanceByCurrencyEntry) x));
          return new FixedList<ITrialBalanceEntryDto>(currencyMappedItems);

        case TrialBalanceType.BalanzaDolarizada:
          var valuedMappedItems = list.Select((x) =>
                MapToValuedTrialBalance((ValuedTrialBalanceEntry) x));
          return new FixedList<ITrialBalanceEntryDto>(valuedMappedItems);

        case TrialBalanceType.BalanzaValorizadaComparativa:

          var mappedItemsComparative = list.Select((x) =>
                MapToTrialBalanceComparative((TrialBalanceComparativeEntry) x));
          return new FixedList<ITrialBalanceEntryDto>(mappedItemsComparative);

        default:
          throw Assertion.EnsureNoReachThisCode(
                $"Unhandled trial balance type {query.TrialBalanceType}.");
      }
    }


    static private TrialBalanceEntryDto MapToBalancesByAccount(TrialBalanceEntry entry,
                                                               TrialBalanceQuery query) {
      var dto = new TrialBalanceEntryDto();
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      if (subledgerAccount.IsEmptyInstance || subledgerAccount.Number == "0") {
        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.SubledgerAccountNumber = subledgerAccount.Number;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.AverageBalance = entry.AverageBalance;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      if (query.WithSubledgerAccount) {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Summary ?
                             entry.DebtorCreditor.ToString() : "";
      } else {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.DebtorCreditor.ToString() : "";
      }

      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      dto.LastChangeDateForBalances = entry.LastChangeDate;

      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                !query.UseDefaultValuation &&
                                query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                query.InitialPeriod.ExchangeRateTypeUID.Length == 0;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                            entry.ItemType == TrialBalanceItemType.Summary) &&
                            !query.UseDefaultValuation &&
                            query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                            query.InitialPeriod.ExchangeRateTypeUID.Length == 0;

      return dto;
    }

    static private TrialBalanceEntryDto MapToBalanceBySubledgerAccount(TrialBalanceEntry entry,
                                                                       TrialBalanceQuery query) {
      var dto = new TrialBalanceEntryDto();
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      if (subledgerAccount.IsEmptyInstance || subledgerAccount.Number == "0") {
        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
      dto.AccountNumberForBalances = entry.Account.Number != "Empty" ? entry.Account.Number : "";

      if (entry.ItemType == TrialBalanceItemType.Entry && subledgerAccount.IsEmptyInstance) {

        subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (!subledgerAccount.IsEmptyInstance) {
          dto.SubledgerAccountNumber = subledgerAccount.Number;
        }
      } else {
        dto.SubledgerAccountNumber = subledgerAccount.Number;
      }

      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      if (entry.ItemType == TrialBalanceItemType.Total ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.CurrentBalance = entry.CurrentBalance;
      }
      if (entry.ItemType == TrialBalanceItemType.Entry) {
        dto.AverageBalance = entry.AverageBalance;
      }
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.DebtorCreditor.ToString() : "";

      dto.LastChangeDate = entry.LastChangeDate;

      dto.LastChangeDateForBalances = dto.LastChangeDate;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                !query.UseDefaultValuation &&
                                query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                query.InitialPeriod.ExchangeRateTypeUID.Length == 0;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                            entry.ItemType == TrialBalanceItemType.Summary) &&
                            !query.UseDefaultValuation &&
                            query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                            query.InitialPeriod.ExchangeRateTypeUID.Length == 0;

      return dto;
    }

    static private TrialBalanceEntryDto MapToTrialBalance(TrialBalanceEntry entry,
                                                          TrialBalanceQuery query) {
      var dto = new TrialBalanceEntryDto();
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.ItemType == TrialBalanceItemType.BalanceTotalConsolidated ? "" :
                         entry.Currency.Code;
      if (subledgerAccount.IsEmptyInstance || subledgerAccount.Number == "0") {
        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
      dto.AccountNumberForBalances = entry.Account.Number;

      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar &&
          entry.ItemType == TrialBalanceItemType.Entry && subledgerAccount.IsEmptyInstance) {

        subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (!subledgerAccount.IsEmptyInstance) {
          dto.SubledgerAccountNumber = subledgerAccount.Number;
        }
      } else {
        dto.SubledgerAccountNumber = subledgerAccount.Number;
      }

      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.AverageBalance = entry.AverageBalance;
      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar) {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.DebtorCreditor.ToString() : "";

        dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.LastChangeDate : ExecutionServer.DateMaxValue;
      } else {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ?
                           entry.DebtorCreditor.ToString() : "";

        dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ||
                           query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      }
      dto.LastChangeDateForBalances = dto.LastChangeDate;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                !query.UseDefaultValuation &&
                                query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                query.InitialPeriod.ExchangeRateTypeUID.Length == 0;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                            entry.ItemType == TrialBalanceItemType.Summary) &&
                            !query.UseDefaultValuation &&
                            query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                            query.InitialPeriod.ExchangeRateTypeUID.Length == 0;

      return dto;
    }


    static private TrialBalanceComparativeDto MapToTrialBalanceComparative(
                                              TrialBalanceComparativeEntry entry) {
      var dto = new TrialBalanceComparativeDto();

      dto.ItemType = entry.ItemType;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.DebtorCreditor = entry.Account.DebtorCreditor;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      dto.SectorCode = entry.Sector.Code;
      dto.AccountParent = entry.Account.FirstLevelAccountNumber;
      dto.AccountNumber = entry.Account.Number;
      dto.AccountName = entry.Account.Name;
      dto.AccountNumberForBalances = entry.Account.Number;

      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      dto.SubledgerAccountName = entry.SubledgerAccountNumber != String.Empty ?
                                 entry.SubledgerAccountName : entry.Account.Name;

      dto.FirstTotalBalance = entry.FirstTotalBalance;
      dto.FirstExchangeRate = entry.FirstExchangeRate;
      dto.FirstValorization = entry.FirstValorization;

      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.SecondTotalBalance = entry.SecondTotalBalance;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.SecondValorization = entry.SecondValorization;

      dto.Variation = entry.Variation;
      dto.VariationByER = entry.VariationByER;
      dto.RealVariation = entry.RealVariation;
      dto.AverageBalance = entry.AverageBalance;
      dto.LastChangeDate = entry.LastChangeDate;

      return dto;
    }

    static private ValuedTrialBalanceDto MapToValuedTrialBalance(
                                              ValuedTrialBalanceEntry entry) {
      var dto = new ValuedTrialBalanceDto();
      dto.ItemType = entry.ItemType;
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountNumber = entry.Account.Number;
      dto.AccountName = entry.ItemType == TrialBalanceItemType.Summary ||
                        entry.ItemType == TrialBalanceItemType.Entry ? entry.Account.Name :
                        entry.ItemType == TrialBalanceItemType.BalanceTotalCurrency ? entry.GroupName : "";
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.SectorCode = entry.Sector.Code;
      dto.ExchangeRate = entry.ExchangeRate;
      if (entry.ItemType != TrialBalanceItemType.BalanceTotalCurrency) {
        dto.TotalBalance = entry.TotalBalance;
        dto.ValuedExchangeRate = entry.ValuedExchangeRate;
      }
      dto.TotalEquivalence = entry.TotalEquivalence;
      dto.GroupName = entry.GroupName;
      dto.GroupNumber = entry.GroupNumber;

      return dto;
    }

    static private TrialBalanceByCurrencyDto MapToTrialBalanceByCurrency(
                                              TrialBalanceByCurrencyEntry entry) {
      var dto = new TrialBalanceByCurrencyDto();
      dto.ItemType = entry.ItemType;
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountNumber = entry.Account.Number;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountName = entry.Account.Name;
      dto.SectorCode = entry.Sector.Code;
      dto.DomesticBalance = entry.DomesticBalance;
      dto.DollarBalance = entry.DollarBalance;
      dto.YenBalance = entry.YenBalance;
      dto.EuroBalance = entry.EuroBalance;
      dto.UdisBalance = entry.UdisBalance;
      dto.GroupName = entry.GroupName;
      dto.GroupNumber = entry.GroupNumber;

      return dto;
    }

    #endregion Helpers

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
