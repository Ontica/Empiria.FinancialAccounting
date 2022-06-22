/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaDolarizadaMapper                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balanza dolarizada.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map balanza dolarizada.</summary>
  static internal class BalanzaDolarizadaMapper {


    #region Public methods

    #endregion Public methods

    static internal BalanzaDolarizadaDto Map(TrialBalanceQuery query,
                                             FixedList<BalanzaDolarizadaEntry> entries) {

      return new BalanzaDolarizadaDto {
        Query = query,
        Columns = DataColumns(),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }


    #region Private methods


    static internal BalanzaDolarizadaEntry BalanzaDolarizadaPartialCopy(
                                           BalanzaDolarizadaEntry valuedEntry) {

      var entry = new BalanzaDolarizadaEntry();
      entry.Account = valuedEntry.Account;
      entry.Currency = valuedEntry.Currency;
      entry.Sector = Sector.Empty;
      entry.GroupName = valuedEntry.GroupName;
      entry.ItemType = valuedEntry.ItemType;

      return entry;
    }


    static public FixedList<DataTableColumn> DataColumns() {

      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("currencyName", "Moneda", "text"));
      columns.Add(new DataTableColumn("currencyCode", "Clave Mon.", "text"));
      columns.Add(new DataTableColumn("totalBalance", "Importe Mon. Ext.", "decimal"));
      columns.Add(new DataTableColumn("valuedExchangeRate", "Tipo cambio", "decimal", 6));
      columns.Add(new DataTableColumn("totalEquivalence", "Equivalencia en dólares", "decimal"));

      return columns.ToFixedList();
    }


    static public BalanzaDolarizadaEntryDto MapEntry(BalanzaDolarizadaEntry entry) {

      var dto = new BalanzaDolarizadaEntryDto();

      dto.ItemType = entry.ItemType;
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountNumber = entry.Account.Number;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.SectorCode = entry.Sector.Code;
      dto.ExchangeRate = entry.ExchangeRate;
      dto.TotalEquivalence = entry.TotalEquivalence;
      dto.GroupName = entry.GroupName;
      dto.GroupNumber = entry.GroupNumber;

      AssignAccountName(dto, entry);
      AssignTotalBalanceAndExchangeRate(dto, entry);

      return dto;
    }


    private static void AssignTotalBalanceAndExchangeRate(BalanzaDolarizadaEntryDto dto,
                                                          BalanzaDolarizadaEntry entry) {
      
      if (entry.ItemType != TrialBalanceItemType.BalanceTotalCurrency) {
        dto.TotalBalance = entry.TotalBalance;
        dto.ValuedExchangeRate = entry.ValuedExchangeRate;
      }

    }


    static private void AssignAccountName(BalanzaDolarizadaEntryDto dto, BalanzaDolarizadaEntry entry) {

      if (entry.ItemType == TrialBalanceItemType.BalanceTotalCurrency) {
        dto.AccountName = entry.GroupName;

      } else {
        dto.AccountName = entry.Account.Name;

      }
    }


    #endregion Private methods

  } // class BalanzaDolarizadaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
