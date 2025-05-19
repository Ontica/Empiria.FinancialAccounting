/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanceEntryMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balance entries.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map balance entries.</summary>
  static internal class BalanceEntryMapper {

    #region Public methods

    static internal FixedList<BalanceEntryDto> Map(FixedList<BalanceEntry> entries) {

      var entriesDto = entries.Select(x => (MapEntries(x)));

      return new FixedList<BalanceEntryDto>(entriesDto);
    }

    #endregion Public methods

    #region Private methods

    static private BalanceEntryDto MapEntries(BalanceEntry x) {
      return new BalanceEntryDto {
        AccountNumber = x.Account.Number,
        InitialBalance = x.InitialBalance,
        Debit = x.Debit,
        Credit = x.Credit,
        CurrentBalance = x.CurrentBalance
      };
    }

    #endregion Private methods

  } // class BalanceEntryMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
