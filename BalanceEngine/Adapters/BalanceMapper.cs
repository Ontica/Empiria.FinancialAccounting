/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanceMapper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balances.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map balances.</summary>
  static internal class BalanceMapper {

    #region Public mappers

    static internal BalanceDto Map(Balance balance) {
      return new BalanceDto {
        Command = balance.Command,
        Columns = MapColumns(),
        Entries = MapToDto(balance.Entries)
      };
    }

    #endregion Public mappers


    #region Private methods

    private static FixedList<DataTableColumn> MapColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text-nowrap"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));

      return columns.ToFixedList();
    }

    static private FixedList<IBalanceEntryDto> MapToDto(FixedList<IBalanceEntry> list) {

      var mappedItems = list.Select((x) => MapToBalance((BalanceEntry) x));

      return new FixedList<IBalanceEntryDto>(mappedItems);
    }

    static private BalanceEntryDto MapToBalance(BalanceEntry entry) {

      var dto = new BalanceEntryDto();
      dto.ItemType = entry.ItemType;
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Number != string.Empty ? entry.Ledger.FullName : "";
      dto.CurrencyCode = entry.Currency.Code;
      dto.AccountNumber = entry.Account.Number == "Empty" ? "" : entry.Account.Number;
      dto.AccountName = entry.GroupName == string.Empty ? entry.Account.Name : entry.GroupName;
      dto.SectorCode = entry.Sector.Code;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.DebtorCreditor = entry.DebtorCreditor.ToString();
      dto.LastChangeDate = entry.LastChangeDate;

      return dto;
    }

    #endregion Private methods

  } // class BalanceMapper

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
