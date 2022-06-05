/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : BalanceExplorerResult                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds result data for a balance explorer query.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer {

  /// <summary>Holds result data for a balance explorer query.</summary>
  public class BalanceExplorerResult {

    #region Constructors and parsers

    internal BalanceExplorerResult(BalanceExplorerQuery query, FixedList<BalanceExplorerEntry> entries) {
      Assertion.Require(query, nameof(query));
      Assertion.Require(entries, nameof(entries));

      Query = query;
      Entries = entries;
    }


    #endregion

    #region Properties

    public BalanceExplorerQuery Query {
      get;
    }

    public FixedList<BalanceExplorerEntry> Entries {
      get;
    }


    #endregion

  } // class BalanceExplorerResult

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
