/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : TrialBalanceValued                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains entries from TrialBalanceValued.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains entries from TrialBalanceValued.</summary>
  public class TrialBalanceValued {

    #region Properties

    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal StandardAccount CuentaEstandar {
      get; private set;
    }


    public string NumeroCuenta {
      get {
        return CuentaEstandar.Number;
      }
    }

    public string NombreCuenta {
      get {
        return CuentaEstandar.Name;
      }
    }

    [DataField("FECHA_AFECTACION", ConvertFrom = typeof(DateTime))]
    public DateTime FechaAfectacion {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DebtorCreditorType Naturaleza {
      get {
        return CuentaEstandar.DebtorCreditor;
      }
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    internal Currency Currency {
      get; private set;
    }


    public string CodigoMoneda {
      get {
        return Currency.ISOCode;
      }
    }

    public decimal SaldoInicial {
      get; private set;
    }

    [DataField("DEBE", ConvertFrom = typeof(decimal))]
    public decimal Debe {
      get; private set;
    }

    [DataField("HABER", ConvertFrom = typeof(decimal))]
    public decimal Haber {
      get; private set;
    }

    public decimal SaldoFinal {
      get {
        if (Naturaleza == DebtorCreditorType.Deudora) {
          return SaldoInicial + Debe - Haber;
        } else {
          return SaldoInicial - Debe + Haber;
        }
      }
    }

    public decimal TipoCambio {
      get; private set;
    }


    public decimal SaldoInicialMXN {
      get {
        return SaldoInicial * TipoCambio;
      }
    }

    public decimal DebeMXN {
      get {
        return Debe * TipoCambio;
      }
    }

    public decimal HaberMXN {
      get {
        return Haber * TipoCambio;
      }
    }

    public decimal SaldoFinalMXN {
      get {
        return SaldoFinal * TipoCambio;
      }
    }

    #endregion Properties

    #region Methods

    internal void SetExchangeRate(FixedList<ExchangeRate> exchangeRates,
                                  DateTime initialDate, DateTime endDate) {
      int lastDay = DateTime.DaysInMonth(FechaAfectacion.Year, FechaAfectacion.Month);

      if (FechaAfectacion.Day == lastDay) {
        var exchangeRate = exchangeRates.Find(x => x.Date == FechaAfectacion && x.ToCurrency.Equals(Currency) &&
                                                   x.ExchangeRateType.Equals(ExchangeRateType.ValorizacionBanxico));
        TipoCambio = exchangeRate.Value;
      } else {
        var exchangeRate = exchangeRates.Find(x => x.Date == FechaAfectacion &&
                                                   x.ToCurrency.Equals(Currency) &&
                                                   x.ExchangeRateType.Equals(ExchangeRateType.Diario));
        TipoCambio = exchangeRate.Value;
      }
    }

    #endregion Methods

  } // class TrialBalanceValued

} // namespace Empiria.FinancialAccounting.BalanceEngine
