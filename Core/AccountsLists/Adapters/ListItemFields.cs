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


    internal void EnsureValid() {
      Assertion.Require(DelegacionUID, "DelegacionUID");
      Assertion.Require(AuxiliarHistorico, "AuxiliarHistorico");
      Assertion.Require(FechaAdquisicion != DateTime.MinValue, "FechaAdquisicion");
      Assertion.Require(FechaInicioDepreciacion != DateTime.MinValue, "FechaInicioDepreciacion");
      Assertion.Require(MesesDepreciacion > 0, "MesesDepreciacion");

      if (SubledgerAccount.TryParse(AccountsChart.IFRS, AuxiliarHistorico) == null) {
        Assertion.RequireFail($"El auxiliar {AuxiliarHistorico} no ha sido registrado.");
      }

      if (AuxiliarRevaluacion.Length != 0 && SubledgerAccount.TryParse(AccountsChart.IFRS, AuxiliarHistorico) == null) {
        Assertion.RequireFail($"El auxiliar de revaluación {AuxiliarRevaluacion} no ha sido registrado.");
      }

      Assertion.Require(FechaAdquisicion <= FechaInicioDepreciacion,
                        "La fecha de adqusición debe ser anterior o igual a la fecha de inicio de depreciación.");
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

}  // namespace Empiria.FinancialAccounting.AccountsLists.Adapters
