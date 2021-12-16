/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanceDto                            License   : Please read LICENSE.txt file                 *
*                                                                                                            *
*  Summary  : Output DTO used to return balances.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  public interface IBalanceEntryDto {

  }

  /// <summary>Output DTO used to return balances.</summary>
  public class BalanceDto {
    public BalanceCommand Command {
      get; internal set;
    } = new BalanceCommand();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<IBalanceEntryDto> Entries {
      get; internal set;
    } = new FixedList<IBalanceEntryDto>();

  } // class BalanceDto


  public class BalanceEntryDto : IBalanceEntryDto {

    public TrialBalanceItemType ItemType {
      get; internal set;
    } = TrialBalanceItemType.BalanceEntry;

    public string LedgerNumber {
      get; internal set;
    } = "";

    public string LedgerName {
      get; internal set;
    } = "";

    public string CurrencyCode {
      get; internal set;
    } = "";

    public string AccountNumber {
      get; internal set;
    } = "";

    public string AccountNumberForBalances {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    } = "";

    public string SubledgerAccountNumber {
      get; internal set;
    } = "";

    public string SectorCode {
      get; internal set;
    } = "";

    public decimal CurrentBalance {
      get; internal set;
    }

    public DateTime LastChangeDate {
      get; internal set;
    } = DateTime.Now;

    public string DebtorCreditor {
      get; internal set;
    }


  } // class BalanceEntryDto

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
