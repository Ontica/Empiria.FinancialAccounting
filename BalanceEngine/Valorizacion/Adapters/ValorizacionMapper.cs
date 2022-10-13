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

      columns.Add(new DataTableColumn("initialBalance", "Saldo inicial", "text-nowrap"));

      //columns.Add(new DataTableColumn("mesAnterior", "Mes anterior", "text-nowrap"));
      columns.Add(new DataTableColumn("currentBalance", "Mes actual", "text-nowrap"));
      columns.Add(new DataTableColumn("effectVal", "Efecto de valorización", "text-nowrap"));

      columns.Add(new DataTableColumn("ValuedEffects", "Efecto de valorización", "text-nowrap"));
      columns.Add(new DataTableColumn("TotalValued", "TOTAL", "text-nowrap"));

      //columns.Add(new DataTableColumn("meses", "Meses", "text-nowrap"));
      columns.Add(new DataTableColumn("totalBalance", "ACUMULADO", "text-nowrap"));

      return columns.ToFixedList();
    }


    public static ValorizacionEntryDto MapEntry(ValorizacionEntry entry, TrialBalanceQuery query) {

      var dto = new ValorizacionEntryDto() {

        ItemType = entry.ItemType,

        StandardAccountId = entry.Account.Id,
        AccountName = entry.Account.Name,
        AccountNumber = entry.Account.Number,
        SectorCode = entry.Sector.Code,
        CurrencyCode = entry.Currency.Code,

        InitialBalance = entry.InitialBalance,
        CurrentBalance = entry.CurrentBalance,
        ValuedEffects = entry.ValuedEffects,
        TotalValued = entry.TotalValued,
        TotalBalance = entry.TotalBalance,

        DollarBalance = entry.DollarBalance,
        EuroBalance = entry.EuroBalance,
        YenBalance = entry.YenBalance,
        UdisBalance = entry.UdisBalance,
        
        ExchangeRate = entry.ExchangeRate,
        ValuedExchangeRate = entry.ValuedExchangeRate

      };

      return dto;
    }


    #endregion Public methods


  } // class ValorizacionMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
