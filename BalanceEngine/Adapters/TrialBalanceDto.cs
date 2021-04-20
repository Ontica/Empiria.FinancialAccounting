/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : TrialBalanceDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return trial balances.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return trial balances.</summary>
  public class TrialBalanceDto {

    public string NUMERO_CUENTA_ESTANDAR {
      get;
      internal set;
    } = string.Empty;

    public string NOMBRE_CUENTA_ESTANDAR {
      get;
      internal set;
    } = string.Empty;

    public decimal SALDO_INICIAL {
      get;
      internal set;
    }

    public decimal DEBE {
      get;
      internal set;
    }

    public decimal HABER {
      get;
      internal set;
    }

    public decimal SALDO_ACTUAL {
      get;
      internal set;
    }

  } // class TrialBalanceDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
