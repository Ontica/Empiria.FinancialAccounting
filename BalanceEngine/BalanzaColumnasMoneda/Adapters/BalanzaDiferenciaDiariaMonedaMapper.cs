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
        Columns = DataColumnsV2(),
        Entries = entries.Select(x => MapEntry(x))
                         .ToFixedList()
      };
    }

    #endregion Public methods


    #region Private methods

    static public FixedList<DataTableColumn> DataColumnsV1() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("toDate", "Fecha", "date"));
      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));

      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("dollarDailyBalance", "Diferencia diaria (Dólares)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForDollar", "Tipo cambio (Dólares)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedDollarBalance", "Saldo valorizado (Dólares)", "decimal"));
      columns.Add(new DataTableColumn("closingExchangeRateForDollar", "Tipo cambio cierre (Dólares)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedDailyDollarBalance", "Valorización diaria (Dólares)", "decimal"));

      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("yenDailyBalance", "Diferencia diaria (Yenes)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForYen", "Tipo cambio (Yenes)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedYenBalance", "Saldo valorizado (Yenes)", "decimal"));
      columns.Add(new DataTableColumn("closingExchangeRateForYen", "Tipo cambio cierre (Yenes)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedDailyYenBalance", "Valorización diaria (Yenes)", "decimal"));

      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("euroDailyBalance", "Diferencia diaria (Euros)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForEuro", "Tipo cambio (Euros)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedEuroBalance", "Saldo valorizado (Euros)", "decimal"));
      columns.Add(new DataTableColumn("closingExchangeRateForEuro", "Tipo cambio cierre (Euros)", "decimal", 6));
      columns.Add(new DataTableColumn("ValorizedDailyEuroBalance", "Valorización diaria (Euros)", "decimal"));

      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));
      columns.Add(new DataTableColumn("udisDailyBalance", "Diferencia diaria (UDIS)", "decimal"));
      columns.Add(new DataTableColumn("exchangeRateForUdi", "Tipo cambio (UDIS)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedUdisBalance", "Saldo valorizado (UDIS)", "decimal"));
      columns.Add(new DataTableColumn("closingExchangeRateForUdi", "Tipo cambio cierre (UDIS)", "decimal", 6));
      columns.Add(new DataTableColumn("valorizedDailyUdisBalance", "Valorización diaria (UDIS)", "decimal"));

      return columns.ToFixedList();
    }


    static public FixedList<DataTableColumn> DataColumnsV2() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre cuenta", "text-nowrap"));

      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));
      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));

      columns.Add(new DataTableColumn("toDate", "Fecha", "date"));
      columns.Add(new DataTableColumn("itemTypeName", "Rol", "text"));
      columns.Add(new DataTableColumn("accountType", "Tipo de cuenta", "text"));

      columns.Add(new DataTableColumn("exchangeRateForDollar", "Tipo cambio (Dólares)", "decimal", 6));
      columns.Add(new DataTableColumn("exchangeRateForYen", "Tipo cambio (Yenes)", "decimal", 6));
      columns.Add(new DataTableColumn("exchangeRateForEuro", "Tipo cambio (Euros)", "decimal", 6));
      columns.Add(new DataTableColumn("exchangeRateForUdi", "Tipo cambio (UDIS)", "decimal", 6));

      columns.Add(new DataTableColumn("closingExchangeRateForDollar", "Tipo cambio cierre (Dólares)", "decimal", 6));
      columns.Add(new DataTableColumn("closingExchangeRateForYen", "Tipo cambio cierre (Yenes)", "decimal", 6));
      columns.Add(new DataTableColumn("closingExchangeRateForEuro", "Tipo cambio cierre (Euros)", "decimal", 6));
      columns.Add(new DataTableColumn("closingExchangeRateForUdi", "Tipo cambio cierre (UDIS)", "decimal", 6));

      columns.Add(new DataTableColumn("yenDailyBalance", "Diferencia diaria (Yenes)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyYenBalance", "Valorización diaria (Yenes)", "decimal"));

      columns.Add(new DataTableColumn("dollarDailyBalance", "Diferencia diaria (Dólares)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyDollarBalance", "Valorización diaria (Dólares)", "decimal"));

      columns.Add(new DataTableColumn("euroDailyBalance", "Diferencia diaria (Euros)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyEuroBalance", "Valorización diaria (Euros)", "decimal"));

      columns.Add(new DataTableColumn("udisDailyBalance", "Diferencia diaria (UDIS)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyUdisBalance", "Valorización diaria (UDIS)", "decimal"));

      columns.Add(new DataTableColumn("eRI", "ERI", "text"));
      columns.Add(new DataTableColumn("complementDescription", "Complemento", "text"));
      columns.Add(new DataTableColumn("complementDetail", "Complemento Detallado", "text"));
      columns.Add(new DataTableColumn("accountLevel", "Cta Nivel", "text"));
      columns.Add(new DataTableColumn("categoryType", "Tipo Rubro", "text"));

      columns.Add(new DataTableColumn("siglasUSD", "USD Cta", "text"));
      columns.Add(new DataTableColumn("siglasYEN", "YENES Cta", "text"));
      columns.Add(new DataTableColumn("siglasEURO", "EUROS Cta", "text"));
      columns.Add(new DataTableColumn("siglasUDI", "UDIS Cta", "text"));

      columns.Add(new DataTableColumn("valorizedDailyDollarBalanceNeg", "Valorización USD (Neg)", "decimal"));
      columns.Add(new DataTableColumn("dollarBalanceNeg", "Dólares (02)(Neg)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyYenBalanceNeg", "Valorización YENES (Neg)", "decimal"));
      columns.Add(new DataTableColumn("yenBalanceNeg", "Yenes (06)(Neg)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyEuroBalanceNeg", "Valorización EUROS (Neg)", "decimal"));
      columns.Add(new DataTableColumn("euroBalanceNeg", "Euros (27)(Neg)", "decimal"));
      columns.Add(new DataTableColumn("valorizedDailyUdisBalanceNeg", "Valorización UDIS (44)(Neg)", "decimal"));
      columns.Add(new DataTableColumn("udisBalanceNeg", "UDIS (44)(Neg)", "decimal"));

      //columns.Add(new DataTableColumn("valorizedDollarBalance", "Saldo valorizado (Dólares)", "decimal"));
      //columns.Add(new DataTableColumn("valorizedYenBalance", "Saldo valorizado (Yenes)", "decimal"));
      //columns.Add(new DataTableColumn("valorizedEuroBalance", "Saldo valorizado (Euros)", "decimal"));
      //columns.Add(new DataTableColumn("valorizedUdisBalance", "Saldo valorizado (UDIS)", "decimal"));

      return columns.ToFixedList();
    }


    static public BalanzaDiferenciaDiariaMonedaEntryDto MapEntry(BalanzaDiferenciaDiariaMonedaEntry entry) {

      var dto = new BalanzaDiferenciaDiariaMonedaEntryDto();

      dto.ItemType = entry.ItemType;
      dto.ItemTypeName = GetItemTypeName(entry.ItemType);
      dto.AccountType = entry.Account.AccountType.Name;

      dto.ERI = string.Empty;
      dto.ComplementDescription = string.Empty;
      dto.ComplementDetail = string.Empty;

      dto.CategoryType = string.Empty;

      dto.AccountNumber = entry.Account.Number;
      dto.AccountName = entry.Account.Name;
      dto.DebtorCreditor = entry.Account.DebtorCreditor;
      dto.SectorCode = "00";

      dto.FromDate = entry.FromDate;
      dto.ToDate = entry.ToDate;
      dto.DomesticBalance = entry.DomesticBalance;
      dto.DomesticDailyBalance = entry.DomesticDailyBalance;

      dto.DollarBalance = entry.DollarBalance;
      dto.DollarDailyBalance = entry.DollarDailyBalance;
      dto.ExchangeRateForDollar = entry.ExchangeRateForDollar;
      dto.ValorizedDollarBalance = entry.ValorizedDollarBalance;
      dto.ClosingExchangeRateForDollar = entry.ClosingExchangeRateForDollar;
      dto.ValorizedDailyDollarBalance = entry.ValorizedDailyDollarBalance;

      dto.YenBalance = entry.YenBalance;
      dto.YenDailyBalance = entry.YenDailyBalance;
      dto.ExchangeRateForYen = entry.ExchangeRateForYen;
      dto.ValorizedYenBalance = entry.ValorizedYenBalance;
      dto.ClosingExchangeRateForYen = entry.ClosingExchangeRateForYen;
      dto.ValorizedDailyYenBalance = entry.ValorizedDailyYenBalance;

      dto.EuroBalance = entry.EuroBalance;
      dto.EuroDailyBalance = entry.EuroDailyBalance;
      dto.ExchangeRateForEuro = entry.ExchangeRateForEuro;
      dto.ValorizedEuroBalance = entry.ValorizedEuroBalance;
      dto.ClosingExchangeRateForEuro = entry.ClosingExchangeRateForEuro;
      dto.ValorizedDailyEuroBalance = entry.ValorizedDailyEuroBalance;

      dto.UdisBalance = entry.UdisBalance;
      dto.UdisDailyBalance = entry.UdisDailyBalance;
      dto.ExchangeRateForUdi = entry.ExchangeRateForUdi;
      dto.ValorizedUdisBalance = entry.ValorizedUdisBalance;
      dto.ClosingExchangeRateForUdi = entry.ClosingExchangeRateForUdi;
      dto.ValorizedDailyUdisBalance = entry.ValorizedDailyUdisBalance;

      MapToAccountLevel(dto, entry);
      MapToSiglas(dto, entry);
      MapToValorizedDailyAndBalanceNeg(dto, entry);

      return dto;
    }


    static private string GetItemTypeName(TrialBalanceItemType itemType) {
      
      if (itemType == TrialBalanceItemType.Entry) {

        return "Registro";
      } else if (itemType == TrialBalanceItemType.Summary) {

        return "Sumaria";
      } else {

        return string.Empty;
      }
    }


    private static void MapToAccountLevel(BalanzaDiferenciaDiariaMonedaEntryDto dto,
                                          BalanzaDiferenciaDiariaMonedaEntry entry) {
      if (entry.ToDate.Year >= 2022 && entry.Account.Level >= 2) {

        dto.AccountLevel = entry.Account.Number.Substring(0, 4);
      } else if (entry.ToDate.Year >= 2022 && entry.Account.Level == 1) {

        dto.AccountLevel = entry.Account.Number.Substring(0, 1);
      } else {

        dto.AccountLevel = string.Empty;
      }
    }


    private static void MapToSiglas(BalanzaDiferenciaDiariaMonedaEntryDto dto,
                                    BalanzaDiferenciaDiariaMonedaEntry entry) {

      dto.SiglasUSD = entry.DollarDailyBalance != 0 ||
                      entry.ValorizedDollarBalance != 0 ||
                      entry.DollarBalance != 0 ? 
                      "USD" : "0";

      dto.SiglasYEN = entry.YenDailyBalance != 0 ||
                      entry.ValorizedYenBalance != 0 ||
                      entry.YenBalance != 0 ?
                      "YENES" : "0";

      dto.SiglasEURO = entry.EuroDailyBalance != 0 ||
                      entry.ValorizedEuroBalance != 0 ||
                      entry.EuroBalance != 0 ?
                      "EUROS" : "0";

      dto.SiglasUDI = entry.UdisDailyBalance != 0 ||
                      entry.ValorizedUdisBalance != 0 ||
                      entry.UdisBalance != 0 ?
                      "UDIS" : "0";
    }


    private static void MapToValorizedDailyAndBalanceNeg(BalanzaDiferenciaDiariaMonedaEntryDto dto,
                                                         BalanzaDiferenciaDiariaMonedaEntry entry) {

      if (entry.Account.DebtorCreditor == DebtorCreditorType.Deudora) {

        dto.ValorizedDailyDollarBalanceNeg = entry.ValorizedDailyDollarBalance;
        dto.DollarBalanceNeg = entry.DollarBalance;

        dto.ValorizedDailyYenBalanceNeg = entry.ValorizedDailyYenBalance;
        dto.YenBalanceNeg = entry.YenBalance;

        dto.ValorizedDailyEuroBalanceNeg = entry.ValorizedDailyEuroBalance;
        dto.EuroBalanceNeg = entry.EuroBalance;

        dto.ValorizedDailyUdisBalanceNeg = entry.ValorizedDailyUdisBalance;
        dto.UdisBalanceNeg = entry.UdisBalance;

      } else if(entry.Account.DebtorCreditor == DebtorCreditorType.Acreedora) {

        dto.ValorizedDailyDollarBalanceNeg = entry.ValorizedDailyDollarBalance * (-1);
        dto.DollarBalanceNeg = entry.DollarBalance * (-1);

        dto.ValorizedDailyYenBalanceNeg = entry.ValorizedDailyYenBalance * (-1);
        dto.YenBalanceNeg = entry.YenBalance * (-1);

        dto.ValorizedDailyEuroBalanceNeg = entry.ValorizedDailyEuroBalance * (-1);
        dto.EuroBalanceNeg = entry.EuroBalance * (-1);

        dto.ValorizedDailyUdisBalanceNeg = entry.ValorizedDailyUdisBalance * (-1);
        dto.UdisBalanceNeg = entry.UdisBalance * (-1);
      }
    }

    #endregion Private methods


  } // class BalanzaDiferenciaDiariaMonedaMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
