/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : CurrentBalanceDto                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return current balances.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return current balances.</summary>
  public class CurrentBalanceDto {

    public TrialBalanceCommand Command {
      get;
      internal set;
    } = new TrialBalanceCommand();


    public FixedList<CurrentBalanceEntryDto> Entries {
      get;
      internal set;
    } = new FixedList<CurrentBalanceEntryDto>();


  } //class CurrentBalanceDto


  public class CurrentBalanceEntryDto {
    public int LedgerId {
      get; internal set;
    }


    public int CurrencyId {
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


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal Balance {
      get; internal set;
    }



  } // class CurrentBalanceEntryDto 

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
