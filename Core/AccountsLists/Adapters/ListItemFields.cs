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

namespace Empiria.FinancialAccounting.AccountsLists.Adapters {

  public class ConciliacionDerivadosListItemFields {

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
                        "La fecha de inicio está fuera de rango");

      Assertion.Require(AccountsChart.IFRS.MasterData.StartDate <= EndDate &&
                        EndDate <= AccountsChart.IFRS.MasterData.EndDate,
                        "La fecha de término está fuera de rango");

      Assertion.Require(StartDate <= EndDate,
                        "La fecha de inicio debe ser anterior a la fecha de término");

    }

  }

  public class DepreciacionActivoFijoListItemFields {

  }

  public class SwapsCoberturaListItemFields {

  }

}
