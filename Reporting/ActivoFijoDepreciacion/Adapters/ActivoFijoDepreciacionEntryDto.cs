/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : ActivoFijoDepreciacionEntryDto                License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return 'Activo fijo depreciación' report data.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.ActivoFijoDepreciacion.Adapters {

  /// <summary>Output DTO used to return 'Activo fijo depreciación' report data.</summary>
  public class ActivoFijoDepreciacionEntryDto : IReportEntryDto {

    public string ItemType {
      get; internal set;
    } = "Entry";

    public string NumContabilidad {
      get; internal set;
    }

    public string NombreContabilidad {
      get; internal set;
    }

    public string AuxiliarHistorico {
      get; internal set;
    }

    public string NumeroInventario {
      get; internal set;
    }

    public string TipoActivoFijoName {
      get; internal set;
    }

    public string NombreAuxiliar {
      get; internal set;
    }

    public DateTime FechaAdquisicion {
      get; internal set;
    }

    public DateTime FechaInicioDepreciacion {
      get; internal set;
    }

    public DateTime FechaTerminoDepreciacion {
      get; internal set;
    }

    public int MesesDepreciacion {
      get; internal set;
    }

    public decimal ValorHistorico {
      get; internal set;
    }

    public decimal DepreciacionMensual {
      get; internal set;
    }

    public int MesesTranscurridos {
      get; internal set;
    }

    public decimal DepreciacionAcumulada {
      get; internal set;
    }

    public decimal DepreciacionAcumuladaRegistradaContablemente {
      get; internal set;
    }

    public decimal DepreciacionPendienteRegistrar {
      get; internal set;
    }

    public string AuxiliarRevaluacion {
      get; internal set;
    }

    public decimal MontoRevaluacion {
      get; internal set;
    }

    public decimal DepreciacionDeLaRevaluacionMensual {
      get; internal set;
    }

    public decimal DepreciacionAcumuladaDeLaRevaluacion {
      get; internal set;
    }

    public decimal DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente {
      get; internal set;
    }

    public decimal DepreciacionPendienteRegistrarDeLaRevaluacion {
      get; internal set;
    }

    public decimal ValorHistoricoEnLibros {
      get; internal set;
    }

  } // class ActivoFijoDepreciacionEntryDto

} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters
