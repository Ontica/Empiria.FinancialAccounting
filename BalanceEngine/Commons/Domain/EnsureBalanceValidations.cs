/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : EnsureBalanceValidations                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Ensures that the balance data is correct.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Ensures that the balance data is correct.</summary>
  internal class EnsureBalanceValidations {

    private readonly TrialBalanceQuery _query;

    internal EnsureBalanceValidations(TrialBalanceQuery query) {
      _query = query;
    }

    #region Public methods

    public void EnsureIsValid(FixedList<ITrialBalanceEntry> entriesList,
                              FixedList<TrialBalanceEntry> postingEntries) {

      CheckSummaryEntriesIsEqualToTotalByGroup(entriesList, postingEntries);
      CheckTotalsByGroupEntries(entriesList);

      CheckEntriesEqualToTotalByDebtorOrCreditor(entriesList, postingEntries);
      CheckTotalsByDebtorAndCreditorEntries(entriesList);

      CheckTotalsByReport(entriesList);


    }



    #endregion Public methods


    #region Private methods

    private void CheckEntriesEqualToTotalByDebtorOrCreditor(FixedList<ITrialBalanceEntry> entriesList,
                                                            FixedList<TrialBalanceEntry> postingEntries) {

      if (_query.TrialBalanceType == TrialBalanceType.Balanza ||
          _query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        TotalByDebtorOrCreditorBalanza(entriesList, postingEntries);

      } else if (_query.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas) {

        TotalByDebtorOrCreditorAnalitico(entriesList);

      }
    }


    private void CheckSummaryEntriesIsEqualToTotalByGroup(FixedList<ITrialBalanceEntry> entriesList,
                                                          FixedList<TrialBalanceEntry> postingEntries) {
      if (_query.TrialBalanceType == TrialBalanceType.Balanza ||
          _query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        int MAX_BALANCE_DIFFERENCE = 10;

        var entries = entriesList.Select(x => (TrialBalanceEntry) x).ToList();

        foreach (var debtorGroup in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {
          var entriesTotal = postingEntries.FindAll(x => x.Account.GroupNumber == debtorGroup.GroupNumber &&
                                                         x.Account.DebtorCreditor == DebtorCreditorType.Deudora &&
                                                         x.Currency.Equals(debtorGroup.Currency))
                                           .Sum(x => x.CurrentBalance);

          Assertion.Require(Math.Abs(debtorGroup.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                           $"La suma del saldo actual de las cuentas no es igual al total por grupo " +
                           $"en {debtorGroup.GroupName} con naturaleza {debtorGroup.DebtorCreditor}, " +
                           $"Total de cuentas: {entriesTotal}, Total del grupo: {debtorGroup.CurrentBalance}");
        }

        foreach (var creditorGroup in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)) {
          var entriesTotal = postingEntries.FindAll(x => x.Account.GroupNumber == creditorGroup.GroupNumber &&
                                                         x.Account.DebtorCreditor == DebtorCreditorType.Acreedora &&
                                                         x.Currency.Equals(creditorGroup.Currency))
                                           .Sum(x => x.CurrentBalance);

          Assertion.Require(Math.Abs(creditorGroup.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                           $"La suma del saldo actual de las cuentas no es igual al total por grupo " +
                           $"en {creditorGroup.GroupName} con naturaleza {creditorGroup.DebtorCreditor}, " +
                           $"Total de cuentas: {entriesTotal}, Total del grupo: {creditorGroup.CurrentBalance}");
        }

      }
    }


    private void CheckTotalsByDebtorAndCreditorEntries(FixedList<ITrialBalanceEntry> entriesList) {
      if ((_query.TrialBalanceType == TrialBalanceType.Balanza ||
          _query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) &&
          (_query.ConsolidateBalancesToTargetCurrency == false)) { //cambiar esta condicion

        int MAX_BALANCE_DIFFERENCE = 10;
        var entries = entriesList.Select(x => (TrialBalanceEntry) x).ToList();

        if (_query.FromAccount == string.Empty && _query.ToAccount == string.Empty) {
          var totalDebtorDebit = entries.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                                .Sum(x => x.Debit);

          var totalCreditorDebit = entries.FindAll(
                                    x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                                    .Sum(x => x.Debit);

          var totalDebtorCredit = entries.FindAll(
                                    x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                                    .Sum(x => x.Credit);

          var totalCreditorCredit = entries.FindAll(
                                      x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                                      .Sum(x => x.Credit);

          decimal totalDebit = totalDebtorDebit + totalCreditorDebit;
          decimal totalCredit = totalDebtorCredit + totalCreditorCredit;

          Assertion.Require(Math.Abs(totalDebit - totalCredit) <= MAX_BALANCE_DIFFERENCE,
            $"La suma de cargos totales ({totalDebit}) no es igual a la suma de abonos totales ({totalCredit}) de la balanza, o excede el " +
            $"límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
        }

      }

    }


    private void CheckTotalsByGroupEntries(FixedList<ITrialBalanceEntry> entriesList) {

      if (_query.TrialBalanceType == TrialBalanceType.Balanza ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta ||
          _query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada) {

        var entries = entriesList.Select(x => (TrialBalanceEntry) x).ToList();
        CheckTotalsByGroupEntriesInBalanza(entries);
      }

    }


    private void CheckTotalsByGroupEntriesInBalanza(List<TrialBalanceEntry> entries) {
      int MAX_BALANCE_DIFFERENCE = 10;

      if (_query.FromAccount == string.Empty && _query.ToAccount == string.Empty &&
          _query.ConsolidateBalancesToTargetCurrency == false &&
          _query.AccountsChartUID == "b2328e67-3f2e-45b9-b1f6-93ef6292204e") {
        var totalByGroupDebtor = entries.FindAll(
                                  x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)
                                  .Sum(x => x.CurrentBalance);
        var totalByGroupCreditor = entries.FindAll(
                                    x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)
                                    .Sum(x => x.CurrentBalance);

        Assertion.Require(Math.Abs(totalByGroupDebtor - totalByGroupCreditor) <= MAX_BALANCE_DIFFERENCE,
          "La suma de saldos del total de cuentas deudoras no es igual al de las cuentas acreedoras, " +
          $"o excede el límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
      }

    }


    private void CheckTotalsConsolidated(FixedList<ITrialBalanceEntry> entriesList) {
      int MAX_BALANCE_DIFFERENCE = 10;

      var entries = entriesList.Select(x => (TrialBalanceEntry) x).ToList();
      var totalsByCurrency = entries.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                                .Sum(x => x.CurrentBalance);

      var totalConsolidated = entries.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalConsolidated)
                                .Sum(x => x.CurrentBalance);

      Assertion.Require(Math.Abs(totalConsolidated - totalsByCurrency) <= MAX_BALANCE_DIFFERENCE,
        "La suma de totales por moneda no es igual al total consolidado, " +
        $"o excede el límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
    }


    private void CheckTotalsByReport(FixedList<ITrialBalanceEntry> entriesList) {
      if (_query.TrialBalanceType == TrialBalanceType.Balanza ||
          _query.TrialBalanceType == TrialBalanceType.BalanzaConContabilidadesEnCascada ||
          _query.TrialBalanceType == TrialBalanceType.SaldosPorCuenta) {

        CheckTotalsConsolidated(entriesList);

      } else if (_query.TrialBalanceType == TrialBalanceType.AnaliticoDeCuentas) {

        CheckTotalsInReport(entriesList);

      }
    }


    private void CheckTotalsInReport(FixedList<ITrialBalanceEntry> entriesList) {
      int MAX_BALANCE_DIFFERENCE = 10;

      var entries = entriesList.Select(x => (AnaliticoDeCuentasEntry) x).ToList();

      var totalDebtor = entries.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                                .Sum(x => x.TotalBalance);

      var totalCreditor = entries.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                                .Sum(x => x.TotalBalance);

      var totalReport = entries.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalConsolidated)
                                .Sum(x => x.TotalBalance);

      Assertion.Require(Math.Abs(totalReport - (totalDebtor - totalCreditor)) <= MAX_BALANCE_DIFFERENCE,
        "La suma de total de deudoras menos acreedoras no es igual al total del reporte, " +
        $"o excede el límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
    }


    private void TotalByDebtorOrCreditorAnalitico(FixedList<ITrialBalanceEntry> entriesList) {
      int MAX_BALANCE_DIFFERENCE = 10;

      var entries = entriesList.Select(x => (AnaliticoDeCuentasEntry) x).ToList();

      foreach (var totalDebtor in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {

        var entriesTotal = entries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Deudora &&
                                                x.ItemType == TrialBalanceItemType.Summary &&
                                                x.Level == 1 && x.Sector.Code == "00")
                                  .Sum(x => x.DomesticBalance);

        if (_query.AccountsChartUID == "b2328e67-3f2e-45b9-b1f6-93ef6292204e") {
          entriesTotal = entries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Deudora &&
                                                x.ItemType == TrialBalanceItemType.Entry)
                                  .Sum(x => x.DomesticBalance);
        }

        Assertion.Require(Math.Abs(totalDebtor.DomesticBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual ({entriesTotal}) de las cuentas deudoras no es " +
                         $"igual al {totalDebtor.GroupName} ({totalDebtor.DomesticBalance})");
      }

      foreach (var totalCreditor in entries.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {
        var entriesTotal = entries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Acreedora &&
                                                x.ItemType == TrialBalanceItemType.Summary &&
                                                x.Level == 1 && x.Sector.Code == "00")
                                  .Sum(x => x.DomesticBalance);
        if (_query.AccountsChartUID == "b2328e67-3f2e-45b9-b1f6-93ef6292204e") {
          entriesTotal = entries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Acreedora &&
                                                x.ItemType == TrialBalanceItemType.Entry)
                                .Sum(x => x.DomesticBalance);
        }
        Assertion.Require(Math.Abs(totalCreditor.DomesticBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual ({entriesTotal}) de las cuentas acreedoras no es " +
                         $"igual al {totalCreditor.GroupName} ({totalCreditor.DomesticBalance})");
      }
    }


    private void TotalByDebtorOrCreditorBalanza(FixedList<ITrialBalanceEntry> entriesList,
                                                FixedList<TrialBalanceEntry> postingEntries) {
      int MAX_BALANCE_DIFFERENCE = 10;

      var entries = entriesList.Select(x => (TrialBalanceEntry) x).ToList();

      foreach (var totalDebtor in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {
        var entriesTotal = postingEntries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Deudora &&
                                                x.Currency.Equals(totalDebtor.Currency))
                                         .Sum(x => x.CurrentBalance);

        Assertion.Require(Math.Abs(totalDebtor.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual ({entriesTotal}) de las cuentas deudoras no es " +
                         $"igual al {totalDebtor.GroupName} ({totalDebtor.CurrentBalance})");
      }

      foreach (var totalCreditor in entries.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {
        var entriesTotal = postingEntries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Acreedora &&
                                                       x.Currency.Equals(totalCreditor.Currency))
                                         .Sum(x => x.CurrentBalance);

        Assertion.Require(Math.Abs(totalCreditor.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual ({entriesTotal}) de las cuentas acreedoras no es " +
                         $"igual al {totalCreditor.GroupName} ({totalCreditor.CurrentBalance})");
      }
    }

    #endregion Private methods


  } // class EnsureBalanceValidations

} // namespace Empiria.FinancialAccounting.BalanceEngine
