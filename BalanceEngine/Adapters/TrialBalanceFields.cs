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

    public int GeneralLedgerId {
      get; set;
    }

    public DateTime InitialDate {
      get; set;
    }

    public DateTime FinalDate {
      get; set;
    }

    public int BalanceGroupId {
      get; set;
    }

  } // class TrialBalanceFields

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
