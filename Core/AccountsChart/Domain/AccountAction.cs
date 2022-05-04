/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Immutable Type                          *
*  Type     : AccountAction                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Immutable type that describes an account edition action.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.FinancialAccounting {

  internal class AccountAction {

    #region Constructors

    internal AccountAction(DataOperation operation, string description) {
      Assertion.AssertObject(operation, nameof(operation));
      Assertion.AssertObject(description, nameof(description));

      this.DataOperations = new FixedList<DataOperation>(new[] { operation });
      this.Description = description;
    }


    internal AccountAction(IEnumerable<DataOperation> operations, string description) {
      Assertion.AssertObject(operations, nameof(operations));
      Assertion.AssertObject(description, nameof(description));

      this.DataOperations = new FixedList<DataOperation>(operations);
      this.Description = description;
    }

    #endregion Constructors

    #region Properties


    public FixedList<DataOperation> DataOperations {
      get;
    }


    public string Description {
      get;
    }

    #endregion Properties

  }  // class AccountAction

}  // namespace Empiria.FinancialAccounting
