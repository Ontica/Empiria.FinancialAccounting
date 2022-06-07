/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : SaldosPorAuxiliarMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map saldos por auxiliar.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map saldos por auxiliar.</summary>
  static internal class SaldosPorAuxiliarMapper {


    #region Public methods


    static internal SaldosPorAuxiliarDto Map(TrialBalance entries) {
      return new SaldosPorAuxiliarDto {
        Query = entries.Query,
        Columns = DataColumns(entries.Query),
        Entries = Map(entries.Entries, entries.Query)
      };
    }


    #endregion Public methods


    #region Private methods


    static public FixedList<DataTableColumn> DataColumns(TrialBalanceQuery Query) {
      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("ledgerNumber", "Cont", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta / Auxiliar", "text-nowrap"));

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


    static private FixedList<SaldosPorAuxiliarEntryDto> Map(FixedList<ITrialBalanceEntry> entries,
                                                            TrialBalanceQuery query) {

      var mappedItems = entries.Select((x) => MapEntry((TrialBalanceEntry) x, query));

      return new FixedList<SaldosPorAuxiliarEntryDto>(mappedItems);
    }


    static public SaldosPorAuxiliarEntryDto MapEntry(TrialBalanceEntry entry, TrialBalanceQuery query) {

      var dto = new SaldosPorAuxiliarEntryDto();
      SubledgerAccount subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountId);

      dto.ItemType = entry.ItemType;
      dto.LedgerUID = entry.Ledger.UID != "Empty" ? entry.Ledger.UID : "";
      dto.LedgerNumber = entry.Ledger.Number;
      if (entry.ItemType == TrialBalanceItemType.Summary ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.LedgerName = entry.Ledger.Name;
      }
      dto.StandardAccountId = entry.Account.Id;
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
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
      dto.AccountNumberForBalances = entry.Account.Number != "Empty" ? entry.Account.Number : "";

      if (entry.ItemType == TrialBalanceItemType.Entry && subledgerAccount.IsEmptyInstance) {

        subledgerAccount = SubledgerAccount.Parse(entry.SubledgerAccountIdParent);
        if (!subledgerAccount.IsEmptyInstance) {
          dto.SubledgerAccountNumber = subledgerAccount.Number;
        }
      } else {
        dto.SubledgerAccountNumber = subledgerAccount.Number;
      }

      dto.AccountRole = entry.Account.Role;
      dto.AccountLevel = entry.Account.Level;
      dto.SectorCode = entry.Sector.Code;
      dto.SubledgerAccountId = entry.SubledgerAccountId;
      dto.InitialBalance = entry.InitialBalance;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      if (entry.ItemType == TrialBalanceItemType.Total ||
          entry.ItemType == TrialBalanceItemType.Entry) {
        dto.CurrentBalance = entry.CurrentBalance;
      }
      if (entry.ItemType == TrialBalanceItemType.Entry) {
        dto.AverageBalance = entry.AverageBalance;
      }
      dto.CurrentBalanceForBalances = entry.CurrentBalance;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.DebtorCreditor = entry.ItemType == TrialBalanceItemType.Entry ?
                             entry.DebtorCreditor.ToString() : "";

      dto.LastChangeDate = entry.LastChangeDate;

      dto.LastChangeDateForBalances = dto.LastChangeDate;
      dto.IsParentPostingEntry = entry.IsParentPostingEntry;
      dto.HasAccountStatement = (entry.ItemType == TrialBalanceItemType.Entry ||
                                 entry.ItemType == TrialBalanceItemType.Summary) &&
                                !query.UseDefaultValuation &&
                                query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                                query.InitialPeriod.ExchangeRateTypeUID.Length == 0;
      dto.ClickableEntry = (entry.ItemType == TrialBalanceItemType.Entry ||
                            entry.ItemType == TrialBalanceItemType.Summary) &&
                            !query.UseDefaultValuation &&
                            query.InitialPeriod.ValuateToCurrrencyUID.Length == 0 &&
                            query.InitialPeriod.ExchangeRateTypeUID.Length == 0;

      return dto;

    }


    #endregion Private methods


  } // class SaldosPorAuxiliarMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
