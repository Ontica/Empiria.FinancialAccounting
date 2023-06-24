/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : SaldosPorCuentaTests                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for Balance explorer report entries.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases;
using Empiria.Tests;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanceExplorer {

  /// <summary>Unit test cases for 'Saldos por cuenta' report entries.</summary>
  public class BalanceExplorerTests {

    private readonly EmpiriaHashTable<BalanceExplorerDto> _cache =
                                        new EmpiriaHashTable<BalanceExplorerDto>(16);

    #region Initialization

    public BalanceExplorerTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories


    
    #endregion Theories

    #region Helpers

    public BalanceExplorerQuery GetDefaultBalanceExplorerQuery() {
      
      return new BalanceExplorerQuery {
        TrialBalanceType = TrialBalanceType.SaldosPorCuentaConsultaRapida,
        AccountsChartUID = "47ec2ec7-0f4f-482e-9799-c23107b60d8a",
        FromAccount = "1",
        WithAllAccounts = false,
        WithSubledgerAccount = false,
        Ledgers = new string[] { },
        InitialPeriod = {
          FromDate = new DateTime(2022, 01, 01),
          ToDate = new DateTime(2022, 01, 31)
        }
      };

    }

    private async Task<FixedList<BalanceExplorerEntryDto>> GetBalanceExplorerEntries(BalanceExplorerTestCase testcase) {
      BalanceExplorerQuery query = testcase.GetInvocationQuery();

      BalanceExplorerDto dto = TryReadBalanceExplorerFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildBalanceExplorer(query)
                                    .ConfigureAwait(false);

      StoreBalanceExplorerIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreBalanceExplorerIntoCache(BalanceExplorerQuery query,
                                                  BalanceExplorerDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private BalanceExplorerDto TryReadBalanceExplorerFromCache(BalanceExplorerQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }


    #endregion Helpers


  } // class SaldosPorCuentaTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanceExplorer
