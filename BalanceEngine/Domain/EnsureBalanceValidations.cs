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

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Ensures that the balance data is correct.</summary>
  internal class EnsureBalanceValidations {


    #region Public methods

    public void EnsureIsValid(List<TrialBalanceEntry> trialBalance, FixedList<TrialBalanceEntry> postingEntries) {
      CheckSumaDeCargosIgualAAbonos(trialBalance);
      CheckSummaryEntriesIsEqualToTotalByGroupEntries(postingEntries);
      //CheckTotalsByGroupEntries(trialBalance);
    }

    private void CheckSummaryEntriesIsEqualToTotalByGroupEntries(FixedList<TrialBalanceEntry> entries) {
      int MAX_BALANCE_DIFFERENCE = 10;

      foreach (var debtorGroup in entries.Where(x=>x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)) {
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

    #endregion Public methods


    #region Private methods

    private void CheckSumaDeCargosIgualAAbonos(List<TrialBalanceEntry> trialBalance) {

    }

    private void CheckTotalsByGroupEntries(List<TrialBalanceEntry> trialBalance) {
      int MAX_BALANCE_DIFFERENCE = 10;

      var totalByGroupDebtor = trialBalance.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupDebtor)
                                 .Sum(x => x.CurrentBalance);
      var totalByGroupCreditor = trialBalance.FindAll(x => x.ItemType == TrialBalanceItemType.BalanceTotalGroupCreditor)
                                 .Sum(x => x.CurrentBalance);

      Assertion.Assert(Math.Abs(totalByGroupDebtor - totalByGroupCreditor) <= MAX_BALANCE_DIFFERENCE,
        "La suma de saldos del total de cuentas deudoras no es igual al de las cuentas acreedoras, o excede el " +
        $"límite máximo de {MAX_BALANCE_DIFFERENCE} pesos de diferencia.");
    }

    #endregion Private methods


  } // class EnsureBalanceValidations

} // namespace Empiria.FinancialAccounting.BalanceEngine
