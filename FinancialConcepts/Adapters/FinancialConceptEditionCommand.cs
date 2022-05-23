/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Command                                 *
*  Type     : FinancialConceptEditionCommand             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : The command used to create or update financial concepts.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>The command used to create or update financial concepts.</summary>
  public class FinancialConceptEditionCommand {

    public string FinancialConceptUID {
      get; set;
    } = string.Empty;


    public string GroupUID {
      get; set;
    } = string.Empty;


    public string Code {
      get; set;
    } = string.Empty;


    public string Name {
      get; set;
    } = string.Empty;


    public PositioningRule PositioningRule {
      get; set;
    } = PositioningRule.Undefined;


    public string PositioningOffsetConceptUID {
      get; set;
    } = string.Empty;


    public int Position {
      get; set;
    } = -1;


    public DateTime FromDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime ToDate {
      get; set;
    } = Account.MAX_END_DATE;


    internal void EnsureIsValid() {

    }

  }  // class FinancialConceptEditionCommand

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
