/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanzaValorizadaRealUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read Real valorize balance.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  internal class BalanzaValorizadaRealMapper {

    #region Mappers

    internal static FixedList<BalanzaValorizadaRealDto> Map(FixedList<BalanzaValorizadaReal> balance) {
      return balance.Select(x => Map(x))
                     .ToFixedList();
    }

    static internal BalanzaValorizadaRealDto Map(BalanzaValorizadaReal entry) {
      return new BalanzaValorizadaRealDto {
        StandarAccount = entry.CuentaEstandar.Number,
        AccountName = entry.CuentaEstandar.Name,
        CurrencyCode = entry.Moneda.Code.Trim(),
        CurrencyName = entry.Moneda.Name,
        InitialBalance = entry.SaldoInicial,
        Credit = entry.Haber,
        Debit = entry.Debe,
        RealCurrencyCode = entry.MonedaReal.Code.Trim(),
        RealCurrencyName = entry.MonedaReal.Name,
        RealInitialBalance = entry.SaldoInicialReal,
        RealCredit = entry.HaberMonedaReal,
        RealDebit = entry.DebeMonedaReal
      };
    }

    #endregion Mappers

  } // class BalanzaValorizadaRealMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters