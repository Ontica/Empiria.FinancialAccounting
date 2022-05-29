/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : SubledgerUseCases                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for subledgers and subledger accounts management.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for subledgers and subledger accounts management.</summary>
  public class SubledgerUseCases : UseCase {

    #region Constructors and parsers

    protected SubledgerUseCases() {
      // no-op
    }

    static public SubledgerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<SubledgerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public SubledgerAccountDto ActivateSubledgerAccount(int subledgerAccountId) {
      Assertion.Require(subledgerAccountId > 0, nameof(subledgerAccountId));

      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      subledgerAccount.Activate();

      subledgerAccount.Save();

      return SubledgerMapper.Map(subledgerAccount);
    }


    public SubledgerAccountDto CreateSubledgerAccount(SubledgerAccountFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var ledger = Ledger.Parse(fields.LedgerUID);

      fields.Number = ledger.FormatSubledgerAccount(fields.Number);
      fields.Name = EmpiriaString.TrimAll(fields.Name);

      var subledgerAccount = ledger.TryGetSubledgerAccount(fields.Number);

      Assertion.Require(subledgerAccount == null,
                        $"Ya existe un auxiliar con número {subledgerAccount.Number}, " +
                        $"y corresponde a {subledgerAccount.Name}.");

      SubledgerAccount createdSubledgerAccount = ledger.CreateSubledgerAccount(fields.Number,
                                                                               fields.SubledgerType(),
                                                                               fields.Name);
      createdSubledgerAccount.Save();

      return SubledgerMapper.Map(createdSubledgerAccount);
    }


    public SubledgerDto GetSubledger(string subledgerUID) {
      Assertion.Require(subledgerUID, nameof(subledgerUID));

      var subledger = Subledger.Parse(subledgerUID);

      return SubledgerMapper.Map(subledger);
    }


    public SubledgerAccountDto GetSubledgerAccount(int subledgerAccountId) {
      Assertion.Require(subledgerAccountId > 0, nameof(subledgerAccountId));

      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      return SubledgerMapper.Map(subledgerAccount);
    }


    public FixedList<SubledgerAccountDescriptorDto> SearchSubledgerAccounts(SubledgerAccountQuery query) {
      Assertion.Require(query, nameof(query));
      Assertion.Require(query.AccountsChartUID, "query.AccountsChartUID");

      var accountsChart = AccountsChart.Parse(query.AccountsChartUID);

      string filter = query.MapToFilterString();

      FixedList<SubledgerAccount> subledgerAccounts = SubledgerAccount.Search(accountsChart, filter);

      return SubledgerMapper.MapToSubledgerAccountDescriptor(subledgerAccounts);
    }


    public SubledgerAccountDto SuspendSubledgerAccount(int subledgerAccountId) {
      Assertion.Require(subledgerAccountId > 0, nameof(subledgerAccountId));

      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      subledgerAccount.Suspend();

      subledgerAccount.Save();

      return SubledgerMapper.Map(subledgerAccount);
    }


    public SubledgerAccountDto UpdateSubledgerAccount(int subledgerAccountId,
                                                      SubledgerAccountFields fields) {
      Assertion.Require(subledgerAccountId > 0, nameof(subledgerAccountId));
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      subledgerAccount.Update(fields);

      subledgerAccount.Save();

      return SubledgerMapper.Map(subledgerAccount);
    }


    #endregion Use cases

  }  // class SubledgerUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
