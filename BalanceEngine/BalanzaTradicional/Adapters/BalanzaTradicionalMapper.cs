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
        Command = entries.Command,
        Columns = DataColumns(entries.Command),
        Entries = Map(entries.Entries, entries.Command)
      };
    }


    #region Private methods

    #endregion Private methods



    static public FixedList<DataTableColumn> DataColumns(TrialBalanceCommand command) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (command.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));

      if (command.WithSubledgerAccount) {
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
      if (command.InitialPeriod.ExchangeRateTypeUID != string.Empty ||
          command.InitialPeriod.UseDefaultValuation) {
        columns.Add(new DataTableColumn("exchangeRate", "TC", "decimal", 6));
      }
      if (command.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
      }

      return columns.ToFixedList();
    }


    static private FixedList<BalanzaTradicionalEntryDto> Map(FixedList<ITrialBalanceEntry> entries,
                                                             TrialBalanceCommand command) {

      var mappedItems = entries.Select((x) => MapEntry((TrialBalanceEntry) x, command));

      return new FixedList<BalanzaTradicionalEntryDto>(mappedItems);
    }


    static public BalanzaTradicionalEntryDto MapEntry(TrialBalanceEntry entry, 
                                                       TrialBalanceCommand command) {
      var dto = new BalanzaTradicionalEntryDto();

      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.ItemType == TrialBalanceItemType.BalanceTotalConsolidated ? "" :
                         entry.Currency.Code;
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
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.SubledgerAccountNumber = subledgerAccount.Number;

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

      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ?
                           entry.DebtorCreditor.ToString() : "";

      dto.LastChangeDate = entry.ItemType == TrialBalanceItemType.Entry ||
                           entry.ItemType == TrialBalanceItemType.Summary ?
                           entry.LastChangeDate : ExecutionServer.DateMaxValue;

      dto.LastChangeDateForBalances = dto.LastChangeDate;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                !command.UseDefaultValuation &&
                                command.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                command.InitialPeriod.ExchangeRateTypeUID.Length == 0;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                            entry.ItemType == TrialBalanceItemType.Summary) &&
                            !command.UseDefaultValuation &&
                            command.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                            command.InitialPeriod.ExchangeRateTypeUID.Length == 0;

      return dto;
    }


    #endregion Public methods

  } // class BalanzaTradicionalMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
