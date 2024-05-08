/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : BalanzaColumnasMonedaMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map balanza en columnas por moneda.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Methods used to map balanza en columnas por moneda.</summary>
  static internal class BalanzaColumnasMonedaMapper {


    #region Public methods

    static internal BalanzaColumnasMonedaDto Map(TrialBalanceQuery query,
                                                 FixedList<BalanzaColumnasMonedaEntry> entries) {
      return new BalanzaColumnasMonedaDto {
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
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));
      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));
      columns.Add(new DataTableColumn("totalValorized", "Total", "decimal"));

      return columns.ToFixedList();
    }


    static public BalanzaColumnasMonedaEntryDto MapEntry(BalanzaColumnasMonedaEntry entry) {

      var dto = new BalanzaColumnasMonedaEntryDto();
      dto.ItemType = entry.ItemType;
      dto.CurrencyCode = entry.Currency.Code;
      dto.CurrencyName = entry.Currency.Name;
      dto.StandardAccountId = entry.Account.Id;
      dto.AccountNumber = entry.Account.Number;
      dto.DebtorCreditor = entry.Account.DebtorCreditor;
      dto.AccountNumberForBalances = entry.Account.Number;
      dto.AccountName = entry.Account.Name;
      dto.SectorCode = entry.Sector.Code;
      dto.DomesticBalance = entry.DomesticBalance;
      dto.DollarBalance = entry.DollarBalance;
      dto.YenBalance = entry.YenBalance;
      dto.EuroBalance = entry.EuroBalance;
      dto.UdisBalance = entry.UdisBalance;
      dto.GroupName = entry.GroupName;
      dto.GroupNumber = entry.GroupNumber;
      dto.TotalValorized = entry.TotalValorized;

      return dto;
    }


    #endregion Private methods


  }
}
