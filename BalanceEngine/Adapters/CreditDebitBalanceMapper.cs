/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : CreditDebitBalanceMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map debit and credit balances.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map debit and credit balances.</summary>
  static internal class CreditDebitBalanceMapper {

    #region Public mappers

    static internal DebitCreditBalanceDto Map(TrialBalanceCommand command, DebitCreditBalance debitCreditBalance) {
      return new DebitCreditBalanceDto {
        Command = command,
        Entries = Map(debitCreditBalance.Entries)
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<DebitCreditBalanceEntryDto> Map(FixedList<DebitCreditBalanceEntry> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<DebitCreditBalanceEntryDto>(mappedItems);
    }

    static private DebitCreditBalanceEntryDto Map(DebitCreditBalanceEntry debitCreditBalance) {
      var dto = new DebitCreditBalanceEntryDto();

      dto.LedgerId = debitCreditBalance.LedgerId;
      dto.AccountId = debitCreditBalance.AccountId;
      dto.SectorId = debitCreditBalance.SectorId;
      dto.SubsidiaryAccountId = debitCreditBalance.SubsidiaryAccountId;
      dto.Balance = debitCreditBalance.Balance;

      return dto;
    }

    #endregion Helpers

  } // class CreditDebitBalanceMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
