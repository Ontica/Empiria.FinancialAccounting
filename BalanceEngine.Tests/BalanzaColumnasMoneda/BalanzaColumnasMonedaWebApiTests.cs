/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Web Api tests                           *
*  Type     : BalanzaColumnasMonedaWebApiTests           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for 'Balanza en columnas por moneda' report web api calls.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;
using Empiria.WebApi.Client;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda {

  /// <summary>Test cases for 'Balanza en columnas por moneda' report web api calls.</summary>
  public class BalanzaColumnasMonedaWebApiTests {

    #region Initialization

    public BalanzaColumnasMonedaWebApiTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaColumnasMonedaTestCase.Default)]
    [InlineData(BalanzaColumnasMonedaTestCase.CatalogoAnterior)]
    public async Task WebServiceReturnsData(BalanzaColumnasMonedaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();


      var http = new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                                   TimeSpan.FromSeconds(TestingConstants.WEB_API_TIMEOUT_SECONDS));

      var sut = await http.PostAsync<ResponseModel<BalanzaColumnasMonedaDto>>(
                            query, "v2/financial-accounting/balance-engine/balanza-columnas-moneda")
                          .ConfigureAwait(false);

      Assert.NotNull(sut.Data);
      Assert.NotNull(sut.Data.Query);
      Assert.NotNull(sut.Data.Columns);
      Assert.NotNull(sut.Data.Entries);
    }

    #endregion Theories

  } // class BalanzaColumnasMonedaWebApiTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaColumnasMoneda
