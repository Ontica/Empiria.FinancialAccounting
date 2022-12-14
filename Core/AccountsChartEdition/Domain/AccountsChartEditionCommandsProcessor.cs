/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountsChartEditionCommandsProcessor      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Processes a set of chart of accounts edition commands.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Processes a set of chart of accounts edition commands.</summary>
  internal class AccountsChartEditionCommandsProcessor {

    public AccountsChartEditionCommandsProcessor() {

    }

    internal void Execute(FixedList<AccountEditionCommand> commands) {
      // no-op
    }

  }  // class AccountsChartEditionCommandsProcessor

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
