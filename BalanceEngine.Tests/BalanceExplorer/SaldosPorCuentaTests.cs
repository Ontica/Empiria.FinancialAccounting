/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : SaldosPorAuxiliarTests                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for balance explorer 'Saldos por cuenta' report entries.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Threading.Tasks;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.Tests;
using Xunit;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanceExplorer {

  /// <summary>Unit test cases for balance explorer 'Saldos por cuenta' report entries.</summary>
  public class SaldosPorCuentaTests {

    #region Initialization

    public SaldosPorCuentaTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories


    [Fact]
    public async Task Should_Build_Saldos_Por_Cuenta() {

      var explorerTest = new BalanceExplorerTests();

      BalanceExplorerQuery query = explorerTest.GetDefaultBalanceExplorerQuery();
      query.TrialBalanceType = TrialBalanceType.SaldosPorCuentaConsultaRapida;
      query.WithSubledgerAccount = true;
      query.FromAccount = "1.05.01.03.01.01.03";
      query.Accounts = new string[] { "1.05.01.03.01.01.03", "1.05.01.02.02.01.01" };//, "1.05.01.01.06.01"
      //"1.05.01.01.06.01", "1.05.01.01.06.02", "1.05.01.02.02.01.01", "1.05.01.02.02.01.02", "1.05.01.03.02.03.01"
      BalanceExplorerDto sut = await BalanceEngineProxy.BuildBalanceExplorer(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);
    }


    [Fact]
    public async Task Sum_Entries_Must_Be_Same_Total() {

      var explorerTest = new BalanceExplorerTests();

      BalanceExplorerQuery query = explorerTest.GetDefaultBalanceExplorerQuery();

      BalanceExplorerDto sut = await BalanceEngineProxy.BuildBalanceExplorer(query);

      var groups = sut.Entries.FindAll(x=>x.ItemType == TrialBalanceItemType.Group);

      foreach (var group in groups) {

        var sumEntries = sut.Entries.FindAll(x => x.CurrencyCode == group.CurrencyCode &&
                                             x.AccountNumberForBalances == group.AccountNumber &&
                                             x.ItemType == TrialBalanceItemType.Entry)
                                    .Sum(x => x.CurrentBalance);

        Assert.Equal(sumEntries, group.CurrentBalance);
      }

    }


    #endregion Theories

  }
}
