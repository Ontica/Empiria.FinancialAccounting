/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information holder                      *
*  Type     : AccountsChartEditionAction                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a charts of accounts edition action, that associates an edition command with             *
*             the list of its corresponding data operations.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Holds a charts of accounts edition action, that associates an edition command with
  /// the list of its corresponding data operations.</summary>
  internal class AccountsChartEditionAction {

    internal AccountsChartEditionAction(AccountEditionCommand command,
                                        DataOperation dataOperation) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(dataOperation, nameof(dataOperation));

      this.Command = command;
      this.DataOperations = new FixedList<DataOperation>( new []{ dataOperation });
    }

    internal AccountsChartEditionAction(AccountEditionCommand command,
                                        IEnumerable<DataOperation> dataOperations) {
      Assertion.Require(command, nameof(command));
      Assertion.Require(dataOperations, nameof(dataOperations));

      this.Command = command;
      this.DataOperations = dataOperations.ToFixedList();
    }


    internal AccountEditionCommand Command {
      get;
    }


    internal FixedList<DataOperation> DataOperations {
      get;
    }

  }  // class AccountsChartEditionAction

}   // namespace Empiria.FinancialAccounting.AccountsChartEdition
