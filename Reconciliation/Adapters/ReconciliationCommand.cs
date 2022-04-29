﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Command payload                         *
*  Type     : ReconciliationCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to execute reconcilation processes.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Command payload used to execute reconcilation processes.</summary>
  public class ReconciliationCommand {

    public ReconciliationCommand() {
      // no-op
    }

    public string ReconciliationTypeUID {
      get; set;
    }

    public DateTime Date {
      get; set;
    } = ExecutionServer.DateMinValue;


    public void EnsureValid() {
      Assertion.AssertObject(ReconciliationTypeUID, "ReconciliationTypeUID");
      Assertion.Assert(Date != ExecutionServer.DateMinValue, "Date");
    }

  }  // class ReconciliationCommand

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters