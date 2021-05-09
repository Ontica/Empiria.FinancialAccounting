/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Use cases tests                         *
*  Type     : LedgerUseCasesTests                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use case tests for accounting ledger book management.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Xunit;

using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Use case tests for accounting ledger book management.</summary>
  public class LedgerUseCasesTests {

    #region Use cases initialization

    private readonly LedgerUseCases _usecases;

    public LedgerUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = LedgerUseCases.UseCaseInteractor();
    }

    ~LedgerUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_A_Ledger() {
      LedgerDto ledger = _usecases.GetLedger(TestingConstants.LEDGER_UID);

      Assert.Equal(TestingConstants.LEDGER_UID, ledger.UID);
    }


    #endregion Facts

  }  // class LedgerUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
