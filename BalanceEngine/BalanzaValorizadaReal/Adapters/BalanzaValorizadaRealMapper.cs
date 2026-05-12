/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanzaValorizadaRealUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read Real valorize balance.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  internal class BalanzaValorizadaRealMapper {

    #region Mappers

    static internal DynamicDto<BalanzaValorizadaRealDto> Map(FixedList<BalanzaValorizadaReal> entries) {
      return new DynamicDto<BalanzaValorizadaRealDto>(MapColumns(), MapEntries(entries));
    }


    static private FixedList<DataTableColumn> MapColumns() {
      return new DataTableColumn[] {
          new DataTableColumn("accountNo", "Cuenta", "text"),
          new DataTableColumn("accountName", "Nombre de la cuenta", "text"),
          new DataTableColumn("currencyCode", "Moneda", "text"),
          new DataTableColumn("initialBalance", "Saldo inicial", "decimal"),
          new DataTableColumn("debits", "Cargos", "decimal"),
          new DataTableColumn("credits", "Abonos", "decimal"),
          new DataTableColumn("finalBalance", "Saldo final", "decimal"),
          new DataTableColumn("realCurrencyCode", "Mon Real", "text"),
          new DataTableColumn("realInitialBalance", "Saldo inicial real", "decimal"),
          new DataTableColumn("realDebits", "Cargos real", "decimal"),
          new DataTableColumn("realCredits", "Abonos real", "decimal"),
          new DataTableColumn("realFinalBalance", "Saldo final real", "decimal")
        }.ToFixedList();
    }


    static private FixedList<BalanzaValorizadaRealDto> MapEntries(FixedList<BalanzaValorizadaReal> entries) {
      return entries.Select(x => Map(x))
                    .ToFixedList();
    }


    static private BalanzaValorizadaRealDto Map(BalanzaValorizadaReal entry) {
      return new BalanzaValorizadaRealDto {
        AccountNo = entry.CuentaEstandar.Number,
        AccountName = entry.CuentaEstandar.Name,
        CurrencyCode = entry.Moneda.ISOCode,
        InitialBalance = entry.SaldoInicial,
        Credits = entry.Haber,
        Debits = entry.Debe,
        FinalBalance = entry.SaldoFinal,
        RealCurrencyCode = entry.MonedaReal.ISOCode,
        RealInitialBalance = entry.SaldoInicialReal,
        RealCredits = entry.HaberMonedaReal,
        RealDebits = entry.DebeMonedaReal,
        RealFinalBalance = entry.SaldoFinalReal
      };
    }

    #endregion Mappers

  } // class BalanzaValorizadaRealMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
