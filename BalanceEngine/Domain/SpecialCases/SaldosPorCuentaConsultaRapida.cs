/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorCuentaConsultaRapida              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta de consulta rápida.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por cuenta de consulta rápida.</summary>
  internal class SaldosPorCuentaConsultaRapida {

    private readonly BalanceCommand _command;

    public SaldosPorCuentaConsultaRapida(BalanceCommand command) {
      _command = command;
    }

    internal Balance Build() {
      var helper = new BalanceHelper(_command);

      FixedList<BalanceEntry> balance = helper.GetBalanceEntries();

      FixedList<BalanceEntry> orderingBalance = helper.GetOrderingBalance(balance);

      var returnedBalance = new FixedList<IBalanceEntry>(orderingBalance.Select(x => (IBalanceEntry) x));

      return new Balance(_command, returnedBalance);
    }
  } // class SaldosPorCuentaConsultaRapida

} // Empiria.FinancialAccounting.BalanceEngine.Domain.SpecialCases
