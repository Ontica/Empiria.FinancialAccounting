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
        Columns = MapColumns(balance.Command),
        Entries = MapToDto(balance.Entries, balance.Command)
      };
    }


    internal static TrialBalanceCommand MapToTrialBalanceCommand(BalanceCommand command) {
      var trialBalanceCommand = new TrialBalanceCommand();

      trialBalanceCommand.AccountsChartUID = command.AccountsChartUID;
      trialBalanceCommand.FromAccount = command.FromAccount;
      trialBalanceCommand.InitialPeriod.FromDate = command.InitialPeriod.FromDate;
      trialBalanceCommand.InitialPeriod.ToDate = command.InitialPeriod.ToDate;
      trialBalanceCommand.SubledgerAccount = command.SubledgerAccount;
      trialBalanceCommand.TrialBalanceType = command.TrialBalanceType;
      //trialBalanceCommand.ShowCascadeBalances = true;
      trialBalanceCommand.WithSubledgerAccount = command.WithSubledgerAccount;

      //if (command.TrialBalanceType != TrialBalanceType.SaldosPorAuxiliarConsultaRapida) {
      //  trialBalanceCommand.ShowCascadeBalances = command.WithSubledgerAccount;
      //}

      return trialBalanceCommand;
    }


    #endregion Public mappers


    #region Private methods

    private static FixedList<DataTableColumn> MapColumns(BalanceCommand command) {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("ledgerNumber", "Deleg", "text"));
      if (command.TrialBalanceType == TrialBalanceType.SaldosPorCuentaConsultaRapida) {
        columns.Add(new DataTableColumn("ledgerName", "Delegación", "text"));
      }
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));

      return columns.ToFixedList();
    }


    static private FixedList<IBalanceEntryDto> MapToDto(
                    FixedList<BalanceEntry> list, BalanceCommand command) {

      switch (command.TrialBalanceType) {
        case TrialBalanceType.SaldosPorAuxiliarConsultaRapida:

          var mapped = list.Select((x) => MapToBalanceBySubledgerAccount(x));

          return new FixedList<IBalanceEntryDto>(mapped);

        case TrialBalanceType.SaldosPorCuentaConsultaRapida:

          var mappedItems = list.Select((x) => MapToBalanceByAccount(x, command));

          return new FixedList<IBalanceEntryDto>(mappedItems);

        default:
          throw Assertion.AssertNoReachThisCode(
                $"Unhandled balance type {command.TrialBalanceType}.");
      }
    }


    static private BalanceEntryDto MapToBalanceByAccount(BalanceEntry entry, BalanceCommand command) {

      var dto = new BalanceEntryDto();

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name != string.Empty ? entry.Ledger.Name : "";
      dto.CurrencyCode = entry.Currency.Code;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCurrency) {
        dto.AccountNumber = "";

      } else if (entry.SubledgerAccountNumber != string.Empty && command.WithSubledgerAccount) {
        dto.AccountNumber = entry.SubledgerAccountNumber;

      } else {
        dto.AccountNumber = entry.Account.Number == "Empty" ? "" : entry.Account.Number;
      }

      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountName = entry.GroupName == string.Empty ? entry.Account.Name : entry.GroupName;
      dto.SectorCode = entry.Sector.Code;
      dto.InitialBalance = entry.InitialBalance;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.DebtorCreditor.ToString() : "";
      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;


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
      dto.AccountNumber = entry.Account.Number == "Empty" ? entry.SubledgerAccountNumber : entry.Account.Number;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountName = entry.GroupName == string.Empty ? entry.Account.Name : entry.GroupName;
      dto.SubledgerAccountNumber = entry.SubledgerAccountNumber;
      dto.SectorCode = entry.Sector.Code;
      dto.InitialBalance = entry.InitialBalance;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.DebtorCreditor.ToString() : "";
      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;
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
        DebtorCreditor = entry.DebtorCreditor,
        LastChangeDate = entry.LastChangeDate
      };
    }

    #endregion Private methods

  } // class BalanceMapper

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
