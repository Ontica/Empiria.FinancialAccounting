/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : SubsidiaryLedgerUseCases                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for subsidiary ledgers management.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for subsidiary ledgers management.</summary>
  public class SubsidiaryLedgerUseCases : UseCase {

    #region Constructors and parsers

    protected SubsidiaryLedgerUseCases() {
      // no-op
    }

    static public SubsidiaryLedgerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<SubsidiaryLedgerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public SubsidiaryLedgerDto GetSubsidiaryLedger(string subsidiaryLedgerUID) {
      Assertion.AssertObject(subsidiaryLedgerUID, "subsidiaryLedgerUID");

      var subsidiaryLedger = SubsidiaryLedger.Parse(subsidiaryLedgerUID);

      return SubsidiaryLedgerMapper.Map(subsidiaryLedger);
    }


    public SubsidiaryAccountDto GetSubsidiaryAccount(string subsidiaryLedgerUID,
                                                     int subsidiaryAccountId) {
      Assertion.AssertObject(subsidiaryLedgerUID, "subsidiaryLedgerUID");
      Assertion.Assert(subsidiaryAccountId > 0, "subsidiaryAccountId");

      var subsidiaryLedger = SubsidiaryLedger.Parse(subsidiaryLedgerUID);

      SubsidiaryAccount subsidiaryAccount = subsidiaryLedger.GetAccountWithId(subsidiaryAccountId);

      return SubsidiaryLedgerMapper.MapAccount(subsidiaryAccount);
    }


    public FixedList<SubsidiaryAccountDto> SearchSubsidiaryAccounts(string keywords) {
      Assertion.AssertObject(keywords, "keywords");

      FixedList<SubsidiaryAccount> subledgerAccountList = SubsidiaryAccount.GetList(keywords);

      return SubsidiaryLedgerMapper.Map(subledgerAccountList);
    }


    #endregion Use cases

  }  // class SubsidiaryLedgerUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
