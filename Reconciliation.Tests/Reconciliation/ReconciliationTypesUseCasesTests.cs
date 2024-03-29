﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation services                      Component : Test cases                            *
*  Assembly : FinancialAccounting.Reconciliation.Tests     Pattern   : Use cases tests                       *
*  Type     : ReconciliationTypesUseCasesTests             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Test cases for voucher related data.                                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Xunit;

using Empiria.Tests;

using Empiria.FinancialAccounting.Reconciliation.UseCases;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Tests.Reconciliation {

  /// <summary>Test cases for vouchers related data.</summary>
  public class ReconciliationTypesUseCasesTests {

    #region Fields

    private readonly ReconciliationTypesUseCases _usecases;

    #endregion Fields

    #region Initialization

    public ReconciliationTypesUseCasesTests() {
      TestsCommonMethods.Authenticate();

      _usecases = ReconciliationTypesUseCases.UseCaseInteractor();
    }

    ~ReconciliationTypesUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Initialization

    #region Facts

    [Fact]
    public void Should_Read_Reconciliation_Types() {
      FixedList<ReconciliationTypeDto> reconciliationTypes = _usecases.GetReconciliationTypes();

      Assert.NotEmpty(reconciliationTypes);
    }

    #endregion Facts

  }  // class ReconciliationTypesUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests.Reconciliation
