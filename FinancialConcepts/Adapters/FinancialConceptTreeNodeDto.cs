/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptTreeNodeDto                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for a financial concept integration entry as a tree node.                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Data transfer object for a financial concept integration entry as a tree node.</summary>
  public class FinancialConceptTreeNodeDto {

    internal FinancialConceptTreeNodeDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public FinancialConceptEntryType Type {
      get; internal set;
    } = FinancialConceptEntryType.FinancialConceptReference;


    public string ItemName {
      get; internal set;
    } = string.Empty;


    public string ItemCode {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccount {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountName {
      get; internal set;
    } = string.Empty;


    public string SectorCode {
      get; internal set;
    } = string.Empty;


    public string CurrencyCode {
      get; internal set;
    } = string.Empty;


    public string Operator {
      get; internal set;
    } = string.Empty;


    public string DataColumn {
      get; internal set;
    } = string.Empty;


    public string ParentCode {
      get; internal set;
    } = string.Empty;


    public int Level {
      get; internal set;
    } = 1;

  }  // class FinancialConceptTreeNodeDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
