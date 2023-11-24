/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                                 Component : Interface adapters              *
*  Assembly : FinancialAccounting.Reporting.dll                  Pattern   : Data Transfer Object            *
*  Type     : IntegracionSaldosCapitalInteresesConsolidadoDto    License   : Please read LICENSE.txt file    *
*                                                                                                            *
*  Summary  : Output DTO used to return 'Integración de saldos de capital e intereses' report data.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters {

  public interface IIntegracionSaldosCapitalInteresesConsolidadoDto : IReportEntryDto {

  }

  public class IntegracionSaldosCapitalInteresesConsolidadoTitleDto : IIntegracionSaldosCapitalInteresesConsolidadoDto {

    public string ItemType {
      get; internal set;
    } = "Total";

    public string Title {
      get; set;
    } = string.Empty;

  }  // class IntegracionSaldosCapitalInteresesConsolidadoTitleDto


  /// <summary>DTO for each account report entry.</summary>
  public class IntegracionSaldosCapitalInteresesConsolidadoEntryDto : IIntegracionSaldosCapitalInteresesConsolidadoDto {

    public string ItemType {
      get; internal set;
    } = "Entry";


    public string Title {
      get; internal set;
    } = string.Empty;

    public string Banco {
      get; internal set;
    } = string.Empty;


    public string PrestamoID {
      get; internal set;
    } = string.Empty;


    public string PrestamoName {
      get; internal set;
    } = string.Empty;


    public string CurrencyName {
      get; internal set;
    } = string.Empty;


    public decimal CapitalCortoPlazoMonedaOrigen {
      get; internal set;
    }

    public decimal CapitalLargoPlazoMonedaOrigen {
      get; internal set;
    }

    public decimal CapitalMonedaOrigenTotal {
      get; internal set;
    }

    public decimal InteresesMonedaOrigenTotal {
      get; internal set;
    }

    public decimal TotalMonedaOrigen {
      get; internal set;
    }

    public decimal TipoCambio {
      get; internal set;
    }

    public decimal CapitalMonedaNacional {
      get; internal set;
    }

    public decimal InteresesMonedaNacional {
      get; internal set;
    }

    public decimal TotalMonedaNacional {
      get; internal set;
    }

  } // class IntegracionSaldosCapitalInteresesConsolidadoEntryDto



  public class IntegracionSaldosCapitalInteresesConsolidadoTotalDto : IIntegracionSaldosCapitalInteresesConsolidadoDto {

    public string ItemType {
      get; internal set;
    } = "Total";


    public string Title {
      get;
      internal set;
    } = string.Empty;

    public decimal CapitalMonedaNacional {
      get; internal set;
    }

    public decimal InteresesMonedaNacional {
      get; internal set;
    }

    public decimal TotalMonedaNacional {
      get; internal set;
    }

  }  // class IntegracionSaldosCapitalInteresesConsolidadoTotalDto

} // namespace Empiria.FinancialAccounting.Reporting.IntegracionSaldosCapitalIntereses.Adapters
