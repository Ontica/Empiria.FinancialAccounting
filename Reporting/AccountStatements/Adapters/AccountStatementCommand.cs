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
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Command payload used to build account statement.</summary>
  public class AccountStatementCommand {

    public BalancesQuery Command {
      get; internal set;
    } = new BalancesQuery();


    public AccountStatementEntry Entry {
      get; internal set;
    } = new AccountStatementEntry();

  } // class AccountStatementCommand



  public class AccountStatementEntry {

    public TrialBalanceItemType ItemType {
      get; set;
    } = TrialBalanceItemType.Entry;


    public string LedgerUID {
      get; set;
    }  = string.Empty;


    public string LedgerNumber {
      get; set;
    } = string.Empty;


    public string LedgerName {
      get; set;
    } = string.Empty;


    public string CurrencyCode {
      get; set;
    } = string.Empty;


    public string AccountNumber {
      get; set;
    } = string.Empty;


    public string AccountNumberForBalances {
      get; set;
    } = string.Empty;


    public string AccountName {
      get; set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; set;
    } = string.Empty;


    public string SectorCode {
      get; set;
    } = string.Empty;


    public decimal InitialBalance {
      get; set;
    }


    public decimal CurrentBalanceForBalances {
      get; set;
    }


    public DateTime LastChangeDateForBalances {
      get; set;
    } = DateTime.Now;


    public string DebtorCreditor {
      get; set;
    }

  }  // class AccountStatementEntry

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
