/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Use case interactor class               *
*  Type     : ReconciliationTypesUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for financial accounts balances reconciliation types.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation.UseCases {

  /// <summary>Use cases for financial accounts balances reconciliation types.</summary>
  public class ReconciliationTypesUseCases : UseCase {

    #region Constructors and parsers

    protected ReconciliationTypesUseCases() {
      // no-op
    }


    static public ReconciliationTypesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ReconciliationTypesUseCases>();
    }



    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> GetReconciliationTypes() {
      FixedList<ReconciliationType> list = ReconciliationType.GetList();

      return list.MapToNamedEntityList();
    }


    #endregion Use cases

  } // class ReconciliationTypesUseCases

} // Empiria.FinancialAccounting.Reconciliation.UseCases
