/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : TrialBalanceFields                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return trial balances.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {
  public class TrialBalanceFields {

    public int ID_MAYOR {
      get; set;
    }

    public DateTime FECHA_INICIAL {
      get; set;
    }

    public DateTime FECHA_FINAL {
      get; set;
    }

    public int ID_GRUPO_SALDO {
      get; set;
    }

  } // class TrialBalanceFields

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
