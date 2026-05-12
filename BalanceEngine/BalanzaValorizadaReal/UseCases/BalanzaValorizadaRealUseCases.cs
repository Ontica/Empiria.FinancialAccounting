/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanzaValorizadaRealUseCases              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to read Real valorize balance.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.DynamicData;
using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to read Real valorize balance.</summary>
  public class BalanzaValorizadaRealUseCases : UseCase {

    #region Constructors and parsers

    protected BalanzaValorizadaRealUseCases() {
      // no-op
    }

    static public BalanzaValorizadaRealUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanzaValorizadaRealUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public DynamicDto<BalanzaValorizadaRealDto> BuildBalances(DateTime fromDate, DateTime toDate) {
      Assertion.Require(fromDate, nameof(fromDate));
      Assertion.Require(toDate, nameof(toDate));

      FixedList<BalanzaValorizadaReal> balances =
                            BalanzaValorizadaRealDataService.GetBalances(fromDate, toDate);

      return BalanzaValorizadaRealMapper.Map(balances);
    }

    #endregion

  } // class BalanzaValorizadaRealUseCases

} // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
