/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : AnaliticoDeCuentasMapper                         License   : Please read LICENSE.txt file      *
*                                                                                                            *
*  Summary  : Methods used to map analitico de cuentas.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map analitico de cuentas.</summary>
  static internal class AnaliticoDeCuentasMapper {

    #region Public methods

    static internal FixedList<ITrialBalanceEntryDto> MapToAnaliticoDeCuentas(
                                                     FixedList<ITrialBalanceEntry> list) {

      var mappedItems = list.Select((x) => MapToAnaliticoDeCuentas((AnaliticoDeCuentasEntry) x));

      return new FixedList<ITrialBalanceEntryDto>(mappedItems);
    }

    #endregion Public methods

    #region Private methods

    static private AnaliticoDeCuentasEntryDto MapToAnaliticoDeCuentas(AnaliticoDeCuentasEntry entry) {
      var dto = new AnaliticoDeCuentasEntryDto();

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.DebtorCreditor = entry.DebtorCreditor;
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
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      dto.LastChangeDate = entry.LastChangeDate != null ? entry.LastChangeDate :
                           ExecutionServer.DateMaxValue;

      AssignLabelNameAndNumber(dto, entry);

      return dto;
    }


    static private void AssignLabelNameAndNumber(AnaliticoDeCuentasEntryDto dto,
                                                 AnaliticoDeCuentasEntry entry) {

      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

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
    }

    #endregion Private methods

  } // class AnaliticoDeCuentasMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.AnaliticoDeCuentas.Adapters
