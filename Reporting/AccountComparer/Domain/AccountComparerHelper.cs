/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : AccountComparerHelper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build account comparer information.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain {

  /// <summary>Helper methods to build account comparer information.</summary>
  internal class AccountComparerHelper {

    private readonly ReportBuilderQuery _query;
    
    internal AccountComparerHelper(ReportBuilderQuery query) {
      Assertion.Require(query, nameof(query));

      _query = query;
    }


    #region Public methods

    #endregion Public methods

  } // class AccountComparerHelper

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Domain
