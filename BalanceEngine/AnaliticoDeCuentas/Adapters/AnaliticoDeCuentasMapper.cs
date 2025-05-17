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
using System.Collections.Generic;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map analitico de cuentas.</summary>
  static internal class AnaliticoDeCuentasMapper {

    #region Public methods


    static internal AnaliticoDeCuentasDto Map(TrialBalanceQuery query,
                                              FixedList<AnaliticoDeCuentasEntry> entries) {
      return new AnaliticoDeCuentasDto {
        Query = query,
        Columns = DataColumns(query),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    static internal AnaliticoDeCuentasEntry CreatePartialCopy(
                                                AnaliticoDeCuentasEntry balanceEntry) {
      var entry = new AnaliticoDeCuentasEntry();
      entry.Account = balanceEntry.Account;
      entry.AccountId = balanceEntry.AccountId;
      entry.SubledgerAccountId = balanceEntry.SubledgerAccountId;
      entry.Ledger = balanceEntry.Ledger;
      entry.Currency = balanceEntry.Currency;
      entry.ItemType = balanceEntry.ItemType;
      entry.Sector = balanceEntry.Sector;
      entry.DebtorCreditor = balanceEntry.DebtorCreditor;
      entry.DomesticBalance = balanceEntry.DomesticBalance;
      entry.ForeignBalance = balanceEntry.ForeignBalance;
      entry.ExchangeRate = balanceEntry.ExchangeRate;

      return entry;
    }


    #endregion Public methods

    #region Private methods

    static public AnaliticoDeCuentasEntryDto MapEntry(AnaliticoDeCuentasEntry entry) {
      var dto = new AnaliticoDeCuentasEntryDto();

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.DebtorCreditor = entry.DebtorCreditor;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.AccountMark = entry.AccountMark;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.DomesticBalance = entry.DomesticBalance;
      dto.ForeignBalance = entry.ForeignBalance;
      dto.TotalBalance = entry.TotalBalance;
      dto.AverageBalance = entry.AverageBalance;
      dto.LastChangeDate = entry.LastChangeDate;

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


    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      if (query.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "Saldo Mon. Nal.", "decimal"));
      columns.Add(new DataTableColumn("foreignBalance", "Saldo Mon. Ext.", "decimal"));
      columns.Add(new DataTableColumn("totalBalance", "Total", "decimal"));

      if (query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
      }

      return columns.ToFixedList();
    }

    #endregion Private methods

  } // class AnaliticoDeCuentasMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.AnaliticoDeCuentas.Adapters
