/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : IntegracionSaldosCapitalInteresesEntry     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for 'Integración de saldos e intereses' report.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses {

  /// <summary>Represents an entry for 'Integración de saldos e intereses' report.</summary>
  public class IntegracionSaldosCapitalInteresesEntry {

    public string ItemType {
      get; internal set;
    } = "Entry";

    public string SubledgerAccount {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountName {
      get; internal set;
    } = string.Empty;


    public string CurrencyCode {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public PrestamosInterbancariosListItem Prestamo {
      get; internal set;
    } = new PrestamosInterbancariosListItem();


    public decimal CapitalCortoPlazoMonedaOrigen {
      get; internal set;
    }

    public decimal CapitalLargoPlazoMonedaOrigen {
      get; internal set;
    }

    public decimal CapitalMonedaOrigen {
      get {
        return CapitalCortoPlazoMonedaOrigen + CapitalLargoPlazoMonedaOrigen;
      }
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

    internal void Sum(IntegracionSaldosCapitalInteresesEntry entry) {
      CapitalCortoPlazoMonedaOrigen += entry.CapitalCortoPlazoMonedaOrigen;
      CapitalLargoPlazoMonedaOrigen += entry.CapitalLargoPlazoMonedaOrigen;
      InteresesMonedaOrigen += entry.InteresesMonedaOrigen;
    }

  }  // class IntegracionSaldosCapitalInteresesEntry

}  // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura
