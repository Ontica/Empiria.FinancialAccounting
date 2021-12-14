/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Command payload                         *
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

  }

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
