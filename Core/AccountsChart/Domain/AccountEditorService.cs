/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountEditorService                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides account edition services.                                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Provides account edition services.</summary>
  internal class AccountEditorService {

    private readonly AccountsChart _accountsChart;
    private readonly AccountEditionCommand _command;

    internal AccountEditorService(AccountsChart accountsChart, AccountEditionCommand command) {
      Assertion.AssertObject(accountsChart, nameof(accountsChart));
      Assertion.AssertObject(_command, nameof(command));

      _accountsChart = accountsChart;
      _command = command;
    }


    internal void AddCurrencies() {
      throw new NotImplementedException();
    }

    internal void AddSectors() {
      throw new NotImplementedException();
    }

    internal void CreateAccount() {
      throw new NotImplementedException();
    }

    internal void RemoveAccount() {
      throw new NotImplementedException();
    }

    internal void RemoveCurrencies() {
      throw new NotImplementedException();
    }

    internal void RemoveSectors() {
      throw new NotImplementedException();
    }

    internal void UpdateAccount() {
      throw new NotImplementedException();
    }

    internal void TryCommit() {
      throw new NotImplementedException();
    }


    internal AccountEditionResult GetResult() {
      throw new NotImplementedException();
    }

  }  // AccountEditorService

}  // namespace Empiria.FinancialAccounting.UseCases
