/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Exchange Rates                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : CurrenciesUseCases                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for currencies.                                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Services;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for exchange rates.</summary>
  public class CurrenciesUseCases : UseCase {

    #region Constructors and parsers

    protected CurrenciesUseCases() {
      // no-op
    }

    static public CurrenciesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CurrenciesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> GetCurrencies() {
      FixedList<Currency> currencies = Currency.GetList();

      return currencies.MapToNamedEntityList();
    }

    #endregion Use cases

  }  // class CurrenciesUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
