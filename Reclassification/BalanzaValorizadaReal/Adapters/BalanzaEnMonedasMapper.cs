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

namespace Empiria.FinancialAccounting.Reclassification.Adapters {

  /// <summary>Maps reclassified trial balances to their respective currency representations in columns.</summary>
  internal class BalanzaEnMonedasMapper {

    #region Mappers

    static internal DynamicDto<BalanzaEnColumnasRealDto> Map(IQuery query, FixedList<BalanzaReal> entries) {
      return new DynamicDto<BalanzaEnColumnasRealDto>(query, MapColumns(), MapEntries(entries));
    }


    static public FixedList<DataTableColumn> MapColumns() {
      return new List<DataTableColumn> {
        new DataTableColumn("operationType", "Tipo de transacción", "text"),
        new DataTableColumn("accountNo", "Cuenta", "text-nowrap"),
        new DataTableColumn("accountName", "Nombre", "text"),
        new DataTableColumn("domesticRealFinalBalance", "MXN", "decimal"),
        new DataTableColumn("dollarRealFinalBalance", "USD", "decimal"),
        new DataTableColumn("yenRealFinalBalance", "JPY", "decimal"),
        new DataTableColumn("euroRealFinalBalance", "EUR", "decimal"),
        new DataTableColumn("udisRealFinalBalance", "UDIS", "decimal"),
        //new DataTableColumn("domesticFinalBalance", "MXN (Captura)", "decimal"),
        //new DataTableColumn("dollarFinalBalance", "USD (Capt)", "decimal"),
        //new DataTableColumn("yenFinalBalance", "JPY (Capt)", "decimal"),
        //new DataTableColumn("euroFinalBalance", "EUR (Capt)", "decimal"),
        //new DataTableColumn("udisFinalBalance", "UDIS (Capt)", "decimal")
      }.ToFixedList();
    }


    static private FixedList<BalanzaEnColumnasRealDto> MapEntries(FixedList<BalanzaReal> entries) {
      return entries.Select(x => Map(x))
                    .ToFixedList();
    }


    static private BalanzaEnColumnasRealDto Map(BalanzaReal entry) {
      BalanzaEnColumnasRealDto balanzRealDto = new BalanzaEnColumnasRealDto();

      balanzRealDto.OperationType = entry.OperationType.Name;
      balanzRealDto.AccountNo = entry.CuentaEstandar.Number;
      balanzRealDto.AccountName = entry.CuentaEstandar.Name;

      foreach (var saldosMonedaReal in entry.SaldosPorMoneda) {

        switch (saldosMonedaReal.Currency.ISOCode) {
          case "MXN":
            balanzRealDto.DomesticFinalBalance = saldosMonedaReal.FinalBalance;
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
            balanzRealDto.UdisFinalBalance = saldosMonedaReal.FinalBalance;
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

} // namespace Empiria.FinancialAccounting.Reclassification.Adapters
