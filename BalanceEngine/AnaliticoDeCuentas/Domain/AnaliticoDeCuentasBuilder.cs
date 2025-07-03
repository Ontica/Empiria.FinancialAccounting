/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : AnaliticoDeCuentasBuilder                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte Analítico de Cuentas.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte Analítico de Cuentas.</summary>
  internal class AnaliticoDeCuentasBuilder {

    private readonly TrialBalanceQuery _query;

    internal AnaliticoDeCuentasBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal FixedList<AnaliticoDeCuentasEntry> Build() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      return Build(baseAccountEntries);
    }


    internal FixedList<AnaliticoDeCuentasEntry> Build(FixedList<TrialBalanceEntry> baseAccountEntries) {

      if (baseAccountEntries.Count == 0) {
        return new FixedList<AnaliticoDeCuentasEntry>();
      }

      FixedList<TrialBalanceEntry> saldosValorizados = SaldosDeCuentasValorizados(baseAccountEntries);

      List<TrialBalanceEntry> balanceEntries = SummariesAndAccountEntries(saldosValorizados);

      var helper = new AnaliticoDeCuentasHelper(_query);

      List<AnaliticoDeCuentasEntry> analyticEntries =
                                          MergeTrialBalanceIntoAnalitico(balanceEntries);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndSubledgerAccounts =
        helper.MergeSubledgerAccountsWithAnalyticEntries(analyticEntries, balanceEntries);

      List<AnaliticoDeCuentasEntry> totalsByGroup =
                                    helper.GetTotalByGroup(analyticEntriesAndSubledgerAccounts);

      var utility = new AnaliticoDeCuentasUtility(_query);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndTotalsByGroup =
        utility.CombineTotalsByGroupAndAccountEntries(
                        analyticEntriesAndSubledgerAccounts, totalsByGroup);

      List<AnaliticoDeCuentasEntry> totalByDebtorsCreditors =
        helper.GetTotalsByDebtorOrCreditorEntries(analyticEntriesAndSubledgerAccounts);

      List<AnaliticoDeCuentasEntry> analyticEntriesAndTotalDebtorCreditor =
        utility.CombineTotalDebtorCreditorAndEntries(
                        analyticEntriesAndTotalsByGroup, totalByDebtorsCreditors);

      List<AnaliticoDeCuentasEntry> totalReport =
                                    helper.GenerateTotalReport(totalByDebtorsCreditors);

      List<AnaliticoDeCuentasEntry> analyticReport = utility.CombineTotalReportAndEntries(
                                            analyticEntriesAndTotalDebtorCreditor, totalReport);

      return analyticReport.ToFixedList();
    }


    #region Private methods


    private IEnumerable<TrialBalanceEntry> GetFilteredAccountEntries(List<TrialBalanceEntry> balanceEntries) {

      if (balanceEntries.Count == 0) {
        return new List<TrialBalanceEntry>();
      }
      
      if (_query.WithSubledgerAccount) {
       
        return balanceEntries.FindAll(a => a.SubledgerAccountId == 0 &&
                                                     a.ItemType == TrialBalanceItemType.Summary);

      } else {
        
        return balanceEntries.FindAll(a => a.SubledgerAccountNumber.Length <= 1);
      }
    }


    private void MergeDomesticBalancesIntoSectorZero(IEnumerable<AnaliticoDeCuentasEntry> analiticoEntries,
                                                     IEnumerable<TrialBalanceEntry> accountEntries) {
      if (!_query.UseNewSectorizationModel) {
        return;
      }

      var helper = new AnaliticoDeCuentasHelper(_query);

      foreach (var analiticoEntry in analiticoEntries.Where(a => a.Sector.Code == "00" && a.DomesticBalance == 0)) {

        var accountsWithDomesticCurrency = accountEntries.Where(
              a => a.Account.Number == analiticoEntry.Account.Number &&
              a.Ledger.Number == analiticoEntry.Ledger.Number &&
              a.Sector.Code != "00" && a.DebtorCreditor == analiticoEntry.DebtorCreditor &&
              (a.Currency.Equals(Currency.MXN) || a.Currency.Equals(Currency.UDI))).ToList();

        if (accountsWithDomesticCurrency.Count > 0) {

          analiticoEntry.ResetBalances();

          foreach (var foreignEntry in accountsWithDomesticCurrency) {
            helper.SumTwoColumnEntry(analiticoEntry, foreignEntry, foreignEntry.Currency);
          }
        }
      }
    }


    private void MergeForeignBalancesIntoSectorZero(IEnumerable<AnaliticoDeCuentasEntry> analiticoEntries,
                                                    IEnumerable<TrialBalanceEntry> accountEntries) {

      if (!_query.UseNewSectorizationModel) {
        return;
      }

      var helper = new AnaliticoDeCuentasHelper(_query);

      foreach (var analiticoEntry in analiticoEntries.Where(a => a.Sector.Code == "00" && a.Level > 1)) {

        var entriesWithForeignCurrency = accountEntries.Where(
              a => a.Account.Number.StartsWith(analiticoEntry.Account.Number) &&
              a.Ledger.Number == analiticoEntry.Ledger.Number &&
              a.DebtorCreditor == analiticoEntry.DebtorCreditor &&
              a.ItemType == TrialBalanceItemType.Entry &&
              a.Currency.Distinct(Currency.MXN) && a.Currency.Distinct(Currency.UDI)).ToList();

        if (entriesWithForeignCurrency.Count > 0) {

          analiticoEntry.ForeignBalance = 0;

          foreach (var foreignEntry in entriesWithForeignCurrency) {
            helper.SumTwoColumnEntry(analiticoEntry, foreignEntry, foreignEntry.Currency);
          }
        }
      }
    }


    private List<AnaliticoDeCuentasEntry> MergeTrialBalanceIntoAnalitico(
                                            List<TrialBalanceEntry> balanceEntries) {
      if (balanceEntries.Count == 0) {
        return new List<AnaliticoDeCuentasEntry>();
      }

      IEnumerable<TrialBalanceEntry> accountEntries = GetFilteredAccountEntries(balanceEntries);

      var hashAnaliticoEntries = new EmpiriaHashTable<AnaliticoDeCuentasEntry>();

      ConvertIntoAnaliticoDeCuentasEntry(accountEntries, hashAnaliticoEntries);

      ICollection<AnaliticoDeCuentasEntry> analiticoEntries = hashAnaliticoEntries.Values;

      MergeDomesticBalancesIntoSectorZero(analiticoEntries, accountEntries);
      MergeForeignBalancesIntoSectorZero(analiticoEntries, accountEntries);

      return analiticoEntries.OrderBy(a => a.Ledger.Number)
                            .ThenByDescending(a => a.DebtorCreditor)
                            .ThenBy(a => a.Account.Number)
                            .ThenBy(a => a.Sector.Code)
                            .ThenBy(a => a.SubledgerAccountId)
                            .ToList();
    }


    private void ConvertIntoAnaliticoDeCuentasEntry(IEnumerable<TrialBalanceEntry> accountEntries,
                                   EmpiriaHashTable<AnaliticoDeCuentasEntry> hashAnaliticoEntries) {

      var helper = new AnaliticoDeCuentasHelper(_query);
      var targetCurrency = Currency.Parse(_query.InitialPeriod.ValuateToCurrrencyUID);

      foreach (var entry in accountEntries) {

        if (entry.CurrentBalance != 0 ||
            _query.BalancesType == BalancesType.AllAccountsInCatalog ||
            _query.BalancesType == BalancesType.AllAccounts) {

          string hash = $"{entry.Account.Number}||{entry.Sector.Code}||{targetCurrency.Id}||" +
                        $"{entry.Ledger.Id}||{entry.DebtorCreditor}";
          Currency currentCurrency = entry.Currency;
          helper.MergeEntriesIntoTwoColumns(hashAnaliticoEntries, entry, hash, currentCurrency);
        }
      }
    }


    private FixedList<TrialBalanceEntry> SaldosDeCuentasValorizados(FixedList<TrialBalanceEntry> baseAccountEntries) {
      var balanceHelper = new TrialBalanceHelper(_query);

      balanceHelper.SetSummaryToParentEntries(baseAccountEntries);

      balanceHelper.ValuateAccountEntriesToExchangeRateV2(baseAccountEntries);

      balanceHelper.RoundDecimals(baseAccountEntries);

      return baseAccountEntries;
    }


    private List<TrialBalanceEntry> SummariesAndAccountEntries(FixedList<TrialBalanceEntry> saldosValorizados) {

      var helper = new AnaliticoDeCuentasHelper(_query);

      List<TrialBalanceEntry> parentAccounts = helper.GetCalculatedParentAccounts(saldosValorizados);

      var utility = new AnaliticoDeCuentasUtility(_query);

      List<TrialBalanceEntry> parentAndAccountEntries = utility.CombineSummaryAndPostingEntries(
                                             parentAccounts, saldosValorizados.ToFixedList());

      var balanceHelper = new TrialBalanceHelper(_query);

      List<TrialBalanceEntry> balanceEntries = helper.GetSummariesWithOrWithoutSectorization(
                                               parentAndAccountEntries.ToList());

      helper.GetSummaryToSectorZeroForPesosAndUdis(balanceEntries);

      balanceEntries = utility.RemoveUnneededAccounts(balanceEntries);

      balanceHelper.RestrictLevels(balanceEntries);

      return balanceEntries;
    }


    #endregion Private methods


  }  // class AnaliticoDeCuentas

}  // namespace Empiria.FinancialAccounting.AnaliticoDeCuentasBuilder
