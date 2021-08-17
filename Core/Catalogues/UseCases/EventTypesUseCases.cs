/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues Management                      Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : EventTypesUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for event types.                                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for functional areas.</summary>
  public class EventTypesUseCases : UseCase {

    #region Constructors and parsers

    protected EventTypesUseCases() {
      // no-op
    }

    static public EventTypesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<EventTypesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> EventTypes() {
      var list = EventType.GetList();

      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }

    #endregion Use cases

  }  // class EventTypesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
