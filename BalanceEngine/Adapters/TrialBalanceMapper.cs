/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : TrialBalanceMapper                       License   : Please read LICENSE.txt file            *
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
      newEntry.LedgerAccountId = entry.LedgerAccountId;
      newEntry.SubledgerAccountId = entry.SubledgerAccountId;
      newEntry.InitialBalance = entry.InitialBalance;
      newEntry.Debit = entry.Debit;
      newEntry.Credit = entry.Credit;
      newEntry.CurrentBalance = entry.CurrentBalance;
      newEntry.GroupNumber = entry.GroupNumber;
      newEntry.GroupName = entry.GroupName;
      newEntry.ItemType = entry.ItemType;

      return newEntry;
    }

    static internal TwoCurrenciesBalanceEntry MapTwoCurrenciesBalance(TwoCurrenciesBalanceEntry balanceEntry) {
      var entry = new TwoCurrenciesBalanceEntry();
      entry.Account = balanceEntry.Account;
      entry.LedgerAccountId = balanceEntry.LedgerAccountId;
      entry.SubledgerAccountId = balanceEntry.SubledgerAccountId;
      entry.Ledger = balanceEntry.Ledger;
      entry.Currency = balanceEntry.Currency;
      entry.ItemType = balanceEntry.ItemType;
      entry.Sector = balanceEntry.Sector;
      entry.DebtorCreditor = balanceEntry.Account.DebtorCreditor;
      entry.DomesticBalance = balanceEntry.DomesticBalance;
      entry.ForeignBalance = balanceEntry.ForeignBalance;
      entry.TotalBalance = balanceEntry.TotalBalance;

      return entry;
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<ITrialBalanceEntryDto> Map(TrialBalanceCommand command,
                                                        FixedList<ITrialBalanceEntry> list) {
      switch (command.TrialBalanceType) {
        case TrialBalanceType.AnaliticoDeCuentas:
          var mi = list.Select((x) => MapToTwoCurrenciesBalanceEntry((TwoCurrenciesBalanceEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(mi);

        case TrialBalanceType.Balanza:
        case TrialBalanceType.BalanzaConAuxiliares:
        case TrialBalanceType.GeneracionDeSaldos:
        case TrialBalanceType.Saldos:
        case TrialBalanceType.SaldosPorAuxiliar:
        case TrialBalanceType.SaldosPorCuenta:
        case TrialBalanceType.SaldosPorCuentaConDelegaciones:
          var mappedItems = list.Select((x) => MapToTrialBalance((TrialBalanceEntry) x));

          return new FixedList<ITrialBalanceEntryDto>(mappedItems);

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled trial balance type {command.TrialBalanceType}.");
      }
    }


    static private TwoColumnsTrialBalanceEntryDto MapToTwoCurrenciesBalanceEntry(TwoCurrenciesBalanceEntry trialBalanceEntry) {
      var dto = new TwoColumnsTrialBalanceEntryDto();

      SubsidiaryAccount subledgerAccount = SubsidiaryAccount.Parse(trialBalanceEntry.SubledgerAccountId);


      dto.ItemType = trialBalanceEntry.ItemType;
      dto.LedgerUID = trialBalanceEntry.Ledger.UID;
      dto.LedgerNumber = trialBalanceEntry.Ledger.Number;
      dto.LedgerAccountId = trialBalanceEntry.LedgerAccountId;
      dto.CurrencyCode = trialBalanceEntry.Currency.Code;

      if (subledgerAccount.IsEmptyInstance) {
        dto.AccountName = trialBalanceEntry.GroupName != "" ?
                          trialBalanceEntry.GroupName :
                          trialBalanceEntry.Account.Name;
        dto.AccountNumber = trialBalanceEntry.GroupNumber != "" ?
                            trialBalanceEntry.GroupNumber :
                            trialBalanceEntry.Account.Number != "Empty" ?
                            trialBalanceEntry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
      dto.AccountRole = trialBalanceEntry.Account.Role;
      dto.AccountLevel = trialBalanceEntry.Account.Level;
      dto.SectorCode = trialBalanceEntry.Sector.Code;
      dto.SubledgerAccountId = trialBalanceEntry.SubledgerAccountId;

      dto.DomesticBalance = trialBalanceEntry.DomesticBalance;
      dto.ForeignBalance = trialBalanceEntry.ForeignBalance;
      dto.TotalBalance = trialBalanceEntry.TotalBalance;

      return dto;
    }

    static private TrialBalanceEntryDto MapToTrialBalance(TrialBalanceEntry trialBalanceEntry) {
      var dto = new TrialBalanceEntryDto();

      SubsidiaryAccount subledgerAccount = SubsidiaryAccount.Parse(trialBalanceEntry.SubledgerAccountId);

      dto.ItemType = trialBalanceEntry.ItemType;
      dto.LedgerUID = trialBalanceEntry.Ledger.UID;
      dto.LedgerNumber = trialBalanceEntry.Ledger.Number;
      dto.LedgerAccountId = trialBalanceEntry.LedgerAccountId;
      dto.CurrencyCode = trialBalanceEntry.ItemType == TrialBalanceItemType.BalanceTotalConsolidated ? "" :
                         trialBalanceEntry.Currency.Code;

      if (subledgerAccount.IsEmptyInstance) {
        dto.AccountName = trialBalanceEntry.GroupName != "" ?
                          trialBalanceEntry.GroupName :
                          trialBalanceEntry.Account.Name;
        dto.AccountNumber = trialBalanceEntry.GroupNumber != "" ?
                            trialBalanceEntry.GroupNumber :
                            trialBalanceEntry.Account.Number != "Empty" ?
                            trialBalanceEntry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
      dto.AccountRole = trialBalanceEntry.Account.Role;
      dto.AccountLevel = trialBalanceEntry.Account.Level;
      dto.SectorCode = trialBalanceEntry.Sector.Code;
      dto.SubledgerAccountId = trialBalanceEntry.SubledgerAccountId;
      dto.InitialBalance = trialBalanceEntry.InitialBalance;
      dto.Debit = trialBalanceEntry.Debit;
      dto.Credit = trialBalanceEntry.Credit;
      dto.CurrentBalance = trialBalanceEntry.CurrentBalance;

      return dto;
    }

    #endregion Helpers

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
