/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanceConstructor                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate a balance.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services to generate a balance.</summary>
  internal class BalanceConstructor {

    internal BalanceConstructor(BalanceCommand command){
      Assertion.AssertObject(command, "command");

      Command = command;
    }


    public BalanceCommand Command {
      get;
    }

    internal Balance BuildBalance() {

      switch (Command.TrialBalanceType) {
        
        case TrialBalanceType.SaldosPorAuxiliar:
          var saldosPorAuxiliar = new SaldosPorAuxiliarConsultaRapida(Command);
          return saldosPorAuxiliar.Build();

        case TrialBalanceType.SaldosPorCuenta:
          var saldosPorCuenta = new SaldosPorCuentaConsultaRapida(Command);
          return saldosPorCuenta.Build();

        default:
          throw Assertion.AssertNoReachThisCode(
                    $"Unhandled trial balance type {this.Command.TrialBalanceType}.");
      }
    }
  } // class BalanceConstructor 

} // Empiria.FinancialAccounting.BalanceEngine.Domain 
