/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : InitialBalanceMapper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map initial balances.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map initial balances.</summary>
  static internal class InitialBalanceMapper {

    #region Public mappers

    static internal InitialBalanceDto Map(TrialBalanceCommand command, InitialBalance initialBalance) {
      return new InitialBalanceDto {
        Command = command,
        Entries = Map(initialBalance.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<InitialBalanceEntryDto> Map(FixedList<InitialBalanceEntry> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<InitialBalanceEntryDto>(mappedItems);
    }

    static private InitialBalanceEntryDto Map(InitialBalanceEntry initialBalance) {
      var dto = new InitialBalanceEntryDto();

      dto.LedgerId = initialBalance.LedgerId;
      dto.LedgerAccountId = initialBalance.LedgerAccountId;
      dto.AccountId = initialBalance.AccountId;
      dto.SectorId= initialBalance.SectorId;
      dto.SubsidiaryAccountId = initialBalance.SubsidiaryAccountId;
      dto.CurrencyId = initialBalance.CurrencyId;
      dto.InitialBalance = initialBalance.Total;

      return dto;
    }

    #endregion Helpers

  } // class InitialBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
