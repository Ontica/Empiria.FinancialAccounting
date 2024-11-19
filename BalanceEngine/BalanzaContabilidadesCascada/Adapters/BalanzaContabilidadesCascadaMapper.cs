/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaContabilidadesCascadaMapper         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balanza con contabilidades en cascada.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map balanza con contabilidades en cascada.</summary>
  static internal class BalanzaContabilidadesCascadaMapper {

    #region Public methods

    static internal BalanzaContabilidadesCascadaDto Map(TrialBalance entries) {
      return new BalanzaContabilidadesCascadaDto {
        Query = entries.Query,
        Columns = DataColumns(entries.Query),
        Entries = Map(entries.Entries, entries.Query)
      };
    }

    #endregion Public methods


    #region Private methods


    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery Query) {

      List<DataTableColumn> columns = new List<DataTableColumn>();

      if (Query.ReturnLedgerColumn) {
        columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      }

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("sectorCode", "Sct", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("initialBalance", "Saldo anterior", "decimal"));
      columns.Add(new DataTableColumn("debit", "Cargos", "decimal"));
      columns.Add(new DataTableColumn("credit", "Abonos", "decimal"));
      columns.Add(new DataTableColumn("currentBalance", "Saldo actual", "decimal"));
      if (Query.InitialPeriod.ExchangeRateTypeUID != string.Empty ||
          Query.InitialPeriod.UseDefaultValuation) {
        columns.Add(new DataTableColumn("exchangeRate", "TC", "decimal", 6));
      }
      if (Query.WithAverageBalance) {
        columns.Add(new DataTableColumn("averageBalance", "Saldo promedio", "decimal"));
        columns.Add(new DataTableColumn("lastChangeDate", "Último movimiento", "date"));
      }

      return columns.ToFixedList();
    }


    static public FixedList<BalanzaContabilidadesCascadaEntryDto> Map(
                    FixedList<ITrialBalanceEntry> entries, TrialBalanceQuery query) {

      var mappedItems = entries.Select((x) => MapEntry((TrialBalanceEntry) x, query));

      return new FixedList<BalanzaContabilidadesCascadaEntryDto>(mappedItems);
    }


    static public BalanzaContabilidadesCascadaEntryDto MapEntry(
                                                        TrialBalanceEntry entry, TrialBalanceQuery query) {

      var dto = new BalanzaContabilidadesCascadaEntryDto();

      AssignLedgerAndCurrencyAndDebtorCreditorInfo(dto, entry);
      AssignLabelNameAndNumberInfo(dto, entry);
      AssignHasAccountStatementAndClickableEntry(dto, entry, query);

      dto.ItemType = entry.ItemType;
      dto.StandardAccountId = entry.Account.Id;
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
      dto.LastChangeDate = entry.LastChangeDate;
      dto.LastChangeDateForBalances = dto.LastChangeDate;

      return dto;
    }


    private static void AssignHasAccountStatementAndClickableEntry(
                        BalanzaContabilidadesCascadaEntryDto dto,
                        TrialBalanceEntry entry, TrialBalanceQuery query) {

      if ((entry.ItemType == TrialBalanceItemType.Entry ||
          entry.ItemType == TrialBalanceItemType.Summary)
          //&& !query.UseDefaultValuation && !query.ValuateBalances
          ) {

        dto.HasAccountStatement = true;
        dto.ClickableEntry = true;
      }
    }


    static private void AssignLabelNameAndNumberInfo(BalanzaContabilidadesCascadaEntryDto dto,
                                                     TrialBalanceEntry entry) {

      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      if (subledgerAccount.IsEmptyInstance || subledgerAccount.Number == "0") {
        dto.AccountName = entry.GroupName != "" ? entry.GroupName :
                          entry.Account.Name;
        dto.AccountNumber = entry.GroupNumber != "" ? entry.GroupNumber :
                            !entry.Account.IsEmptyInstance ?
                            entry.Account.Number : "";
      } else {
        dto.AccountName = subledgerAccount.Name;
        dto.AccountNumber = subledgerAccount.Number;
        dto.SubledgerAccountNumber = subledgerAccount.Number;
      }

      dto.AccountNumberForBalances = entry.Account.Number;
    }


    static private void AssignLedgerAndCurrencyAndDebtorCreditorInfo(
                        BalanzaContabilidadesCascadaEntryDto dto, TrialBalanceEntry entry) {

      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;

      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {

        dto.LedgerName = entry.Ledger.Name;
        dto.DebtorCreditor = entry.DebtorCreditor.ToString();
      }

      if (entry.ItemType != TrialBalanceItemType.BalanceTotalConsolidated) {
        dto.CurrencyCode = entry.Currency.Code;
      }
    }


    #endregion Private methods

  } // class BalanzaContabilidadesCascadaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
