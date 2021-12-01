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


    public string SubledgerAccount {
      get; set;
    } = string.Empty;


    public bool WithSubledgerAccount {
      get; set;
    } = false;


    public BalancesType BalancesType {
      get; set;
    } = BalancesType.WithCurrentBalanceOrMovements;


    public TrialBalanceCommandPeriod InitialPeriod {
      get; set;
    } = new TrialBalanceCommandPeriod();



    internal static TrialBalanceCommand MapToTrialBalanceCommand(BalanceCommand command) {
      var trialBalanceCommand = new TrialBalanceCommand();

      trialBalanceCommand.AccountsChartUID = command.AccountsChartUID;
      trialBalanceCommand.FromAccount = command.FromAccount;
      trialBalanceCommand.InitialPeriod.FromDate = command.InitialPeriod.FromDate;
      trialBalanceCommand.InitialPeriod.ToDate = command.InitialPeriod.ToDate;
      trialBalanceCommand.SubledgerAccount = command.SubledgerAccount;
      trialBalanceCommand.TrialBalanceType = command.TrialBalanceType;
      trialBalanceCommand.ShowCascadeBalances = true;
      trialBalanceCommand.WithSubledgerAccount = command.TrialBalanceType == TrialBalanceType.SaldosPorAuxiliar ?
                                                 true : command.WithSubledgerAccount;

      return trialBalanceCommand;
    }

  } // class BalanceCommand 


} // Empiria.FinancialAccounting.BalanceEngine.Adapters
