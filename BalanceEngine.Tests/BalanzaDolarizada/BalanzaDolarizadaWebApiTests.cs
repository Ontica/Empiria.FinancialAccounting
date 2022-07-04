/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Web Api tests                           *
*  Type     : BalanzaDolarizadaWebApiTests               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for 'Balanza dolarizada' report web api calls.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;
using Empiria.WebApi.Client;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaDolarizada {

  /// <summary>Test cases for 'Balanza dolarizada' report web api calls.</summary>
  public class BalanzaDolarizadaWebApiTests {

    #region Initialization

    public BalanzaDolarizadaWebApiTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaDolarizadaTestCase.Default)]
    [InlineData(BalanzaDolarizadaTestCase.CatalogoAnterior)]
    public async Task WebServiceReturnsData(BalanzaDolarizadaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();


      var http = new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                                   TimeSpan.FromSeconds(TestingConstants.WEB_API_TIMEOUT_SECONDS));

      var sut = await http.PostAsync<ResponseModel<BalanzaDolarizadaDto>>(
                            query, "v2/financial-accounting/balance-engine/balanza-dolarizada")
                          .ConfigureAwait(false);

      Assert.NotNull(sut.Data);
      Assert.NotNull(sut.Data.Query);
      Assert.NotNull(sut.Data.Columns);
      Assert.NotNull(sut.Data.Entries);
    }

    #endregion Theories

  } // class BalanzaDolarizadaWebApiTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaDolarizada
