/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Web Api tests                           *
*  Type     : BalanzaTradicionalWebApiTests              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Test cases for 'Balanza Tradicional' report web api calls.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Xunit;

using Empiria.Tests;
using Empiria.WebApi.Client;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional {

  /// <summary>Test cases for 'Balanza Tradicional' report web api calls.</summary>
  public class BalanzaTradicionalWebApiTests {

    #region Initialization

    public BalanzaTradicionalWebApiTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaTradicionalTestCase.Default)]
    [InlineData(BalanzaTradicionalTestCase.EnCascada)]
    public async Task WebServiceReturnsData(BalanzaTradicionalTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();


      var http = new HttpApiClient(TestingConstants.WEB_API_BASE_ADDRESS,
                                   TimeSpan.FromSeconds(TestingConstants.WEB_API_TIMEOUT_SECONDS));

      var sut = await http.PostAsync<ResponseModel<BalanzaTradicionalDto>>(query, "v2/financial-accounting/balance-engine/balanza-tradicional")
                          .ConfigureAwait(false);

      Assert.NotNull(sut.Data);
      Assert.NotNull(sut.Data.Query);
      Assert.NotNull(sut.Data.Columns);
      Assert.NotNull(sut.Data.Entries);
    }

    #endregion Theories

  } // class BalanzaTradicionalWebApiTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaTradicional
