/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Service provider                        *
*  Type     : ReconciliationEngine                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Processes a financial accounting reconciliation.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Datasets;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Processes a financial accounting reconciliation.</summary>
  internal class ReconciliationEngine {

    private readonly ReconciliationCommand _command;
    private readonly ReconciliationType _reconciliationType;

    public ReconciliationEngine(ReconciliationCommand command) {
      Assertion.AssertObject(command, "command");

      command.EnsureValid();

      _command = command;
      _reconciliationType = ReconciliationType.Parse(_command.ReconciliationTypeUID);
    }


    #region Methods

    internal ReconciliationResult Reconciliate() {
      FixedList<Dataset> operationalDatasets = GetOperationalDatasets();

      FixedList<OperationalEntryDto> operationalEntries = GetOperationalEntries(operationalDatasets);

      FixedList<AccountsListItem> accountsListItems = GetReconciliationAccountsListItems();

      FixedList<TrialBalanceEntryDto> balances = GetBalances();

      FixedList<ReconciliationResultEntry> resultEntries = ReconciliateAccounts(accountsListItems,
                                                                                operationalEntries,
                                                                                balances);

      return BuildReconciliationResult(operationalDatasets, resultEntries);
    }


    private FixedList<ReconciliationResultEntry> ReconciliateAccounts(FixedList<AccountsListItem> accountsListItems,
                                                                      FixedList<OperationalEntryDto> operationalEntries,
                                                                      FixedList<TrialBalanceEntryDto> balances) {

      FixedList<TrialBalanceEntryDto> filteredBalances =
            balances.FindAll(x => accountsListItems.Exists(y => y.AccountNumber == x.AccountNumber));

      var entriesListBuilder = new ReconciliationResultEntryListBuilder(operationalEntries,
                                                                        filteredBalances,
                                                                        2 * accountsListItems.Count);

      foreach (var account in accountsListItems) {
        entriesListBuilder.InsertEntriesFor(account);
      }

      return entriesListBuilder.ToFixedList();
    }


    #endregion Methods

    #region Helpers

    private ReconciliationResult BuildReconciliationResult(FixedList<Dataset> operationalDatasets,
                                                           FixedList<ReconciliationResultEntry> reconciliationEntries) {
      return new ReconciliationResult(_command, operationalDatasets, reconciliationEntries);
    }


    private FixedList<AccountsListItem> GetReconciliationAccountsListItems() {
      AccountsList list = _reconciliationType.AccountsList;

      FixedList<AccountsListItem> items = list.GetItems();

      Assertion.Assert(items.Count > 0,
        $"No se han definido las cuentas a conciliar del tipo '{_reconciliationType.Name}'."
      );

      return items;
    }


    private FixedList<OperationalEntryDto> GetOperationalEntries(FixedList<Dataset> datasets) {
      var list = new List<OperationalEntryDto>(3 * 4096);

      foreach (var dataset in datasets) {
        var reader = new OperationalEntriesReader(dataset);

        FixedList<OperationalEntryDto> entries = reader.GetEntries();

        list.AddRange(entries);
      }

      return list.ToFixedList();
    }


    private FixedList<Dataset> GetOperationalDatasets() {
      FixedList<Dataset> list = _reconciliationType.GetDatasetsList(_command.Date);

      Assertion.Assert(list.Count > 0,
        $"No se han cargado conjuntos o archivos de datos para " +
        $"la conciliación del día {_command.Date.ToString("dd/MMM/yyyy")}.");

      return list;
    }


    private FixedList<TrialBalanceEntryDto> GetBalances() {
      TrialBalanceCommand trialBalanceCommand = DetermineTrialBalanceCommand();

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(trialBalanceCommand);

        IEnumerable<TrialBalanceEntryDto> balances = trialBalance.Entries.Select(x => (TrialBalanceEntryDto) x);

        return new FixedList<TrialBalanceEntryDto>(balances);
      }
    }


    private TrialBalanceCommand DetermineTrialBalanceCommand() {
      var ledger = AccountsChart.IFRS.GetLedger("09");

      return new TrialBalanceCommand {
        TrialBalanceType = TrialBalanceType.Balanza,
        AccountsChartUID = AccountsChart.IFRS.UID,
        BalancesType = BalancesType.WithMovements,
        ShowCascadeBalances = true,
        Ledgers = new string[] { ledger.UID },
        InitialPeriod = new TrialBalanceCommandPeriod {
          FromDate = _command.Date,
          ToDate = _command.Date
        }
      };
    }

    #endregion Helpers

  }  // class ReconciliationEngine

}  // namespace Empiria.FinancialAccounting.Reconciliation
