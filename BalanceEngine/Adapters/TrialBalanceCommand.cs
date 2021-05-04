/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : TrialBalanceCommand                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build trial balances                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Command payload used to build trial balances.</summary>
  public class TrialBalanceCommand {

    public int LedgerId {
      get; set;
    } = -1;

    public int AccountCatalogueId {
      get; set;
    } = -1;

    public string AccountNumber {
      get; set;
    } = string.Empty;

    public string AccountName {
      get; set;
    } = string.Empty;

    public int LedgerAccountId {
      get; set;
    } = -1;


    public int AccountId {
      get; set;
    } = -1;


    public int SectorId {
      get; set;
    } = -1;


    public int SubsidiaryAccountId {
      get; set;
    } = -1;

    public int CurrencyId {
      get; set;
    } = -1; 

    public decimal InitialBalance {
      get; set;
    }

    public decimal Debit {
      get; set;
    }

    public decimal Credit {
      get; set;
    }

    public decimal Balance {
      get; set;
    }


    public DateTime InitialDate {
      get; set;
    } = DateTime.Today;


    public DateTime FinalDate {
      get; set;
    } = DateTime.Today.AddDays(1);


    public string Fields {
      get; set;
    } = string.Empty;

    public string Condition {
      get; set;
    } = string.Empty;

    public string Grouping {
      get; set;
    } = string.Empty;

    public string Having {
      get; set;
    } = string.Empty;

    public string Ordering {
      get; set;
    } = string.Empty;


    public int OutputFormat {
      get; set;
    } = 1;


  } // class TrialBalanceCommand

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
