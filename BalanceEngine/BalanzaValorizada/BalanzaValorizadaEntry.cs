/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : BalanzaValorizadaEntry                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Representa una entrada de la balanza valorizada.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Representa una entrada de la balanza valorizada.</summary>
  public class BalanzaValorizadaEntry {

    internal BalanzaValorizadaEntry(TrialBalanceEntry balance, DateTime date,
                                    decimal initialBalance,
                                    MovimientosPorDia dateEntry,
                                    ExchangeRate exchangeRate, ExchangeRate lastExchangeRate) {
      CuentaEstandar = balance.Account;
      Moneda = balance.Currency;
      FechaAfectacion = date;
      SaldoInicial = initialBalance;
      Cargos = dateEntry?.Cargos ?? 0;
      Abonos = dateEntry?.Abonos ?? 0;
      TipoCambio = exchangeRate.Value;
      TipoCambioAnterior = lastExchangeRate.Value;
    }

    public BalanzaValorizadaEntry(TrialBalanceEntry balance, DateTime date,
                                  decimal initialBalance, decimal cargos, decimal abonos) {
      CuentaEstandar = balance.Account;
      Moneda = balance.Currency;
      FechaAfectacion = date;
      SaldoInicial = initialBalance;
      Cargos = cargos;
      Abonos = abonos;
      TipoCambio = 1;
      TipoCambioAnterior = 1;
    }

    #region Properties

    internal StandardAccount CuentaEstandar {
      get; private set;
    }


    internal Currency Moneda {
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

    public DateTime FechaAfectacion {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DebtorCreditorType Naturaleza {
      get {
        return CuentaEstandar.DebtorCreditor;
      }
    }


    public string CodigoMoneda {
      get {
        return Moneda.ISOCode;
      }
    }


    public decimal SaldoInicial {
      get; private set;
    }


    public decimal Cargos {
      get; private set;
    }


    public decimal Abonos {
      get; private set;
    }


    public decimal SaldoFinal {
      get {
        if (Naturaleza == DebtorCreditorType.Deudora) {
          return SaldoInicial + Cargos - Abonos;
        } else {
          return SaldoInicial - Cargos + Abonos;
        }
      }
    }

    public decimal TipoCambioAnterior {
      get; private set;
    }


    public decimal TipoCambio {
      get; private set;
    }


    public decimal SaldoInicialMXN {
      get {
        return SaldoInicial * TipoCambio;
      }
    }

    public decimal CargosMXN {
      get {
        return Cargos * TipoCambio;
      }
    }

    public decimal AbonosMXN {
      get {
        return Abonos * TipoCambio;
      }
    }

    public decimal SaldoFinalMXN {
      get {
        return SaldoFinal * TipoCambio;
      }
    }

    public decimal UtilidadCambiaria {
      get {
        return (TipoCambio - TipoCambioAnterior) * SaldoInicial;
      }
    }

    #endregion Properties

  } // class BalanzaValorizadaEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
