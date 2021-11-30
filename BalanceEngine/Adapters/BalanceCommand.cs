/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : BalanceCommand                        License   : Please read LICENSE.txt file                 *
*                                                                                                            *
*  Summary  : Command payload used to build balances.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Command payload used to build balances.</summary>
  public class BalanceCommand {

    public TrialBalanceType TrialBalanceType {
      get; set;
    }


    public string AccountsChartUID {
      get; set;
    }


    public string FromAccount {
      get; set;
    } = string.Empty;


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    }


    public string SubledgerAccount {
      get; set;
    } = string.Empty;


    public bool WithSubledgerAccount {
      get; set;
    } = false;


    internal static TrialBalanceCommand MapToTrialBalanceCommand(BalanceCommand command) {
      var trialBalanceCommand = new TrialBalanceCommand();

      trialBalanceCommand.AccountsChartUID = command.AccountsChartUID;
      trialBalanceCommand.FromAccount = command.FromAccount;
      trialBalanceCommand.InitialPeriod.FromDate = command.FromDate;
      trialBalanceCommand.InitialPeriod.ToDate = command.ToDate;
      trialBalanceCommand.SubledgerAccount = command.SubledgerAccount;
      trialBalanceCommand.TrialBalanceType = command.TrialBalanceType;
      trialBalanceCommand.WithSubledgerAccount = command.WithSubledgerAccount;

      return trialBalanceCommand;
    }
  } // class BalanceCommand 

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
