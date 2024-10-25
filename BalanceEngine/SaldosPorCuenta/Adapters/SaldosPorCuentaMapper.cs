/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : SaldosPorCuentaMapper                   License   : Please read LICENSE.txt file               *
*                                                                                                            *
*  Summary  : Methods used to map Saldos por cuenta entries.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map Saldos por cuenta entries.</summary>
  static internal class SaldosPorCuentaMapper {

    #region Public methods


    static internal SaldosPorCuentaDto Map(TrialBalance entries) {
      return new SaldosPorCuentaDto {
        Query = entries.Query,
        Columns = DataColumns(entries.Query),
        Entries = Map(entries.Entries, entries.Query)
      };
    }


    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery Query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      LedgerAndSubledgerAccountColumns(columns, Query);

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      columns.Add(new DataTableColumn("debtorCreditor", "Naturaleza", "text"));
      if (Query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));

      }
      columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));

      return columns.ToFixedList();
    }


    static private FixedList<SaldosPorCuentaEntryDto> Map(FixedList<ITrialBalanceEntry> entries,
                                                             TrialBalanceQuery query) {
      var mappedItems = entries.Select((x) => MapEntry((TrialBalanceEntry) x, query));

      return new FixedList<SaldosPorCuentaEntryDto>(mappedItems);
    }


    #endregion Public methods


    #region Private methods


    static public SaldosPorCuentaEntryDto MapEntry(TrialBalanceEntry entry, TrialBalanceQuery query) {
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      var dto = new SaldosPorCuentaEntryDto();

      dto.ItemType = entry.ItemType;
      AssignLedgerProperties(dto, entry);
      AssignLabelNameAndNumber(dto, entry, subledgerAccount);
      dto.CurrencyCode = entry.Currency.Code;
      dto.SectorCode = entry.Sector.Code;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.SubledgerAccountNumber = subledgerAccount.Number;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.AverageBalance = entry.AverageBalance;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      AssignDebtorCreditorAndLastChangeDate(dto, entry, query);
      AssignHasAccountStatementAndClickableEntry(dto, entry, query);
      return dto;
    }


    static private void AssignDebtorCreditorAndLastChangeDate(SaldosPorCuentaEntryDto dto,
                                                              TrialBalanceEntry entry,
                                                              TrialBalanceQuery query) {
      string debtorCreditor;

      if (!query.WithSubledgerAccount) {
        debtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                         entry.DebtorCreditor.ToString() : "";

      } else {
        debtorCreditor = entry.ItemType == TrialBalanceItemType.Summary ?
                             entry.DebtorCreditor.ToString() : "";

      }
      dto.DebtorCreditor = debtorCreditor;

      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;

      dto.LastChangeDateForBalances = entry.LastChangeDate;
    }


    static private void AssignHasAccountStatementAndClickableEntry(SaldosPorCuentaEntryDto dto,
                                                                   TrialBalanceEntry entry,
                                                                   TrialBalanceQuery query) {
      if ((entry.ItemType == TrialBalanceItemType.Entry ||
          entry.ItemType == TrialBalanceItemType.Summary)
          && !query.UseDefaultValuation && !query.ValuateBalances
          ) {

        dto.HasAccountStatement = true;
        dto.ClickableEntry = true;
      }
    }


    static private void AssignLabelNameAndNumber(SaldosPorCuentaEntryDto dto, TrialBalanceEntry entry,
                                                SubledgerAccount subledgerAccount) {


      if (!subledgerAccount.IsEmptyInstance && subledgerAccount.Number != "0") {

        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      } else {

        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      }
    }


    static private void AssignLedgerProperties(SaldosPorCuentaEntryDto dto, TrialBalanceEntry entry) {

      dto.LedgerUID = !entry.Ledger.IsEmptyInstance ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
    }



    static private void LedgerAndSubledgerAccountColumns(List<DataTableColumn> columns,
                                                            TrialBalanceQuery Query) {
      if (Query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));

      }
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (!Query.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));

      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      }
    }


    #endregion Private methods

  } // class SaldosPorCuentaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
