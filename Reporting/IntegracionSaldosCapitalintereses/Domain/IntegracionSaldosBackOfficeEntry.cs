/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : IntegracionSaldosBackOfficeEntry           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for 'Integración de saldos e intereses' backoffice sub report.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses {

  public enum BackOfficeRow {

    None = 0,

    CallMoneyMXN = 1,

    CallMoneyUSD = 2,

    SubastasMXN = 3,

    PrestamosCortoPlazoUSD = 4,

    EfectoValuacionDerivados = 5,

    Subtotal = 6,

    Total = 7
  }

  static public class BackOfficeRowExtensions {

    static public string DisplayName(this BackOfficeRow row) {
      switch (row) {

        case BackOfficeRow.None:
          return "No proporcionado";

        case BackOfficeRow.CallMoneyMXN:
          return "Call Money MXN";

        case BackOfficeRow.CallMoneyUSD:
          return "Call Money USD";

        case BackOfficeRow.SubastasMXN:
          return "Subastas";

        case BackOfficeRow.PrestamosCortoPlazoUSD:
          return "Préstamos a CP USD";

        case BackOfficeRow.EfectoValuacionDerivados:
          return "Efecto de valuación de derivados (A)";

        case BackOfficeRow.Subtotal:
          return string.Empty;

        case BackOfficeRow.Total:
          return "SUBTOTAL REGISTROS BACK OFFICE";

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled backoffice row '{row}'.");

      }
    }
  }


  public class IntegracionSaldosBackOfficeTotal : IIntegracionSaldosCapitalInteresesEntry {

    public string ItemType {
      get; internal set;
    } = "Total";

    public BackOfficeRow BackOfficeEntry {
      get; internal set;
    } = BackOfficeRow.None;


    public PrestamoBaseClasificacion Classification {
      get {
        return PrestamoBaseClasificacion.RegistroBackOffice;
      }
    }

    public decimal CapitalMonedaNacional {
      get; internal set;
    }

    public decimal InteresesMonedaNacional {
      get; internal set;
    }

    public decimal TotalMonedaNacional {
      get {
        return CapitalMonedaNacional + InteresesMonedaNacional;
      }
    }

    internal void Sum(IntegracionSaldosBackOfficeEntry entry) {
      CapitalMonedaNacional += entry.CapitalMonedaNacional;
      InteresesMonedaNacional += entry.InteresesMonedaNacional;
    }

  }


  /// <summary>Represents an entry for 'Integración de saldos e intereses' backoffice sub report.</summary>
  public class IntegracionSaldosBackOfficeEntry : IIntegracionSaldosCapitalInteresesEntry {

    public string ItemType {
      get; internal set;
    } = "Entry";

    public BackOfficeRow BackOfficeEntry {
      get; internal set;
    } = BackOfficeRow.None;


    public PrestamoBaseClasificacion Classification {
      get {
        return PrestamoBaseClasificacion.RegistroBackOffice;
      }
    }

    public string CurrencyCode {
      get; internal set;
    }

    public decimal CapitalMonedaOrigen {
      get; internal set;
    }

    public decimal InteresesMonedaOrigen {
      get; internal set;
    }

    public decimal TotalMonedaOrigen {
      get {
        return CapitalMonedaOrigen + InteresesMonedaOrigen;
      }
    }

    public decimal TipoCambio {
      get; internal set;
    }

    public decimal CapitalMonedaNacional {
      get {
        return CapitalMonedaOrigen * TipoCambio;
      }
    }

    public decimal InteresesMonedaNacional {
      get {
        return InteresesMonedaOrigen * TipoCambio;
      }
    }

    public decimal TotalMonedaNacional {
      get {
        return CapitalMonedaNacional + InteresesMonedaNacional;
      }
    }

    internal void Sum(IntegracionSaldosBackOfficeEntry entry) {
      CapitalMonedaOrigen += entry.CapitalMonedaOrigen;
      InteresesMonedaOrigen += entry.InteresesMonedaOrigen;
    }

  }  // class IntegracionSaldosBackOfficeEntry

}  // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura
