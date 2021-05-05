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

    static internal TrialBalanceDto Map(TrialBalanceCommand command, TrialBalance trialBalance) {
      return new TrialBalanceDto {
        Command = command,
        Entries = Map(trialBalance.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<TrialBalanceEntryDto> Map(FixedList<TrialBalanceEntry> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<TrialBalanceEntryDto>(mappedItems);
    }

    static private TrialBalanceEntryDto Map(TrialBalanceEntry trialBalance) {
      var dto = new TrialBalanceEntryDto();


      dto.LedgerId = trialBalance.LedgerId;
      dto.CurrencyId = trialBalance.CurrencyId;
      dto.SectorId = trialBalance.SectorId;
      dto.LedgerAccountId = trialBalance.LedgerAccountId;
      dto.AccountId = trialBalance.AccountId;
      
      return dto;
    }

    #endregion Helpers

  } // class TrialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
