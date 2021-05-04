/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : DebitCreditBalanceDto                          License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Output DTO used to return credit and debit balances.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return credit and debit balances.</summary>
  public class DebitCreditBalanceDto {

    public TrialBalanceCommand Command {
      get;
      internal set;
    } = new TrialBalanceCommand();


    public FixedList<DebitCreditBalanceEntryDto> Entries {
      get;
      internal set;
    } = new FixedList<DebitCreditBalanceEntryDto>();

  } // class DebitCreditBalanceDto


  public class DebitCreditBalanceEntryDto {
    public int LedgerId {
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


    public decimal Balance {
      get; internal set;
    }

  } // class DebitCreditBalanceEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
