/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : CurrentBalanceMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map current balances.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map current balances.</summary>
  static internal class CurrentBalanceMapper {

    #region Public mappers

    static internal CurrentBalanceDto Map(TrialBalanceCommand command, CurrentBalance currentBalance) {
      return new CurrentBalanceDto {
        Command = command,
        Entries = Map(currentBalance.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<CurrentBalanceEntryDto> Map(FixedList<CurrentBalanceEntry> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<CurrentBalanceEntryDto>(mappedItems);
    }

    static private CurrentBalanceEntryDto Map(CurrentBalanceEntry currentBalance) {
      var dto = new CurrentBalanceEntryDto();

      dto.LedgerId = currentBalance.LedgerId;
      dto.CurrencyId = currentBalance.CurrencyId;
      dto.LedgerAccountId = currentBalance.LedgerAccountId;
      dto.AccountId = currentBalance.AccountId;
      dto.SectorId = currentBalance.SectorId;
      dto.SubsidiaryAccountId = currentBalance.SubsidiaryAccountId;
      dto.Debit = currentBalance.Debit;
      dto.Credit = currentBalance.Credit;
      dto.Balance = currentBalance.Total;

      return dto;
    }

    #endregion Helpers

  } // class CurrentBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
