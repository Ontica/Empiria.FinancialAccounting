/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Command payload                         *
*  Type     : AccountStatementCommand                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build account statement.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Adapters {


  /// <summary>Command payload used to build account statement.</summary>
  public class AccountStatementCommand {

    public BalanceCommand Command {
      get; internal set;
    } = new BalanceCommand();


    public AccountStatementEntry Entry {
      get; internal set;
    } = new AccountStatementEntry();
    
  } // class AccountStatementCommand 

  public class AccountStatementEntry {
    public TrialBalanceItemType ItemType {
      get; set;
    } = TrialBalanceItemType.BalanceEntry;

    public string LedgerUID {
      get; set;
    } = "";

    public string LedgerNumber {
      get; set;
    } = "";

    public string LedgerName {
      get; set;
    } = "";

    public string CurrencyCode {
      get; set;
    } = "";

    public string AccountNumber {
      get; set;
    } = "";

    public string AccountNumberForBalances {
      get; set;
    } = "";

    public string AccountName {
      get; set;
    } = "";

    public string SubledgerAccountNumber {
      get; set;
    } = "";

    public string SectorCode {
      get; set;
    } = "";

    public decimal InitialBalance {
      get; set;
    }

    public decimal CurrentBalance {
      get; set;
    }

    public DateTime LastChangeDate {
      get; set;
    } = DateTime.Now;

    public string DebtorCreditor {
      get; set;
    }

  }

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
