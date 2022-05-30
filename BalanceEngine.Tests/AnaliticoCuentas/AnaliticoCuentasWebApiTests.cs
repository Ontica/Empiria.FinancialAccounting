/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Web Api tests                           *
*  Type     : AnaliticoCuentasWebApiTests                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for 'Analitico de Cuentas' report web api calls.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;
using Empiria.WebApi.Client;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.AnaliticoCuentas {

  /// <summary>Test cases for 'Analitico de Cuentas' report web api calls.</summary>
  public class AnaliticoCuentasWebApiTests {

    #region Initialization

    public AnaliticoCuentasWebApiTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(AnaliticoDeCuentasTestCase.Default)]
    [InlineData(AnaliticoDeCuentasTestCase.EnCascada)]
    public async Task WebServiceReturnsData(AnaliticoDeCuentasTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();


      var http = new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                                   TimeSpan.FromSeconds(TestingConstants.WEB_API_TIMEOUT_SECONDS));

      var sut = await http.PostAsync<ResponseModel<AnaliticoDeCuentasDto>>(query, "v2/financial-accounting/balance-engine/analitico-de-cuentas")
                          .ConfigureAwait(false);

      Assert.NotNull(sut.Data);
      Assert.NotNull(sut.Data.Query);
      Assert.NotNull(sut.Data.Columns);
      Assert.NotNull(sut.Data.Entries);
    }

    #endregion Theories

  } // class AnaliticoCuentasWebApiTests

} // namespace Empiria.FinancialAccounting.Tests.Balances.AnaliticoCuentas
