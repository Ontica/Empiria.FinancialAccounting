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


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<ITrialBalanceEntryDto> Entries {
      get; internal set;
    } = new FixedList<ITrialBalanceEntryDto>();

  }  // class TrialBalanceDto

  public interface ITrialBalanceEntryDto {

  }

  /// <summary>Output DTO used to return the entries of a trial balance.</summary>
  public class TwoColumnsTrialBalanceEntryDto : ITrialBalanceEntryDto {

    public string ItemType {
      get; internal set;
    }


    public string LedgerUID {
      get; internal set;
    }

    public string LedgerNumber {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public int LedgerAccountId {
      get; internal set;
    }


    public int SubledgerAccountId {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }

    public AccountRole AccountRole {
      get; internal set;
    }


    public int AccountLevel {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }

    public decimal DomesticBalance {
      get; internal set;
    }

    public decimal ForeignBalance {
      get; internal set;
    }

  }

  /// <summary>Output DTO used to return the entries of a trial balance.</summary>
  public class TrialBalanceEntryDto : ITrialBalanceEntryDto {

    public string ItemType {
      get; internal set;
    }


    public string LedgerUID {
      get; internal set;
    }

    public string LedgerNumber {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public int LedgerAccountId {
      get; internal set;
    }


    public int SubledgerAccountId {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }

    public AccountRole AccountRole {
      get; internal set;
    }


    public int AccountLevel {
      get; internal set;
    }


    public string SectorCode {
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
