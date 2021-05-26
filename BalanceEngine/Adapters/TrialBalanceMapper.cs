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

      dto.Ledger = trialBalanceEntry.Ledger.MapToNamedEntity();
      dto.Currency = trialBalanceEntry.Currency.MapToNamedEntity();
      dto.AccountUID = trialBalanceEntry.Account.UID;
      dto.AccountNumber = trialBalanceEntry.Account.Number;
      dto.AccountName = trialBalanceEntry.Account.Name;
      dto.Sector = trialBalanceEntry.Sector.MapToNamedEntity();
      dto.LedgerAccountId = trialBalanceEntry.LedgerAccountId;
      dto.InitialBalance = trialBalanceEntry.InitialBalance;
      dto.Debit = trialBalanceEntry.Debit;
      dto.Credit = trialBalanceEntry.Credit;
      dto.CurrentBalance = trialBalanceEntry.CurrentBalance;

      return dto;
    }

    #endregion Helpers

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
