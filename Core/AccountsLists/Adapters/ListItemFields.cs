/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Use cases Layer                         *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Use case interactor class               *
*  Type     : AccountsListsUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounts lists.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

namespace Empiria.FinancialAccounting.AccountsLists.Adapters {

  public class ConciliacionDerivadosListItemFields {

    public string UID {
      get; set;
    } = string.Empty;


    public string AccountNumber {
      get; set;
    }

    public DateTime StartDate {
      get; set;
    }

    public DateTime EndDate {
      get; set;
    }

    internal void EnsureValid() {
      Assertion.Require(AccountNumber, "accountNumber");

      _ = AccountsChart.IFRS.GetAccount(this.AccountNumber);

      Assertion.Require(AccountsChart.IFRS.MasterData.StartDate <= StartDate &&
                        StartDate <= AccountsChart.IFRS.MasterData.EndDate,
                        "La fecha de inicio está fuera de rango.");

      Assertion.Require(AccountsChart.IFRS.MasterData.StartDate <= EndDate &&
                        EndDate <= AccountsChart.IFRS.MasterData.EndDate,
                        "La fecha de término está fuera de rango.");

      Assertion.Require(StartDate <= EndDate,
                        "La fecha de inicio debe ser anterior a la fecha de término.");

    }

  }  // class ConciliacionDerivadosListItemFields



  public class DepreciacionActivoFijoListItemFields {

    public string UID {
      get; set;
    }

    public string DelegacionUID {
      get; set;
    }

    public string AuxiliarHistorico {
      get; set;
    }

    public string TipoActivoFijoUID {
      get; set;
    }

    public DateTime FechaAdquisicion {
      get; set;
    } = DateTime.MinValue;


    public DateTime FechaInicioDepreciacion {
      get; set;
    } = DateTime.MinValue;


    public int MesesDepreciacion {
      get; set;
    }

    public string AuxiliarRevaluacion {
      get; set;
    } = string.Empty;


    public decimal MontoRevaluacion {
      get; set;
    }

    internal void EnsureValid() {
      Assertion.Require(DelegacionUID, "DelegacionUID");
      Assertion.Require(AuxiliarHistorico, "AuxiliarHistorico");
      Assertion.Require(TipoActivoFijoUID, "TipoActivoFijoUID");
      Assertion.Require(FechaAdquisicion != DateTime.MinValue, "FechaAdquisicion");
      Assertion.Require(FechaInicioDepreciacion != DateTime.MinValue, "FechaInicioDepreciacion");
      Assertion.Require(MesesDepreciacion > 0, "MesesDepreciacion");

      if (SubledgerAccount.TryParse(AccountsChart.IFRS, AuxiliarHistorico) == null) {
        Assertion.RequireFail($"El auxiliar {AuxiliarHistorico} no ha sido registrado.");
      }

      if (AuxiliarRevaluacion.Length != 0 && SubledgerAccount.TryParse(AccountsChart.IFRS, AuxiliarHistorico) == null) {
        Assertion.RequireFail($"El auxiliar de revaluación {AuxiliarRevaluacion} no ha sido registrado.");
      }

      if (AuxiliarRevaluacion.Length != 0 && MontoRevaluacion <= 0) {
        Assertion.RequireFail($"Se requiere el monto de revaluación del auxiliar {AuxiliarRevaluacion}.");
      }

      Assertion.Require(FechaAdquisicion <= FechaInicioDepreciacion,
                        "La fecha de adqusición debe ser anterior o igual a la fecha de inicio de depreciación.");

      Assertion.Require(DepreciacionActivoFijoList.Parse()
                                                  .TiposActivoFijo.Contains(x => x.UID == TipoActivoFijoUID),
                        $"No reconozco el tipo de activo fijo con UID '{TipoActivoFijoUID}'.");

    }

  }  // class DepreciacionActivoFijoListItemFields



  public class SwapsCoberturaListItemFields {

    public string UID {
      get; set;
    }

    public string SubledgerAccountNumber {
      get; set;
    }

    public string Classification {
      get; set;
    }

    public DateTime StartDate {
      get; set;
    }

    public DateTime EndDate {
      get; set;

    }

    internal void EnsureValid() {
      Assertion.Require(SubledgerAccountNumber, "subledgerAccountNumber");

      Assertion.Require(Classification, "classification");

      var classifications = SwapsCoberturaList.Parse().GetClassificationValues();

      Assertion.Require(classifications.Contains(Classification),
                        $"No reconozco la clasificación del auxiliar: '{Classification}'.");

      if (SubledgerAccount.TryParse(AccountsChart.IFRS, SubledgerAccountNumber) == null) {
        Assertion.RequireFail($"El auxiliar {SubledgerAccountNumber} no ha sido registrado.");
      }

      Assertion.Require(AccountsChart.IFRS.MasterData.StartDate <= StartDate &&
                        StartDate <= AccountsChart.IFRS.MasterData.EndDate,
                        "La fecha de inicio está fuera de rango.");

      Assertion.Require(AccountsChart.IFRS.MasterData.StartDate <= EndDate &&
                        EndDate <= AccountsChart.IFRS.MasterData.EndDate,
                        "La fecha de término está fuera de rango.");

      Assertion.Require(StartDate <= EndDate,
                        "La fecha de inicio debe ser anterior a la fecha de término.");

    }

  }  // class SwapsCoberturaListItemFields



  public class PrestamosInterbancariosListItemFields {

      public string UID {
        get; set;
      }

      public string SubledgerAccountNumber {
        get; set;
      }

      public string CurrencyCode {
        get; set;
      }

      public string SectorCode {
        get; set;
      }

      public string PrestamoUID {
        get; set;
      }

      public DateTime Vencimiento {
        get; set;
      }

      internal void EnsureValid() {
        Assertion.Require(SubledgerAccountNumber, "subledgerAccountNumber");
        Assertion.Require(CurrencyCode, "currencyCode");
        Assertion.Require(SectorCode, "sectorCode");
        Assertion.Require(PrestamoUID, "prestamoUID");

        var prestamos = PrestamosInterbancariosList.Parse().GetPrestamosBase();

        Assertion.Require(prestamos.Contains(x => x.UID == PrestamoUID),
                          $"No reconozco el préstamo: '{PrestamoUID}'.");

        if (SubledgerAccount.TryParse(AccountsChart.IFRS, SubledgerAccountNumber) == null) {
          Assertion.RequireFail($"El auxiliar {SubledgerAccountNumber} no ha sido registrado.");
        }

        Assertion.Require(Sector.TryParse(SectorCode), $"No reconozco el sector {SectorCode}");

        Assertion.Require(Currency.TryParse(CurrencyCode), $"No reconozco la moneda {CurrencyCode}");

      }

  }  // class PrestamosInterbancariosListItemFields

}  // namespace Empiria.FinancialAccounting.AccountsLists.Adapters
