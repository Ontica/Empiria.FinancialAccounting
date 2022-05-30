/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : Balances                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of a balance                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of a balance.</summary>
  public class Balances {

    #region Constructors and parsers

    internal Balances(BalancesQuery query, FixedList<BalanceEntry> entries) {
      Assertion.Require(query, nameof(query));
      Assertion.Require(entries, nameof(entries));

      Query = query;
      Entries = entries;
    }


    #endregion

    #region Properties

    public BalancesQuery Query {
      get;
    }

    public FixedList<BalanceEntry> Entries {
      get;
    }


    #endregion

  } // class Balances

} // Empiria.FinancialAccounting.BalanceEngine
