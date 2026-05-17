/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : BalanzaValorizadaReal                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to read Real valorize balance.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Used to read Real valorize balance.</summary>
  public class BalanzaValorizadaReal {

    #region Constructors and Parsers

    internal BalanzaValorizadaReal() {
      // Required by Empiria Framework
    }

    #endregion Constructors and Parsers

    #region Properties

    [DataField("ID_CUENTA_ESTANDAR")]
    public StandardAccount CuentaEstandar {
      get; set;
    }

    [DataField("ID_MONEDA")]
    public Currency Moneda {
      get; set;
    }

    [DataField("ID_MONEDA_REAL")]
    public Currency MonedaReal {
      get; set;
    }

    [DataField("SALDO_INICIAL", ConvertFrom = typeof(decimal))]
    public decimal SaldoInicial {
      get; set;
    }

    [DataField("DEBE", ConvertFrom = typeof(decimal))]
    public decimal Debe {
      get; set;
    }

    [DataField("HABER", ConvertFrom = typeof(decimal))]
    public decimal Haber {
      get; set;
    }

    public decimal SaldoFinal {
      get {
        if (CuentaEstandar.DebtorCreditor == DebtorCreditorType.Deudora) {
          return SaldoInicial + Debe - Haber;
        } else {
          return SaldoInicial - Debe + Haber;
        }
      }
    }

    [DataField("SALDO_INICIAL_REAL", ConvertFrom = typeof(decimal))]
    public decimal SaldoInicialReal {
      get; set;
    }

    [DataField("DEBE_MONEDA_REAL", ConvertFrom = typeof(decimal))]
    public decimal DebeMonedaReal {
      get; set;
    }

    [DataField("HABER_MONEDA_REAL", ConvertFrom = typeof(decimal))]
    public decimal HaberMonedaReal {
      get; set;
    }

    public decimal SaldoFinalReal {
      get {
        if (CuentaEstandar.DebtorCreditor == DebtorCreditorType.Deudora) {
          return SaldoInicialReal + DebeMonedaReal - HaberMonedaReal;
        } else {
          return SaldoInicialReal - DebeMonedaReal + HaberMonedaReal;
        }
      }
    }

    #endregion Properties

  } // class BalanzaValorizadaReal

} // namespace Empiria.FinancialAccounting.BalanceEngine
