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

    private readonly TrialBalanceCommand _command;

    public EnsureBalanceValidations(TrialBalanceCommand command) {
      _command = command;
    }

    #region Public methods

    public void EnsureIsValid(List<TrialBalanceEntry> trialBalance,
                              FixedList<TrialBalanceEntry> postingEntries) {

      CheckSummaryEntriesIsEqualToTotalByGroup(postingEntries);
      CheckTotalsByGroupEntries(trialBalance);

      CheckEntriesIsEqualToTotalByDebtorOrCreditor(postingEntries);
      CheckTotalsByDebtorAndCreditorEntries(trialBalance);

      CheckTotalsConsolidated(trialBalance);

    }


    #endregion Public methods


    #region Private methods

    private void CheckEntriesIsEqualToTotalByDebtorOrCreditor(FixedList<TrialBalanceEntry> entries) {
      int MAX_BALANCE_DIFFERENCE = 10;

      foreach (var totalDebtor in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)) {
        var entriesTotal = entries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Deudora &&
                                                x.Currency.Code == totalDebtor.Currency.Code)
                                  .Sum(x => x.CurrentBalance);

        Assertion.Assert(Math.Abs(totalDebtor.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual ({entriesTotal}) de las cuentas deudoras no es " +
                         $"igual al {totalDebtor.GroupName} ({totalDebtor.CurrentBalance})");
      }

      foreach (var totalCreditor in entries.Where(a => a.ItemType == TrialBalanceItemType.BalanceTotalCreditor)) {
        var entriesTotal = entries.FindAll(x => x.DebtorCreditor == DebtorCreditorType.Acreedora &&
                                                x.Currency.Code == totalCreditor.Currency.Code)
                                  .Sum(x => x.CurrentBalance);

        Assertion.Assert(Math.Abs(totalCreditor.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual ({entriesTotal}) de las cuentas acreedoras no es " +
                         $"igual al {totalCreditor.GroupName} ({totalCreditor.CurrentBalance})");
      }
    }


    private void CheckSummaryEntriesIsEqualToTotalByGroup(FixedList<TrialBalanceEntry> entries) {
      int MAX_BALANCE_DIFFERENCE = 10;

      foreach (var debtorGroup in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {
        var entriesTotal = entries.FindAll(x => x.Account.GroupNumber == debtorGroup.GroupNumber &&
                                                x.Currency.Code == debtorGroup.Currency.Code)
                                  .Sum(x => x.CurrentBalance);

        Assertion.Assert(Math.Abs(debtorGroup.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual de las cuentas no es igual al total por grupo " +
                         $"en {debtorGroup.GroupName} con naturaleza {debtorGroup.DebtorCreditor}, " +
                         $"Total de cuentas: {entriesTotal}, Total del grupo: {debtorGroup.CurrentBalance}");
      }

      foreach (var creditorGroup in entries.Where(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)) {
        var entriesTotal = entries.FindAll(x => x.Account.GroupNumber == creditorGroup.GroupNumber &&
                                                x.Currency.Code == creditorGroup.Currency.Code)
                                  .Sum(x => x.CurrentBalance);

        Assertion.Assert(Math.Abs(creditorGroup.CurrentBalance - entriesTotal) <= MAX_BALANCE_DIFFERENCE,
                         $"La suma del saldo actual de las cuentas no es igual al total por grupo " +
                         $"en {creditorGroup.GroupName} con naturaleza {creditorGroup.DebtorCreditor}, " +
                         $"Total de cuentas: {entriesTotal}, Total del grupo: {creditorGroup.CurrentBalance}");
      }
    }


    private void CheckTotalsByDebtorAndCreditorEntries(List<TrialBalanceEntry> trialBalance) {
      int MAX_BALANCE_DIFFERENCE = 10;

      if (_command.FromAccount == string.Empty && _command.ToAccount == string.Empty) {
        var totalDebtorDebit = trialBalance.FindAll(
                              x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                              .Sum(x => x.Debit);

        var totalCreditorDebit = trialBalance.FindAll(
                                  x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                                  .Sum(x => x.Debit);

        var totalDebtorCredit = trialBalance.FindAll(
                                  x => x.ItemType == TrialBalanceItemType.BalanceTotalDebtor)
                                  .Sum(x => x.Credit);

        var totalCreditorCredit = trialBalance.FindAll(
                                    x => x.ItemType == TrialBalanceItemType.BalanceTotalCreditor)
                                    .Sum(x => x.Credit);

        decimal totalDebit = totalDebtorDebit + totalCreditorDebit;
        decimal totalCredit = totalDebtorCredit + totalCreditorCredit;

        Assertion.Assert(Math.Abs(totalDebit - totalCredit) <= MAX_BALANCE_DIFFERENCE,
          "La suma de cargos totales no es igual a la suma de abonos totales de la balanza, o excede el " +
          $"límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
      }

    }


    private void CheckTotalsByGroupEntries(List<TrialBalanceEntry> trialBalance) {
      CheckTotalsByGroupEntriesInBalanza(trialBalance);
    }


    private void CheckTotalsByGroupEntriesInBalanza(List<TrialBalanceEntry> trialBalance) {
      int MAX_BALANCE_DIFFERENCE = 10;

      if (_command.FromAccount == string.Empty && _command.ToAccount == string.Empty) {
        var totalByGroupDebtor = trialBalance.FindAll(
                                  x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)
                                  .Sum(x => x.CurrentBalance);
        var totalByGroupCreditor = trialBalance.FindAll(
                                    x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)
                                    .Sum(x => x.CurrentBalance);

        Assertion.Assert(Math.Abs(totalByGroupDebtor - totalByGroupCreditor) <= MAX_BALANCE_DIFFERENCE,
          "La suma de saldos del total de cuentas deudoras no es igual al de las cuentas acreedoras, " +
          $"o excede el límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
      }

    }


    private void CheckTotalsConsolidated(List<TrialBalanceEntry> trialBalance) {
      int MAX_BALANCE_DIFFERENCE = 10;

      var totalsByCurrency = trialBalance.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalCurrency)
                                .Sum(x => x.CurrentBalance);

      var totalConsolidated = trialBalance.FindAll(
                                x => x.ItemType == TrialBalanceItemType.BalanceTotalConsolidated)
                                .Sum(x => x.CurrentBalance);

      Assertion.Assert(Math.Abs(totalConsolidated - totalsByCurrency) <= MAX_BALANCE_DIFFERENCE,
        "La suma de totales por moneda no es igual al total consolidado, " +
        $"o excede el límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
    }
    #endregion Private methods


  } // class EnsureBalanceValidations

} // namespace Empiria.FinancialAccounting.BalanceEngine
