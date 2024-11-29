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
      columns.Add(new DataTableColumn("domesticDailyBalance", "Diferencia M.N. (01)", "decimal"));

      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("dollarDailyBalance", "Diferencia Dólares (02)", "decimal"));

      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("yenDailyBalance", "Diferencia Yenes (06)", "decimal"));

      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("euroDailyBalance", "Euros (27)", "decimal"));

      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));
      columns.Add(new DataTableColumn("udisDailyBalance", "UDIS (44)", "decimal"));

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

        YenBalance = entry.YenBalance,
        YenDailyBalance = entry.YenDailyBalance,

        EuroBalance = entry.EuroBalance,
        EuroDailyBalance = entry.EuroDailyBalance,

        UdisBalance = entry.UdisBalance,
        UdisDailyBalance = entry.UdisDailyBalance,
      };
    }

    #endregion Private methods


  } // class BalanzaDiferenciaDiariaMonedaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
