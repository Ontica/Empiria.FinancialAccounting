/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues Management                      Component : Test cases                              *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Use cases tests                         *
*  Type     : FunctionalAreasUseCasesTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for the functional areas catalogue.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Xunit;

using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Test cases for the functional areas catalogue.</summary>
  public class FunctionalAreasUseCasesTests {

    #region Use cases initialization

    private readonly FunctionalAreasUseCases _usecases;

    public FunctionalAreasUseCasesTests() {
      CommonMethods.Authenticate();

      _usecases = FunctionalAreasUseCases.UseCaseInteractor();
    }

    ~FunctionalAreasUseCasesTests() {
      _usecases.Dispose();
    }

    #endregion Use cases initialization


    #region Facts

    [Fact]
    public void Should_Get_The_List_Of_Functional_Areas() {
      FixedList<NamedEntityDto> list = _usecases.FunctionalAreas();

      Assert.NotEmpty(list);
    }

    #endregion Facts

  }  // class FunctionalAreasUseCasesTests

}  // namespace Empiria.FinancialAccounting.Tests
