/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : AccountBalance                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an account balance.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Holds information about an account balance.</summary>
  internal class AccountBalance {

    private AccountBalance() {
      // Required by Empiria Framework.
    }

    [DataField("ID_CUENTA_ESTANDAR")]
    public LedgerAccount LedgerAccount {
      get;
      private set;
    }


    [DataField("Total")]
    public decimal Total {
      get;
      private set;
    }

  }  // class AccountBalance

}  // namespace Empiria.FinancialAccounting.BalanceEngine
