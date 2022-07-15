/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Unit tests                              *
*  Type     : BalanzaContabilidadesCascadaEntriesTests   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Unit test cases for 'Balanza con contabilidades en cascada' report entries.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using Empiria.Collections;
using Empiria.Tests;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada {

  /// <summary>Unit test cases for 'Balanza con contabilidades en cascada' report entries.</summary>
  public class BalanzaContabilidadesCascadaEntriesTests {

    private readonly EmpiriaHashTable<BalanzaContabilidadesCascadaDto> _cache =
                                        new EmpiriaHashTable<BalanzaContabilidadesCascadaDto>(16);

    #region Initialization

    public BalanzaContabilidadesCascadaEntriesTests() {
      TestsCommonMethods.Authenticate();
    }

    #endregion Initialization

    #region Theories

    [Theory]
    [InlineData(BalanzaContabilidadesCascadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Default)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Valorizada)]
    public async Task TotalLedgersByCurrency_Must_Be_The_Sum_Of_Its_DebtorAccounts(
                                      BalanzaContabilidadesCascadaTestCase testcase) {

      FixedList<BalanzaContabilidadesCascadaEntryDto> sut = await GetBalanzaContabilidadesCascadaEntries(testcase)
                                                       .ConfigureAwait(false);

      var SumLedgersByCurrency = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)
                          .Distinct();

      foreach (var sumOfLedgers in SumLedgersByCurrency) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == sumOfLedgers.CurrencyCode &&
                                                 x.DebtorCreditor == sumOfLedgers.DebtorCreditor)
                                       .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) sumOfLedgers.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all debtor accounts for Sum of ledgers '{sumOfLedgers}' was {expected}, " +
                    $"but it is not equals to the debtor total entries value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaContabilidadesCascadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Default)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Valorizada)]
    public async Task TotalDebtors_Must_Be_The_Sum_Of_Its_DebtorAccounts(
                                      BalanzaContabilidadesCascadaTestCase testcase) {

      FixedList<BalanzaContabilidadesCascadaEntryDto> sut =
        await GetBalanzaContabilidadesCascadaEntries(testcase).ConfigureAwait(false);

      var totalByDebtors = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                          .Distinct();

      foreach (var totalDebtor in totalByDebtors) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == totalDebtor.CurrencyCode &&
                                                 x.DebtorCreditor == totalDebtor.DebtorCreditor)
                                       .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) totalDebtor.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all debtor accounts for Total debtor '{totalDebtor}' was {expected}, " +
                    $"but it is not equals to the debtor total entries value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaContabilidadesCascadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Default)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Valorizada)]
    public async Task TotalCreditors_Must_Be_The_Sum_Of_Its_CreditorAccounts(
                                      BalanzaContabilidadesCascadaTestCase testcase) {

      FixedList<BalanzaContabilidadesCascadaEntryDto> sut =
        await GetBalanzaContabilidadesCascadaEntries(testcase).ConfigureAwait(false);

      var totalByCreditors = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                          .Distinct();

      foreach (var totalCreditor in totalByCreditors) {

        decimal expected = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.Entry &&
                                                 !x.IsParentPostingEntry &&
                                                 x.CurrencyCode == totalCreditor.CurrencyCode &&
                                                 x.DebtorCreditor == totalCreditor.DebtorCreditor)
                                       .Sum(x => x.CurrentBalance);

        decimal actual = (decimal) totalCreditor.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum of all creditor accounts for Total creditor '{totalCreditor}' was {expected}, " +
                    $"but it is not equals to the creditor total entries value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaContabilidadesCascadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Default)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Valorizada)]
    public async Task TotalByCurrency_Must_Be_The_Sum_Of_TotalDebtors_Minus_TotalCreditors(
                                              BalanzaContabilidadesCascadaTestCase testcase) {

      FixedList<BalanzaContabilidadesCascadaEntryDto> sut =
        await GetBalanzaContabilidadesCascadaEntries(testcase).ConfigureAwait(false);

      var totalsByCurrency = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                          .Distinct();

      foreach (var totalByCurrency in totalsByCurrency) {

        decimal totalDebtor = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor &&
                                                 x.CurrencyCode == totalByCurrency.CurrencyCode)
                                        .Sum(x => x.CurrentBalance);

        decimal totalCreditor = (decimal) sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor &&
                                                 x.CurrencyCode == totalByCurrency.CurrencyCode)
                                        .Sum(x => x.CurrentBalance);

        decimal expected = totalDebtor - totalCreditor;

        decimal actual = (decimal) totalByCurrency.CurrentBalance;

        Assert.True(expected == actual,
                    $"The sum between total debtors minus total creditors for Total by currency '{totalByCurrency}' " +
                    $"was {expected}, but it is not equals to the Total by currency entry value {actual}.");
      }
    }


    [Theory]
    [InlineData(BalanzaContabilidadesCascadaTestCase.CatalogoAnterior)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Default)]
    [InlineData(BalanzaContabilidadesCascadaTestCase.Valorizada)]
    public async Task TotalReport_Must_Be_The_Sum_Of_Currencies(
                         BalanzaContabilidadesCascadaTestCase testcase) {
      
      FixedList<BalanzaContabilidadesCascadaEntryDto> sut =
        await GetBalanzaContabilidadesCascadaEntries(testcase).ConfigureAwait(false);

      var expected = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                                .Sum(x => x.CurrentBalance);

      var actual = sut.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalConsolidated)
                                .Sum(x => x.CurrentBalance);

      Assert.True(expected == actual,
                    $"The sum of Total by currency was '{expected}', " +
                    $"but it is not equals to the Total for report value {actual}.");
    }


    #endregion Theories

    #region Helpers

    private async Task<FixedList<BalanzaContabilidadesCascadaEntryDto>> GetBalanzaContabilidadesCascadaEntries(
                                                        BalanzaContabilidadesCascadaTestCase testcase) {
      TrialBalanceQuery query = testcase.GetInvocationQuery();

      BalanzaContabilidadesCascadaDto dto = TryReadBalanzaContabilidadesCascadaFromCache(query);

      if (dto != null) {
        return dto.Entries;
      }

      dto = await BalanceEngineProxy.BuildBalanzaContabilidadesCascada(query)
                                    .ConfigureAwait(false);

      StoreContabilidadesCascadaIntoCache(query, dto);

      return dto.Entries;
    }


    private void StoreContabilidadesCascadaIntoCache(TrialBalanceQuery query,
                                                  BalanzaContabilidadesCascadaDto dto) {
      string key = query.ToString();

      _cache.Insert(key, dto);
    }


    private BalanzaContabilidadesCascadaDto TryReadBalanzaContabilidadesCascadaFromCache(
                                            TrialBalanceQuery query) {
      string key = query.ToString();

      if (_cache.ContainsKey(key)) {
        return _cache[key];
      }

      return null;
    }

    #endregion Helpers

  } // class BalanzaContabilidadesCascadaEntriesTests

} // namespace Empiria.FinancialAccounting.Tests.BalanceEngine.BalanzaContabilidadesCascada
