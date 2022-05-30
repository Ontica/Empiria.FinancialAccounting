/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaTradicionalMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balanza tradicional.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map balanza tradicional.</summary>
  static internal class BalanzaTradicionalMapper {

    #region Public methods


    static internal BalanzaTradicionalDto Map(TrialBalance entries) {
      return new BalanzaTradicionalDto {
        Query = entries.Query,
        Columns = DataColumns(entries.Query),
        Entries = Map(entries.Entries, entries.Query)
      };
    }


    #region Private methods

    #endregion Private methods



    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (query.WithSubledgerAccount) {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));
      } else {
        columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      }

      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));

      columns.Add(new DataTableColumn("initialBalance", "Saldo anterior", "decimal"));
      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      if (query.InitialPeriod.ExchangeRateTypeUID != string.Empty ||
          query.InitialPeriod.UseDefaultValuation) {
        columns.Add(new DataTableColumn("exchangeRate", "TC", "decimal", 6));
      }
      if (query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
      }

      return columns.ToFixedList();
    }


    static private FixedList<BalanzaTradicionalEntryDto> Map(FixedList<ITrialBalanceEntry> entries,
                                                             TrialBalanceQuery query) {

      var mappedItems = entries.Select((x) => MapEntry((TrialBalanceEntry) x, query));

      return new FixedList<BalanzaTradicionalEntryDto>(mappedItems);
    }


    static public BalanzaTradicionalEntryDto MapEntry(TrialBalanceEntry entry,
                                                      TrialBalanceQuery query) {
      var dto = new BalanzaTradicionalEntryDto();

      AssignLedgerCurrencyLabelNameAndNumber(dto, entry);
      AssignDebtorCreditorAndLastChangeDate(dto, entry);
      AssignHasAccountStatementAndClickableEntry(dto, entry, query);

      dto.ItemType = entry.ItemType;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.CurrentBalance = entry.CurrentBalance;
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.SecondExchangeRate = entry.SecondExchangeRate;
      dto.AverageBalance = entry.AverageBalance;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      return dto;
    }


    #endregion Public methods


    #region Private methods


    static private void AssignDebtorCreditorAndLastChangeDate(BalanzaTradicionalEntryDto dto,
                                                              TrialBalanceEntry entry) {

      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ?
                           entry.DebtorCreditor.ToString() : "";

      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;

      dto.LastChangeDateForBalances = dto.LastChangeDate;
    }


    private static void AssignHasAccountStatementAndClickableEntry(BalanzaTradicionalEntryDto dto,
                                                                   TrialBalanceEntry entry,
                                                                   TrialBalanceQuery query) {
      if ((entry.ItemType == TrialBalanceItemType.Entry ||
          entry.ItemType == TrialBalanceItemType.Summary) &&
          !query.UseDefaultValuation && !query.ValuateBalances) {

        dto.HasAccountStatement = true;
        dto.ClickableEntry = true;
      }
    }


    static private void AssignLedgerCurrencyLabelNameAndNumber(BalanzaTradicionalEntryDto dto,
                                                               TrialBalanceEntry entry) {

      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.SubledgerAccountNumber = subledgerAccount.Number;
      dto.LedgerNumber = entry.Ledger.Number;

      if (entry.Ledger.UID != "Empty") {
        dto.LedgerUID = entry.Ledger.UID;
      }

      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }

      if (entry.ItemType != TrialBalanceItemType.BalanceTotalConsolidated) {
        dto.CurrencyCode = entry.Currency.Code;
      }

      if (subledgerAccount.IsEmptyInstance || subledgerAccount.Number == "0") {
        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
      }
    }


    #endregion Private methods

  } // class BalanzaTradicionalMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
