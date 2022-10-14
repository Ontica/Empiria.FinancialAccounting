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

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Methods used to map valorized report.</summary>
  static internal class ValorizacionMapper {


    #region Public methods

    static internal ValorizacionDto Map(TrialBalanceQuery query,
                                             FixedList<ValorizacionEntry> entries) {

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

      columns.Add(new DataTableColumn("usd", "USD", "text-nowrap"));
      columns.Add(new DataTableColumn("yen", "YEN", "text-nowrap"));
      columns.Add(new DataTableColumn("eur", "EUR", "text-nowrap"));
      columns.Add(new DataTableColumn("udi", "UDI", "text-nowrap"));

      //columns.Add(new DataTableColumn("currentBalance", "Mes actual", "text-nowrap"));
      //columns.Add(new DataTableColumn("effectVal", "Efecto de valorización", "text-nowrap"));

      //columns.Add(new DataTableColumn("ValuedEffects", "Efecto de valorización", "text-nowrap"));
      columns.Add(new DataTableColumn("TotalValued", "TOTAL", "text-nowrap"));

      //columns.Add(new DataTableColumn("meses", "Meses", "text-nowrap"));
      columns.Add(new DataTableColumn("totalBalance", "ACUMULADO", "text-nowrap"));

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


      return dto;
    }


    #endregion Public methods


    #region Private methods


    static private void AssignValuesByCurrency(ValorizacionEntryDto dto, ValorizacionEntry entry) {
      
      dto.USD = entry.USD;
      dto.EUR = entry.EUR;
      dto.YEN = entry.YEN;
      dto.UDI = entry.UDI;

      dto.LastUSD = entry.USD * entry.LastExchangeRateUSD;
      dto.LastYEN = entry.YEN * entry.LastExchangeRateYEN;
      dto.LastEUR = entry.EUR * entry.LastExchangeRateEUR;
      dto.LastUDI = entry.UDI * entry.LastExchangeRateUDI;

      dto.CurrentUSD = entry.USD * entry.ExchangeRateUSD;
      dto.CurrentYEN = entry.YEN * entry.ExchangeRateYEN;
      dto.CurrentEUR = entry.EUR * entry.ExchangeRateEUR;
      dto.CurrentUDI = entry.UDI * entry.ExchangeRateUDI;

      dto.ValuedEffectUSD = dto.LastUSD - dto.CurrentUSD;
      dto.ValuedEffectYEN = dto.LastYEN - dto.CurrentYEN;
      dto.ValuedEffectEUR = dto.LastEUR - dto.CurrentEUR;
      dto.ValuedEffectUDI = dto.LastUDI - dto.CurrentUDI;

      dto.TotalValued = dto.ValuedEffectUSD + dto.ValuedEffectYEN + dto.ValuedEffectEUR + dto.ValuedEffectUDI;
    }


    #endregion Private methods



  } // class ValorizacionMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
