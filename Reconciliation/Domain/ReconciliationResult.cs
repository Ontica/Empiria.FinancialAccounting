/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Information Holder                      *
*  Type     : ReconciliationResult                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds the result of reconciliation process.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets;
using Empiria.FinancialAccounting.Reconciliation.Adapters;

namespace Empiria.FinancialAccounting.Reconciliation {

  internal class ReconciliationResult {

    internal ReconciliationResult(ReconciliationCommand command,
                                  FixedList<Dataset> datasets,
                                  FixedList<ReconciliationResultEntry> resultEntries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(datasets, "datasets");
      Assertion.AssertObject(resultEntries, "resultEntries");

      this.Command = command;
      this.Datasets = datasets;
      this.ResultEntries = resultEntries;
    }

    #region Properties

    public ReconciliationCommand Command {
      get;
    }

    public FixedList<Dataset> Datasets {
      get;
    }

    public FixedList<ReconciliationResultEntry> ResultEntries {
      get;
    }

    #endregion Properties

  }  // class ReconciliationResult

}  // namespace Empiria.FinancialAccounting.Reconciliation
