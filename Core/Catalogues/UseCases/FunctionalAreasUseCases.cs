/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues Management                      Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : FunctionalAreasUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for functional areas.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for functional areas.</summary>
  public class FunctionalAreasUseCases : UseCase {

    #region Constructors and parsers

    protected FunctionalAreasUseCases() {
      // no-op
    }

    static public FunctionalAreasUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FunctionalAreasUseCases>();
    }


    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> FunctionalAreas() {
      var list = FunctionalArea.GetList();

      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }

    #endregion Use cases

  }  // class FunctionalAreasUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
