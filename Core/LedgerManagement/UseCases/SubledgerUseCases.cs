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


    public SubledgerAccountDto CreateSubledgerAccount(SubledgerAccountFields fields) {
      Assertion.AssertObject(fields, "fields");

      var ledger = Ledger.Parse(fields.LedgerUID);

      fields.Number = ledger.FormatSubledgerAccount(fields.Number);
      fields.Name = EmpiriaString.TrimAll(fields.Name);


      var sla = ledger.TryGetSubledgerAccount(fields.Number);

      if (sla != null) {
        Assertion.AssertFail("El auxiliar ya existe.");
      }


      SubledgerAccount createdSubledgerAccount = ledger.CreateSubledgerAccount(fields.Number,
                                                                               fields.SubledgerType(),
                                                                               fields.Name);

      createdSubledgerAccount.Save();

      return SubledgerMapper.Map(createdSubledgerAccount);

      //var subledger = Subledger.Parse(fields.SubledgerUID);

      //fields.Number = subledger.FormatSubledgerAccount(fields.Number);
      //fields.Name = EmpiriaString.TrimAll(fields.Name);

      //var sla = subledger.BaseLedger.TryGetSubledgerAccount(fields.Number);

      //if (sla != null) {
      //  Assertion.AssertFail("El auxiliar ya existe.");
      //}


      //SubledgerAccount subledgerAccount = subledger.CreateAccount(fields);

      //subledgerAccount.Save();

      // return SubledgerMapper.Map(subledgerAccount);
    }


    public SubledgerDto GetSubledger(string subledgerUID) {
      Assertion.AssertObject(subledgerUID, "subledgerUID");

      var subledger = Subledger.Parse(subledgerUID);

      return SubledgerMapper.Map(subledger);
    }


    public SubledgerAccountDto GetSubledgerAccount(int subledgerAccountId) {
      Assertion.Assert(subledgerAccountId > 0, "subledgerAccountId");

      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      return SubledgerMapper.Map(subledgerAccount);
    }


    public FixedList<SubledgerAccountDescriptorDto> SearchSubledgerAccounts(SearchSubledgerAccountCommand command) {
      Assertion.AssertObject(command, "command");

      string filter = command.BuildFilter();

      FixedList<SubledgerAccount> subledgerAccounts = SubledgerAccount.Search(command.AccountsChart(), filter);

      return SubledgerMapper.MapToSubledgerAccountDescriptor(subledgerAccounts);
    }


    public SubledgerAccountDto UpdateSubledgerAccount(int subledgerAccountId,
                                                      SubledgerAccountFields fields) {
      Assertion.Assert(subledgerAccountId > 0, "subledgerAccountId");
      Assertion.AssertObject(fields, "fields");

      var subledgerAccount = SubledgerAccount.Parse(subledgerAccountId);

      subledgerAccount.Update(fields.Number, fields.Name);

      subledgerAccount.Save();

      return SubledgerMapper.Map(subledgerAccount);
    }

    #endregion Use cases

  }  // class SubledgerUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
