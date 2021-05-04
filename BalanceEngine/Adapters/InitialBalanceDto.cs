/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : InitialBalanceDto                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Output DTO used to return initial balances.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return initial balances.</summary>
  public class InitialBalanceDto {

    public TrialBalanceCommand Command {
      get;
      internal set;
    } = new TrialBalanceCommand();


    public FixedList<InitialBalanceEntryDto> Entries {
      get;
      internal set;
    } = new FixedList<InitialBalanceEntryDto>();

  } //class InitialBalanceDto


  public class InitialBalanceEntryDto {
    public int LedgerId {
      get; internal set;
    }


    public int LedgerAccountId {
      get; internal set;
    }


    public int AccountId {
      get; internal set;
    }


    public int SectorId {
      get; internal set;
    }


    public int SubsidiaryAccountId {
      get; internal set;
    }

    public int CurrencyId {
      get; internal set;
    }

    public decimal InitialBalance {
      get; internal set;
    }



  } // class InitialBalanceEntryDto 

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
