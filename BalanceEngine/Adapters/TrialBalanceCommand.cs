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

    public int GeneralLedgerId {
      get; set;
    } = -1;

    public int StdAccountTypeId {
      get; set;
    } = -1;

    public string StdAccountNumber {
      get; set;
    } = string.Empty;

    public string StdAccountName {
      get; set;
    } = string.Empty; 

    public DateTime InitialDate {
      get; set;
    } = DateTime.Today;


    public DateTime FinalDate {
      get; set;
    } = DateTime.Today.AddDays(1);


    public int OutputFormat {
      get; set;
    } = 1;


  } // class TrialBalanceCommand

} // Empiria.FinancialAccounting.BalanceEngine.Adapters
