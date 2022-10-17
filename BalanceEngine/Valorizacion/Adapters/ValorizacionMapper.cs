/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : ValorizacionMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map valorized report.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map valorized report.</summary>
  static internal class ValorizacionMapper {

    #region Public methods

    static internal ValorizacionDto Map(TrialBalanceQuery query,
                                             FixedList<ValorizacionEntry> entries) {

      //var columnsByCurrency = GetColumnsByCurrency(entries);

      return new ValorizacionDto {
        Query = query,
        Columns = DataColumns(),
        Entries = entries.Select(x => MapEntry(x, query))
                         .ToFixedList()
      };
    }


    public static FixedList<DataTableColumn> DataColumns() {

      List<DataTableColumn> columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text-nowrap"));

      columns.Add(new DataTableColumn("usd", "USD", "decimal"));
      columns.Add(new DataTableColumn("yen", "YEN", "decimal"));
      columns.Add(new DataTableColumn("eur", "EUR", "decimal"));
      columns.Add(new DataTableColumn("udi", "UDI", "decimal"));

      columns.Add(new DataTableColumn("lastUSD", "Mes anterior USD", "decimal"));
      columns.Add(new DataTableColumn("lastYEN", "Mes anterior YEN", "decimal"));
      columns.Add(new DataTableColumn("lastEUR", "Mes anterior EUR", "decimal"));
      columns.Add(new DataTableColumn("lastUDI", "Mes anterior UDI", "decimal"));

      columns.Add(new DataTableColumn("currentUSD", "Mes actual USD", "decimal"));
      columns.Add(new DataTableColumn("currentYEN", "Mes actual YEN", "decimal"));
      columns.Add(new DataTableColumn("currentEUR", "Mes actual EUR", "decimal"));
      columns.Add(new DataTableColumn("currentUDI", "Mes actual UDI", "decimal"));

      columns.Add(new DataTableColumn("valuedEffectUSD", "Efecto valorización USD", "decimal"));
      columns.Add(new DataTableColumn("valuedEffectYEN", "Efecto valorización YEN", "decimal"));
      columns.Add(new DataTableColumn("valuedEffectEUR", "Efecto valorización EUR", "decimal"));
      columns.Add(new DataTableColumn("valuedEffectUDI", "Efecto valorización UDI", "decimal"));

      columns.Add(new DataTableColumn("totalValued", "TOTAL", "decimal"));

      //for (int i = 1; i <= 12; i++) {
      //  columns.Add(new DataTableColumn("meses", $"Mes {i}", "decimal"));
      //}

      columns.Add(new DataTableColumn("totalBalance", "ACUMULADO", "decimal"));

      return columns.ToFixedList();
    }

    
    public static ValorizacionEntryDto MapEntry(ValorizacionEntry entry, TrialBalanceQuery query) {

      var dto = new ValorizacionEntryDto();

      dto.ItemType = entry.ItemType;

      dto.StandardAccountId = entry.Account.Id;
      dto.AccountName = entry.Account.Name;
      dto.AccountNumber = entry.Account.Number;
      dto.CurrencyCode = entry.Currency.Code;

      //dto.TotalBalance = entry.TotalBalance;

      AssignValuesByCurrency(dto, entry);

      dto.ValuedExchangeRate = entry.ValuedExchangeRate;
      dto.LastChangeDate = entry.LastChangeDate;


      return dto;
    }


    #endregion Public methods


    #region Private methods


    static private void AssignValuesByCurrency(ValorizacionEntryDto dto, ValorizacionEntry entry) {
      
      dto.USD = entry.ValuesByCurrency.USD;
      dto.EUR = entry.ValuesByCurrency.EUR;
      dto.YEN = entry.ValuesByCurrency.YEN;
      dto.UDI = entry.ValuesByCurrency.UDI;

      dto.LastUSD = entry.ValuesByCurrency.USD * entry.ValuesByCurrency.LastExchangeRateUSD;
      dto.LastYEN = entry.ValuesByCurrency.YEN * entry.ValuesByCurrency.LastExchangeRateYEN;
      dto.LastEUR = entry.ValuesByCurrency.EUR * entry.ValuesByCurrency.LastExchangeRateEUR;
      dto.LastUDI = entry.ValuesByCurrency.UDI * entry.ValuesByCurrency.LastExchangeRateUDI;

      dto.CurrentUSD = entry.ValuesByCurrency.USD * entry.ValuesByCurrency.ExchangeRateUSD;
      dto.CurrentYEN = entry.ValuesByCurrency.YEN * entry.ValuesByCurrency.ExchangeRateYEN;
      dto.CurrentEUR = entry.ValuesByCurrency.EUR * entry.ValuesByCurrency.ExchangeRateEUR;
      dto.CurrentUDI = entry.ValuesByCurrency.UDI * entry.ValuesByCurrency.ExchangeRateUDI;

      dto.ValuedEffectUSD = dto.LastUSD - dto.CurrentUSD;
      dto.ValuedEffectYEN = dto.LastYEN - dto.CurrentYEN;
      dto.ValuedEffectEUR = dto.LastEUR - dto.CurrentEUR;
      dto.ValuedEffectUDI = dto.LastUDI - dto.CurrentUDI;

      dto.TotalValued = dto.ValuedEffectUSD + dto.ValuedEffectYEN + dto.ValuedEffectEUR + dto.ValuedEffectUDI;
    }


    private static ColumnsByCurrency GetColumnsByCurrency(FixedList<ValorizacionEntry> entries) {

      var columns = new ColumnsByCurrency();

      columns.USD = entries.Sum(a => a.ValuesByCurrency.USD) > 0 ? true : false;
      columns.YEN = entries.Sum(a => a.ValuesByCurrency.YEN) > 0 ? true : false;
      columns.EUR = entries.Sum(a => a.ValuesByCurrency.EUR) > 0 ? true : false;
      columns.UDI = entries.Sum(a => a.ValuesByCurrency.UDI) > 0 ? true : false;

      return columns;
    }


    private class ColumnsByCurrency {
      
      public bool USD {
        get; internal set;
      }

      public bool YEN {
        get; internal set;
      }

      public bool EUR {
        get; internal set;
      }

      public bool UDI {
        get; internal set;
      }

    }


    #endregion Private methods



  } // class ValorizacionMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
