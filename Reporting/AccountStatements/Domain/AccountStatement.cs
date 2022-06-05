/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Information Holder                      *
*  Type     : AccountStatement                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an account statement.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Represents an account statement.</summary>
  public class AccountStatement {

    #region Constructors and parsers

    internal AccountStatement(BalanceExplorerQuery query,
                              FixedList<IVouchersByAccountEntry> entries,
                              string title) {
      Assertion.Require(query,    nameof(query));
      Assertion.Require(entries,  nameof(entries));

      Query = query;
      Entries = entries;
      Title   = title;
    }


    #endregion

    #region Properties

    public BalanceExplorerQuery Query {
      get;
    }

    public FixedList<IVouchersByAccountEntry> Entries {
      get;
    }


    public string Title {
      get;
    }


    #endregion

  } // class AccountStatement

} // namespace Empiria.FinancialAccounting.Reporting
