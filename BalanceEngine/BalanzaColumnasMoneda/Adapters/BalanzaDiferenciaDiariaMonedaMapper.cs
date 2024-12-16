/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaDiferenciaDiariaMonedaMapper        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balanza diferencia diaria por moneda.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaMapper {


    #region Public methods

    static internal BalanzaDiferenciaDiariaMonedaDto Map(TrialBalanceQuery query,
                                                 FixedList<BalanzaDiferenciaDiariaMonedaEntry> entries) {
      return new BalanzaDiferenciaDiariaMonedaDto {
        Query = query,
        Columns = DataColumns(),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }

    #endregion Public methods


    #region Private methods

    static public FixedList<DataTableColumn> DataColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("toDate", "Fecha", "date"));
      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));

      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("dollarDailyBalance", "Diferencia diaria (Dólares)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForDollar", "Tipo cambio (Dólares)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedDollarBalance", "Saldo valorizado (Dólares)", "decimal"));

      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("yenDailyBalance", "Diferencia diaria (Yenes)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForYen", "Tipo cambio (Yenes)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedYenBalance", "Saldo valorizado (Yenes)", "decimal"));

      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("euroDailyBalance", "Diferencia diaria (Euros)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForEuro", "Tipo cambio (Euros)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedEuroBalance", "Saldo valorizado (Euros)", "decimal"));

      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));
      columns.Add(new DataTableColumn("udisDailyBalance", "Diferencia diaria (UDIS)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForUdi", "Tipo cambio (UDIS)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedUdisBalance", "Saldo valorizado (UDIS)", "decimal"));

      return columns.ToFixedList();
    }


    static public BalanzaDiferenciaDiariaMonedaEntryDto MapEntry(BalanzaDiferenciaDiariaMonedaEntry entry) {

      return new BalanzaDiferenciaDiariaMonedaEntryDto() {
        AccountNumber = entry.Account.Number,
        DebtorCreditor = entry.Account.DebtorCreditor,
        SectorCode = "00",

        FromDate = entry.FromDate,
        ToDate = entry.ToDate,
        DomesticBalance = entry.DomesticBalance,
        DomesticDailyBalance = entry.DomesticDailyBalance,

        DollarBalance = entry.DollarBalance,
        DollarDailyBalance = entry.DollarDailyBalance,
        ExchangeRateForDollar = entry.ExchangeRateForDollar,
        ValorizedDollarBalance = entry.ValorizedDollarBalance,

        YenBalance = entry.YenBalance,
        YenDailyBalance = entry.YenDailyBalance,
        ExchangeRateForYen = entry.ExchangeRateForYen,
        ValorizedYenBalance = entry.ValorizedYenBalance,

        EuroBalance = entry.EuroBalance,
        EuroDailyBalance = entry.EuroDailyBalance,
        ExchangeRateForEuro = entry.ExchangeRateForEuro,
        ValorizedEuroBalance = entry.ValorizedEuroBalance,

        UdisBalance = entry.UdisBalance,
        UdisDailyBalance = entry.UdisDailyBalance,
        ExchangeRateForUdi = entry.ExchangeRateForUdi,
        ValorizedUdisBalance = entry.ValorizedUdisBalance
      };
    }

    #endregion Private methods


  } // class BalanzaDiferenciaDiariaMonedaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
