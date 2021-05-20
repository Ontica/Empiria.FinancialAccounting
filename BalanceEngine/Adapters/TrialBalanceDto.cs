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

    public TrialBalanceCommand Command {
      get; internal set;
    } = new TrialBalanceCommand();


    public FixedList<TrialBalanceEntryDto> Entries {
      get; internal set;
    } = new FixedList<TrialBalanceEntryDto>();


  }  // class TrialBalanceDto


  /// <summary>Output DTO used to return the entries of a trial balance.</summary>
  public class TrialBalanceEntryDto {

    public NamedEntityDto Ledger {
      get; internal set;
    }


    public NamedEntityDto Currency {
      get; internal set;
    }


    public string AccountUID {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }


    public NamedEntityDto Sector {
      get; internal set;
    }


    public decimal InitialBalance {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal CurrentBalance {
      get; internal set;
    }

  } // class TrialBalanceEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
