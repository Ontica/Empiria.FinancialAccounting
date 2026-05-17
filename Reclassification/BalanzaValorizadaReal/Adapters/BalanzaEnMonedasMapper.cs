/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Adapters Layer                          *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Mapper                                  *
*  Type     : BalanzaEnMonedasMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps reclassified trial balances to their respective currency representations in columns.      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Maps reclassified trial balances to their respective currency representations in columns.</summary>
  internal class BalanzaEnMonedasMapper {

    #region Mappers

    static internal DynamicDto<BalanzaEnColumnasRealDto> Map(FixedList<BalanzaReal> entries) {
      return new DynamicDto<BalanzaEnColumnasRealDto>(MapColumns(), MapEntries(entries));
    }


    static public FixedList<DataTableColumn> MapColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      new DataTableColumn("accountNumber", "Cuenta", "text-nowrap");
      new DataTableColumn("accountName", "Nombre", "text");
      new DataTableColumn("domesticRealBalance", "MXN (Rec)", "decimal");
      new DataTableColumn("dollarRealBalance", "USD", "decimal");
      new DataTableColumn("yenRealBalance", "JPY", "decimal");
      new DataTableColumn("euroRealBalance", "EUR", "decimal");
      new DataTableColumn("udisRealBalance", "UDIS", "decimal");
      new DataTableColumn("domesticBalance", "MXN", "decimal");
      new DataTableColumn("dollarBalance", "USD", "decimal");
      new DataTableColumn("yenBalance", "JPY", "decimal");
      new DataTableColumn("euroBalance", "EUR", "decimal");
      new DataTableColumn("udisBalance", "UDIS", "decimal");
      return columns.ToFixedList();
    }


    static private FixedList<BalanzaEnColumnasRealDto> MapEntries(FixedList<BalanzaReal> entries) {
      return entries.Select(x => Map(x))
                    .ToFixedList();
    }


    static private BalanzaEnColumnasRealDto Map(BalanzaReal entry) {
      BalanzaEnColumnasRealDto balanzRealDto = new BalanzaEnColumnasRealDto();

      balanzRealDto.AccountNo = entry.CuentaEstandar.Number;
      balanzRealDto.AccountName = entry.CuentaEstandar.Name;

      decimal totalMxn = 0;

      foreach (var saldosMonedaReal in entry.SaldosPorMoneda) {
        switch (saldosMonedaReal.Currency.ISOCode) {
          case "MXN":
            totalMxn += saldosMonedaReal.FinalBalance;
            balanzRealDto.DomesticFinalBalance = totalMxn;
            break;
          case "USD":
            balanzRealDto.DollarFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "JPY":
            balanzRealDto.YenFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "EUR":
            balanzRealDto.EuroFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "MXV":
          case "UDI":
            balanzRealDto.UdisRealFinalBalance = saldosMonedaReal.FinalBalance;
            break;
        }
      }

      foreach (var saldosMonedaReal in entry.SaldosPorMonedaReal) {
        switch (saldosMonedaReal.Currency.ISOCode) {
          case "MXN":
            balanzRealDto.DomesticRealFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "USD":
            balanzRealDto.DollarRealFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "JPY":
            balanzRealDto.YenRealFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "EUR":
            balanzRealDto.EuroRealFinalBalance = saldosMonedaReal.FinalBalance;
            break;
          case "MXV":
          case "UDI":
            balanzRealDto.UdisRealFinalBalance = saldosMonedaReal.FinalBalance;
            break;
        }
      }

      return balanzRealDto;
    }

    #endregion Mappers

  } // class BalanzaEnMonedasMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
