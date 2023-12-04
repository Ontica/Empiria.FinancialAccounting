/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Fixed Assets Depreciation                         Component : Domain Layer                     *
*  Assembly : FinancialAccounting.FixedAssetsDepreciation.dll   Pattern   : Information Holder               *
*  Type     : FixedAssetsDepreciationEntry                      License   : Please read LICENSE.txt file     *
*                                                                                                            *
*  Summary  : Represents an entry for fixed assets depreciation.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.FixedAssetsDepreciation {

  /// <summary>Represents an entry for fixed assets depreciation.</summary>
  public class FixedAssetsDepreciationEntry {

    public FixedAssetsDepreciationEntry(Ledger ledger, SubledgerAccount auxiliarHistorico, DateTime fechaCalculo) {
      Ledger = ledger;
      AuxiliarHistorico = auxiliarHistorico;
      FechaCalculo = fechaCalculo;
    }

    public Ledger Ledger {
      get;
    }

    public SubledgerAccount AuxiliarHistorico {
      get;
    }

    public TipoActivoFijo TipoActivoFijo {
      get;
      private set;
    } = TipoActivoFijo.Empty;


    public DateTime FechaCalculo {
      get;
    }


    public DateTime FechaAdquisicion {
      get;
      private set;
    } = ExecutionServer.DateMinValue;


    public DateTime FechaInicioDepreciacion {
      get;
      private set;
    } = ExecutionServer.DateMinValue;


    public DateTime FechaTerminoDepreciacion {
      get {
        return FechaInicioDepreciacion.AddMonths(MesesDepreciacion);
      }
    }

    public int MesesDepreciacion {
      get;
      private set;
    }


    public SubledgerAccount AuxiliarRevaluacion {
      get;
      private set;
    } = SubledgerAccount.Empty;


    public string NumeroInventario {
      get;
      private set;
    }


    public decimal ValorHistorico {
      get;
      internal set;
    }


    public decimal DepreciacionMensual {
      get {
        if (MesesDepreciacion == 0) {
          return 0;
        }
        return ValorHistorico / MesesDepreciacion;
      }
    }


    public int MesesTranscurridos {
      get {
        if (FechaAdquisicion == ExecutionServer.DateMinValue) {
          return 0;
        }

        return (FechaCalculo.Year - FechaAdquisicion.Year) * 12 + FechaCalculo.Month - FechaAdquisicion.Month;
      }
    }


    public decimal DepreciacionAcumulada {
      get {
        return Math.Round(DepreciacionMensual * MesesTranscurridos, 2);
      }
    }


    public decimal DepreciacionAcumuladaRegistradaContablemente {
      get;
      internal set;
    }


    public decimal DepreciacionPendienteRegistrar {
      get {
        if (MesesDepreciacion == 0) {
          return 0;
        }
        return Math.Round(DepreciacionAcumulada - DepreciacionAcumuladaRegistradaContablemente, 2);
      }
    }


    public decimal MontoRevaluacion {
      get;
      private set;
    }


    public decimal DepreciacionDeLaRevaluacionMensual {
      get {
        if (MesesDepreciacion == 0) {
          return 0;
        }
        return MontoRevaluacion / MesesDepreciacion;
      }
    }


    public decimal DepreciacionAcumuladaDeLaRevaluacion {
      get {
        return Math.Round(DepreciacionDeLaRevaluacionMensual * MesesTranscurridos);
      }
    }


    public decimal DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente {
      get;
      internal set;
    }


    public decimal DepreciacionPendienteRegistrarDeLaRevaluacion {
      get {
        return Math.Round(DepreciacionAcumuladaDeLaRevaluacion - DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente, 2);
      }
    }


    public decimal ValorHistoricoEnLibros {
      get {
        return Math.Round(ValorHistorico - DepreciacionAcumuladaRegistradaContablemente +
                          MontoRevaluacion - DepreciacionAcumuladaDeLaRevaluacionRegistradaContablemente, 2);
      }
    }

    public bool Depreciado {
      get {
        return (ValorHistorico <= DepreciacionAcumuladaRegistradaContablemente);
      }
    }


    internal void SetValues(DepreciacionActivoFijoListItem activoFijoEntry) {
      NumeroInventario = activoFijoEntry.NumeroInventario;
      TipoActivoFijo = activoFijoEntry.TipoActivoFijo;
      FechaAdquisicion = activoFijoEntry.FechaAdquisicion;
      FechaInicioDepreciacion = activoFijoEntry.FechaInicioDepreciacion;
      MesesDepreciacion = activoFijoEntry.MesesDepreciacion;
      AuxiliarRevaluacion = activoFijoEntry.AuxiliarRevaluacion;
      MontoRevaluacion = activoFijoEntry.MontoRevaluacion;
    }

  }  // class FixedAssetsDepreciationEntry

}  // namespace Empiria.FinancialAccounting.FixedAssetsDepreciation
