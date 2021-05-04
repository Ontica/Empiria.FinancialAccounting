﻿/* Empiria Financial *****************************************************************************************
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
      get;
      internal set;
    } = new TrialBalanceCommand();


    public FixedList<TrialBalanceEntryDto> Entries {
      get;
      internal set;
    } = new FixedList<TrialBalanceEntryDto>();


  }  // class TrialBalanceDto


  /// <summary>Output DTO used to return the entries of a trial balance.</summary>
  public class TrialBalanceEntryDto {

    public int LedgerId {
      get; internal set;
    }

    public int LedgerAccountId {
      get; set;
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
      get; set;
    }

    public string AccountNumber {
      get;
      internal set;
    } = string.Empty;


    public string AccountName {
      get;
      internal set;
    } = string.Empty;


    public decimal InitialBalance {
      get;
      internal set;
    }


    public decimal Debit {
      get;
      internal set;
    }


    public decimal Credit {
      get;
      internal set;
    }


    public decimal CurrentBalance {
      get;
      internal set;
    }

  } // class TrialBalanceEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
