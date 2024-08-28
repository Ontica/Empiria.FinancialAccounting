/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : SaldosPorAuxiliarTests                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for balance explorer 'Saldos por auxiliar' report entries.                     *
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

  /// <summary>Unit test cases for balance explorer 'Saldos por auxiliar' report entries.</summary>
  public class SaldosPorAuxiliarTests {

    #region Initialization

    public SaldosPorAuxiliarTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories


    [Fact]
    public async Task Should_Build_Saldos_Por_Auxiliar() {

      var explorerTest = new BalanceExplorerTests();

      BalanceExplorerQuery query = explorerTest.GetDefaultBalanceExplorerQuery();
      query.TrialBalanceType = TrialBalanceType.SaldosPorAuxiliarConsultaRapida;
      //query.SubledgerAccountID = 292736;
      query.SubledgerAccounts = new string[] {"LA VIA ONTICA", "SOFTEK", "EL PERRO AGUAYO"};//90000000000439543
      query.WithSubledgerAccount = true;

      BalanceExplorerDto sut = await BalanceEngineProxy.BuildBalanceExplorer(query);

      Assert.NotNull(sut);
      Assert.Equal(query, sut.Query);
      Assert.NotEmpty(sut.Entries);

    }


    [Fact]
    public async Task Sum_Entries_Must_Be_Same_Total() {

      var explorerTest = new BalanceExplorerTests();

      BalanceExplorerQuery query = explorerTest.GetDefaultBalanceExplorerQuery();
      query.TrialBalanceType = TrialBalanceType.SaldosPorAuxiliarConsultaRapida;
      query.SubledgerAccount = "0000000001010636";

      BalanceExplorerDto sut = await BalanceEngineProxy.BuildBalanceExplorer(query);

      var headers = sut.Entries.FindAll(x => x.ItemType == TrialBalanceItemType.Summary);

      foreach (var header in headers) {

        var sumEntries = sut.Entries.FindAll(x => x.CurrencyCode == header.CurrencyCode &&
                                             x.SubledgerAccountNumber == header.SubledgerAccountNumber &&
                                             x.ItemType == TrialBalanceItemType.Entry)
                                    .Sum(x => x.CurrentBalance);

        Assert.Equal(sumEntries, header.CurrentBalance);
      }

    }


    #endregion Theories

  } // class SaldosPorAuxiliarTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanceExplorer
