/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanzaRealMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read Real valorize balance.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;
using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  internal class BalanzaRealMapper {

    #region Mappers

    static internal DynamicDto<BalanzaRealDto> Map(FixedList<BalanzaReal> entries) {
      return new DynamicDto<BalanzaRealDto>(MapColumns(), MapEntries(entries));
    }


    static public FixedList<DataTableColumn> MapColumns() {
      List<DataTableColumn> columns = new List<DataTableColumn>();
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));
      columns.Add(new DataTableColumn("domesticBalance", "M.N. (01)", "decimal"));
      columns.Add(new DataTableColumn("dollarBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("yenBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("euroBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("udisBalance", "UDIS (44)", "decimal"));
      columns.Add(new DataTableColumn("domesticRealBalance", "M.N. (01)", "decimal"));
      columns.Add(new DataTableColumn("dollarRealBalance", "Dólares (02)", "decimal"));
      columns.Add(new DataTableColumn("yenRealBalance", "Yenes (06)", "decimal"));
      columns.Add(new DataTableColumn("euroRealBalance", "Euros (27)", "decimal"));
      columns.Add(new DataTableColumn("udisRealBalance", "UDIS (44)", "decimal"));

      return columns.ToFixedList();
    }


    static private FixedList<BalanzaRealDto> MapEntries(FixedList<BalanzaReal> entries) {
      return entries.Select(x => Map(x))
                    .ToFixedList();
    }


    static private BalanzaRealDto Map(BalanzaReal entry) {
      BalanzaRealDto balanzRealDto = new BalanzaRealDto();

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

  } // class BalanzaValorizadaRealMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
