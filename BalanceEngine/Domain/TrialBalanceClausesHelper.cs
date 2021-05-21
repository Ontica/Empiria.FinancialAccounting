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

    internal string GetFilterString() {
      string ledgerFilter = GetLedgerFilter();
      string sectorFilter = GetSectorFilter();
      string rangeFilter = GetAccountRangeFilter();

      var filter = new Filter(ledgerFilter);

      if (filter.ToString().Length == 0 && sectorFilter.Length > 0) {
        filter.AppendAnd(" AND " + sectorFilter);
      } else {
        filter.AppendAnd(sectorFilter);
      }

      if (filter.ToString().Length == 0 && rangeFilter.Length > 0) {
        filter.AppendAnd(" AND " + rangeFilter);
      } else {
        filter.AppendAnd(rangeFilter);
      }

      return filter.ToString();
    }


    internal string GetGroupingClause() {
      if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
          _command.Consolidated) {

        return "ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";

      } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                 !_command.Consolidated) {

        return "ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_CUENTA_ESTANDAR_HIST, ID_CUENTA, ID_SECTOR";

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

      } else if (_command.BalancesType == BalancesType.WithMovements) {
        clause = "CARGOS <> 0 OR ABONOS <> 0";

      } else {
        throw Assertion.AssertNoReachThisCode();
      }

      return $"HAVING {clause}";
    }


    internal string GetOrderClause() {
      if (_command.Consolidated) {
        return "ORDER BY ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_SECTOR";
      } else {
        return "ORDER BY ID_MAYOR, ID_MONEDA, NUMERO_CUENTA_ESTANDAR, ID_CUENTA, ID_SECTOR";
      }
    }


    internal string GetOutputFields() {
      if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
          _command.Consolidated) {

        return "-1 AS ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_SECTOR, " +
               "SUM(SALDO_ANTERIOR) AS SALDO_ANTERIOR, SUM(CARGOS) AS CARGOS, " +
               "SUM(ABONOS) AS ABONOS, SUM(SALDO_ACTUAL) AS SALDO_ACTUAL";

      } else if (_command.TrialBalanceType == TrialBalanceType.Traditional &&
                 !_command.Consolidated) {

        return "ID_MAYOR, ID_MONEDA, ID_CUENTA_ESTANDAR_HIST, NUMERO_CUENTA_ESTANDAR, ID_CUENTA, ID_SECTOR, " +
               "SUM(SALDO_ANTERIOR) AS SALDO_ANTERIOR, SUM(CARGOS) AS CARGOS, " +
               "SUM(ABONOS) AS ABONOS, SUM(SALDO_ACTUAL) AS SALDO_ACTUAL";

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

      string[] ledgerIds = _command.Ledgers.Select(x => $"'{x}'")
                                           .ToArray();

      return $"ID_MAYOR IN ({String.Join(", ", ledgerIds)})";
    }


    private string GetSectorFilter() {
      if (_command.Sectors.Length == 0) {
        return string.Empty;
      }

      string[] SectorIds = _command.Sectors.Select(x => $"'{x}'")
                                           .ToArray();

      return $"ID_SECTOR IN ({String.Join(", ", SectorIds)})";
    }


    #endregion Private methods


  } // class TrialBalanceClausesHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
