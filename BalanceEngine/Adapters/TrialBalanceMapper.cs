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

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  static internal class TrialBalanceMapper {

    #region Public mappers

    static internal TrialBalanceDto Map(TrialBalance trialBalance) {
      return new TrialBalanceDto {
        Command = trialBalance.Command,
        Entries = Map(trialBalance.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<TrialBalanceEntryDto> Map(FixedList<TrialBalanceEntry> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<TrialBalanceEntryDto>(mappedItems);
    }

    static private TrialBalanceEntryDto Map(TrialBalanceEntry trialBalanceEntry) {
      var dto = new TrialBalanceEntryDto();

      dto.ItemType = trialBalanceEntry.ItemType ?? "BalanceEntry";
      dto.LedgerUID = trialBalanceEntry.Ledger.UID;
      dto.LedgerAccountId = trialBalanceEntry.LedgerAccountId;
      dto.CurrencyUID = trialBalanceEntry.Currency.UID;
      dto.AccountName = trialBalanceEntry.Account.Name;
      dto.AccountNumber = trialBalanceEntry.Account.Number;
      dto.AccountRole = trialBalanceEntry.Account.Role;
      dto.AccountLevel = trialBalanceEntry.Account.Level;
      dto.SectorCode = trialBalanceEntry.Sector.Code;
      dto.InitialBalance = trialBalanceEntry.InitialBalance;
      dto.Debit = trialBalanceEntry.Debit;
      dto.Credit = trialBalanceEntry.Credit;
      dto.CurrentBalance = trialBalanceEntry.CurrentBalance;

      return dto;
    }

    #endregion Helpers

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
