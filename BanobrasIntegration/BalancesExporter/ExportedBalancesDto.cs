/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Balances Exporter                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : ExportedBalancesDto                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Data Transfer Object with balance information for other systems (Banobras).                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.Adapters {

  public class ExportedBalancesDto {

    public int Anio {
      get; internal set;
    }

    public int Mes {
      get; internal set;
    }

    public int Dia {
      get; internal set;
    }

    public string Area {
      get; internal set;
    }

    public int Moneda {
      get; internal set;
    }

    public string NumeroMayor {
      get; internal set;
    }

    public string Cuenta {
      get; internal set;
    }

    public string Sector {
      get; internal set;
    }

    public string Auxiliar {
      get; internal set;
    }

    public DateTime FechaUltimoMovimiento {
      get; internal set;
    }

    public decimal Saldo {
      get; internal set;
    }

    public int MonedaOrigen {
      get; internal set;
    }

    public int NaturalezaCuenta {
      get; internal set;
    }

    public decimal SaldoPromedio {
      get; internal set;
    }

    public decimal MontoDebito {
      get; internal set;
    }

    public decimal MontoCredito {
      get; internal set;
    }

    public decimal SaldoAnterior {
      get; internal set;
    }

    public int Empresa {
      get; internal set;
    }

    public string CalificaMoneda {
      get; internal set;
    }

  }  // class ExportedBalancesDto

} // namespace Empiria.FinancialAccounting.BanobrasIntegration.Adapters
