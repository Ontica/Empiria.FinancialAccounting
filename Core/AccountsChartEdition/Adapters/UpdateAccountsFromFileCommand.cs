/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Command payload                         *
*  Type     : UpdateAccountsFromFileCommand              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command object used to update a chart of accounts from a file.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters {

  /// <summary>Command object used to update a chart of accounts from a file.</summary>
  public class UpdateAccountsFromFileCommand {

    public string AccountsChartUID {
      get; set;
    } = string.Empty;


    public DateTime ApplicationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public bool DryRun {
      get; set;
    } = true;


  }  // class UpdateAccountsFromFileCommand



  /// <summary>Extension methods for UpdateAccountsFromFileCommand interface adapter.</summary>
  static internal class UpdateAccountsFromFileCommandExtension {

    #region Methods

    static internal AccountsChart GetAccountsChart(this UpdateAccountsFromFileCommand command) {
      Assertion.Require(command.AccountsChartUID, "command.AccountsChartUID");

      return AccountsChart.Parse(command.AccountsChartUID);
    }

    #endregion Methods

  }  // class UpdateAccountsFromFileCommandExtension

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Adapters
