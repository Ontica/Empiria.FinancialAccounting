﻿/* Empiria Financial *****************************************************************************************
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

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters {

  /// <summary>Methods used to map balances.</summary>
  static internal class BalanceMapper {

    #region Public mappers

    static internal BalancesDto Map(Balances balances) {
      return new BalancesDto {
        Query = balances.Query,
        Columns = MapColumns(balances.Query),
        Entries = MapToDto(balances.Entries, balances.Query)
      };
    }


    static internal FixedList<BalanceEntry> MapToBalance(FixedList<TrialBalanceEntry> entries) {
      var mappedEntries = new List<BalanceEntry>();

      foreach (var entry in entries) {
        var balanceEntry = new BalanceEntry();
        balanceEntry.ItemType = entry.ItemType == TrialBalanceItemType.Entry ?
                                TrialBalanceItemType.Entry : entry.ItemType;
        balanceEntry.Ledger = entry.Ledger;
        balanceEntry.Currency = entry.Currency;
        balanceEntry.Account = entry.Account;
        balanceEntry.Sector = entry.Sector;
        balanceEntry.SubledgerAccountId = entry.SubledgerAccountId;
        balanceEntry.InitialBalance = Math.Round(entry.InitialBalance, 2);
        balanceEntry.CurrentBalance = Math.Round(entry.CurrentBalance, 2);
        balanceEntry.LastChangeDate = entry.LastChangeDate;
        balanceEntry.DebtorCreditor = entry.DebtorCreditor;
        mappedEntries.Add(balanceEntry);
      }

      return mappedEntries.ToFixedList();
    }

    #endregion Public mappers


    #region Private methods

    private static FixedList<DataTableColumn> MapColumns(BalancesQuery query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("ledgerNumber", "Deleg", "text"));
      columns.Add(new DataTableColumn("ledgerName", "Delegación", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      if (query.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliarConsultaRapida) {
        columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
      }
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));

      return columns.ToFixedList();
    }


    static private FixedList<IBalanceEntryDto> MapToDto(
                    FixedList<BalanceEntry> list, BalancesQuery query) {

      switch (query.TrialBalanceType) {
        case TrialBalanceType.SaldosPorAuxiliarConsultaRapida:

          var mapped = list.Select((x) => MapToBalanceBySubledgerAccount(x));

          return new FixedList<IBalanceEntryDto>(mapped);

        case TrialBalanceType.SaldosPorCuentaConsultaRapida:

          var mappedItems = list.Select((x) => MapToBalanceByAccount(x, query));

          return new FixedList<IBalanceEntryDto>(mappedItems);

        default:
          throw Assertion.EnsureNoReachThisCode(
                $"Unhandled balance type {query.TrialBalanceType}.");
      }
    }


    static private BalanceEntryDto MapToBalanceByAccount(BalanceEntry entry, BalancesQuery query) {

      var dto = new BalanceEntryDto();

      dto.ItemType = entry.ItemType;
      if (entry.ItemType != TrialBalanceItemType.Total &&
          entry.ItemType != TrialBalanceItemType.Group) {
        dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
        dto.LedgerNumber = entry.Ledger.Number;
        dto.LedgerName = entry.Ledger.Name != string.Empty ? entry.Ledger.Name : "";
      } else {
        dto.LedgerUID = "";
        dto.LedgerNumber = "";
        dto.LedgerName = "";
      }
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      dto.subledgerAccountName = entry.SubledgerAccountName;
      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCurrency) {
        dto.AccountNumber = "";

      } else if (entry.SubledgerAccountNumber != string.Empty && query.WithSubledgerAccount) {
        dto.AccountNumber = entry.SubledgerAccountNumber;
      } else {
        dto.AccountNumber = entry.Account.Number == "Empty" ? "" : entry.Account.Number;
      }

      dto.AccountNumberForBalances = entry.Account.Number;

      if (entry.ItemType == TrialBalanceItemType.Group || entry.ItemType == TrialBalanceItemType.Total) {
        dto.AccountName = entry.ItemType == TrialBalanceItemType.Total ?
                          $"{entry.GroupName}, Naturaleza: {entry.DebtorCreditor}" : entry.GroupName;
      } else if (entry.SubledgerAccountName != "") {
        dto.AccountName = entry.SubledgerAccountName;
      } else {
        dto.AccountName = entry.Account.Name;
      }
      dto.DebtorCreditor = entry.DebtorCreditor.ToString();
      dto.SectorCode = entry.Sector.Code;
      dto.InitialBalance = entry.InitialBalance;
      if (entry.ItemType != TrialBalanceItemType.Total) {
        dto.CurrentBalance = entry.CurrentBalance;
      }
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      dto.LastChangeDateForBalances = entry.LastChangeDate;


      dto.HasAccountStatement = entry.ItemType == TrialBalanceItemType.Total || entry.ItemType == TrialBalanceItemType.Entry;
      dto.ClickableEntry = dto.HasAccountStatement;

      return dto;
    }


    static private BalanceEntryDto MapToBalanceBySubledgerAccount(BalanceEntry entry) {

      var dto = new BalanceEntryDto();
      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name != string.Empty ? entry.Ledger.Name : "";
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      dto.AccountNumber = entry.Account.Number == "Empty" ? entry.SubledgerAccountNumber : entry.Account.Number;
      dto.AccountNumberForBalances = entry.Account.Number;
      if (entry.ItemType == TrialBalanceItemType.Entry) {
        dto.AccountName = entry.GroupName == string.Empty ?
                          entry.Account.Name : entry.GroupName;
      } else {
        dto.AccountName = (entry.GroupName == string.Empty ?
                         entry.Account.Name : entry.GroupName)
                         + $" [({entry.Currency.Code}) {entry.Currency.Name}]";
      }
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      dto.subledgerAccountName = entry.SubledgerAccountName;
      dto.SectorCode = entry.Sector.Code;
      dto.InitialBalance = entry.InitialBalance;
      if (entry.ItemType == TrialBalanceItemType.Entry) {
        dto.CurrentBalance = entry.CurrentBalance;
      }
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.DebtorCreditor.ToString() : "";
      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
      dto.LastChangeDateForBalances = entry.LastChangeDate;
      dto.HasAccountStatement = true;
      dto.ClickableEntry = true;
      return dto;
    }


    static internal BalanceEntry MapToBalanceEntry(BalanceEntry entry) {
      return new BalanceEntry {
        ItemType = entry.ItemType,
        Ledger = entry.Ledger,
        Currency = entry.Currency,
        Sector = entry.Sector,
        Account = entry.Account,
        GroupName = entry.GroupName,
        GroupNumber = entry.GroupNumber,
        CurrentBalance = entry.CurrentBalance,
        DebtorCreditor = entry.Account.DebtorCreditor,
        LastChangeDate = entry.LastChangeDate
      };
    }

    #endregion Private methods

  } // class BalanceMapper

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
