/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : ListadoPolizasPorCuentaHelper              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build vouchers by account information.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Helper methods to build vouchers by account information.</summary>
  internal class ListadoPolizasPorCuentaHelper {
    
    private readonly BuildReportCommand Command;

    public ListadoPolizasPorCuentaHelper(BuildReportCommand command) {
      Assertion.AssertObject(command, "command");

      Command = command;
    }


    #region Public methods


    internal FixedList<AccountStatementEntry> GetVoucherEntries() {
      var commandExtensions = new PolizasPorCuentaCommandExtensions();

      PolizaCommandData commandData = commandExtensions.MapToPolizaCommandData(Command);

      FixedList<AccountStatementEntry> vouchers = 
              ListadoPolizasPorCuentaDataService.GetVouchersByAccountEntries(commandData);

      FixedList<AccountStatementEntry> orderingVouchers = OrderingVouchers(vouchers);

      return orderingVouchers;
    }


    #endregion Public methods


    #region Private methods

    private FixedList<AccountStatementEntry> OrderingVouchers(FixedList<AccountStatementEntry> vouchers) {
      var ordering = vouchers.OrderBy(a => a.AccountingDate)
                             .ThenBy(a => a.Ledger.Number)
                             .ThenBy(a => a.AccountNumber)
                             .ThenBy(a => a.SubledgerAccountNumber)
                             .ThenBy(a => a.VoucherNumber)
                             .ToList();
      return ordering.ToFixedList();
    }

    #endregion Private methods

  } // class ListadoPolizasPorCuentaHelper

} // namespace Empiria.FinancialAccounting.Reporting
