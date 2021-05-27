/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria General Object                  *
*  Type     : TrialBalanceClausesHelper                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services used to build trial balance sql clauses.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

/// <summary>Provides services used to build trial balance sql clauses.</summary>
  internal class TrialBalanceClausesHelper {

    private readonly TrialBalanceCommand _command;

    internal TrialBalanceClausesHelper(TrialBalanceCommand command) {
      this._command = command;
    }


    #region Public methods

    internal string GetAccountFilterString() {
      string rangeFilter = GetAccountRangeFilter();

      var filter = new Filter("");

      if (filter.ToString().Length == 0 && rangeFilter.Length > 0) {
        filter.AppendAnd(" AND " + rangeFilter);
      } else {
        filter.AppendAnd(rangeFilter);
      }

      return filter.ToString();
    }


    internal string GetFilterString() {
      string ledgerFilter = GetLedgerFilter();
      string sectorFilter = GetSectorFilter();
      string accountRangeFilter = GetAccountRangeFilter();

      var filter = new Filter(ledgerFilter);

      filter.AppendAnd(sectorFilter);
      filter.AppendAnd(accountRangeFilter);

      return $"AND ({filter.ToString()})";
    }


    internal string GetGroupingClause() {
      if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
          _command.Consolidated) {

        return  "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
                "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

      } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                 !_command.Consolidated) {

        return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA, " +
               "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

      } else {
         throw Assertion.AssertNoReachThisCode();
      }
    }


    internal string GetHavingClause() {
      if (_command.BalancesType == BalancesType.AllAccounts) {
        return string.Empty;
      }

      string clause;

      if (_command.BalancesType == BalancesType.WithCurrentBalance) {
        clause = "SALDO_ACTUAL <> 0";

      } else if (_command.BalancesType == BalancesType.WithCurrenBalanceOrMovements) {
        clause = "SALDO_ACTUAL <> 0 OR DEBE <> 0 OR HABER <> 0";


      } else if (_command.BalancesType == BalancesType.WithMovements) {
        clause = "DEBE <> 0 OR HABER <> 0";

      } else {
        throw Assertion.AssertNoReachThisCode();
      }

      return $"HAVING {clause}";
    }


    internal string GetInitialFields() {
      if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
          _command.Consolidated) {

        return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, -1 AS ID_CUENTA, ";

      } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                 !_command.Consolidated) {

        return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA, ";

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    internal string GetInitialGroupingClause() {
      if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
          _command.Consolidated) {

        return "GROUP BY ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR";

      } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                 !_command.Consolidated) {

        return "GROUP BY ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA";

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    internal string GetOrderClause() {
      if (_command.Consolidated) {
        return "ORDER BY ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
      } else {
        return "ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA";
      }
    }


    internal string GetOutputFields() {
      if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
          _command.Consolidated) {

        return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, -1 AS ID_CUENTA, " +
               "NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
               "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

      } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                 !_command.Consolidated) {

        return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, ID_CUENTA, " +
               "SALDO_ANTERIOR, DEBE, HABER, SALDO_ACTUAL";

      } else {
        throw Assertion.AssertNoReachThisCode();
      }
    }


    #endregion Public methods


    #region Private methods

    private string GetAccountRangeFilter() {
      string filter = String.Empty;

      if (_command.FromAccount.Length != 0) {
        filter = $"NUMERO_CUENTA_ESTANDAR >= '{_command.FromAccount}'";
      }
      if (_command.ToAccount.Length != 0) {
        if (filter.Length != 0) {
          filter += " AND ";
        }
        filter += $"NUMERO_CUENTA_ESTANDAR <= '{_command.ToAccount}'";
      }

      return filter;
    }


    private string GetLedgerFilter() {
      if (_command.Ledgers.Length == 0) {
        return string.Empty;
      }

      int[] ledgerIds = _command.Ledgers.Select(uid => Ledger.Parse(uid).Id)
                                        .ToArray();

      return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
    }


    private string GetSectorFilter() {
      if (_command.Sectors.Length == 0) {
        return string.Empty;
      }

      int[] sectorIds = _command.Sectors.Select(uid => Sector.Parse(uid).Id)
                                           .ToArray();

      return $"ID_SECTOR IN ({String.Join(", ", sectorIds)})";
    }


    #endregion Private methods


  } // class TrialBalanceClausesHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
