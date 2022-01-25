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
        Command = trialBalance.Command,
        Columns = trialBalance.DataColumns(),
        Entries = Map(trialBalance.Command, trialBalance.Entries)
      };
    }

    static internal TrialBalanceEntry MapToTrialBalanceEntry(TrialBalanceEntry entry) {
      var newEntry = new TrialBalanceEntry();

      newEntry.Account = entry.Account;
      newEntry.Ledger = entry.Ledger;
      newEntry.Currency = entry.Currency;
      newEntry.Sector = entry.Sector;
      newEntry.SubledgerAccountId = entry.SubledgerAccountId;
      newEntry.InitialBalance = entry.InitialBalance;
      newEntry.Debit = entry.Debit;
      newEntry.Credit = entry.Credit;
      newEntry.CurrentBalance = entry.CurrentBalance;
      newEntry.GroupNumber = entry.GroupNumber;
      newEntry.GroupName = entry.GroupName;
      newEntry.ItemType = entry.ItemType;
      newEntry.ExchangeRate = entry.ExchangeRate;
      //newEntry.LastChangeDate = entry.LastChangeDate;

      return newEntry;
    }

    static internal TwoCurrenciesBalanceEntry MapTwoCurrenciesBalance(
                                                TwoCurrenciesBalanceEntry balanceEntry) {
      var entry = new TwoCurrenciesBalanceEntry();
      entry.Account = balanceEntry.Account;
      entry.AccountId = balanceEntry.AccountId;
      entry.SubledgerAccountId = balanceEntry.SubledgerAccountId;
      entry.Ledger = balanceEntry.Ledger;
      entry.Currency = balanceEntry.Currency;
      entry.ItemType = balanceEntry.ItemType;
      entry.Sector = balanceEntry.Sector;
      entry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;
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

    static private FixedList<ITrialBalanceEntryDto> Map(TrialBalanceCommand command,
                                                        FixedList<ITrialBalanceEntry> list) {
      switch (command.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          return MapToAnaliticoCuentas(list);

        case TrialBalanceType.Balanza:
        case TrialBalanceType.BalanzaConContabilidadesEnCascada:
        case TrialBalanceType.GeneracionDeSaldos:
        case TrialBalanceType.Saldos:
        case TrialBalanceType.SaldosPorAuxiliar:

          var mappedItems = list.Select((x) => MapToTrialBalance((TrialBalanceEntry) x, command));
          return new FixedList<ITrialBalanceEntryDto>(mappedItems);

        case TrialBalanceType.SaldosPorCuenta:

          var balanceItems = list.Select((x) => MapToBalancesByAccount((TrialBalanceEntry) x, command));
          return new FixedList<ITrialBalanceEntryDto>(balanceItems);

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
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled trial balance type {command.TrialBalanceType}.");
      }
    }

    private static FixedList<ITrialBalanceEntryDto> MapToAnaliticoCuentas(FixedList<ITrialBalanceEntry> list) {
      var mappedItems = list.Select((x) => MapToAnaliticoCuentas((TwoCurrenciesBalanceEntry) x));

      return new FixedList<ITrialBalanceEntryDto>(mappedItems);
    }


    static private TwoColumnsTrialBalanceEntryDto MapToAnaliticoCuentas(TwoCurrenciesBalanceEntry entry) {
      var dto = new TwoColumnsTrialBalanceEntryDto();

      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);
       
      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      if (entry.GroupName.Length == 0) {
        dto.StandardAccountNumber = entry.Account.Number;
      } else {
        dto.StandardAccountNumber = entry.GroupNumber;
      }
      if (!subledgerAccount.IsEmptyInstance) {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
        dto.SubledgerAccountNumber = subledgerAccount.Number;

      } else if (entry.HasSector) {
        dto.AccountName = entry.Sector.Name;
        dto.AccountNumber = entry.Account.Number;

      } else if (entry.GroupName.Length != 0) {
        dto.AccountName = entry.GroupName;
        dto.AccountNumber = entry.GroupNumber;
      } else {
        dto.AccountName = entry.Account.Name;
        dto.AccountNumber = entry.Account.Number != "Empty" ? entry.Account.Number : "";
      }
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.DomesticBalance = entry.DomesticBalance;
      dto.ForeignBalance = entry.ForeignBalance;
      dto.TotalBalance = entry.TotalBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.AverageBalance = entry.AverageBalance;
      dto.LastChangeDate = entry.LastChangeDate != null ? entry.LastChangeDate : 
                           ExecutionServer.DateMaxValue;

      return dto;
    }


    static private TrialBalanceEntryDto MapToBalancesByAccount(TrialBalanceEntry entry,
                                                          TrialBalanceCommand command) {
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
      if (command.WithSubledgerAccount) {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Summary ?
                             entry.DebtorCreditor.ToString() : "";
      } else {
        dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.DebtorCreditor.ToString() : "";
      }
      
      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      dto.LastChangeDateForBalances = dto.LastChangeDate;

      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                command.UseDefaultValuation == false &&
                                command.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                command.InitialPeriod.ExchangeRateTypeUID.Length == 0 ? true : false;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                command.UseDefaultValuation == false &&
                                command.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                command.InitialPeriod.ExchangeRateTypeUID.Length == 0 ? true : false;

      return dto;
    }


    static private TrialBalanceEntryDto MapToTrialBalance(TrialBalanceEntry entry,
                                                          TrialBalanceCommand command) {
      var dto = new TrialBalanceEntryDto();
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      
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

      if (command.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar &&
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
      if (command.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar) {
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
                           command.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      }
      dto.LastChangeDateForBalances = dto.LastChangeDate;
      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                command.UseDefaultValuation == false &&
                                command.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                command.InitialPeriod.ExchangeRateTypeUID.Length == 0 ? true : false;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                command.UseDefaultValuation == false &&
                                command.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                command.InitialPeriod.ExchangeRateTypeUID.Length == 0 ? true : false;

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
